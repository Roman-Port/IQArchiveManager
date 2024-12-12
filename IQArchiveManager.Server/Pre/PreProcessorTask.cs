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
        private const int DEMOD_BW = 200000;
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
            PreProcessorFileStreamWriter outputRdsWriter = outputWriter.CreateStream("RDS");

            //Create temp stuff
            int samplesSinceLastSegment = 0;

            //Open WAV file
            FileStream inputFile = new FileStream(wavFilePath, FileMode.Open, FileAccess.Read);
            WavFileReader inputReader = new WavFileReader(inputFile);

            //Create sample buffer
            UnsafeBuffer iqBuffer = UnsafeBuffer.Create(BUFFER_SIZE, out Complex* iqBufferPtr);
            UnsafeBuffer audioBuffer = UnsafeBuffer.Create(BUFFER_SIZE, out float* audioBufferPtr);

            //Create IQ decimator
            var iqFilterBuilder = new LowPassFilterBuilder(inputReader.SampleRate, DEMOD_BW / 2)
                .SetAutomaticTapCount(DEMOD_BW * 0.2f, 60)
                .SetWindow(WindowType.BlackmanHarris7);
            IComplexFirFilter iqFilter = ComplexFirFilter.CreateFirFilter(iqFilterBuilder, iqFilterBuilder.GetDecimation(out float decimatedSampleRate));

            //Create audio decimator
            var audioFilterBuilder = new LowPassFilterBuilder(decimatedSampleRate, 16000)
                .SetAutomaticTapCount(6000, 40)
                .SetWindow(WindowType.BlackmanHarris7);
            IRealFirFilter audioFilter = RealFirFilter.CreateFirFilter(audioFilterBuilder, audioFilterBuilder.GetDecimation(out float decimatedAudioRate));

            //Create audio resampler
            ArbitraryFloatResampler audioResampler = new ArbitraryFloatResampler(decimatedAudioRate, AUDIO_SAMPLE_RATE, BUFFER_SIZE);

            //Create FM
            FmBasebandDemodulator fm = new FmBasebandDemodulator(FmBasebandDemodulator.DEVIATION_BROADCAST);
            fm.Configure(BUFFER_SIZE, decimatedSampleRate);

            //Create FFT
            FFTGenerator fft = new FFTGenerator(SCALED_FFT_SIZE * FFT_SCALE_MULTIPLIER, false);
            FFTSmoothener fftSmoothener = new FFTSmoothener(fft);

            //Create RDS
            float sampleRateScale = 20000.0f / inputReader.SampleRate;
            long totalSamplesRead = 0;
            List<byte[]> rdsFrames = new List<byte[]>();
            long rdsFramesLen = 0;
            RDSDecoder rds = new RDSDecoder();
            rds.Configure(decimatedSampleRate);
            rds.OnFrameDecoded += (ulong frame) =>
            {
                rdsFrames.Add(BitConverter.GetBytes((uint)((totalSamplesRead * sampleRateScale) / RDS_FRAME_SCALE)));
                rdsFrames.Add(BitConverter.GetBytes(frame));
                rdsFramesLen += 12;
            };

            //Create output buffers
            byte[] outputAudioBuffer = new byte[BUFFER_SIZE];
            byte[] outputFftBuffer = new byte[SCALED_FFT_SIZE];

            //Loop
            int samplesToFftFrame = inputReader.SampleRate / FFTS_RATE;
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

                //Decimate + filter samples
                read = iqFilter.Process(iqBufferPtr, read);

                //Demodulate
                fm.Demodulate(iqBufferPtr, audioBufferPtr, read);

                //Decode RDS
                rds.Process(audioBufferPtr, read);

                //Decimate + filter audio
                read = audioFilter.Process(audioBufferPtr, read);

                //Resample audio
                audioResampler.Input(audioBufferPtr, read, 1);
                do
                {
                    //Read
                    read = audioResampler.Output(audioBufferPtr, BUFFER_SIZE, 1);

                    //Convert
                    for (int i = 0; i < read; i++)
                        outputAudioBuffer[i] = (byte)((Math.Max(Math.Min(audioBufferPtr[i] * 0.7f, 1), -1) * 127.5f) + 127.5f);

                    //Write to file
                    outputAudioWriter.Write(outputAudioBuffer, 0, read);
                } while (read != 0);
            }

            //Write all RDS frames
            foreach(var f in rdsFrames)
            {
                outputRdsWriter.Write(f, 0, f.Length);
            }

            //Close
            outputWriter.End();
            outputFile.Close();
            inputFile.Close();

            //Clean up
            iqBuffer.Dispose();
            audioBuffer.Dispose();
            audioResampler.Dispose();

            //Finalize file
            File.Move(wavFilePath + "._iqpre", wavFilePath + ".iqpre");
        }
    }
}
