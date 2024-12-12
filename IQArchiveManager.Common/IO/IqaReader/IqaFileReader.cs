using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Common.IO.IqaReader
{
    public class IqaFileReader : IDisposable
    {
        public IqaFileReader(string path)
        {
            file = new FileStream(path, FileMode.Open, FileAccess.Read);
            closeOnDispose = true;
        }

        public IqaFileReader(Stream file)
        {
            this.file = file;
            closeOnDispose = false;
        }

        private Stream file;
        private bool closeOnDispose;

        public IqaSegmentReader OpenSegment(string tag, int skip = 0)
        {
            //Go to beginning
            file.Position = 0;

            //Seek for this segment
            int offset = 0;
            while (true)
            {
                //Get info
                long start = file.Position;
                if (!IqaSegmentReader.ReadSegmentInfo(file, out string readTag, out long readLen, out uint readCrc))
                    break;

                //If this matches, contain this
                if (tag == readTag && offset++ == skip)
                    return new IqaSegmentReader(file, start);

                //Skip
                file.Position += readLen;
            }
            return null;
        }

        public void Dispose()
        {
            if(closeOnDispose)
                file.Close();
        }
    }
}
