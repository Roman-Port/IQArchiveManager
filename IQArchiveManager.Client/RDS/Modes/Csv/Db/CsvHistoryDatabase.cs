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

                //Sort list
                items.Sort((x, y) => x.Time.CompareTo(y.Time));

                //Patch
                RemoveDuplicates();
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
        public List<CsvPlaybackItem> Items => items;

        /// <summary>
        /// Removes duplicate items. This works around a bug in my extractor program that sometimes had items from the previous day in the log.
        /// This assumes the list is sorted.
        /// </summary>
        private void RemoveDuplicates()
        {
            int i = 0;
            while (i < (items.Count - 1))
            {
                //Compare this to the next item
                if (items[i].Equals(items[i + 1]))
                {
                    items.RemoveAt(i);
                } else
                {
                    i++;
                }
            }
        }
    }
}
