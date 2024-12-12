using Csv;
using IQArchiveManager.Client.Components;
using IQArchiveManager.Common;
using IQArchiveManager.Common.IO.IqaReader;
using Newtonsoft.Json;
using RomanPort.LibSDR.Components.IO.WAV;
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

namespace IQArchiveManager.Client
{
    public partial class MainMenu : Form
    {
        public MainMenu(IQEnviornment iqEnv)
        {
            this.iqEnv = iqEnv;
            InitializeComponent();
        }

        private IQEnviornment iqEnv;
        private Timer searchTimer;

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            //Restart timer
            if (searchTimer.Enabled)
                searchTimer.Stop();
            searchTimer.Start();
        }

        private void RefreshClips()
        {
            //Get the current search query
            string query = searchBox.Text.ToUpper();

            //Fetch clips
            clipGrid.AddClips(iqEnv.Db.Clips.Where(x => SearchHelper(query, x.Station, x.Title, x.Artist, x.Prefix, x.Suffix, x.Notes)), true);

            //Set button
            btnClipExtract.Enabled = clipGrid.SelectedItem != null;
        }

        private bool SearchHelper(string query, params string[] challenges)
        {
            //If the query is empty, always return true
            if (query.Length == 0)
                return true;

            //Check each challenge
            bool ok = false;
            foreach (var c in challenges)
                ok = ok || c.ToUpper().Contains(query);

            return ok;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            //Setup timer
            searchTimer = new Timer();
            searchTimer.Interval = 350;
            searchTimer.Tick += SearchTimer_Tick;

            //Refresh clips
            RefreshClips();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            RefreshClips();
        }

        private void clipGrid_OnSelectionChanged(Components.ClipGrid grid, TrackClipInfo clip)
        {
            btnClipExtract.Enabled = clip != null;
        }

        private void btnClipExtract_Click(object sender, EventArgs e)
        {
            //Get clip
            TrackClipInfo clip = clipGrid.SelectedItem;

            //Locate IQ file
            string filename;
            if (!iqEnv.GetIqFileById(clip.Id, out filename))
            {
                MessageBox.Show($"Unable to locate the IQ file with ID {clip.Id}. Check the file paths, or rebuild the database.", "Cannot Locate File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Prompt for output filename
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "WAV Files (*.wav)|*.wav";
            fd.FileName = $"{clip.Station} - {clip.Artist} - {clip.Title}.wav";
            if (fd.ShowDialog() != DialogResult.OK)
                return;

            //Open worker
            WorkerDialog worker = new WorkerDialog($"Extracting \"{filename}\"...", (IWorkerDialogControl ctx) =>
            {
                using (FileStream output = new FileStream(fd.FileName, FileMode.Create))
                using (IqaFileReader reader = new IqaFileReader(filename))
                using (IqaSegmentReader segment = reader.OpenSegment("DATA"))
                using (IqaFlacReader flac = new IqaFlacReader(segment))
                {
                    //Write WAV header data
                    byte[] header = WavHeaderUtil.CreateHeader(new WavFileInfo
                    {
                        bitsPerSample = 16,
                        channels = 2,
                        sampleRate = clip.SampleRate
                    });
                    output.Write(header, 0, header.Length);

                    //Read FLAC
                    byte[] buffer = new byte[65536];
                    int read;
                    do
                    {
                        read = flac.Read(buffer, 0, buffer.Length);
                        ctx.UpdateStatusBar((double)segment.Position / segment.Length);
                        output.Write(buffer, 0, read);
                    } while (read != 0);

                    //Update WAV file lengths
                    WavHeaderUtil.UpdateLength(output, (int)(output.Length - WavHeaderUtil.HEADER_LENGTH));

                    return DialogResult.OK;
                }
            });
            worker.ShowDialog();
        }

        private void btnRebuildDb_Click(object sender, EventArgs e)
        {
            //Show
            new RebuildDatabaseDialog(iqEnv).ShowDialog();

            //Refresh
            RefreshClips();
        }

        private void btnOpenEditor_Click(object sender, EventArgs e)
        {
            new MainEditor(iqEnv).ShowDialog();
            RefreshClips();
        }

        private void btnExportStats_Click(object sender, EventArgs e)
        {
            new ExportStatsDialog(iqEnv.Db).ShowDialog();
        }

        private void btnExportResults_Click(object sender, EventArgs e)
        {
            //Convert
            List<string[]> output = new List<string[]>();
            foreach (var i in clipGrid.CurrentItems)
            {
                output.Add(new string[]
                {
                    i.Station,
                    i.Artist,
                    i.Title,
                    i.Time.ToShortDateString() + " " + i.Time.ToLongTimeString(),
                    i.Prefix,
                    i.Suffix,
                    i.GetRadioString(),
                    i.Notes
                });
            }

            //Serialize
            string csv = CsvWriter.WriteToText(new string[] {
                "Station",
                "Artist",
                "Title",
                "Time",
                "Prefix",
                "Suffix",
                "Radio",
                "Notes"
            }, output, ',');

            //Prompt for save location
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "CSV Files (*.csv)|*.csv";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                //Save
                File.WriteAllText(fd.FileName, csv);
            }
        }
    }
}
