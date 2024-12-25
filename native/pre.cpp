#include "pre.h"

#include <mutex>
#include <dsp/taps/low_pass.h>
#include <dsp/taps/band_pass.h>
#include <dsp/convert/complex_to_real.h>
#include <dsp/digital/binary_slicer.h>
#include <cassert>

iqam_pre::iqam_pre(int bufferSize) :
    buffer_size(bufferSize)
{
    //Clear properties
    for (int i = 0; i < (sizeof(properties) / sizeof(properties[0])); i++)
        properties[i] = 0;
}

iqam_pre::~iqam_pre() {
}

void iqam_pre::set_property(int index, double value) {
    properties[index] = value;
}

double iqam_pre::get_property(int index) {
    return properties[index];
}

void iqam_pre::init() {
    //Determine decimation factor for baseband
    int bbDecim = calculate_decimation(PRE_PROP_INPUT_SAMP_RATE, PRE_PROP_BB_CUTOFF + (PRE_PROP_BB_TRANSITION * 2));
    double bbSampleRate = PRE_PROP_INPUT_SAMP_RATE / bbDecim;

    //Create baseband filter
    filter_bb_taps = dsp::taps::lowPass(PRE_PROP_BB_CUTOFF, PRE_PROP_BB_TRANSITION, PRE_PROP_INPUT_SAMP_RATE);
    filter_bb.init(NULL, filter_bb_taps, bbDecim);
    filter_bb.out.setBufferSize(buffer_size);

    //Configure FM demod
    fm_demod.init(NULL, PRE_PROP_FM_DEVIATION, bbSampleRate);
    fm_demod.out.setBufferSize(buffer_size);

    //Determine decimation factor for MPX
    int mpxDecim = calculate_decimation(bbSampleRate, PRE_PROP_MPX_CUTOFF + (PRE_PROP_MPX_TRANSITION * 2));
    double mpxSampleRate = bbSampleRate / mpxDecim;

    //Create composite filter
    filter_mpx_taps = dsp::taps::lowPass(PRE_PROP_MPX_CUTOFF, PRE_PROP_MPX_TRANSITION, bbSampleRate);
    filter_mpx.init(NULL, filter_mpx_taps, mpxDecim);
    filter_mpx.out.setBufferSize(buffer_size);

    //Init components
    init_audio(mpxSampleRate);
    init_rds(mpxSampleRate);
}

void iqam_pre::init_audio(double mpxSampleRate) {
    //Determine decimation factor for audio
    int audioDecim = calculate_decimation(mpxSampleRate, PRE_PROP_AUD_CUTOFF + (PRE_PROP_AUD_TRANSITION * 2));
    double audioSampleRateCoarse = mpxSampleRate / audioDecim;

    //Create audio filter
    filter_aud_taps = dsp::taps::lowPass(PRE_PROP_AUD_CUTOFF, PRE_PROP_AUD_TRANSITION, mpxSampleRate);
    filter_aud.init(NULL, filter_aud_taps, audioDecim);
    filter_aud.out.setBufferSize(buffer_size);

    //Reset and calculate deemphesis alpha
    deemphasis_alpha = 0;
    deemphasis_state = 0;
    if (PRE_PROP_DEEMPHASIS_RATE != 0)
        deemphasis_alpha = 1.0f - exp(-1.0f / (audioSampleRateCoarse * (PRE_PROP_DEEMPHASIS_RATE * 1e-6f)));

    //Set up resampler for audio to resample it to the specific output sample rate
    aud_resamp.init(NULL, audioSampleRateCoarse, PRE_PROP_AUDIO_SAMP_RATE);
    aud_resamp.out.setBufferSize(buffer_size);
}

void iqam_pre::init_rds(double mpxSampleRate) {
    //Init RTOC
    rtoc.init(NULL);
    rtoc.out.setBufferSize(buffer_size);

    //Init xlator
    xlator.init(NULL, -57000.0, mpxSampleRate);
    xlator.out.setBufferSize(buffer_size);

    //Init resampler
    rds_resamp.init(NULL, mpxSampleRate, 5000.0);
    rds_resamp.out.setBufferSize(buffer_size);

    //Init AGC
    agc.init(NULL, 1.0, 1e6, 0.1);
    agc.out.setBufferSize(buffer_size);

    //Init costas loop
    costas.init(NULL, 0.005f);
    costas.out.setBufferSize(buffer_size);

    //Init filter
    taps = dsp::taps::bandPass<dsp::complex_t>(0, 2375, 100, 5000);
    fir.init(NULL, taps);
    fir.out.setBufferSize(buffer_size);

    //Init second costas loop
    double baudfreq = dsp::math::hzToRads(2375.0 / 2.0, 5000);
    costas2.init(NULL, 0.01, 0.0, baudfreq, baudfreq - (baudfreq * 0.1), baudfreq + (baudfreq * 0.1));
    costas2.out.setBufferSize(buffer_size);

    //Init clock recovery
    recov.init(NULL, 5000.0 / (2375.0 / 2.0), 1e-6, 0.01, 0.01);
    recov.out.setBufferSize(buffer_size);

    //Init differential decoder
    diff.init(NULL, 2);
    diff.out.setBufferSize(buffer_size);
}

