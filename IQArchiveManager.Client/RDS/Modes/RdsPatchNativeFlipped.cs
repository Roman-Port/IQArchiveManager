using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Modes
{
    public class RdsPatchNativeFlipped : RdsPatchNative
    {
        public override string Label => base.Label + " (flipped)";
        public override RdsModeId Id => RdsModeId.NATIVE_FLIPPED;

        public override bool IsRecommended(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            return false;
        }

        public override bool TryParse(RdsValue<string> rt, out string trackTitle, out string trackArtist, out string stationName, bool fast)
        {
            return base.TryParse(rt, out trackArtist, out trackTitle, out stationName, fast); // Intentionally flipped
        }
    }
}
