﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client
{
    public class RdsValue<T>
    {
        public long first;
        public long last;
        public T value;

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

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
