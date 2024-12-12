using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.Db
{
    public class EnviornmentPath
    {
        [JsonProperty("selected_index")]
        public int SelectedIndex { get; set; } = -1;
        
        [JsonProperty("paths")]
        public List<string> Paths { get; set; } = new List<string>();

        [JsonIgnore]
        public string SelectedValue => Paths[SelectedIndex];

        [JsonIgnore]
        public bool IsValid => SelectedIndex >= 0 && SelectedIndex < Paths.Count;
    }
}
