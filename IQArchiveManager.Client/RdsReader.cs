using IQArchiveManager.Client.Pre;
using IQArchiveManager.Client.RdsModes;
using RomanPort.LibSDR.Components.Digital.RDS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client
{
    public delegate void RdsReader_ProgressEventArgs(int progress, int max);
    public delegate void RdsReader_StatusEventArgs(bool ready);
    public delegate void RdsReader_PatchersEventArgs(RdsReader ctx, BaseRdsMode selected);
    
    public class RdsReader
    {
        private List<RdsValue<string>> rdsRtFramesRaw = new List<RdsValue<string>>();
        private List<RdsValue<string>> rdsRtFrames = new List<RdsValue<string>>();
        private List<RdsValue<string>> rdsPsFrames = new List<RdsValue<string>>();
        private List<RdsValue<ushort>> rdsPiFrames = new List<RdsValue<ushort>>();

        private RdsClient decoder;
        private long latestSample;

        public event RdsReader_StatusEventArgs OnStatusChanged;
        public event RdsReader_ProgressEventArgs OnProgressUpdated;
        public event RdsReader_PatchersEventArgs OnPatcherUpdated;

        public BaseRdsMode[] rdsModes;

        private const int SAMPLE_RATE = 20000;
        private const int PACKET_SIZE = 12;
        private const int PACKETS_PER_BUFFER = 65536;
        private const int RDS_FRAME_NUMBER_SCALE = 10;

        public RdsReader(ClipDatabase db)
        {
            //Create RDS modes
            rdsModes = new BaseRdsMode[]
            {
                new RdsPatchNative(),
                new RdsPatchKzcr(db),
                new RdsPatchKzcrLegacy(db),
                new RdsPatchCumulus(),
                new RdsPatchNoDelimiters(db)
            };

            //Set up decoder
            decoder = new RdsClient();
            decoder.PiCode.OnPiCodeChanged += PiCode_OnPiCodeChanged;
            decoder.ProgramService.OnFullTextReceived += ProgramService_OnFullTextReceived;
            decoder.RadioText.OnFullTextReceived += RadioText_OnFullTextReceived;
        }

        public IReadOnlyList<RdsValue<string>> PsFrames => new List<RdsValue<string>>(rdsPsFrames);
        public IReadOnlyList<RdsValue<string>> ParsedRtFrames => new List<RdsValue<string>>(rdsRtFrames);
        public IReadOnlyList<RdsValue<string>> RawRtFrames => new List<RdsValue<string>>(rdsRtFramesRaw);

        public void Process(PreProcessorFileStreamReader stream)
        {
            //Reset decoder
            decoder.Reset();
            rdsRtFrames.Clear();
            rdsPsFrames.Clear();
            rdsPiFrames.Clear();

            //Send event
            OnStatusChanged?.Invoke(false);

            //Allocate buffer
            byte[] buffer = new byte[PACKETS_PER_BUFFER * PACKET_SIZE];

            //Loop until there are no segments remaining
            int read;
            do
            {
                //Send event
                OnProgressUpdated?.Invoke(stream.CurrentSegment, stream.SegmentCount);

                //Read from file
                read = stream.Read(buffer, 0, buffer.Length);

                //Validate
                if (read % PACKET_SIZE != 0)
                    throw new Exception("Invalid number of bytes read, not divisible by packet size!");

                //Process all packets
                ulong frame;
                for (int offset = 0; offset < read; offset += PACKET_SIZE)
                {
                    //Read the two parts
                    latestSample = BitConverter.ToUInt32(buffer, offset) * RDS_FRAME_NUMBER_SCALE;
                    frame = BitConverter.ToUInt64(buffer, offset + 4);

                    //Decode
                    decoder.ProcessFrame(frame);
                }
            } while (stream.CurrentSegment < stream.SegmentCount);

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
            lock (rdsRtFramesRaw)
            {
                rdsRtFrames = patcher.Patch(rdsPsFrames, rdsRtFramesRaw, rdsPiFrames);
                MergeRt();
            }
        }

        public bool IsPatcherRecommended(BaseRdsMode patcher)
        {
            return patcher.IsRecommended(rdsPsFrames, rdsRtFramesRaw, rdsPiFrames);
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
            if (GetValueAtSample(sample, rdsPsFrames, out string value, out start, out end))
                return value;
            else
                return null;
        }

        public string GetRtAtSample(long sample, out long start, out long end)
        {
            if (GetValueAtSample(sample, rdsRtFrames, out string value, out start, out end))
                return value;
            else
                return null;
        }

        public ushort GetPiAtSample(long sample, out long start, out long end)
        {
            if (GetValueAtSample(sample, rdsPiFrames, out ushort value, out start, out end))
                return value;
            else
                return 0;
        }

        private void RadioText_OnFullTextReceived(RdsClient ctx, string text)
        {
            AddIfNotMatching(rdsRtFramesRaw, text);
        }

        private void ProgramService_OnFullTextReceived(RdsClient ctx, string text)
        {
            AddIfNotMatching(rdsPsFrames, text);
        }

        private void PiCode_OnPiCodeChanged(RdsClient ctx, ushort pi)
        {
            AddIfNotMatching(rdsPiFrames, pi);
        }

        private void AddIfNotMatching<T>(List<RdsValue<T>> frames, T value)
        {
            //Get the last one, if any
            RdsValue<T> last = frames.LastOrDefault();

            //Check if the last matches
            if (last != null && last.value.Equals(value))
            {
                //They are equal, so just update the "last" variable and don't bother adding a new one
                last.last = latestSample;
            }
            else
            {
                //Not seen previously, so add a new one
                frames.Add(new RdsValue<T>(latestSample, value));
            }
        }

        private void MergeRt()
        {
            //Check if there are duplicate RT frames
            for (int i = 1; i < rdsRtFrames.Count; i++)
            {
                if (rdsRtFrames[i - 1].value == rdsRtFrames[i].value)
                {
                    rdsRtFrames.RemoveAt(i);
                    i--;
                }
            }
        }        

        private void PsToRt()
        {
            //This parses the Cumulus Media scrolling PS frames to RT for stations that only have
            //PS, but no RT, like KXXR-FM over here. The incoming PS frames look like this, offsetting each by 2 characters:
            //CREEP by
            //EEP by S
            //P by STO
            //by STONE
            // STONE T
            //TONE TEM
            //NE TEMPL
            // TEMPLE 
            //EMPLE PI
            //PLE PILO
            //E PILOTS
            //PILOTS o
            //LOTS on 
            //TS on 93
            // on 93X
            //(repeats)

            //If we already have RT frames, this is unnecessary. Skip
            if (rdsRtFrames.Count > 0)
                return;

            //Parse
            char[] rtBuffer = new char[128];
            char[] lastPsFrame = new char[8];
            int rtBufferIndex = 0;
            List<RdsValue<string>> newFrames = new List<RdsValue<string>>();
            bool hasFirstFrame = false;
            long time = 0;
            foreach (var frame in rdsPsFrames)
            {
                //Compare to last frame and check how much they're offset
                char[] frameData = frame.value.ToCharArray();
                int matchingOffset = 0;
                while (matchingOffset < 8)
                {
                    bool check = true;
                    for (int i = 0; i < 8 - matchingOffset; i++)
                    {
                        check = (frameData[i] == lastPsFrame[i + matchingOffset]) && check;
                    }
                    if (check)
                        break;
                    matchingOffset += 2;
                }

                //Determine case
                bool matches = matchingOffset == 0;
                bool offset = matchingOffset < 8;

                //If this is a match, ignore this frame
                if (matches == true)
                    continue;

                //If this was not a match, this might be an error. Try to guess if this is an error or not by
                //checking each segment for "stuck" segments. Segments are two characters each
                if (!matches)
                {
                    bool hasStuckSegments = false;
                    for (int i = 0; i < 8; i += 2)
                    {
                        hasStuckSegments = hasStuckSegments || ((frameData[i] == lastPsFrame[i]) && (frameData[i + 1] == lastPsFrame[i + 1]));
                    }
                    if (hasStuckSegments)
                        continue; //Drop
                }

                //If this is not offset, but we have more than a few frames, add this as a new record
                if (!offset && rtBufferIndex >= 10)
                {
                    //Create and reset
                    string text = new string(rtBuffer, 0, rtBufferIndex);
                    rtBufferIndex = 0;

                    //Make sure we have the first frame before adding
                    if (hasFirstFrame)
                    {
                        //Add
                        newFrames.Add(new RdsValue<string>(time, text));

                        //Check if we already have a frame somewhere with the same text. If we do, remove all frames
                        //between that and the end, including the one we just added. This prevents error frames.
                        //BUT...have a maximum amount of time between each in case the same text (slogan, for example) is displayed multiple times
                        bool removing = false;
                        for (int i = 0; i < newFrames.Count; i++)
                        {
                            if (removing)
                            {
                                newFrames.RemoveAt(i);
                                i--;
                            }
                            removing = removing || (newFrames[i].value == text && (time - newFrames[i].first) < (60 * 6));
                        }
                    }
                    else
                    {
                        hasFirstFrame = true;
                    }
                }

                //If this is not offset, reset
                if (!offset)
                {
                    time = frame.first;
                    for (int i = 0; i < rtBuffer.Length; i++)
                        rtBuffer[i] = default(char);
                    rtBufferIndex = 0;
                }

                //Copy to RT buffer
                if (rtBufferIndex == 0)
                {
                    //First frame, just copy everything
                    for (int i = 0; i < 8; i++)
                        rtBuffer[rtBufferIndex++] = frameData[i];
                }
                else
                {
                    //Copy just the updated bits
                    for (int i = 0; i < matchingOffset && rtBufferIndex < rtBuffer.Length; i++)
                        rtBuffer[rtBufferIndex++] = frameData[(8 - matchingOffset) + i];
                }

                //Copy to the last ps buffer
                for (int i = 0; i < 8; i++)
                    lastPsFrame[i] = frameData[i];
            }

            //Add all
            rdsRtFrames.AddRange(newFrames);
        }
    }
}
