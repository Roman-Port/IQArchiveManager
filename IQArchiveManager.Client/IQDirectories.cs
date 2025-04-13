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
    public class IQDirectories
    {
        [JsonProperty("iqa")]
        public List<string> IqaDirs { get; set; } = new List<string>();

        [JsonProperty("edit")]
        public List<string> EditDirs { get; set; } = new List<string>();

        // Util

        /// <summary>
        /// Searches all paths for the IQA file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool GetIqFileById(string id, out string filename)
        {
            filename = "";
            foreach (var d in IqaDirs)
            {
                filename = d + Path.DirectorySeparatorChar + id + ".iqa";
                if (File.Exists(filename))
                    return true;
            }
            return false;
        }
    }
}
