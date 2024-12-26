using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS
{
    class BasicRdsDecoder
    {
        public BasicRdsDecoder()
        {
            Reset();
        }

        private readonly List<RdsValue<string>> rdsRtFrames = new List<RdsValue<string>>();
        private readonly List<RdsValue<string>> rdsPsFrames = new List<RdsValue<string>>();
        private readonly List<RdsValue<ushort>> rdsPiFrames = new List<RdsValue<ushort>>();

        private RdsValue<string> lastFrameRt;
        private RdsValue<string> lastFramePs;
        private RdsValue<ushort> lastFramePi;

        public List<RdsValue<ushort>> PiFrames => new List<RdsValue<ushort>>(rdsPiFrames);
        public List<RdsValue<string>> PsFrames => new List<RdsValue<string>>(rdsPsFrames);
        public List<RdsValue<string>> RtFrames => new List<RdsValue<string>>(rdsRtFrames);

        /// <summary>
        /// Clears internal buffers
        /// </summary>
        public void Reset()
        {
            //Clear lists
            rdsRtFrames.Clear();
            rdsPsFrames.Clear();
            rdsPiFrames.Clear();

            //Clear last values
            lastFrameRt = null;
            lastFramePs = null;
            lastFramePi = null;

            //Reset decoder
            ResetDecoder();
        }

        private ushort lastPi;
        private char[] lastPs = new char[8];
        private string lastRt;

        private int regPsLastOffset = -1; // Last offset recieved
        private bool regPsInvalid = true; // Set if segments were recieved out of order
        private char[] regPs = new char[8];

        private int regRtAb = -1;
        private int regRtRecv; // Bitfield where every segment sets its bit when recieved
        private char[] regRt = new char[64];

        private void ResetDecoder()
        {
            //Clear lasts
            lastPi = 0;
            for (int i = 0; i < 8; i++)
                lastPs[i] = ' ';
            lastRt = null;

            //Reset components
            ResetPs();
            ResetRt();
        }

        public void ProcessFrame(long timestamp, ushort a, ushort b, ushort c, ushort d)
        {
            //Check if PI code changed
            if (a != lastPi)
            {
                lastPi = a;
                PushNewPi(timestamp, lastPi);
            }

            //Decode group type and version
            int groupType = (b >> 12) & 0xF;
            int groupVer = (b >> 11) & 1;

            //Switch on type
            switch (groupType)
            {
                case 0:
                    DecodePs(timestamp, b, c, d);
                    break;
                case 2:
                    DecodeRt(timestamp, groupVer, b, c, d);
                    break;
            }
        }

        private void ResetPs()
        {
            //Clear
            for (int i = 0; i < regPs.Length; i++)
                regPs[i] = ' ';

            //Reset state
            regPsLastOffset = -1;
            regPsInvalid = true;
        }

        private void DecodePs(long timestamp, ushort b, ushort c, ushort d)
        {
            //Decode info
            int offset = (b >> 0) & 0b11;
            int psOffset = offset * 2;

            //Get characers
            char c0 = (char)((d >> 8) & 0xFF);
            char c1 = (char)((d >> 0) & 0xFF);

            //Check state
            if (offset == 0 && regPsInvalid) // Recovery from corrupt frame OR start of new frame
            {
                //Reset
                regPsInvalid = false;
                regPsLastOffset = -1;
            }

            //Check state
            if (offset == regPsLastOffset) // Resending of same segment -- Check that it hasn't changed
            {
                //Check that the same thing has been recieved. If not, mark this entire frame as invalid
                regPsInvalid = regPsInvalid || regPs[psOffset] != c0 || regPs[psOffset + 1] != c1;
            } else if (offset == (regPsLastOffset + 1)) // Advanced to next segment -- Write it
            {
                //Write PS characters
                regPs[psOffset] = c0;
                regPs[psOffset + 1] = c1;
            } else if (offset > regPsLastOffset || offset < regPsLastOffset) // Advanced more than one segment at once or went backwards -- Invalid!
            {
                regPsInvalid = true;
            }

            //Check if ready for submission
            if (offset == 3 && !regPsInvalid)
            {
                //Check if has actually been updated
                if (!CompareBuffers(regPs, lastPs))
                    PushNewPs(timestamp);

                //Invalidate to prevent multiple pushes
                regPsInvalid = true;
            }

            //Update state
            regPsLastOffset = offset;
        }

        private void ResetRt()
        {
            //Clear
            for (int i = 0; i < regRt.Length; i++)
                regRt[i] = ' ';

            //Reset state
            regRtAb = -1;
            regRtRecv = 0;
        }

        private void DecodeRt(long timestamp, int version, ushort b, ushort c, ushort d)
        {
            //Decode info
            int abFlag = (b >> 4) & 1;
            int offset = (b >> 0) & 0xF;

            //Clear RT if the flag changes
            if (abFlag != regRtAb)
            {
                ResetRt();
                regRtAb = abFlag;
            }

            //Write state
            regRtRecv |= 1 << offset;

            //Write characters into the register
            int rtOffset;
            int rtMult;
            if (version == 0)
            {
                rtMult = 4;
                rtOffset = offset * rtMult;
                regRt[rtOffset] = (char)((c >> 8) & 0xFF);
                regRt[rtOffset + 1] = (char)((c >> 0) & 0xFF);
                regRt[rtOffset + 2] = (char)((d >> 8) & 0xFF);
                regRt[rtOffset + 3] = (char)((d >> 0) & 0xFF);
            }
            else
            {
                rtMult = 2;
                rtOffset = offset * rtMult;
                regRt[rtOffset] = (char)((d >> 8) & 0xFF);
                regRt[rtOffset + 1] = (char)((d >> 0) & 0xFF);
            }

            //Check and see if we've reached the end. First, check to see if this was the last segment
            int endIndex = -1;
            if (offset == 15)
                endIndex = (offset + 1) * rtMult;

            //Also check to see if we indicated the end using 0x0D as the spec mentions
            for (int i = rtOffset; i < rtOffset + 4; i++)
            {
                if (regRt[i] == (char)0x0D)
                {
                    endIndex = i;
                    break;
                }
            }

            //If the end was found, do some additional checks
            if (endIndex != -1)
            {
                //Check if the entire block was recieved by checking each bit
                int sentSegments = endIndex / rtMult;
                bool valid = true;
                for (int i = 0; i < sentSegments; i++)
                    valid = valid && ((regRtRecv >> i) & 1) == 1;

                //Wrap into string
                string rtOut = new string(regRt, 0, endIndex);

                //Check if it has changed and valid and push in
                if (valid && lastRt != rtOut)
                    PushNewRt(timestamp, rtOut);
            }
        }

        private void PushNewRt(long timestamp, string value)
        {
            //Terminate old
            if (lastFrameRt != null)
                lastFrameRt.last = timestamp;

            //Copy to register
            lastRt = value;

            //Create new
            lastFrameRt = new RdsValue<string>(timestamp, value);
            rdsRtFrames.Add(lastFrameRt);
        }

        private void PushNewPs(long timestamp)
        {
            //Terminate old
            if (lastFramePs != null)
                lastFramePs.last = timestamp;

            //Copy to register
            Array.Copy(regPs, lastPs, regPs.Length);

            //Create new
            lastFramePs = new RdsValue<string>(timestamp, new string(regPs));
            rdsPsFrames.Add(lastFramePs);
        }

        private void PushNewPi(long timestamp, ushort value)
        {
            //Terminate old
            if (lastFramePi != null)
                lastFramePi.last = timestamp;

            //Create new
            lastFramePi = new RdsValue<ushort>(timestamp, value);
            rdsPiFrames.Add(lastFramePi);
        }

        private static bool CompareBuffers(char[] a, char[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }
    }
}
