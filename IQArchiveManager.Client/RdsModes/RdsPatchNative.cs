using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RdsModes
{
    public class RdsPatchNative : BaseRdsMode
    {
        public override string Label => "Native";

        public override RdsModeId Id => RdsModeId.NATIVE;

        public override bool IsRecommended(List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            return true;
        }

        public override List<RdsValue<string>> Patch(List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            return rdsRtFrames;
        }
    }
}
