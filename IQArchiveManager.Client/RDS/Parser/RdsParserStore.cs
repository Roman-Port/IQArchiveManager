using IQArchiveManager.Client.RDS.Parser.Modes.Csv;
using IQArchiveManager.Client.RDS.Parser.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IQArchiveManager.Client.RDS.Parser.Modes.KZCR;

namespace IQArchiveManager.Client.RDS.Parser
{
    public class RdsParserStore
    {
        public RdsParserStore(ClipDatabase db)
        {
            modes = CreateModes(db);
        }

        /// <summary>
        /// Create modes (ADD NEW MODES HERE!)
        /// </summary>
        /// <returns></returns>
        private static BaseRdsMode[] CreateModes(ClipDatabase db)
        {
            return new BaseRdsMode[]
            {
                new RdsPatchNative(),
                new RdsPatchNativeFlipped(),
                new RdsPatchKzcrSponsors(),
                new RdsPatchCsv(db),
                new RdsPatchKzcr(db),
                new RdsPatchKzcrLegacy(db),
                new RdsPatchCumulus(),
                new RdsPatchNoDelimiters(db)
            };
        }

        private BaseRdsMode[] modes;

        public BaseRdsMode[] Modes => modes;
    }
}
