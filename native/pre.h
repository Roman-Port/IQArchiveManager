#pragma once

#include <stdint.h>
#include <mutex>
#include <dsp/convert/real_to_complex.h>
#include <dsp/channel/frequency_xlator.h>
#include <dsp/filter/decimating_fir.h>
#include <dsp/demod/quadrature.h>
#include <dsp/loop/fast_agc.h>
#include <dsp/loop/costas.h>
#include <dsp/multirate/rational_resampler.h>
#include <dsp/filter/fir.h>
#include <dsp/clock_recovery/mm.h>
#include <dsp/digital/differential_decoder.h>

#define PRE_PROP_INPUT_SAMP_RATE   this->properties[0]
#define PRE_PROP_AUDIO_SAMP_RATE   this->properties[1]
#define PRE_PROP_BB_CUTOFF         this->properties[2]
#define PRE_PROP_BB_TRANSITION     this->properties[3]
#define PRE_PROP_FM_DEVIATION      this->properties[4]
#define PRE_PROP_MPX_CUTOFF        this->properties[5]
#define PRE_PROP_MPX_TRANSITION    this->properties[6]
#define PRE_PROP_AUD_CUTOFF        this->properties[7]
#define PRE_PROP_AUD_TRANSITION    this->properties[8]
#define PRE_PROP_DEEMPHASIS_RATE   this->properties[9]

class iqam_pre {

public:
	iqam_pre(int bufferSize);
	~iqam_pre();

	void set_property(int index, double value);
	double get_property(int index);

	void init();

	void process(const dsp::complex_t* input, uint8_t* audioOut, int* audioOutCount, uint8_t* rdsOut, int* rdsOutCount, int count);

private:
	int buffer_size;
	double properties[32];

	dsp::tap<float> filter_bb_taps;
	dsp::filter::DecimatingFIR<dsp::complex_t, float> filter_bb;

	dsp::demod::Quadrature fm_demod;

	dsp::tap<float> filter_mpx_taps;
	dsp::filter::DecimatingFIR<float, float> filter_mpx;

	dsp::tap<float> filter_aud_taps;
	dsp::filter::DecimatingFIR<float, float> filter_aud;
	dsp::multirate::RationalResampler<float> aud_resamp;

	float deemphasis_alpha;
	float deemphasis_state;

	// Following are for RDS

	dsp::convert::RealToComplex rtoc;
	dsp::channel::FrequencyXlator xlator;
	dsp::multirate::RationalResampler<dsp::complex_t> rds_resamp;
	dsp::loop::FastAGC<dsp::complex_t> agc;
	dsp::loop::Costas<2> costas;
	dsp::tap<dsp::complex_t> taps;
	dsp::filter::FIR<dsp::complex_t, dsp::complex_t> fir;
	dsp::loop::Costas<2> costas2;
	dsp::clock_recovery::MM<float> recov;
	dsp::digital::DifferentialDecoder diff;

	/// <summary>
	/// Calculates the max decimation factor for an input sample rate to an output sample rate.
	/// </summary>
	/// <returns></returns>
	int calculate_decimation(double input, double output);

	void init_audio(double mpxSampleRate);
	void init_rds(double mpxSampleRate);

	void process_audio(const float* mpxInput, uint8_t* audioOut, int* audioOutCount, int count);
	void process_rds(const float* mpxInput, uint8_t* rdsOut, int* rdsOutCount, int count);

};