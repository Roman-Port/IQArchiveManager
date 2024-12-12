using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Common.IO.IqaReader
{
    public static class IqaInfoReader
    {
        public static TrackClipInfo ReadInfo(IqaFileReader reader)
        {
            //Create the object we'll be writing into
            JObject payload = new JObject();

            //Read the normal INFO segment
            using (IqaSegmentReader segment = reader.OpenSegment(IqaDefines.IQA_TAG_INFO))
                MergeSegment(segment, payload);

            //Find and merge all "extended" info segments
            int count = 0;
            while (true)
            {
                using (IqaSegmentReader segment = reader.OpenSegment(IqaDefines.IQA_TAG_EXTENDED_INFO, count))
                {
                    //Exit if this is the last one
                    if (segment == null)
                        break;

                    //Skip the 8 byte "previous date modified" field
                    segment.Position += 8;

                    //Read
                    MergeSegment(segment, payload);

                    //Advance index
                    count++;
                }
            }

            //Convert type
            return payload.ToObject<TrackClipInfo>();
        }

        private static void MergeSegment(IqaSegmentReader segment, JObject payload)
        {
            //Read the incoming data
            JObject incoming = JsonConvert.DeserializeObject<JObject>(segment.ReadAsText());

            //Merge
            foreach (var p in incoming)
                payload[p.Key] = p.Value;
        }
    }
}
