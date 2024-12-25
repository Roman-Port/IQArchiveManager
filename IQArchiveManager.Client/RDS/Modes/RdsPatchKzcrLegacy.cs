using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Modes
{
    public class RdsPatchKzcrLegacy : RdsPatchKzcr
    {
        public RdsPatchKzcrLegacy(ClipDatabase db) : base(db)
        {
        }

        public override string Label => base.Label + " (legacy)";

        public override RdsModeId Id => RdsModeId.KZCR_LEGACY;

        public override bool IsRecommended(List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            return false;
        }

        protected override List<RdsValue<string>> Patch(List<RdsValue<string>> tokens)
        {
            //Do some special cleanup. Find tokens beginning with "by" and split them. We do this because the "by" is combined
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].value.Length >= 3 && tokens[i].value.StartsWith("by"))
                {
                    string next = tokens[i].value.Substring(2);
                    long sample = tokens[i].last;
                    tokens.RemoveAt(i);
                    tokens.Insert(i, new RdsValue<string>(sample, next));
                    tokens.Insert(i, new RdsValue<string>(sample, "by"));
                }
            }

            return base.Patch(tokens);
        }

        protected override bool TryParseToken(string token, out string title, out string artist)
        {
            //Search for "~ Album - "
            int albumIndex = token.IndexOf("~ Album - ");

            //Also, search for the ending date "~(XXXX)~"
            bool hasDate = token.Length >= 8 && token[token.Length - 1] == '~' && token[token.Length - 2] == ')' && token[token.Length - 7] == '(' && token[token.Length - 8] == '~';

            //If these criteria are not matched, don't do any extra processing
            if (albumIndex == -1 || !hasDate)
            {
                title = null;
                artist = null;
                return false;
            }

            //Extract parts
            artist = token.Substring(0, albumIndex);
            title = token.Substring(albumIndex + "~ Album - ".Length);
            title = title.Substring(0, title.Length - 8);
            return true;
        }
    }
}
