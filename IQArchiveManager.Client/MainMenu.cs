using Csv;
using IQArchiveManager.Client.Components;
using IQArchiveManager.Client.RDS.Modes;
using IQArchiveManager.Client.RDS;
using IQArchiveManager.Client.Util;
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
using IQArchiveManager.Client.RDS.Modes.Csv;

namespace IQArchiveManager.Client
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            RefreshButtonStatus();
        }

        private ClipDatabase db = null;
        private BaseRdsMode[] rdsModes;
        private List<ToolStripMenuItem> rdsMenuItems = new List<ToolStripMenuItem>();
        private Timer searchTimer;

        private static string LastDbFilename => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.iq_manager_last_db";

        private void MainMenu_Load(object sender, EventArgs e)
        {
            //Setup timer
            searchTimer = new Timer();
            searchTimer.Interval = 350;
            searchTimer.Tick += SearchTimer_Tick;

            //Load a database
            InitialLoadDatabase();
        }

        /// <summary>
        /// Called on init and attempts to load database from the last filename
        /// </summary>
        private void InitialLoadDatabase()
        {
            //Check if there is a file containing the last loaded database filename
            if (File.Exists(LastDbFilename))
            {
                //Load this
                string dbFilename = File.ReadAllText(LastDbFilename);
                if (File.Exists(dbFilename))
                {
                    //Load
                    ClipDatabase db = new ClipDatabase(dbFilename);
                    LoadDatabase(db, false);
                    return;
                }
            }

            //Leave database blank but show alert
            MessageBox.Show("No database is loaded.\r\n\r\nOpen or initialize a new one under the Database tab.", "No Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            statusLabel.Text = "No databse is loaded.";
        }

        /// <summary>
        /// Loads a new database. May be call at any time in lifecycle.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="updateLastFile"></param>
        private void LoadDatabase(ClipDatabase db, bool updateLastFile)
        {
            //Set
            this.db = db;

            //Init RDS modes
            rdsModes = new BaseRdsMode[]
            {
                new RdsPatchNative(),
                new RdsPatchNativeFlipped(),
                new RdsPatchCsv(db),
                new RdsPatchKzcr(db),
                new RdsPatchKzcrLegacy(db),
                new RdsPatchCumulus(),
                new RdsPatchNoDelimiters(db)
            };

            //Refresh
            RefreshClips();
            RefreshButtonStatus();
            CreateSetupTab();

            //Write the database filename to the last DB file if requested
            if (updateLastFile)
                File.WriteAllText(LastDbFilename, db.DatabaseFilename);

            //If enviornment items aren't set, show alert
            if (!db.Enviornment.IqaDirs.IsValid)
                MessageBox.Show("No IQA directory is set. You won't be able to extract files.\r\n\r\nSet up directories under Setup > Folders.", "Missing Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Refreshes if buttons are enabled/disabled
        /// </summary>
        private void RefreshButtonStatus()
        {
            bool databaseAvailable = db != null;
            searchBox.Enabled = databaseAvailable;
            btnClipExtract.Enabled = databaseAvailable && clipGrid.SelectedItem != null;
            btnOpenEditor.Enabled = databaseAvailable;
            rebuildToolStripMenuItem.Enabled = databaseAvailable;
            setupToolStripMenuItem.Enabled = databaseAvailable;
            exportResultsToolStripMenuItem.Enabled = databaseAvailable;
            exportStatsToolStripMenuItem.Enabled = databaseAvailable;
        }

        /// <summary>
        /// Checks if the IQA path is set. If not, returns false and shows a dialog. If it's fine, returns true.
        /// </summary>
        /// <returns></returns>
        private bool CheckIqaPath()
        {
            if (db.Enviornment.IqaDirs.IsValid)
                return true;
            MessageBox.Show("No IQA directory is set. Add it to extract files.\r\n\r\nSet up directories under Setup > Folders.", "Missing Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        /// <summary>
        /// Adds special items to the "setup" tab.
        /// </summary>
        private void CreateSetupTab()
        {
            //Remove custom items
            foreach (var v in rdsMenuItems)
                setupToolStripMenuItem.DropDownItems.Remove(v);
            rdsMenuItems.Clear();

            //Create RDS items
            foreach (var r in rdsModes)
            {
                if (r.HasSetupWindow)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(r.Label + " RDS...");
                    item.Tag = r;
                    item.Click += RdsItemSetupClick;
                    setupToolStripMenuItem.DropDownItems.Add(item);
                }
            }
        }

        private void RdsItemSetupClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            BaseRdsMode rds = (BaseRdsMode)item.Tag;
            rds.ShowSetupWindow();
            db.Save();
        }

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
            IEnumerable<TrackClipInfo> foundClips = db.Clips.Where(x => SearchHelper(query, x.Station, x.Title, x.Artist, x.Prefix, x.Suffix, x.Notes));
            int foundCount = foundClips.Count();
            clipGrid.AddClips(foundClips, true);

            //Set button
            btnClipExtract.Enabled = clipGrid.SelectedItem != null;

            //Set label
            if (db.Clips.Count == foundCount)
            {
                //Calculate estimated filesize
                long filesize = 0;
                foreach (var c in db.Clips)
                    filesize += c.OriginalFileSize;

                //Update
                statusLabel.Text = $"{db.Clips.Count} clips ({filesize/1000/1000/1000/1000} TB total edited) in database since {db.CreationDate.ToShortDateString()}";
            } else
            {
                statusLabel.Text = $"{foundCount} of {db.Clips.Count} clips found";
            }
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

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            if (db != null)
                RefreshClips();
        }

        private void clipGrid_OnSelectionChanged(Components.ClipGrid grid, TrackClipInfo clip)
        {
            btnClipExtract.Enabled = db != null && clip != null;
        }

        private void btnClipExtract_Click(object sender, EventArgs e)
        {
            //Check that the directory is set
            if (!CheckIqaPath())
                return;

            //Get clip
            TrackClipInfo clip = clipGrid.SelectedItem;

            //Locate IQ file
            string filename;
            if (!db.Enviornment.GetIqFileById(clip.Id, out filename))
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

        private void btnOpenEditor_Click(object sender, EventArgs e)
        {
            new MainEditor(db, rdsModes).ShowDialog();
            RefreshClips();
        }

        private void rebuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check that the directory is set
            if (!CheckIqaPath())
                return;

            //Show
            new RebuildDatabaseDialog(db).ShowDialog();

            //Refresh
            RefreshClips();
        }

        private void exportStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ExportStatsDialog(db).ShowDialog();
        }

        private void exportResultsToolStripMenuItem_Click(object sender, EventArgs e)
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

        private const string DATABASE_FILE_FILTER = "Database Files (*.iqd) | *.iqd|Legacy Database Files (*.json)|*.json";

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Prompt for a file to save
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = DATABASE_FILE_FILTER;
            if (fd.ShowDialog() != DialogResult.OK)
                return;

            //If file exists, delete it
            if (File.Exists(fd.FileName))
                File.Delete(fd.FileName);

            //Initialize a new database
            LoadDatabase(new ClipDatabase(fd.FileName), true);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Prompt for a file to load
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = DATABASE_FILE_FILTER;
            if (fd.ShowDialog() != DialogResult.OK)
                return;

            //Load
            LoadDatabase(new ClipDatabase(fd.FileName), true);
        }

        private void foldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EnviornmentPicker(db.Enviornment).ShowDialog();
            db.Save();
        }
    }
}
