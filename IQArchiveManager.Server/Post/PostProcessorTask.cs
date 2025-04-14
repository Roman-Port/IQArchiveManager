using IQArchiveManager.Common;
using IQArchiveManager.Common.IO.Editor.Post;
using IQArchiveManager.Common.IO.IqaWriter;
using Newtonsoft.Json;
using RomanPort.LibSDR.Components.IO.WAV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server.Post
{
    public class PostProcessorTask : ArchiveTask
    {
        public PostProcessorTask(string wavFilePath, string outputDir, string finishedDir) : base("POST-PROCESSING", wavFilePath)
        {
            this.wavFilePath = wavFilePath;
            this.outputDir = outputDir;
            this.finishedDir = finishedDir;
            preFilePath = wavFilePath + ".iqpre";
            postFilePath = wavFilePath + ".iqedit";
        }

        private string wavFilePath;
        private string preFilePath;
        private string postFilePath;

        private string outputDir;
        private string finishedDir;

        private PostEditFile ReadInfo()
        {
            //Read text
            string rawInfo = File.ReadAllText(postFilePath);

            //Check if it's a newer format file or an older format file
            PostEditFile info;
            if (rawInfo.StartsWith("[")) // Old format
            {
                //This older format just has an array of edits -- Read them
                List<TrackEditInfo> edits = JsonConvert.DeserializeObject<List<TrackEditInfo>>(rawInfo);

                //Wrap into the new file format
                info = new PostEditFile
                {
                    EditorVersion = 0,
                    Edits = edits,
                    LastEdited = DateTime.MinValue,
                    Delete = false
                };
            } else if (rawInfo.StartsWith("{")) // New format
            {
                //Deserialize
                info = JsonConvert.DeserializeObject<PostEditFile>(rawInfo);
            } else // Invalid
            {
                throw new Exception("Unknown post file.");
            }

            //Check that we support this editor version. If not, abort out because the data struct may be altered!
            if (info.EditorVersion > Constants.CURRENT_EDITOR_VERSION)
                throw new Exception($"Editor version of file exceeds current editor version. Obtain a newer version of this program.");

            return info;
        }

        public override void Process()
        {
            //Load the info
            PostEditFile editorInfo = ReadInfo();
            TrackEditInfo[] edits = editorInfo.Edits.ToArray();

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
                editSamplesStart[i] = (long)(edits[i].Start * (long)info.sampleRate);
                editSamplesEnd[i] = (long)(edits[i].End * (long)info.sampleRate);
            }

            //Total up the number of samples. This is just for status reporting
            long totalSamples = 0;
            long totalSamplesComputed = 0;
            for (int i = 0; i < edits.Length; i++)
                totalSamples += (editSamplesEnd[i] - editSamplesStart[i]) * bytesPerSample;
            ProgressMax = totalSamples;

            //Begin encoding each file
            for (int i = 0; i < edits.Length; i++)
            {
                //Validate
                if (edits[i].Data == null || edits[i].Data.Id == null)
                    throw new Exception("Clip has no ID.");

                //Calculate the start and end byte of the file
                long startByte = WavHeaderUtil.HEADER_LENGTH + (editSamplesStart[i] * bytesPerSample);
                long endByte = WavHeaderUtil.HEADER_LENGTH + (editSamplesEnd[i] * bytesPerSample);

                //Jump to the start
                wav.Position = startByte;

                //Open the IQA file writer
                using (FileStream output = new FileStream(outputDir + Path.DirectorySeparatorChar + edits[i].Data.Id + ".iqa", FileMode.Create))
                using (IqaFileWriter writer = new IqaFileWriter(output))
                {
                    //Write ID block
                    using (IqaSegmentWriter segment = writer.BeginBlock("IQID"))
                    {
                        byte[] idData = Encoding.ASCII.GetBytes(edits[i].Data.Id);
                        segment.Write(idData, 0, idData.Length);
                    }

                    //Write info block
                    byte[] infoPayload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(edits[i].Data));
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
                            StatusText = $"Clip {i + 1} of {edits.Length} [{edits[i].Data.Id}]";
                            ProgressValue = totalSamplesComputed;
                            
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

            //Now that all files have been processed, delete or move everything involved
            if (editorInfo.Delete)
            {
                //Delete
                File.Delete(wavFilePath);
                File.Delete(postFilePath);
                if (File.Exists(preFilePath))
                    File.Delete(preFilePath);
            } else
            {
                //Move
                MoveToFinal(wavFilePath, finishedDir);
                MoveToFinal(postFilePath, finishedDir);
                if (File.Exists(preFilePath))
                    MoveToFinal(preFilePath, finishedDir);
            }
        }

        private static void MoveToFinal(string inputFile, string outputDir)
        {
            //Get the standalone filename
            string name = new FileInfo(inputFile).Name;

            //Find unique name in the output
            outputDir = outputDir.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string outputFile;
            int index = 0;
            do
            {
                outputFile = outputDir + name;
                for (int i = 0; i < index; i++)
                    outputFile += "_";
            } while (File.Exists(outputFile));

            //Perform move
            File.Move(inputFile, outputFile);
        }
    }
}
