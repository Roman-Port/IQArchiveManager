using IQArchiveManager.Client.Pre;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.Util
{
    class AudioPlaybackProvider : IWaveProvider
    {
        public PreProcessorFileStreamReader Stream { get; set; }

        public WaveFormat WaveFormat => new WaveFormat(MainEditor.AUDIO_SAMPLE_RATE, 8, 1);

        public int Read(byte[] buffer, int offset, int count)
        {
            //Perform as normal
            int read = Stream == null ? 0 : Stream.Read(buffer, offset, count);

            //Fill remaining buffer with 0
            while (read < count)
                buffer[read++] = 0;

            return count;
        }
    }
}
