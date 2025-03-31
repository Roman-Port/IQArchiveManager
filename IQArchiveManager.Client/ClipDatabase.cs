using IQArchiveManager.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client
{
    public class ClipDatabase
    {
        public ClipDatabase(string filename = null)
        {
            this.filename = filename;
            if (File.Exists(filename))
                db = ReadDatabase(File.ReadAllText(filename));
            else
                db = new DbRoot();
        }

        private string filename;
        private DbRoot db;

        public string DatabaseFilename => filename;
        public List<TrackClipInfo> Clips => db.Clips;
        public Dictionary<string, JObject> PersistentData => db.Persistent;
        public IQEnviornment Enviornment => db.Enviornment;
        public DateTime CreationDate
        {
            get
            {
                //If it is set, return it. Otherwise calculate it (for old instances)
                if (db.CreatedAt == DateTime.MinValue)
                    CalculateCreatedAt();

                return db.CreatedAt;
            }
        }

        private static DbRoot ReadDatabase(string json)
        {
            //Determine if migration is required (12/8/2024)
            if (json.StartsWith("["))
            {
                //Migration from old clips database is required. Read it as an array...
                List<TrackClipInfo> clips = JsonConvert.DeserializeObject<List<TrackClipInfo>>(json);

                //Wrap into new object
                Console.WriteLine("Performed clips DB migration.");
                return new DbRoot
                {
                    Clips = clips
                };
            } else if (json.StartsWith("{"))
            {
                //Load as normal
                return JsonConvert.DeserializeObject<DbRoot>(json);
            } else
            {
                throw new Exception("Invalid JSON.");
            }
        }

        public int GetStationIndex(string call)
        {
            List<string> calls = new List<string>();
            foreach(var c in Clips)
            {
                if (c.Station == call)
                    return calls.Count;
                if (!calls.Contains(c.Station))
                    calls.Add(c.Station);
            }
            return calls.Count;
        }

        public string GetNewId(string stationCall, DateTime clipTime)
        {
            //Define charset to use
            char[] charset = "0123456789ABCDEF".ToCharArray();

            //Find station call index from other clips
            int callIndex = GetStationIndex(stationCall);

            //Create buffer and set constant parts
            char[] idBuf = new char[8];
            idBuf[0] = charset[callIndex >= charset.Length ? (charset.Length - 1) : callIndex];
            idBuf[1] = charset[clipTime.Year % 10];
            idBuf[2] = charset[clipTime.Month];
            idBuf[3] = charset[clipTime.Day / 2];

            //Generate unique part and check
            Random rand = new Random();
            string id;
            do
            {
                for (int i = 4; i < idBuf.Length; i++)
                    idBuf[i] = charset[rand.Next(0, idBuf.Length)];
                id = new string(idBuf);
            } while (Clips.Where(x => x.Id == id).Count() > 0);

            return id;
        }

        public void AddClip(TrackClipInfo clip)
        {
            Clips.Add(clip);
            Save();
        }

        /// <summary>
        /// Gets a persistent store from the db that can be used to store data.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public JObject GetPersistentStore(string key)
        {
            //Create if it doesn't exist
            if (!db.Persistent.ContainsKey(key))
                db.Persistent.Add(key, new JObject());

            return db.Persistent[key];
        }

        public void Save()
        {
            //We shouldn't ever call this if no save is loaded but confirm this is the case
            if (filename == null)
                throw new Exception("Attempted to save when no filename is yet set.");

            //Delete backup
            string backupFilename = filename + ".bak";
            if (File.Exists(backupFilename))
                File.Delete(backupFilename);

            //Move existing to backup
            if (File.Exists(filename))
                File.Move(filename, backupFilename);

            //Write
            File.WriteAllText(filename, JsonConvert.SerializeObject(db));
        }

        /// <summary>
        /// Calculates a new created at time by finding the earliest clip.
        /// Older databases didn't have this set.
        /// </summary>
        private void CalculateCreatedAt()
        {
            //Abort if there are no clips.
            if (Clips.Count == 0)
                return;

            //Find oldest
            DateTime oldest = DateTime.MaxValue;
            foreach (var c in Clips)
                oldest = new DateTime(Math.Min(oldest.Ticks, c.Time.Ticks));

            //Set
            db.CreatedAt = oldest;
        }

        /// <summary>
        /// Estimates the size of all original (unedited) files. This may take a moment!
        /// </summary>
        /// <returns></returns>
        public long CalculateOriginalFileSizeTotal()
        {
            //Find all UNIQUE original filesizes. This is why it's an estimate.
            //If an original file happened to share a size with another (very very unlikely) it would only be counted once.
            List<long> uniqueSizes = new List<long>();
            long totalSize = 0;
            foreach (var c in Clips)
            {
                //Check if a source file of this size has been counted yet
                if (uniqueSizes.Contains(c.OriginalFileSize))
                    continue;

                //Unique, add to total and list
                uniqueSizes.Add(c.OriginalFileSize);
                totalSize += c.OriginalFileSize;
            }

            return totalSize;
        }

        class DbRoot
        {
            [JsonProperty("clips")]
            public List<TrackClipInfo> Clips { get; set; } = new List<TrackClipInfo>();

            [JsonProperty("persistent")]
            public Dictionary<string, JObject> Persistent { get; set; } = new Dictionary<string, JObject>();

            [JsonProperty("enviornment")]
            public IQEnviornment Enviornment { get; set; } = new IQEnviornment();

            [JsonProperty("created_at")]
            public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        }
    }
}
