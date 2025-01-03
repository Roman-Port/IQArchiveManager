using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IQArchiveManager.Common.IO.Editor.Post
{
    /// <summary>
    /// JSON class written to the post-editor file.
    /// </summary>
    public class PostEditFile
    {
        [JsonProperty("last_edited")]
        public DateTime LastEdited { get; set; }

        [JsonProperty("editor_version")]
        public int EditorVersion { get; set; }

        [JsonProperty("delete")]
        public bool Delete { get; set; }

        [JsonProperty("edits")]
        public List<TrackEditInfo> Edits { get; set; }
    }
}
