using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IQArchiveManager.Client.Util
{
    class TuneGenieCache
    {
        public TuneGenieCache(DateTime recordingStart, DateTime recordingEnd)
        {
            this.recordingStart = recordingStart;
            this.recordingEnd = recordingEnd;
        }

        private DateTime recordingStart;
        private DateTime recordingEnd;

        private Dictionary<string, TuneGenieItem[]> cache = new Dictionary<string, TuneGenieItem[]>();

        private static string GetUrlEncodedTime(DateTime time)
        {
            return HttpUtility.UrlEncode(JsonConvert.SerializeObject(time.ToUniversalTime()).Trim('"'));
        }

        private bool TryLoadOrFetchCacheBlock(string stationCall, out TuneGenieItem[] items)
        {
            //Check if it already exists
            if (cache.TryGetValue(stationCall, out items))
                return true;

            //Build URL
            string url = $"https://api.tunegenie.com/v2/brand/nowplaying/?apiid=m2g_bar&b={HttpUtility.UrlEncode(stationCall)}&since={GetUrlEncodedTime(recordingStart)}&until={GetUrlEncodedTime(recordingEnd)}";

            //Fetch from HTTP
            string body;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                body = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch
            {
                return false;
            }

            //Parse data
            items = JsonConvert.DeserializeObject<TuneGenieItem[]>(body);

            //Compute the estimated time of the song (working from the latest to the oldest)
            DateTime? nextItem = null;
            foreach (var i in items)
            {
                //Set end time to start time of next item, if set
                if (nextItem.HasValue)
                    i.EstimatedEndTime = nextItem.Value;
                else
                    i.EstimatedEndTime = i.PlayedAt;

                //Update register
                nextItem = i.PlayedAt;
            }

            //Add to cache
            cache.Add(stationCall, items);

            return true;
        }

        public bool TryFetchItem(string stationCall, DateTime time, out ITuneGenieItem item)
        {
            //Download or retrieve from cache
            item = null;
            TuneGenieItem[] cache;
            if (!TryLoadOrFetchCacheBlock(stationCall, out cache))
                return false;

            //Search for an item within this time
            foreach (var i in cache)
            {
                if (time >= i.PlayedAt && time < i.EstimatedEndTime)
                {
                    item = i;
                    return true;
                }
            }

            return false;
        }

        class TuneGenieItem : ITuneGenieItem
        {
            [JsonProperty("song")]
            public string Name { get; set; }

            [JsonProperty("artist")]
            public string Artist { get; set; }

            [JsonProperty("played_at")]
            public DateTime PlayedAt { get; set; }

            [JsonIgnore]
            public DateTime EstimatedEndTime { get; set; }
        }
    }
}
