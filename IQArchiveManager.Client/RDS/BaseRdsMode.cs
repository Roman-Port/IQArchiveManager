using IQArchiveManager.Client.RDS.Modes;
using RomanPort.LibSDR.Components.Digital.RDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.RDS
{
    public abstract class BaseRdsMode
    {
        public abstract RdsModeId Id { get; }

        public abstract string Label { get; }

        public virtual bool HasSetupWindow => false;

        public virtual void ShowSetupWindow()
        {
            throw new NotSupportedException();
        }

        public virtual bool TryParse(RdsValue<string> rt, out string trackTitle, out string trackArtist, out string stationName, bool fast)
        {
            return TryParse(rt.value, out trackTitle, out trackArtist, out stationName, fast);
        }

        public virtual bool TryParse(string rt, out string trackTitle, out string trackArtist, out string stationName, bool fast)
        {
            //Trim whitespace
            rt = rt.Trim();

            //Trim off "Now playing" header if it exists
            if (rt.ToLower().StartsWith("now playing "))
                rt = rt.Substring("now playing ".Length);

            //Find deliminers
            int deliminerA = rt.IndexOfAny(out int deliminerAEnd, " - ", " by ", " By ", " BY ");
            int deliminerB = rt.LastIndexOfAny(out int deliminerBEnd, " - ", " on ", " On ", " ON ");

            //Determine from this state
            if (deliminerA != -1 && deliminerB != -1 && deliminerA == deliminerB)
            {
                //Only one deliminer...only use the first one
                trackTitle = rt.Substring(0, deliminerA);
                trackArtist = rt.Substring(deliminerAEnd);
                stationName = null;
                return true;
            }
            if (deliminerA != -1 && deliminerB != -1 && deliminerB > deliminerA)
            {
                //Two deliminers...contains title, artist, and station name
                trackTitle = rt.Substring(0, deliminerA);
                trackArtist = rt.Substring(deliminerAEnd, deliminerB - deliminerAEnd);
                stationName = rt.Substring(deliminerBEnd);
                return true;
            }

            //Failed
            trackTitle = null;
            trackArtist = null;
            stationName = null;
            return false;
        }

        public abstract bool IsRecommended(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames);
        public abstract List<RdsValue<string>> Patch(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames);
    }
}
