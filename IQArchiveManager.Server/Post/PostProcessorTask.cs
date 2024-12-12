﻿using IQArchiveManager.Common;
using IQArchiveManager.Common.IO.IqaWriter;
using Newtonsoft.Json;
using RomanPort.LibSDR.Components.IO.WAV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server.Post
{
    public class PostProcessorTask : IArchiveTask
    {
        public PostProcessorTask(string wavFilePath, string outputDir, string tempDir)
        {
            this.wavFilePath = wavFilePath;
            this.outputDir = outputDir;
            this.tempDir = tempDir;
            preFilePath = wavFilePath + ".iqpre";
            postFilePath = wavFilePath + ".iqedit";
        }

        private string wavFilePath;
        private string preFilePath;
        private string postFilePath;

        private string outputDir;
        private string tempDir;

        public void Process(ref string status)
        {
            //Update status
            status = $"POST-PROCESSING - Preparing... - {wavFilePath}";

            //Load the info
            TrackEditInfo[] edits = JsonConvert.DeserializeObject<TrackEditInfo[]>(File.ReadAllText(postFilePath));

            //Open the WAV file
            FileStream wav = new FileStream(wavFilePath, FileMode.Open, FileAccess.Read);
            WavFileInfo info;
            if (!WavHeaderUtil.ParseWavHeader(wav, out info))
                throw new Exception("Failed to parse WAV header!");
            int bytesPerSample = (info.bitsPerSample / 8) * info.channels;

            //Abort if we aren't 16 bits per sample stereo
            if (info.bitsPerSample != 16 || info.channels != 2)
                throw new Exception("Only 16 bit-per-sample stereo files are supported.");

            //Allocate a working buffer
            byte[] buffer = new byte[65536 * bytesPerSample];

            //Convert each edit from seconds to samples
            long[] editSamplesStart = new long[edits.Length];
            long[] editSamplesEnd = new long[edits.Length];
            for (int i = 0; i < edits.Length; i++)
            {
                editSamplesStart[i] = (long)(edits[i].start * (long)info.sampleRate);
                editSamplesEnd[i] = (long)(edits[i].end * (long)info.sampleRate);
            }

            //Total up the number of samples. This is just for status reporting
            long totalSamples = 0;
            long totalSamplesComputed = 0;
            for (int i = 0; i < edits.Length; i++)
                totalSamples += (editSamplesEnd[i] - editSamplesStart[i]) * bytesPerSample;

            //Begin encoding each file
            for (int i = 0; i < edits.Length; i++)
            {
                //Check that we support this editor version. If not, abort out because the data struct may be altered!
                if (edits[i].editorVersion > Constants.CURRENT_EDITOR_VERSION)
                    throw new Exception($"Editor version of clip exceeds current editor version. Obtain a newer version of this program.");

                //Validate
                if (edits[i].data == null || edits[i].data.Id == null)
                    throw new Exception("Clip has no ID.");

                //Calculate the start and end byte of the file
                long startByte = WavHeaderUtil.HEADER_LENGTH + (editSamplesStart[i] * bytesPerSample);
                long endByte = WavHeaderUtil.HEADER_LENGTH + (editSamplesEnd[i] * bytesPerSample);

                //Jump to the start
                wav.Position = startByte;

                //Open the IQA file writer
                using (FileStream output = new FileStream(outputDir + Path.DirectorySeparatorChar + edits[i].data.Id + ".iqa", FileMode.Create))
                using (IqaFileWriter writer = new IqaFileWriter(output))
                {
                    //Write ID block
                    using (IqaSegmentWriter segment = writer.BeginBlock("IQID"))
                    {
                        byte[] idData = Encoding.ASCII.GetBytes(edits[i].data.Id);
                        segment.Write(idData, 0, idData.Length);
                    }

                    //Write info block
                    byte[] infoPayload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(edits[i].data));
                    using (IqaSegmentWriter segment = writer.BeginBlock("INFO"))
                        segment.Write(infoPayload, 0, infoPayload.Length);

                    //Begin encoding clip
                    using (IqaSegmentWriter segment = writer.BeginBlock("DATA"))
                    using (IqaFlacWriter flac = new IqaFlacWriter(segment))
                    {
                        //Transfer
                        int read;
                        do
                        {
                            //Update status
                            status = $"POST-PROCESSING - Clip {i + 1} of {edits.Length} [{edits[i].data.Id}] - {(((double)totalSamplesComputed / totalSamples) * 100).ToString("F")}% total - {wavFilePath}";
                            
                            //Calculate how much is readable and then read
                            read = wav.Read(buffer, 0, (int)Math.Min(buffer.Length, endByte - wav.Position));
                            totalSamplesComputed += read;

                            //Transfer
                            flac.Write(buffer, 0, read);
                        } while (read != 0);
                    }
                }
            }

            //Clean up
            wav.Close();

            //Now that all files have been processed, delete everything involved
            File.Move(wavFilePath, wavFilePath.Replace("input", "temp"));
            File.Move(postFilePath, postFilePath.Replace("input", "temp"));
            if (File.Exists(preFilePath))
                File.Move(preFilePath, preFilePath.Replace("input", "temp"));
        }
    }
}