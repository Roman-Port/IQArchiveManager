using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IQArchiveManager.Server.Pre
{
    public class PreProcessorFileWriter
    {
        public PreProcessorFileWriter(FileStream file)
        {
            this.file = file;
        }

        private FileStream file;
        private List<PreProcessorFileStreamWriter> streams = new List<PreProcessorFileStreamWriter>();

        private const int FILE_HEADER_LEN = 8;

        public void Start()
        {
            //Make room for header
            for (int i = 0; i < FILE_HEADER_LEN; i++)
                file.WriteByte(0);
        }

        public PreProcessorFileStreamWriter CreateStream(string tag)
        {
            PreProcessorFileStreamWriter stream = new PreProcessorFileStreamWriter(file, tag);
            stream.Start();
            streams.Add(stream);
            return stream;
        }

        public void StartNewSegment()
        {
            foreach (var s in streams)
                s.End();
        }

        public void End()
        {
            //Finish streams first
            foreach (var s in streams)
                s.End();

            //Create file header
            byte[] header = new byte[FILE_HEADER_LEN];
            BitConverter.GetBytes(1380995401).CopyTo(header, 0); //"IQPO"
            BitConverter.GetBytes(streams.Count).CopyTo(header, 4);

            //Go back to beginning and write header
            file.Position = 0;
            file.Write(header, 0, FILE_HEADER_LEN);

            //Flush
            file.Flush();
        }
    }
}
