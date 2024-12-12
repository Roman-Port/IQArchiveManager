using IQArchiveManager.Client.Db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client
{
    public class IQEnviornment
    {
        // Here for old code

        [JsonIgnore]
        public string IqaDir => IqaDirs.SelectedValue;

        [JsonIgnore]
        public string EditDir => EditDirs.SelectedValue;

        [JsonIgnore]
        public string MoveDir => MoveDirs.SelectedValue;

        //

        [JsonProperty("iqa")]
        public EnviornmentPath IqaDirs { get; set; } = new EnviornmentPath();

        [JsonProperty("edit")]
        public EnviornmentPath EditDirs { get; set; } = new EnviornmentPath();

        [JsonProperty("move")]
        public EnviornmentPath MoveDirs { get; set; } = new EnviornmentPath();

        // Util

        public bool GetIqFileById(string id, out string filename)
        {
            filename = IqaDir + id + ".iqa";
            return File.Exists(filename);
        }

        public void FindIqaFiles(List<string> files)
        {
            string[] names = Directory.GetFiles(IqaDir);
            foreach(var n in names)
            {
                if (n.EndsWith(".iqa"))
                    files.Add(n);
            }
        }
    }
}
