using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS
{
    public class RdsValue<T>
    {
        public const byte DEFAULT_SELEXTRA_START = 15;
        public const byte DEFAULT_SELEXTRA_END = 10;

        public long first;
        public long last;
        public T value;
        public byte selectExtraStart = DEFAULT_SELEXTRA_START; // Number of seconds to add to the selection at start
        public byte selectExtraEnd = DEFAULT_SELEXTRA_END; // Number of seconds to add to the selection at end

        public TimeSpan StartTime
        {
            get => TimeSpan.FromSeconds((double)first / MainEditor.AUDIO_SAMPLE_RATE);
            set => first = (long)(value.TotalSeconds * MainEditor.AUDIO_SAMPLE_RATE);
        }

        public TimeSpan EndTime
        {
            get => TimeSpan.FromSeconds((double)last / MainEditor.AUDIO_SAMPLE_RATE);
            set => last = (long)(value.TotalSeconds * MainEditor.AUDIO_SAMPLE_RATE);
        }

        public TimeSpan Duration => EndTime - StartTime;

        public RdsValue(long sample, T value)
        {
            first = sample;
            last = sample;
            this.value = value;
        }

        public RdsValue(long first, long last, T value) : this(first, value)
        {
            this.last = last;
        }

        public RdsValue(long first, long last, T value, byte selectExtraStart, byte selectExtraEnd) : this(first, last, value)
        {
            this.selectExtraStart = selectExtraStart;
            this.selectExtraEnd = selectExtraEnd;
        }

        public RdsValue<T> Clone()
        {
            return new RdsValue<T>(first, last, value, selectExtraStart, selectExtraEnd);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
