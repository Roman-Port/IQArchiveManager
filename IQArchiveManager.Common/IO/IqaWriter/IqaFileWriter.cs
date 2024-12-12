using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Common.IO.IqaWriter
{
    public class IqaFileWriter : IDisposable
    {
        public IqaFileWriter(Stream f)
        {
            this.f = f;
        }

        private Stream f;

        public IqaSegmentWriter BeginBlock(string tag)
        {
            f.Position = f.Length;
            return new IqaSegmentWriter(f, tag);
        }

        public void Dispose()
        {
            
        }
    }
}
