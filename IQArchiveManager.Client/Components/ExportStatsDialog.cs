using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.Components
{
    public partial class ExportStatsDialog : Form
    {
        public ExportStatsDialog(ClipDatabase db)
        {
            this.db = db;
            InitializeComponent();
        }

        private ClipDatabase db;

        private static DateTime EPOCH = new DateTime(2010, 1, 1);

        private void btnExport_Click(object sender, EventArgs e)
        {
            //Prompt for a file location first
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "CSV Files (*.csv)|*.csv";
            if (fd.ShowDialog() != DialogResult.OK)
                return;

            //Get settings
            TimeSpan binDuration = TimeSpan.FromDays((int)daysPerLine.Value);
            int minPerStation = (int)minRecordingsPerStation.Value;

            //Split into bins
            Dictionary<string, int> totalCount = new Dictionary<string, int>();
            Dictionary<long, Dictionary<string, int>> bins = CreateBins(binDuration, totalCount, out long minBin, out long maxBin);

            //Create station filter
            List<string> stationFilter = new List<string>();
            foreach (var s in totalCount)
            {
                if (s.Value >= minPerStation)
                    stationFilter.Add(s.Key);
            }

            //Export
            ExportBins(fd.FileName, bins, stationFilter, binDuration, optionIncludeOther.Checked, minBin, maxBin);
        }

        private Dictionary<long, Dictionary<string, int>> CreateBins(TimeSpan binDuration, Dictionary<string, int> totalCount, out long minBin, out long maxBin)
        {
            Dictionary<long, Dictionary<string, int>> result = new Dictionary<long, Dictionary<string, int>>();
            minBin = long.MaxValue;
            maxBin = long.MinValue;
            foreach (var i in db.Clips)
            {
                //Calculate bin index and update stats
                long index = (i.Time - EPOCH).Ticks / binDuration.Ticks;
                minBin = Math.Min(minBin, index);
                maxBin = Math.Max(maxBin, index);

                //Create or get bin
                Dictionary<string, int> bin;
                if (!result.TryGetValue(index, out bin))
                {
                    bin = new Dictionary<string, int>();
                    result.Add(index, bin);
                }

                //Create or get station
                if (bin.ContainsKey(i.Station))
                    bin[i.Station]++;
                else
                    bin.Add(i.Station, 1);

                //Create or get total count station
                if (totalCount.ContainsKey(i.Station))
                    totalCount[i.Station]++;
                else
                    totalCount.Add(i.Station, 1);
            }
            return result;
        }

        private void ExportBins(string filename, Dictionary<long, Dictionary<string, int>> bins, List<string> stationFilter, TimeSpan binDuration, bool createOther, long firstBin, long lastBin)
        {
            //Open writer
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                //Create and write header
                List<string> line = new List<string>();
                line.Add("DATE");
                line.AddRange(stationFilter);
                if (createOther)
                    line.Add("Other");
                WriteCsvLine(writer, line);
                
                //Enumerate bins
                for (long bin = firstBin; bin <= lastBin; bin++)
                {
                    //Convert bin index to time
                    DateTime time = EPOCH + new TimeSpan(bin * binDuration.Ticks);

                    //Start line
                    line.Clear();
                    line.Add(time.ToShortDateString());

                    //Get bin data
                    Dictionary<string, int> binData;
                    if (!bins.TryGetValue(bin, out binData))
                        binData = null;

                    //Sum each
                    int sum = 0;
                    if (binData != null)
                    {
                        foreach (var s in binData)
                            sum += s.Value;
                    }

                    //Add each we care about
                    foreach (var s in stationFilter)
                    {
                        //Get data
                        int count;
                        if (binData == null || !binData.TryGetValue(s, out count))
                            count = 0;

                        //Write
                        line.Add(count.ToString());

                        //Subtract from sum
                        sum -= count;
                    }

                    //Add other
                    if (createOther)
                        line.Add(sum.ToString());

                    //Write line
                    WriteCsvLine(writer, line);
                }
            }
        }

        private void WriteCsvLine(StreamWriter writer, List<string> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (i != 0)
                    writer.Write(",");
                writer.Write("\"" + values[i] + "\"");
            }
            writer.WriteLine();
        }
    }
}
