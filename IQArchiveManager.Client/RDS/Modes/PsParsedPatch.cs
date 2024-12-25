using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Modes
{
    /// <summary>
    /// Patcher for pulling parts from PS (say that ten times fast).
    /// </summary>
    public abstract class PsParsedPatch : BaseRdsMode
    {
        protected abstract List<RdsValue<string>> Patch(List<RdsValue<string>> tokens);

        private int FindOffset(char[] previous, char[] current)
        {
            int offset = -1;
            bool matching;
            do {
                //Increment
                offset++;

                //Loop through each character and make sure there's a match on the previous one, index offset by offset
                matching = true;
                for (int i = 0; matching && i < current.Length; i++)
                {
                    //Calculate previous index
                    int prevIndex = i + offset;

                    //If it is legal to check, look for a match
                    if (prevIndex >= 0 && prevIndex < previous.Length)
                        matching = matching && previous[prevIndex] == current[i];
                }
            } while (!matching);
            return offset;
        }

        public override List<RdsValue<string>> Patch(List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            //If it's shifted over by one character, it is a continuation. Otherwise, it's a new word and a space should be added:
            //Money
            //Talks~(1
            //alks~(19
            //lks~(199
            //ks~(1990
            //s~(1990)
            //~(1990)~
            //byAC/DC~
            //Album - 
            //The
            //Razors
            //Edge

            //First, find all tokens
            List<RdsValue<string>> tokens = new List<RdsValue<string>>();
            char[] lastPsFrame = new char[8];
            char[] tokenBuffer = new char[1024];
            int tokenIndex = 0;
            long tokenStartSample = 0;
            foreach (var frame in rdsPsFrames)
            {
                //Compare to last frame and check how much they're offset
                char[] frameData = frame.value.ToCharArray();
                int offsetBy = FindOffset(lastPsFrame, frameData);
                bool containsSpaces = frame.value.Contains(' ');
                bool offset = offsetBy <= 2 && !containsSpaces && tokenIndex >= 8;

                //If this is a match, ignore this frame
                if (offsetBy == 0)
                    continue;

                //If this wasn't a match, look for stuck segments. If stuck segments were found, drop the frame
                //if (!offset && PatchHelper.HelperDetectDroppedSegments(lastPsFrame, frameData, 1))
                //    continue;

                //if (frame.value.Trim() == "Welcome")
                //    Console.WriteLine();

                //If this was not offset then this must be a new token
                if (!offset)
                {
                    //Submit any token stored in the scrolling buffer
                    if (tokenIndex > 0)
                    {
                        tokens.Add(new RdsValue<string>(tokenStartSample, frame.last, new string(tokenBuffer, 0, tokenIndex)));
                        tokenIndex = -1;
                    }

                    //If this frame contains any spaces at all, it is not possibly a scrolling one
                    if (containsSpaces)
                    {
                        //Add each word
                        string[] words = frame.value.Split(' ');
                        foreach (var w in words)
                        {
                            if (w.Length > 0)
                                tokens.Add(new RdsValue<string>(frame.first, frame.last, w));
                        }
                    } else
                    {
                        //It is possible that this is an exactly eight character word. Start the new token buffer with it
                        frameData.CopyTo(tokenBuffer, 0);
                        tokenIndex = 8;
                        tokenStartSample = frame.first;
                    }
                } else
                {
                    //This has scrolled; copy just the updated characters to the buffer
                    for (int i = frameData.Length - offsetBy; i < frameData.Length && tokenIndex < tokenBuffer.Length; i++)
                        tokenBuffer[tokenIndex++] = frameData[i];
                }

                //Copy to the last ps buffer
                for (int i = 0; i < 8; i++)
                    lastPsFrame[i] = frameData[i];
            }

            return Patch(tokens);
        }
    }
}
