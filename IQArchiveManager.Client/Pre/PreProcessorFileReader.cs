using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.Pre
{
    public class PreProcessorFileReader : IDisposable
    {
        public PreProcessorFileReader(string path)
        {
            this.path = path;
        }

        private string path;

        private int fileMagic;
        private int streamCount;
        private FileStreamInfo[] streams;
        private List<FileStream> openedFiles = new List<FileStream>();

        public void Open()
        {
            using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                //Read file header
                fs.Position = 0;
                byte[] fileHeader = new byte[8];
                fs.Read(fileHeader, 0, 8);
                fileMagic = BitConverter.ToInt32(fileHeader, 0);
                streamCount = BitConverter.ToInt32(fileHeader, 4);

                //Read stream infos
                byte[] streamHeader = new byte[16 + 8 + 8];
                streams = new FileStreamInfo[streamCount];
                for (int i = 0; i < streamCount; i++)
                {
                    fs.Read(streamHeader, 0, streamHeader.Length);
                    streams[i].totalLen = BitConverter.ToInt64(streamHeader, 16);
                    streams[i].segmentTablePos = BitConverter.ToInt64(streamHeader, 24);
                    char[] tag = new char[16];
                    for (int j = 0; j < tag.Length; j++)
                        tag[j] = (char)streamHeader[j];
                    streams[i].tag = new string(tag).TrimEnd('\0');
                }
            }
        }

        public bool TryGetStreamByTag(string tag, out PreProcessorFileStreamReader output)
        {
            output = null;
            foreach (var s in streams)
            {
                if (tag != s.tag)
                    continue;
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                output = new PreProcessorFileStreamReader(stream, s.tag, s.totalLen, s.segmentTablePos);
                output.Open();
                openedFiles.Add(stream);
                return true;
            }
            return false;
        }

        public PreProcessorFileStreamReader GetStreamByTag(string tag)
        {
            if (TryGetStreamByTag(tag, out PreProcessorFileStreamReader output))
                return output;
            throw new Exception("Stream not found.");
        }

        public void Dispose()
        {
            foreach (var s in openedFiles)
                s.Close();
            openedFiles.Clear();
        }

        struct FileStreamInfo
        {
            public string tag;
            public long totalLen;
            public long segmentTablePos;
        }
    }
}
