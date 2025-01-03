using IQArchiveManager.Client.Pre;
using IQArchiveManager.Client.RDS.Parser.Modes;
using IQArchiveManager.Client.RDS.Parser;
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
    public delegate void RdsReader_PatchersEventArgs(RdsReader ctx, BaseRdsMode selected);
    
    public class RdsReader
    {
        private List<RdsValue<string>> rdsRtFramesParsed = new List<RdsValue<string>>();
        private ulong cachedRecommended; // bitfield of recommended patchers

        private BasicRdsDecoder decoder = new BasicRdsDecoder();

        public event RdsReader_PatchersEventArgs OnPatcherUpdated;

        public BaseRdsMode[] rdsModes;

        public RdsReader(RdsParserStore parser)
        {
            //Set
            this.rdsModes = parser.Modes;
        }

        public List<RdsValue<string>> PsFrames => decoder.PsFrames;
        public List<RdsValue<string>> ParsedRtFrames => rdsRtFramesParsed;
        public List<RdsValue<string>> RawRtFrames => decoder.RtFrames;
        public List<RdsValue<ushort>> RawPiFrames => decoder.PiFrames;

        public void Reset()
        {
            //Reset decoder
            decoder.Reset();
            rdsRtFramesParsed.Clear();
        }

        /// <summary>
        /// Loads from frames
        /// </summary>
        /// <param name="frames"></param>
        public void Load(List<RdsPacket> frames, IRdsPatchContext ctx)
        {
            //Reset decoder and reload
            Reset();
            foreach (var f in frames)
                decoder.ProcessFrame(f.timestamp, f.a, f.b, f.c, f.d);

            //Check all patchers to see which are recommended, recreating the cache
            cachedRecommended = 0;
            foreach (var m in rdsModes)
            {
                if (m.IsRecommended(ctx, decoder.PsFrames, decoder.RtFrames, decoder.PiFrames))
                    cachedRecommended |= 1UL << (int)m.Id;
            }

            //Determine the best method of patching (first recommended one, aside from first)
            BaseRdsMode patcher = rdsModes[0];
            for (int i = 1; i < rdsModes.Length; i++)
            {
                if (IsPatcherRecommended(rdsModes[i]))
                {
                    patcher = rdsModes[i];
                    break;
                }
            }

            //Apply patch
            SwitchPatcher(patcher, ctx);
        }

        public void SwitchPatcher(BaseRdsMode patcher, IRdsPatchContext ctx)
        {
            OnPatcherUpdated?.Invoke(this, patcher);
            rdsRtFramesParsed = patcher.Patch(ctx, new List<RdsValue<string>>(decoder.PsFrames.Select(x => x.Clone())), new List<RdsValue<string>>(decoder.RtFrames.Select(x => x.Clone())), new List<RdsValue<ushort>>(decoder.PiFrames.Select(x => x.Clone())));
            MergeRt();
        }

        public bool IsPatcherRecommended(BaseRdsMode patcher)
        {
            return IsPatcherRecommended(patcher.Id);
        }

        public bool IsPatcherRecommended(RdsModeId id)
        {
            return (cachedRecommended & (1UL << (int)id)) != 0;
        }

        private bool GetValueAtSample<T>(long sample, List<RdsValue<T>> frames, out RdsValue<T> value)
        {
            lock(frames)
            {
                //Set defaults
                long start = -1;
                long end = -1;
                value = default(RdsValue<T>);

                //Work backwards to find one
                for (int i = frames.Count - 1; i >= 0; i--)
                {
                    if (frames[i].first <= sample)
                    {
                        start = frames[i].first;
                        value = frames[i];
                        if (i < frames.Count - 1)
                            end = frames[i + 1].first;
                        return true;
                    }
                }
                return false;
            }
        }

        public RdsValue<string> GetPsAtSample(long sample)
        {
            if (GetValueAtSample(sample, decoder.PsFrames, out RdsValue<string> value))
                return value;
            else
                return null;
        }

        public RdsValue<string> GetRtAtSample(long sample)
        {
            if (GetValueAtSample(sample, rdsRtFramesParsed, out RdsValue<string> value))
                return value;
            else
                return null;
        }

        public RdsValue<ushort> GetPiAtSample(long sample)
        {
            if (GetValueAtSample(sample, decoder.PiFrames, out RdsValue<ushort> value))
                return value;
            else
                return null;
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
