using IQArchiveManager.Common.IO.RDS;
using IQArchiveManager.Server.Native;
using RomanPort.LibSDR.Components;
using RomanPort.LibSDR.Components.Analog.Primitive;
using RomanPort.LibSDR.Components.Digital.RDS.Client;
using RomanPort.LibSDR.Components.Digital.RDS.Physical;
using RomanPort.LibSDR.Components.FFT.Generators;
using RomanPort.LibSDR.Components.FFT.Mutators;
using RomanPort.LibSDR.Components.Filters;
using RomanPort.LibSDR.Components.Filters.Builders;
using RomanPort.LibSDR.Components.Filters.FIR.ComplexFilter;
using RomanPort.LibSDR.Components.Filters.FIR.Real;
using RomanPort.LibSDR.Components.IO.WAV;
using RomanPort.LibSDR.Components.Resamplers.Arbitrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server.Pre
{
    public unsafe class PreProcessorTask : IArchiveTask
    {
        private string wavFilePath;

        private const int BUFFER_SIZE = 32768;
        private const int DEMOD_BW = 230000;
        private const int AUDIO_SAMPLE_RATE = 20000;
        private const int SCALED_FFT_SIZE = 1024;
        private const int FFT_SCALE_MULTIPLIER = 8;
        private const int FFTS_RATE = 25;
        private const int RDS_FRAME_SCALE = 10;

        public PreProcessorTask(string wavFilePath)
        {
            this.wavFilePath = wavFilePath;
        }

        public void Process(ref string status)
        {
            //Open output file
            FileStream outputFile = new FileStream(wavFilePath + "._iqpre", FileMode.Create);
            PreProcessorFileWriter outputWriter = new PreProcessorFileWriter(outputFile);
            outputWriter.Start();

            //Create streams
            PreProcessorFileStreamWriter outputAudioWriter = outputWriter.CreateStream("AUDIO");
            PreProcessorFileStreamWriter outputFftWriter = outputWriter.CreateStream("SPECTRUM_MAIN");
            PreProcessorFileStreamWriter outputRdsWriter = outputWriter.CreateStream("RDS2");

            //Open WAV file
            FileStream inputFile = new FileStream(wavFilePath, FileMode.Open, FileAccess.Read);
            WavFileReader inputReader = new WavFileReader(inputFile);

            //Create sample buffer
            UnsafeBuffer iqBuffer = UnsafeBuffer.Create(BUFFER_SIZE, out Complex* iqBufferPtr);
            UnsafeBuffer audioBuffer = UnsafeBuffer.Create(BUFFER_SIZE, out float* audioBufferPtr);

            //Create FFT
            FFTGenerator fft = new FFTGenerator(SCALED_FFT_SIZE * FFT_SCALE_MULTIPLIER, false);
            FFTSmoothener fftSmoothener = new FFTSmoothener(fft);

            //Create RDS bit decoder
            float sampleRateScale = 20000.0f / inputReader.SampleRate;
            long totalSamplesRead = 0;
            RdsBlockDecoder rdsDec = new RdsBlockDecoder();
            RdsSerializer rdsEnc = new RdsSerializer();

            //Create output buffers
            byte[] outputAudioBuffer = new byte[BUFFER_SIZE];
            byte[] rdsBuffer = new byte[BUFFER_SIZE];
            byte[] outputFftBuffer = new byte[SCALED_FFT_SIZE];

            //Create native
            IQAMPreNative native = new IQAMPreNative(BUFFER_SIZE);
            native.InputSampleRate = inputReader.SampleRate;
            native.AudioSampleRate = AUDIO_SAMPLE_RATE;
            native.BasebandFilterCutoff = DEMOD_BW / 2;
            native.BasebandFilterTransition = DEMOD_BW * 0.2;
            native.FmDeviation = 85000;
            native.MpxFilterCutoff = 58000 + 6000;
            native.MpxFilterTransition = 6000;
            native.AudioFilterCutoff = 16000;
            native.AudioFilterTransition = 3000;
            native.DeemphasisRate = 75;
            native.Init();

            //Loop
            List<RdsPacket> blockFrames = new List<RdsPacket>();
            int samplesToFftFrame = inputReader.SampleRate / FFTS_RATE;
            int samplesSinceLastSegment = 0;
            fixed (byte* outputAudioBufferPtr = outputAudioBuffer)
            fixed (byte* rdsBufferPtr = rdsBuffer)
            {
                while (true)
                {
                    //Set status
                    status = $"PRE-PROCESSING - {((int)((inputReader.PositionSamples / (double)inputReader.LengthSamples) * 100)).ToString().PadLeft(2, '0')}% - {wavFilePath}";

                    //Read
                    int read = inputReader.Read(iqBufferPtr, Math.Min(inputReader.SampleRate - samplesSinceLastSegment, BUFFER_SIZE));
                    totalSamplesRead += read;
                    if (read == 0)
                        break;

                    //Update and check if we went over
                    samplesSinceLastSegment += read;
                    if (samplesSinceLastSegment >= inputReader.SampleRate)
                    {
                        outputWriter.StartNewSegment();
                        samplesSinceLastSegment = 0;
                    }

                    //Process FFT
                    for (int i = 0; i < read; i++)
                    {
                        //Check
                        if (samplesToFftFrame == 0)
                        {
                            //Write FFT frame
                            float* power = fftSmoothener.ProcessFFT(out int fftBins);
                            for (int b = 0; b < SCALED_FFT_SIZE; b++)
                            {
                                //Sum
                                double max = 0;
                                for (int s = 0; s < FFT_SCALE_MULTIPLIER; s++)
                                    max = Math.Max(max, -power[s]);
                                power += FFT_SCALE_MULTIPLIER;

                                //Write
                                outputFftBuffer[b] = (byte)max;
                            }

                            //Write
                            outputFftWriter.Write(outputFftBuffer, 0, SCALED_FFT_SIZE);

                            //Reset state
                            samplesToFftFrame = inputReader.SampleRate / FFTS_RATE;
                        }

                        //Add
                        fft.AddSamples(iqBufferPtr + i, 1);
                        samplesToFftFrame--;
                    }

                    //Process native
                    native.Process(iqBufferPtr, outputAudioBufferPtr, out int audioOutCount, rdsBufferPtr, out int rdsOutCount, read);

                    //Write audio
                    outputAudioWriter.Write(outputAudioBuffer, 0, audioOutCount);

                    //Push RDS bits into encoder
                    blockFrames.Clear();
                    uint rdsTimestamp = (uint)((totalSamplesRead * sampleRateScale) / RDS_FRAME_SCALE);
                    rdsDec.Process(rdsBuffer, rdsOutCount, rdsTimestamp, blockFrames);

                    //Write this block of frames to the output
                    foreach (RdsPacket frame in blockFrames)
                        rdsEnc.Write(frame);
                }
            }

            //Destroy native
            native.Dispose();

            //Write all RDS frames
            byte[] serRds = rdsEnc.Serialize();
            outputRdsWriter.Write(serRds, 0, serRds.Length);

            //Close
            outputWriter.End();
            outputFile.Close();
            inputFile.Close();

            //Clean up
            iqBuffer.Dispose();
            audioBuffer.Dispose();

            //Finalize file
            File.Move(wavFilePath + "._iqpre", wavFilePath + ".iqpre");
        }
    }
}
