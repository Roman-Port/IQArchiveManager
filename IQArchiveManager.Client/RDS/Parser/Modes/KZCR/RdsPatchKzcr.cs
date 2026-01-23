using IQArchiveManager.Client.Components;
using IQArchiveManager.Client.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace IQArchiveManager.Client.RDS.Parser.Modes.KZCR
{
    public class RdsPatchKzcr : PsParsedPatch
    {
        private const int MERGE_MAX_SECONDS = 120;
        private const int MERGE_MAX_SAMPLES = MERGE_MAX_SECONDS * MainEditor.AUDIO_SAMPLE_RATE;

        private const int CURRENT_ALGORITHM_VERSION = 0;

        public RdsPatchKzcr(ClipDatabase db)
        {
            this.db = db;
            cache = db.GetPersistentStore("PATCHER_KZCR");
        }

        private readonly ClipDatabase db;
        private readonly JObject cache;

        public override string Label => "KZCR";

        public override RdsModeId Id => RdsModeId.KZCR;

        protected override long BadPsFrameConcealmentThreshold => MainEditor.AUDIO_SAMPLE_RATE; // samples

        public override bool IsRecommended(IRdsPatchContext ctx, List<RdsValue<string>> rdsPsFrames, List<RdsValue<string>> rdsRtFrames, List<RdsValue<ushort>> rdsPiFrames)
        {
            return KzcrCommon.Identify(rdsPiFrames, ctx.FileStartTime) == KzcrMatchMode.PS_SCROLL_NEW;
        }

        public override bool TryParse(string rt, out string trackTitle, out string trackArtist, out string stationName, bool fast)
        {
            //Check for any with multiple dashes. Many times we mis-interpret items with multiple dashes and keep the album when we don't mean to.
            //In the case we do find one with a caps word after, trim off everything til the caps
            //For example: "The Cars - MASTER DISC RECORDING Just What I Needed - The Cars"
            bool skipAlbum = false;
            if (rt.IndexOf('-') != -1 && rt.IndexOf('-') != rt.LastIndexOf('-'))
            {
                //Attempt to match the rexex
                Match m = twoDashCleanupRegex.Match(rt);
                if (m != null && m.Success && m.Value.Contains("MASTER"))
                {
                    //Remove everything before and including this regex
                    rt = rt.Substring(m.Index + m.Length);

                    //The album is no longer included, so skip it
                    skipAlbum = true;
                }
            }

            //Pull out data as usual
            if (!base.TryParse(rt, out trackTitle, out trackArtist, out stationName, fast))
                return false;

            //Extract album using online database
            if (!skipAlbum)
            {
                //Determine deliminer for album and title by looking it up. This creates a worker dialog
                int albumLength = MatchAlbum(trackTitle, trackArtist, fast);

                //Split title out if successful
                if (albumLength != -1)
                    trackTitle = trackTitle.Substring(albumLength).Trim();
            }

            return true;
        }

        /// <summary>
        /// This will find the deliminer in the "[album] [title]" string by searching, then saving it in the database.
        /// </summary>
        /// <param name="inText"></param>
        /// <param name="artist"></param>
        /// <param name="offlineOnly">If true, only the local cache is checked and we don't attempt to resolve online.</param>
        /// <returns></returns>
        private int MatchAlbum(string inText, string artist, bool offlineOnly)
        {
            //Create cache key
            string cacheKey = GetCacheKey(inText);

            //Attempt to read from the cache
            if (cache.TryGetValue(cacheKey, out JToken token) && token.Type == JTokenType.Integer)
            {
                //Break value into components
                DecodeCacheItem((int)token, out int offset, out bool valid, out int version);

                //Check if version matches or exceeds current version
                if (version >= CURRENT_ALGORITHM_VERSION)
                {
                    //Return the offset if valid, otherwise -1
                    if (valid)
                        return offset;
                    else
                        return -1;
                }
            }

            //Local cache unsuccessful; Request from online database if applicable
            if (!offlineOnly)
            {
                //Access online database
                int offset = inText.Length;
                WorkerDialog dialog = new WorkerDialog("Attempting to resolve title...", (IWorkerDialogControl ctx) =>
                {
                    while (true)
                    {
                        //Find next word index
                        int nextIndex = inText.Substring(0, offset).LastIndexOf(' ');
                        if (nextIndex == -1)
                        {
                            //Failed
                            offset = -1;
                            return DialogResult.No;
                        }

                        //Update
                        offset = nextIndex;

                        //Create query
                        string query = $"\"{inText.Substring(0, offset)}\" AND artist:\"{artist}\"";
                        ctx.UpdateStatusText($"Trying: {query}");

                        //Make request
                        int hits;
                        try
                        {
                            hits = MusicBrainzUtils.QueryCount("release-group", query);
                        }
                        catch
                        {
                            offset = -1;
                            return DialogResult.Cancel;
                        }

                        //If there were any hits, stop here!
                        if (hits > 0)
                            return DialogResult.Yes;
                    }
                });
                dialog.ShowDialog();

                //This will now be stored into the cache. Create value
                int cacheData;
                if (offset == -1)
                    cacheData = EncodeCacheItem(0, false, CURRENT_ALGORITHM_VERSION);
                else
                    cacheData = EncodeCacheItem(offset, true, CURRENT_ALGORITHM_VERSION);

                //Remove any old items matching the key from the cache
                cache.Remove(cacheKey);

                //Add this 
                cache.Add(cacheKey, cacheData);

                return offset;
            }

            //Offline only; Return failure
            return -1;
        }

        protected virtual bool TryParseToken(string token, out string title, out string artist)
        {
            //Search for "~ Album - "
            int albumIndex = token.IndexOf(" Album - ");

            //If album wasn't found, don't do any extra processing
            if (albumIndex == -1)
            {
                title = null;
                artist = null;
                return false;
            }

            //Extract parts
            artist = token.Substring(0, albumIndex);
            title = token.Substring(albumIndex + " Album - ".Length);
            //title = title.Substring(0, title.Length - 8);

            //If it has the date "(XXXX)" remove that from the artist to clean it up
            if (title.Length >= 6 && title[title.Length - 1] == ')' && title[title.Length - 6] == '(')
                title = title.Substring(0, title.Length - 6);

            return true;
        }

        /// <summary>
        /// Cleans corrupted output RT frames.
        /// </summary>
        /// <param name="newFrames"></param>
        protected virtual void CleanCorruptedFrames(List<RdsValue<string>> newFrames)
        {
            for (int i = 0; i < newFrames.Count; i++)
            {
                //Look ahead to all frames within a few minutes by checking the difference in sample timing to find the latest one equal to this
                int lastMatchingIndex = i;
                for (int t = i + 1; t < newFrames.Count && newFrames[t].first - newFrames[i].last <= MERGE_MAX_SAMPLES; t++)
                {
                    if (newFrames[t].value == newFrames[i].value)
                        lastMatchingIndex = t;
                }

                //Get new last sample
                long lastSample = newFrames[lastMatchingIndex].last;

                //Remove all up to this matched one, including it
                for (int d = i; d < lastMatchingIndex; d++)
                    newFrames.RemoveAt(i + 1);

                //Update last time of our own frame
                newFrames[i].last = lastSample;
            }
        }

        private static Regex twoDashCleanupRegex = new Regex("- ([A-Z]| )* ");

        protected override List<RdsValue<string>> Patch(List<RdsValue<string>> tokens)
        {
            //Split tokens by "by" since it's the only thing always there
            List<RdsValue<string>> groupedTokens = new List<RdsValue<string>>();
            string group = "";
            long groupStartSample = -1;
            for (int i = 1; i < tokens.Count; i++)
            {
                if (groupStartSample == -1)
                    groupStartSample = tokens[i].first;
                if (tokens[i].value == "by")
                {
                    if (group.Length != 0)
                        groupedTokens.Add(new RdsValue<string>(tokens[i].last, group.TrimEnd(' ')));
                    group = "";
                }
                else
                {
                    group += tokens[i].value + " ";
                }
            }

            //Shift things around to create the emulated RT frames
            List<RdsValue<string>> newFrames = new List<RdsValue<string>>();
            foreach (var t in groupedTokens)
            {
                if (TryParseToken(t.value, out string title, out string artist))
                {
                    newFrames.Add(new RdsValue<string>(t.first, t.last, title.Trim() + " - " + artist.Trim()));
                } else
                {
                    newFrames.Add(t);
                }
            }

            //Clean things up by taking out corrupted frames between identical ones that appear near each other
            CleanCorruptedFrames(newFrames);

            return newFrames;
        }

        /* CACHE UTILS */

        /// <summary>
        /// Convert RT to base64 because in theory there could be weird characters in there we don't want to write to the JSON file.
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        private static string GetCacheKey(string rt)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(rt.Trim()));
        }

        private static void DecodeCacheItem(int value, out int offset, out bool valid, out int version)
        {
            offset =  (value >> 0) & 0b111111;
            valid =  ((value >> 6) & 0b1) == 1;
            version = (value >> 7) & 0b11;
        }

        private static int EncodeCacheItem(int offset, bool valid, int version)
        {
            //Bounds check
            if (offset < 0 || offset > 0b111111)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (version < 0 || version > 0b11)
                throw new ArgumentOutOfRangeException(nameof(version));

            //Encode
            return ((offset & 0b111111) << 0) |
                ((valid ? 1 : 0) << 6) |
                ((version & 0b11) << 7);
        }
    }
}
