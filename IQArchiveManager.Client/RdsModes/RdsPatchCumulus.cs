using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RdsModes
{
    public class RdsPatchCumulus : BaseRdsMode
    {
        public override string Label => "\"2-char\" PS";

        public override RdsModeId Id => RdsModeId.TWO_CHAR;

        public override bool IsRecommended(List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            //If there are no RT frames, use the Cumulus method
            return rdsPsFrames.Count != 0 && rdsRtFrames.Count == 0;
        }

        public override List<RdsValue<string>> Patch(List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
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
                bool offset = PatchHelper.HelperCheckOffset(lastPsFrame, frameData, 2, out int matchingOffset);
                bool matches = matchingOffset == 0;

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

            return newFrames;
        }
    }
}
