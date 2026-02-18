using IQArchiveManager.Client.RDS.Parser;
using RomanPort.LibSDR.Components.Digital.RDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.RDS.Parser
{
    public abstract class BaseRdsMode
    {
        /// <summary>
        /// Event raised when this parser requests the editor to reload.
        /// </summary>
        public event Action<BaseRdsMode> RefreshRequested;

        public abstract RdsModeId Id { get; }

        public abstract string Label { get; }

        public virtual bool HasSetupWindow => false;

        public virtual void ShowSetupWindow()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// An optional function called when the editor is first created
        /// </summary>
        /// <param name="editor"></param>
        public virtual void EditorInitialized(MainEditor editor)
        {

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

            //Split into segments
            string[] segments = new string[3];
            if (deliminerA != -1 && deliminerB != -1 && deliminerA == deliminerB)
            {
                //Only one deliminer...only use the first one
                segments[0] = rt.Substring(0, deliminerA);
                segments[1] = rt.Substring(deliminerAEnd);
                segments[2] = null;
            }
            else if (deliminerA != -1 && deliminerB != -1 && deliminerB > deliminerA)
            {
                //Two deliminers...contains title, artist, and station name
                segments[0] = rt.Substring(0, deliminerA);
                segments[1] = rt.Substring(deliminerAEnd, deliminerB - deliminerAEnd);
                segments[2] = rt.Substring(deliminerBEnd);
            }
            else
            {
                //Failed
                trackTitle = null;
                trackArtist = null;
                stationName = null;
                return false;
            }

            //Decode segments
            SelectFromSegments(segments, out trackTitle, out trackArtist, out stationName);
            return true;
        }

        /// <summary>
        /// Splits three segments into title, artist and station name
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="title"></param>
        /// <param name="artist"></param>
        /// <param name="stationName"></param>
        protected virtual void SelectFromSegments(string[] seg, out string title, out string artist, out string stationName)
        {
            //HACK: Flip for specific station...this should probably be made into a decoding mode
            if (seg[0] == "KX92")
            {
                //Decode as <station> - <title> - <artist>
                title = seg[1];
                artist = seg[2];
                stationName = seg[0];
                return;
            }

            //Decode
            title = seg[0];
            artist = seg[1];
            stationName = seg[2];
        }

        public abstract bool IsRecommended(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames);
        public abstract List<RdsValue<string>> Patch(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames);

        /// <summary>
        /// Requests the editor to reload parsed frames.
        /// </summary>
        protected void RequestReload()
        {
            RefreshRequested?.Invoke(this);
        }
    }
}
