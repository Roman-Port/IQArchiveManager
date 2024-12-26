using IQArchiveManager.Client.Pre;
using IQArchiveManager.Client.RDS.Modes;
using IQArchiveManager.Common.IO.RDS;
using RomanPort.LibSDR.Components.Digital.RDS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS
{
    public delegate void RdsReader_ProgressEventArgs(int progress, int max);
    public delegate void RdsReader_StatusEventArgs(bool ready);
    public delegate void RdsReader_PatchersEventArgs(RdsReader ctx, BaseRdsMode selected);
    
    public class RdsReader
    {
        private List<RdsValue<string>> rdsRtFramesParsed = new List<RdsValue<string>>();

        private BasicRdsDecoder decoder = new BasicRdsDecoder();

        public event RdsReader_StatusEventArgs OnStatusChanged;
        public event RdsReader_ProgressEventArgs OnProgressUpdated;
        public event RdsReader_PatchersEventArgs OnPatcherUpdated;

        public BaseRdsMode[] rdsModes;

        private const int SAMPLE_RATE = 20000;
        private const int PACKET_SIZE = 12;
        private const int PACKETS_PER_BUFFER = 65536;

        public RdsReader(BaseRdsMode[] rdsModes)
        {
            //Set
            this.rdsModes = rdsModes;
        }

        public IReadOnlyList<RdsValue<string>> PsFrames => decoder.PsFrames;
        public IReadOnlyList<RdsValue<string>> ParsedRtFrames => rdsRtFramesParsed;
        public IReadOnlyList<RdsValue<string>> RawRtFrames => decoder.RtFrames;

        public void Reset()
        {
            //Reset decoder
            decoder.Reset();
            rdsRtFramesParsed.Clear();

            //Send event
            OnStatusChanged?.Invoke(false);
        }

        /// <summary>
        /// Loads from frames
        /// </summary>
        /// <param name="frames"></param>
        public void Load(List<RdsPacket> frames)
        {
            //Reset decoder and reload
            Reset();
            foreach (var f in frames)
                decoder.ProcessFrame(f.timestamp, f.a, f.b, f.c, f.d);

            //Determine the best method of patching
            BaseRdsMode patcher = null;
            for (int i = 0; i < rdsModes.Length; i++)
            {
                if (!IsPatcherRecommended(rdsModes[i]))
                    break;
                patcher = rdsModes[i];
            }

            //Apply patch
            SwitchPatcher(patcher);

            //Send event
            OnStatusChanged?.Invoke(true);
        }

        public void SwitchPatcher(BaseRdsMode patcher)
        {
            OnPatcherUpdated?.Invoke(this, patcher);
            rdsRtFramesParsed = patcher.Patch(new List<RdsValue<string>>(decoder.PsFrames.Select(x => x.Clone())), new List<RdsValue<string>>(decoder.RtFrames.Select(x => x.Clone())), new List<RdsValue<ushort>>(decoder.PiFrames.Select(x => x.Clone())));
            MergeRt();
        }

        public bool IsPatcherRecommended(BaseRdsMode patcher)
        {
            return patcher.IsRecommended(decoder.PsFrames, decoder.RtFrames, decoder.PiFrames);
        }

        private bool GetValueAtSample<T>(long sample, List<RdsValue<T>> frames, out T value, out long start, out long end)
        {
            lock(frames)
            {
                //Set defaults
                start = -1;
                end = -1;
                value = default(T);

                //Work backwards to find one
                for (int i = frames.Count - 1; i >= 0; i--)
                {
                    if (frames[i].first <= sample)
                    {
                        start = frames[i].first;
                        value = frames[i].value;
                        if (i < frames.Count - 1)
                            end = frames[i + 1].first;
                        return true;
                    }
                }
                return false;
            }
        }

        public string GetPsAtSample(long sample, out long start, out long end)
        {
            if (GetValueAtSample(sample, decoder.PsFrames, out string value, out start, out end))
                return value;
            else
                return null;
        }

        public string GetRtAtSample(long sample, out long start, out long end)
        {
            if (GetValueAtSample(sample, rdsRtFramesParsed, out string value, out start, out end))
                return value;
            else
                return null;
        }

        public ushort GetPiAtSample(long sample, out long start, out long end)
        {
            if (GetValueAtSample(sample, decoder.PiFrames, out ushort value, out start, out end))
                return value;
            else
                return 0;
        }

        private void MergeRt()
        {
            //Check if there are duplicate RT frames
            for (int i = 1; i < rdsRtFramesParsed.Count; i++)
            {
                if (rdsRtFramesParsed[i - 1].value == rdsRtFramesParsed[i].value)
                {
                    rdsRtFramesParsed.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
