using RomanPort.LibSDR.Components;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IQArchiveManager.Server.Native
{
    unsafe class IQAMPreNative : IDisposable
    {
        public IQAMPreNative(int bufferSize)
        {
            //Set
            this.bufferSize = bufferSize;

            //Create native object
            ctx = iqam_pre_create(bufferSize);
            if (ctx == IntPtr.Zero)
                throw new Exception("Failed to create native object.");
        }

        ~IQAMPreNative()
        {
            Dispose();
        }

        private readonly IntPtr ctx;
        private readonly int bufferSize;
        private bool disposed = false;

        /* NATIVE */

        [DllImport(IQAMNative.LIB_NAME)]
        private static extern IntPtr iqam_pre_create(int bufferSize);

        [DllImport(IQAMNative.LIB_NAME)]
        private static extern void iqam_pre_destroy(IntPtr ctx);

        [DllImport(IQAMNative.LIB_NAME)]
        private static extern double iqam_pre_get_property(IntPtr ctx, int index);

        [DllImport(IQAMNative.LIB_NAME)]
        private static extern void iqam_pre_set_property(IntPtr ctx, int index, double value);

        [DllImport(IQAMNative.LIB_NAME)]
        private static extern void iqam_pre_init(IntPtr ctx);

        [DllImport(IQAMNative.LIB_NAME)]
        private static extern void iqam_pre_process(IntPtr ctx, Complex* input, byte* audioOut, int* audioOutCount, byte* rdsOut, int* rdsOutCount, int count);

        /* UTIL */

        private void EnsureValid()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private double GetProperty(int index)
        {
            EnsureValid();
            return iqam_pre_get_property(ctx, index);
        }

        private void SetProperty(int index, double value)
        {
            EnsureValid();
            iqam_pre_set_property(ctx, index, value);
        }

        /* PROPERTIES */

        public double InputSampleRate
        {
            get => GetProperty(0);
            set => SetProperty(0, value);
        }

        public double AudioSampleRate
        {
            get => GetProperty(1);
            set => SetProperty(1, value);
        }

        public double BasebandFilterCutoff
        {
            get => GetProperty(2);
            set => SetProperty(2, value);
        }

        public double BasebandFilterTransition
        {
            get => GetProperty(3);
            set => SetProperty(3, value);
        }

        public double FmDeviation
        {
            get => GetProperty(4);
            set => SetProperty(4, value);
        }

        public double MpxFilterCutoff
        {
            get => GetProperty(5);
            set => SetProperty(5, value);
        }

        public double MpxFilterTransition
        {
            get => GetProperty(6);
            set => SetProperty(6, value);
        }

        public double AudioFilterCutoff
        {
            get => GetProperty(7);
            set => SetProperty(7, value);
        }

        public double AudioFilterTransition
        {
            get => GetProperty(8);
            set => SetProperty(8, value);
        }

        public double DeemphasisRate
        {
            get => GetProperty(9);
            set => SetProperty(9, value);
        }

        /* API */

        public void Init()
        {
            EnsureValid();
            iqam_pre_init(ctx);
        }

        public void Process(Complex* input, byte* audioOut, out int audioOutCount, byte* rdsOut, out int rdsOutCount, int count)
        {
            EnsureValid();
            int _audioOutCount;
            int _rdsOutCount;
            iqam_pre_process(ctx, input, audioOut, &_audioOutCount, rdsOut, &_rdsOutCount, count);
            if (_audioOutCount > bufferSize || _rdsOutCount > bufferSize)
                throw new Exception("!!! NATIVE BUFFER OVERRUN !!! - The native component has written more data than expected. APPLICATION CRASH IS LIKELY.");
            audioOutCount = _audioOutCount;
            rdsOutCount = _rdsOutCount;
        }

        public void Dispose()
        {
            //If not already disposed, destroy it
            if (!disposed)
                iqam_pre_destroy(ctx);

            //Set flag
            disposed = true;
        }
    }
}
