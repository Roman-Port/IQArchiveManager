using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Parser.Modes.KZCR
{
    public class RdsPatchKzcrSponsors : RdsPatchNative
    {
        private const string SONG_HEADER = "Z103 - ";
        private const string SPONSOR = "There is a lot to love at R&G Subaru online at RGSUBARU.COM";
        private const double SPONSOR_LEN_SECS = 65;
        private const long LAG_BACK_ADJUST = 30 * MainEditor.AUDIO_SAMPLE_RATE;
        private const long LAG_FORWARD_ADJUST = 25 * MainEditor.AUDIO_SAMPLE_RATE;

        public override string Label => "KZCR w/ sponsors";
        public override RdsModeId Id => RdsModeId.KZCR_2025;

        public override List<RdsValue<string>> Patch(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            //Remove any sponsors
            List<RdsValue<string>> result = new List<RdsValue<string>>();
            RdsValue<string> lastSong = null;
            foreach (var f in rdsRtFrames)
            {
                //Check if this is a sponsor or a song
                if (!f.value.StartsWith(SPONSOR))
                {
                    //Check if this is returning to the same song, in which case it will be extended
                    if (lastSong != null && lastSong.value == f.value)
                    {
                        //Extend last song
                        lastSong.last = f.last;
                    } else
                    {
                        //New song; Add to list
                        lastSong = f.Clone();
                        lastSong.first = Math.Max(0, f.first - LAG_BACK_ADJUST);
                        lastSong.last += LAG_FORWARD_ADJUST;
                        result.Add(lastSong);
                    }

                    //Bail out and do not process sponsors
                    continue;
                }

                //If the sponsor length is <= 30 seconds, remove it
                double sponsorLen = (double)(f.last - f.first) / MainEditor.AUDIO_SAMPLE_RATE;
                if (sponsorLen <= SPONSOR_LEN_SECS && lastSong != null)
                {
                    //Extend the last song up to this
                    lastSong.last = f.last;
                } else
                {
                    //Insert this
                    result.Add(f.Clone());
                }
            }

            return result;
        }

        public override bool IsRecommended(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            return KzcrCommon.Identify(rdsPiFrames, ctx.FileStartTime) == KzcrMatchMode.RT_SPONSORED;
        }

        public override bool TryParse(RdsValue<string> rt, out string trackTitle, out string trackArtist, out string stationName, bool fast)
        {
            //Clear
            trackTitle = null;
            trackArtist = null;
            stationName = null;

            //Try to find the "Z103 - " leading text
            if (!rt.value.StartsWith(SONG_HEADER))
                return false; // fail

            //Get the title/artist component
            string info = rt.value.Substring(SONG_HEADER.Length);

            //Find the split
            int split = info.IndexOf(" - ");
            if (split == -1)
                return false;

            //Split out the title and artist
            trackTitle = info.Substring(0, split);
            trackArtist = info.Substring(split + 3);

            //There should be a year in the title; If so, remove it
            int yearIndex = trackTitle.LastIndexOf('(');
            if (yearIndex != -1)
                trackTitle = trackTitle.Substring(0, yearIndex);

            return true;
        }
    }
}
