using IQArchiveManager.Common;
using IQArchiveManager.Common.IO.IqaReader;
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
    public partial class RebuildDatabaseDialog : Form
    {
        public RebuildDatabaseDialog(ClipDatabase db)
        {
            this.db = db;
            InitializeComponent();
        }

        private ClipDatabase db;

        private void RebuildDatabaseDialog_Load(object sender, EventArgs e)
        {
            scanDirList.Items.Add(db.Enviornment.IqaDir);
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            //Get filenames for IQA files
            List<string> files = new List<string>();
            foreach (var d in scanDirList.Items)
                files.AddRange(Directory.GetFiles((string)d, "*.iqa"));

            //Hide this
            Hide();

            //Load all files and their info. It'll take a while, so do it in a worker
            TrackClipInfo[] loaded = new TrackClipInfo[0];
            new WorkerDialog("Fetching file list...", (IWorkerDialogControl ctx) =>
            {
                //Create array
                loaded = new TrackClipInfo[files.Count];

                //Loop
                int totalLoaded = 0;
                Parallel.For(0, loaded.Length, (int i) =>
                {
                    ctx.UpdateStatusText($"Querying file {totalLoaded + 1} of {files.Count}...");
                    ctx.UpdateStatusBar(totalLoaded / (double)files.Count);
                    using (IqaFileReader reader = new IqaFileReader(files[i]))
                        loaded[i] = IqaInfoReader.ReadInfo(reader);
                    totalLoaded++;
                });
                return DialogResult.OK;
            }).ShowDialog();

            //Find
            string missingInDb = RebuildHelperFindMissing(db.Clips, loaded);
            string missingOnDisk = optRemoveLost.Checked ? RebuildHelperFindMissing(loaded, db.Clips) : "[Unchecked; Disabled by user]";

            //Create dialog box
            string body = $"Database rebuild complete. The following inconsistencies were found. Would you like to apply changes?\n\nFILES MISSING IN DATABASE: (to be added to database)\n{missingInDb}\n\nFILES MISSING ON DISK: (to be removed from database)\n{missingOnDisk}\n\n{db.Clips.Count} IN DATABASE - {loaded.Length} FOUND ON DISK";
            if (new ScrollableDialog("Database Rebuild Completed", body.Replace("\n", "\r\n")).ShowDialog() == DialogResult.Yes)
            {
                //Depending on the mode, update the clips
                if (optRemoveLost.Checked)
                {
                    //Simply remove and replace all
                    db.Clips.Clear();
                    db.Clips.AddRange(loaded);
                } else
                {
                    //Loop through and UPDATE values
                    for(int i = 0; i < db.Clips.Count; i++)
                    {
                        foreach (var f in loaded)
                        {
                            if (db.Clips[i].Id == f.Id)
                                db.Clips[i] = f;
                        }
                    }
                }
                
                //Save to disk
                db.Save();
            }

            //Close this
            Close();
        }

        //Returns values not in dataset but are in incoming
        private string RebuildHelperFindMissing(IEnumerable<TrackClipInfo> dataset, IEnumerable<TrackClipInfo> incoming)
        {
            string output = "";
            foreach (var i in incoming)
            {
                //Look for a matching ID
                bool exists = false;
                foreach (var d in dataset)
                    exists = exists || d.Id == i.Id;

                //Write to output
                if (!exists)
                    output += $"{i.Id} - {i.Station} - {i.Time.ToShortDateString()} {i.Time.ToShortTimeString()} - {i.Artist} - {i.Title}\n";
            }
            if (output.Length == 0)
                output = "[OK, none reported!]\n";
            return output.TrimEnd('\n');
        }

        private void btnAddScanDir_Click(object sender, EventArgs e)
        {
            //Prompt
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
                scanDirList.Items.Add(fd.SelectedPath + Path.DirectorySeparatorChar);
        }
    }
}
