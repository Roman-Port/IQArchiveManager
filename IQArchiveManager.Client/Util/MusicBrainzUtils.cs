using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IQArchiveManager.Client.Util
{
    static class MusicBrainzUtils
    {
        public static int QueryCount(string entityType, string query)
        {
            //Request
            string url = $"https://musicbrainz.org/ws/2/{HttpUtility.UrlEncode(entityType)}?query={HttpUtility.UrlEncode(query)}&limit=15&fmt=json";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "RomanPort.IQArchiveManager.KzcrPatcher/1.0";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            string responseRaw;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                    responseRaw = reader.ReadToEnd();
            } catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (Stream stream = wex.Response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                        responseRaw = reader.ReadToEnd();
                } else
                {
                    throw new Exception("No response", wex);
                }
            }

            //Parse
            QueryResponse responseData = JsonConvert.DeserializeObject<QueryResponse>(responseRaw);
            return responseData.Count;
        }

        class QueryResponse
        {
            [JsonProperty("created")]
            public DateTime Created { get; set; }
            [JsonProperty("count")]
            public int Count { get; set; }
            [JsonProperty("offset")]
            public int Offset { get; set; }
        }
    }
}
