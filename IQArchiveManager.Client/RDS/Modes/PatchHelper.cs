using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Modes
{
    public static class PatchHelper
    {
        public static int IndexOfAny(this string target, out int end, params string[] values)
        {
            int index = -1;
            end = -1;
            foreach (var v in values)
            {
                int newIndex = target.IndexOf(v);
                if (newIndex > index)
                {
                    index = newIndex;
                    end = index + v.Length;
                }
            }
            return index;
        }

        public static int LastIndexOfAny(this string target, out int end, params string[] values)
        {
            int index = -1;
            end = -1;
            foreach (var v in values)
            {
                int newIndex = target.LastIndexOf(v);
                if (newIndex > index)
                {
                    index = newIndex;
                    end = index + v.Length;
                }
            }
            return index;
        }

        public static bool HelperCheckOffset(char[] lastPsFrame, char[] frameData, int increment, out int matchingOffset)
        {
            matchingOffset = 0;
            while (matchingOffset < 8)
            {
                bool check = true;
                for (int i = 0; i < 8 - matchingOffset; i++)
                {
                    check = (frameData[i] == lastPsFrame[i + matchingOffset]) && check;
                }
                if (check)
                    return true;
                matchingOffset += increment;
            }
            return false;
        }

        /// <summary>
        /// Call if HelperCheckOffset failed. Looks for segments that didn't get recieved correctly and chooses if the frame should be dropped. Returns true if the frame should be dropped, otherwise false if it really was a new segment.
        /// </summary>
        /// <param name="lastPsFrame"></param>
        /// <param name="frameData"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static bool HelperDetectDroppedSegments(char[] lastPsFrame, char[] frameData, int increment)
        {
            bool hasStuckSegments = false;
            for (int i = 0; i < 7; i += increment)
            {
                hasStuckSegments = hasStuckSegments || ((frameData[i] == lastPsFrame[i]) && (frameData[i + 1] == lastPsFrame[i + 1]));
            }
            return hasStuckSegments;
        }
    }
}
