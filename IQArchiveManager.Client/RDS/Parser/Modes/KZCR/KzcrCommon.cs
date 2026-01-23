using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Parser.Modes.KZCR
{
    static class KzcrCommon
    {
        private static readonly DateTime DATE_NEW_PS = new DateTime(2023, 1, 1);
        private static readonly DateTime DATE_RT_SPONSORS = new DateTime(2025, 10, 20);

        /// <summary>
        /// Scans for PI frames matching KZCR
        /// </summary>
        /// <param name="rdsPiFrames"></param>
        /// <returns></returns>
        public static bool MatchPi(List<RdsValue<ushort>> rdsPiFrames)
        {
            //Search for KZCR-FM's PI code. If it's found, use the KZCR method
            foreach (var pi in rdsPiFrames)
            {
                if (pi.value == 0x5249)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Uses the date to identify the suggested match node
        /// </summary>
        /// <param name="rdsPiFrames"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static KzcrMatchMode Identify(List<RdsValue<ushort>> rdsPiFrames, DateTime date)
        {
            //Check if this even matches the station
            if (!MatchPi(rdsPiFrames))
                return KzcrMatchMode.NONE;

            //Match the date
            if (date < DATE_NEW_PS)
                return KzcrMatchMode.PS_SCROLL_TILDE;
            if (date < DATE_RT_SPONSORS)
                return KzcrMatchMode.PS_SCROLL_NEW;
            return KzcrMatchMode.RT_SPONSORED;
        }
    }
}
