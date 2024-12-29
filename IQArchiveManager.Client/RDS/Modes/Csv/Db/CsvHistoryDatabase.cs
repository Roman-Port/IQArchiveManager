using Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Modes.Csv.Db
{
    /// <summary>
    /// Manages items in a station's history, loaded from CSV
    /// </summary>
    class CsvHistoryDatabase
    {
        private List<CsvPlaybackItem> items = new List<CsvPlaybackItem>();
        private List<string> loadedFiles = new List<string>();
        private bool isSortUpdated = false;

        /// <summary>
        /// Loads a day from CSV in. If it's already loaded, this does nothing. If it's already determined to not exist, this does nothing.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public void LoadPage(CsvProfile profile, int year, int month, int day)
        {
            //Resolve to filename
            string filename = RdsPatchCsv.ResolveTemplate(profile.PathTemplate, year, month, day);

            //Abort out if it's already loaded
            if (loadedFiles.Contains(filename))
                return;

            //Check if the file exists
            if (File.Exists(filename))
            {
                //Load file
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    items.AddRange(CsvReader.Read(sr).Select(x => new CsvPlaybackItem(x.Values, profile)));
                }

                //Mark the list as unsorted
                isSortUpdated = false;
            }

            //Add to loaded files
            loadedFiles.Add(filename);
        }

        /// <summary>
        /// Loads all days between the inclusive start and inclusive end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void LoadPages(CsvProfile profile, DateTime start, DateTime end)
        {
            DateTime target = start;
            while (target <= end.AddDays(1))
            {
                LoadPage(profile, target.Year, target.Month, target.Day);
                target = target.AddDays(1);
            }
        }

        /// <summary>
        /// Sorted list of items.
        /// </summary>
        public List<CsvPlaybackItem> Items
        {
            get
            {
                //Sort if marked as unsorted
                if (!isSortUpdated)
                {
                    items.Sort((x, y) => x.Time.CompareTo(y.Time));
                    isSortUpdated = true;
                }

                return items;
            }
        }
    }
}