static void process_deemphasis(float alpha, float* state, float* buffer, int count) {
    for (int i = 0; i < count; i++)
    {
        *state += alpha * (buffer[i] - *state);
        buffer[i] = *state;
    }
}

void iqam_pre::process(const dsp::complex_t* input, uint8_t* audioOut, int* audioOutCount, uint8_t* rdsOut, int* rdsOutCount, int count) {
    //Decimate + filter baseband
    count = filter_bb.process(count, input, filter_bb.out.writeBuf);

    //Demodulate FM
    count = fm_demod.process(count, filter_bb.out.writeBuf, fm_demod.out.writeBuf);

    //Decimate + filter composite
    count = filter_mpx.process(count, fm_demod.out.writeBuf, filter_mpx.out.writeBuf);

    //Process components
    process_audio(filter_mpx.out.writeBuf, audioOut, audioOutCount, count);
    process_rds(filter_mpx.out.writeBuf, rdsOut, rdsOutCount, count);
}

void iqam_pre::process_audio(const float* mpxInput, uint8_t* audioOut, int* audioOutCount, int count) {
    //Filter audio for output
    int audioCount = filter_aud.process(count, mpxInput, filter_aud.out.writeBuf);

    //Apply deemphasis
    if (deemphasis_alpha != 0)
        process_deemphasis(deemphasis_alpha, &deemphasis_state, filter_aud.out.writeBuf, audioCount);

    //Resample to final output rate
    audioCount = aud_resamp.process(audioCount, filter_aud.out.writeBuf, aud_resamp.out.writeBuf);

    //Scale all samples for conversion
    volk_32f_s32f_multiply_32f(aud_resamp.out.writeBuf, aud_resamp.out.writeBuf, 127.5f, audioCount);

    //Ensure all samples are between -127 and 127 and convert to bytes
    for (int i = 0; i < audioCount; i++) {
        if (aud_resamp.out.writeBuf[i] > 127)
            aud_resamp.out.writeBuf[i] = 127;
        if (aud_resamp.out.writeBuf[i] < -127)
            aud_resamp.out.writeBuf[i] = -127;
        audioOut[i] = aud_resamp.out.writeBuf[i] + 127.5f;
    }

    //Set output count
    *audioOutCount = audioCount;
}

void iqam_pre::process_rds(const float* mpxInput, uint8_t* rdsOut, int* rdsOutCount, int count) {
    //Convert MPX to complex
    rtoc.process(count, mpxInput, rtoc.out.writeBuf);

    //Translate to 0Hz
    xlator.process(count, rtoc.out.writeBuf, rtoc.out.writeBuf);

    //Resample to the output samplerate
    count = rds_resamp.process(count, rtoc.out.writeBuf, rds_resamp.out.writeBuf);

    //Decode everything else until we have the bytes
    count = agc.process(count, rds_resamp.out.writeBuf, costas.out.readBuf);
    count = costas.process(count, costas.out.readBuf, costas.out.writeBuf);
    count = fir.process(count, costas.out.writeBuf, costas.out.writeBuf);
    count = costas2.process(count, costas.out.writeBuf, costas.out.readBuf);
    count = dsp::convert::ComplexToReal::process(count, costas.out.readBuf, recov.out.readBuf);
    count = recov.process(count, recov.out.readBuf, recov.out.writeBuf);
    count = dsp::digital::BinarySlicer::process(count, recov.out.writeBuf, diff.out.readBuf);
    count = diff.process(count, diff.out.readBuf, rdsOut);
    *rdsOutCount = count;
}

int iqam_pre::calculate_decimation(double inputSampleRate, double bandwidth) {
    //Calculate the rate by finding the LOWEST we can go without it becoming a rate lower than the desired rate
    int decimationRate = 1;
    while (inputSampleRate / (decimationRate + 1) >= (bandwidth * 2)) //Multiply the bandwidth so we can run this without any aliasing
    {
        decimationRate++;
    }

    return decimationRate;
}