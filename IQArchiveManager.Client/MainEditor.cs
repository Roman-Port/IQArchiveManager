using Csv;
using IQArchiveManager.Client.Pre;
using IQArchiveManager.Client.RdsModes;
using IQArchiveManager.Client.Util;
using IQArchiveManager.Common;
using NAudio.Wave;
using Newtonsoft.Json;
using RomanPort.LibSDR.Components;
using RomanPort.LibSDR.Components.Digital.RDS;
using RomanPort.LibSDR.Components.IO.WAV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client
{
    public unsafe partial class MainEditor : Form
    {
        public MainEditor(ClipDatabase db, BaseRdsMode[] rdsModes)
        {
            this.db = db;
            rds = new RdsReader(rdsModes);
            InitializeComponent();
        }

        private volatile bool active = true;

        private void MainEditor_Load(object sender, EventArgs e)
        {
            //Bind
            MouseWheel += (object mSender, MouseEventArgs mE) => transportControls.ChangeZoom(Math.Max(-1, Math.Min(1, mE.Delta)));
            
            //Prepare output audio
            audioPlayer = new AudioPlaybackProvider();
            audioOutput = new WaveOut();
            audioOutput.Init(audioPlayer);
            audioOutput.Play();

            //Set up FFT
            fftRawBuffer = new byte[1024];
            fftBuffer = UnsafeBuffer.Create(1024, out fftBufferPtr);
            fftThread = new Thread(FftWorkerThread);
            fftThread.Name = "FFT Worker Thread";
            fftThread.Start();

            //Set up RDS
            rdsTimer = new System.Windows.Forms.Timer();
            rdsTimer.Interval = 50;
            rdsTimer.Tick += RdsTimer_Tick;
            rdsTimer.Start();
            rds.OnStatusChanged += Rds_OnStatusChanged;
            rds.OnProgressUpdated += Rds_OnProgressUpdated;
            rds.OnPatcherUpdated += Rds_OnPatcherUpdated;

            //Set up project
            OpenNextFile();
            UpdateEntryLayout();
        }

        private void Rds_OnPatcherUpdated(RdsReader ctx, BaseRdsMode selected)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                //Clear options
                rdsPatchMethod.SelectedIndexChanged -= rdsPatchMethod_SelectedIndexChanged;
                rdsPatchMethod.BeginUpdate();
                rdsPatchMethod.Items.Clear();

                //Add all
                for (int i = 0; i < rds.rdsModes.Length; i++)
                {
                    bool supported = ctx.IsPatcherRecommended(rds.rdsModes[i]);
                    rdsPatchMethod.Items.Add("[" + (supported ? "✓" : "⚠") + "] " + rds.rdsModes[i].Label);
                    if (rds.rdsModes[i] == selected)
                        rdsPatchMethod.SelectedIndex = i;
                }

                //Finalize
                rdsPatchMethod.EndUpdate();
                rdsPatchMethod.SelectedIndexChanged += rdsPatchMethod_SelectedIndexChanged;
            });
        }

        private void Rds_OnProgressUpdated(int progress, int max)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                rdsLoadProgress.Maximum = max + 1;
                rdsLoadProgress.Value = progress;
            });
        }

        private void Rds_OnStatusChanged(bool ready)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                rdsLoadPanel.Visible = !ready;
            });
        }

        private bool forceRawRtDisplay = false;

        private void UpdateRdsDisplay()
        {
            //Validate
            if (audioPlayer.Stream == null)
                return;

            //Get values
            string ps = rds.GetPsAtSample(audioPlayer.Stream.Position, out long psStart, out long psEnd);
            string rt = rds.GetRtAtSample(audioPlayer.Stream.Position, out long start, out long end);

            //Update PS
            rdsPsLabel.Text = ps == null ? "" : ps;

            //Process RT
            try
            {
                //Attempt to format RT if enabled
                bool showingRaw = true;
                string formattedRt = rt == null ? "" : rt;
                if (!forceRawRtDisplay && rt != null && rds.rdsModes[rdsPatchMethod.SelectedIndex].TryParse(rt, out string trackTitle, out string trackArtist, out string stationName, true))
                {
                    formattedRt = trackArtist + " - " + trackTitle;
                    showingRaw = false;
                }

                //Apply
                rdsRtLabel.Text = formattedRt;
                rdsRtLabel.BackColor = showingRaw ? SystemColors.ControlLight : Color.FromArgb(192, 192, 255);
            } catch
            {
                //Error
                rdsRtLabel.Text = rt == null ? "" : rt;
                rdsRtLabel.BackColor = Color.FromArgb(255, 192, 192);
            }
        }

        private void RdsTimer_Tick(object sender, EventArgs e)
        {
            UpdateRdsDisplay();
        }

        private void FftWorkerThread()
        {
            Stopwatch tsu = new Stopwatch();
            tsu.Start();
            while (active)
            {
                //If there is no stream, wait
                if (fftStream == null)
                {
                    Thread.Sleep(50);
                    continue;
                }
                
                //Reset
                tsu.Restart();
                
                //Read
                fftStream.Read(fftRawBuffer, 0, 1024);

                //Convert
                for (int i = 0; i < 1024; i++)
                    fftBufferPtr[i] = -fftRawBuffer[i];

                //Set
                try
                {
                    Invoke((MethodInvoker)delegate
                    {
                        if (!active)
                            return;
                        spectrumView1.WritePowerSamples(fftBufferPtr, 1024);
                        waterfallView1.WritePowerSamples(fftBufferPtr, 1024);
                        spectrumView1.DrawFrame();
                        waterfallView1.DrawFrame();
                    });
                } catch
                {

                }

                //Wait for delay
                Thread.Sleep((int)Math.Max(0, (1000 / 25) - tsu.ElapsedMilliseconds));
            }
            fftBuffer.Dispose();
        }

        private ClipDatabase db;

        private FileInfo wavFile;
        private FileInfo infoFile;
        private FileInfo postFile;
        private FileInfo postFinalFile;

        private PreProcessorFileReader inputReader;

        private WaveOut audioOutput;
        private AudioPlaybackProvider audioPlayer;

        private Thread fftThread;
        private byte[] fftRawBuffer;
        private UnsafeBuffer fftBuffer;
        private float* fftBufferPtr;
        private PreProcessorFileStreamReader fftStream;

        private RdsReader rds;
        private System.Windows.Forms.Timer rdsTimer;

        private TuneGenieCache tuneGenie;

        private string matchedRds = null; // This and the following are written to the file when it was matched via RDS and may be used for future anaylsis
        private int matchedRdsParser = -1;

        private void Open(string wavPath)
        {
            //Get files
            wavFile = new FileInfo(wavPath);
            infoFile = new FileInfo(wavPath + ".iqpre");
            postFile = new FileInfo(wavPath + ".iqpost");
            postFinalFile = new FileInfo(wavPath + ".iqedit");

            //Set UI
            Text = $"Editing {wavFile.Name}...";
            editedClipsList.Items.Clear();
            btnAddClip.Enabled = false;
            inputFlagHd.Checked = true;
            inputFlagOk.Checked = true;
            inputFlagRds.Checked = true;
            inputNotes.Text = "";

            //It's unlikely, but if we have a post file, load it into the list
            if (postFile.Exists)
            {
                foreach (var e in JsonConvert.DeserializeObject<List<TrackEditInfo>>(File.ReadAllText(postFile.FullName)))
                    editedClipsList.Items.Add(e.tip);
            }

            //Set buttons
            btnFileSave.Enabled = editedClipsList.Items.Count != 0;
            btnFileDelete.Enabled = editedClipsList.Items.Count == 0;

            //Show duplicate clips in the grid
            clipGrid1.AddClips(db.Clips.Where(x => x.Station.ToUpper() == inputCall.Text.ToUpper() && x.Artist.ToUpper() == inputArtist.Text.ToUpper() && x.Title.ToUpper() == inputTitle.Text.ToUpper()), true);

            //Open
            inputReader = new PreProcessorFileReader(infoFile.FullName);
            inputReader.Open();

            //Start audio
            audioPlayer.Stream = inputReader.GetStreamByTag("AUDIO");

            //Set up FFT
            fftStream = inputReader.GetStreamByTag("SPECTRUM_MAIN");

            //Set up transport controls
            transportControls.SetStreams(wavFile.LastWriteTime, inputReader.GetStreamByTag("AUDIO"), audioPlayer.Stream, fftStream);

            //Read RDS -- Detect which version we are using
            rds.Reset();
            PreProcessorFileStreamReader rdsStream;
            if (inputReader.TryGetStreamByTag("RDS", out rdsStream))
                rds.LoadV1(rdsStream);
            else if (inputReader.TryGetStreamByTag("RDS2", out rdsStream))
                rds.LoadV2(rdsStream);
            else
                MessageBox.Show("No valid RDS stream found. Outdated client?", "No RDS", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //Create TuneGenie instance for fetching metadata
            tuneGenie = new TuneGenieCache(CalculateOffsetTime(0), CalculateOffsetTime(transportControls.StreamAudio.Length));
        }

        class AudioPlaybackProvider : IWaveProvider
        {
            public PreProcessorFileStreamReader Stream { get; set; }

            public WaveFormat WaveFormat => new WaveFormat(20000, 8, 1);

            public int Read(byte[] buffer, int offset, int count)
            {
                //Perform as normal
                int read = Stream == null ? 0 : Stream.Read(buffer, offset, count);

                //Fill remaining buffer with 0
                while (read < count)
                    buffer[read++] = 0;

                return count;
            }
        }

        private void MainEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Signal
            transportControls.Close();
            active = false;

            //Stop audio
            audioOutput.Stop();
            audioOutput.Dispose();

            //Set up RDS
            rdsTimer.Stop();
            rds.OnStatusChanged -= Rds_OnStatusChanged;
            rds.OnProgressUpdated -= Rds_OnProgressUpdated;
            rds.OnPatcherUpdated -= Rds_OnPatcherUpdated;
        }

        private void btnAddClip_Click(object sender, EventArgs e)
        {
            //Get the current edit times
            double timeIn = transportControls.EditStartSeconds;
            double timeOut = transportControls.EditEndSeconds;
            double timeToEnd = ((double)audioPlayer.Stream.Length / 20000) - timeOut;

            //Validate
            if (timeIn > timeOut || timeToEnd < 0)
            {
                MessageBox.Show("Invalid trim.");
                return;
            }

            //Calculate the time this started recording
            DateTime time = wavFile.LastWriteTimeUtc.AddSeconds(-timeToEnd);

            //Query the sample rate of the file
            WavFileInfo header;
            using (FileStream fs = new FileStream(wavFile.FullName, FileMode.Open, FileAccess.Read))
                WavHeaderUtil.ParseWavHeader(fs, out header);

            //Create clip info
            TrackClipInfo clip = new TrackClipInfo
            {
                Artist = GetEntryArtist(),
                Title = GetEntryTitle(),
                Station = inputCall.Text,
                Notes = inputNotes.Text,
                Prefix = GetEntryPrefix(),
                Suffix = GetEntrySuffix(),
                FlagHd = inputFlagHd.Checked,
                FlagOk = inputFlagOk.Checked,
                FlagRds = inputFlagRds.Checked,
                Sha256 = null,
                Time = time,
                Id = db.GetNewId(inputCall.Text, time),
                OriginalFileSize = wavFile.Length,
                SampleRate = header.sampleRate,
                RdsParsed = matchedRds == null ? null : Convert.ToBase64String(Encoding.ASCII.GetBytes(matchedRds)),
                RdsParser = matchedRds == null ? -1 : matchedRdsParser,
                EditedAt = DateTime.UtcNow,
                EditorVersion = Constants.CURRENT_EDITOR_VERSION
            };

            //Load the existing edit file, if any
            List<TrackEditInfo> addedEdits;
            if (File.Exists(postFile.FullName))
                addedEdits = JsonConvert.DeserializeObject<List<TrackEditInfo>>(File.ReadAllText(postFile.FullName));
            else
                addedEdits = new List<TrackEditInfo>();

            //Add edit
            string tip = clip.Station + " / " + clip.Artist + " / " + clip.Title;
            addedEdits.Add(new TrackEditInfo
            {
                data = clip,
                start = timeIn,
                end = timeOut,
                tip = tip,
                editorVersion = Constants.CURRENT_EDITOR_VERSION
            });

            //Write edit file
            File.WriteAllText(postFile.FullName, JsonConvert.SerializeObject(addedEdits));

            //Add to clip database
            db.AddClip(clip);

            //Partially reset metadata
            inputArtist.Text = "";
            inputTitle.Text = "";
            matchedRds = null;
            matchedRdsParser = -1;

            //Add name to list just for notetaking
            editedClipsList.Items.Add(tip);
            btnFileSave.Enabled = editedClipsList.Items.Count != 0;
            btnFileDelete.Enabled = editedClipsList.Items.Count == 0;
            btnAddClip.Enabled = false;
        }

        private void btnAutoRds_Click(object sender, EventArgs e)
        {
            //Autodetect range
            string rt;
            if (!transportControls.AutoDetectRange(rds, out rt))
            {
                MessageBox.Show("Failed to auto-detect RDS.", "Automatic Recognization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Autodetect callsign
            string call;
            if (!RDSClient.TryGetCallsign(rds.GetPiAtSample(audioPlayer.Stream.Position, out long piStart, out long piEnd), out call))
            {
                MessageBox.Show("Failed to auto-detect PI to get callsign.", "Automatic Recognization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Apply
            inputCall.Text = call + "-FM";
            inputArtist.Text = rt;

            //Only proceed if we're trying to parse a song
            if (typeBtnSong.Checked)
            {
                //Attempt to parse
                if (rds.rdsModes[rdsPatchMethod.SelectedIndex].TryParse(rt, out string trackTitle, out string trackArtist, out string stationName, false))
                {
                    inputArtist.Text = trackArtist;
                    inputTitle.Text = trackTitle;
                    inputCall.Text = call + "-FM";
                    UpdateAddBtnStatus();
                    matchedRds = rt;
                    matchedRdsParser = (int)rds.rdsModes[rdsPatchMethod.SelectedIndex].Id;
                    return;
                }
            }

            //Update the button
            UpdateAddBtnStatus();

            //Failed!
            MessageBox.Show("Failed to recognize the audio.", "Automatic Recognization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void autoTgCall_TextChanged(object sender, EventArgs e)
        {
            autoTgBtn.Enabled = autoTgCall.Text.Length > 0;
        }

        private void autoTgBtn_Click(object sender, EventArgs e)
        {
            //Get the estimated time we're looking at now
            DateTime time = CalculateOffsetTime(transportControls.StreamAudio.Position);

            //Attempt to fetch item
            ITuneGenieItem item;
            if (!tuneGenie.TryFetchItem(autoTgCall.Text.ToLower(), time, out item))
            {
                //Failed!
                MessageBox.Show("Failed to get an item at this time. This might be caused by a variety of factors:\n\n" +
                    "* The selected time is before the start of the first item\n" +
                    "* The station might not have been sending events at this time (during satellite syndication, for example)\n" +
                    "* The station's call letters are incorrectly typed\n" +
                    "* The station may not use TuneGenie",
                    "Tune Genie Lookup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Set values
            inputArtist.Text = item.Artist;
            inputTitle.Text = item.Name;
            inputCall.Text = autoTgCall.Text.ToUpper() + "-FM";
            transportControls.SetSelectionRegion(item.PlayedAt, item.EstimatedEndTime);
            UpdateAddBtnStatus();
        }

        private void InputUpdated(object sender, EventArgs e)
        {
            //Update
            UpdateAddBtnStatus();

            //Show duplicate clips in the grid
            clipGrid1.AddClips(
                db.Clips.Where(
                    x => x.Station.ToUpper() == inputCall.Text.ToUpper() &&
                    x.Artist.ToUpper() == GetEntryArtist().ToUpper() &&
                    x.Title.ToUpper() == GetEntryTitle().ToUpper()
                    )
                , true);
        }

        private void UpdateAddBtnStatus()
        {
            btnAddClip.Enabled = inputArtist.Text.Length > 0 &&
                GetEntryTitle().Length > 0 &&
                (inputCall.Text.EndsWith("-FM") || inputCall.Text.EndsWith("-AM")) &&
                GetEntryPrefix().Length > 0 &&
                GetEntrySuffix().Length > 0;
        }

        private void UpdateEntryLayout()
        {
            if (typeBtnSong.Checked)
            {
                labelArtist.Text = "Artist";
                labelTitle.Visible = true;
                inputTitle.Visible = true;
                prefixSuffixPanel.Visible = true;
                rdsPatchMethod.Visible = true;
                btnAutoRds.Visible = true;
            }
            if (typeBtnLiner.Checked)
            {
                labelArtist.Text = "Title";
                labelTitle.Visible = false;
                inputTitle.Visible = false;
                prefixSuffixPanel.Visible = false;
                rdsPatchMethod.Visible = false;
                btnAutoRds.Visible = false;
            }
            InputUpdated(null, null);
        }

        private string GetEntryArtist()
        {
            if (typeBtnSong.Checked)
                return inputArtist.Text;
            if (typeBtnLiner.Checked)
                return "NON-SONG";
            throw new Exception("Invalid state.");
        }

        private string GetEntryTitle()
        {
            if (typeBtnSong.Checked)
                return inputTitle.Text;
            if (typeBtnLiner.Checked)
                return inputArtist.Text;
            throw new Exception("Invalid state.");
        }

        private string GetEntryPrefix()
        {
            if (typeBtnSong.Checked)
                return inputPrefix.Text;
            if (typeBtnLiner.Checked)
                return "N/A";
            throw new Exception("Invalid state.");
        }

        private string GetEntrySuffix()
        {
            if (typeBtnSong.Checked)
                return inputSuffix.Text;
            if (typeBtnLiner.Checked)
                return "N/A";
            throw new Exception("Invalid state.");
        }

        private void btnFileSave_Click(object sender, EventArgs e)
        {
            //Check if the user really wants to delete the file
            if (sender == btnFileDelete && MessageBox.Show($"Are you sure you want to delete {wavFile.Name}?", "Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            
            //Close all associated streams
            inputReader.Dispose();

            //Wait a sec for threads to catch up
            Thread.Sleep(500);

            //If we're moving it, transfer all the files rather than deleting them
            if (sender == btnFileMove)
            {
                MoveEditFileHelper(wavFile);
                MoveEditFileHelper(postFile);
                MoveEditFileHelper(infoFile);
            } else
            {
                //Delete the metadata file
                infoFile.Delete();

                //Finalize the post file
                if (sender == btnFileSave)
                    File.Move(postFile.FullName, postFinalFile.FullName);

                //If we made no edits, delete the WAV file too
                if (sender == btnFileDelete)
                    wavFile.Delete();
            }

            //Finally, open the next file
            OpenNextFile();
        }

        private void MoveEditFileHelper(FileInfo file)
        {
            if (file == null || !file.Exists)
                return;
            File.Move(file.FullName, file.FullName.Replace(db.Enviornment.EditDir, db.Enviornment.MoveDir));
        }

        public void OpenNextFile()
        {
            //Get list of files in working dir
            string[] files = Directory.GetFiles(db.Enviornment.EditDir);

            //Loop through and look for files
            foreach(var f in files)
            {
                //Make sure it's a WAV file
                if (!f.EndsWith(".wav"))
                    continue;

                //Look for a metadata file
                if (File.Exists(f + ".iqpre"))
                {
                    Open(f);
                    return;
                }
            }

            //Done
            MessageBox.Show("All files have been edited!");
            Close();
        }

        private bool showDate = false;

        /// <summary>
        /// Calculates the estimated time was when a byte in the file was captured
        /// </summary>
        /// <param name="fileOffset"></param>
        /// <returns></returns>
        public DateTime CalculateOffsetTime(long fileOffset)
        {
            return transportControls.SampleToTime(fileOffset);
        }

        private void transportControls_TimeChanged(PreProcessorFileStreamReader reader)
        {
            DateTime time = CalculateOffsetTime(reader.Position);
            recordingTimeLabel.Text = showDate ? time.ToShortDateString() : time.ToLongTimeString();
        }

        private void recordingTimeLabel_Click(object sender, EventArgs e)
        {
            showDate = !showDate;
        }

        private void rdsPatchMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            rds.SwitchPatcher(rds.rdsModes[rdsPatchMethod.SelectedIndex]);
        }

        private void btnShiftSuffix_Click(object sender, EventArgs e)
        {
            inputPrefix.Text = inputSuffix.Text;
            inputSuffix.Text = "";
        }

        private void moveStartToPlayheadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.SetStartToCurrent();
        }

        private void moveEndToPlayheadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.SetEndToCurrent();
        }

        private void jumpToStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.MovePlayheadToStart();
        }

        private void jumpToEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.MovePlayheadToEnd();
        }

        private static readonly TimeSpan NUDGE_AMOUNT_COARSE = TimeSpan.FromSeconds(20);
        private static readonly TimeSpan NUDGE_AMOUNT_FINE = TimeSpan.FromSeconds(2);

        private void nudgeStartForward20sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeStart(NUDGE_AMOUNT_COARSE);
        }

        private void nudgeStartBackwards20sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeStart(-NUDGE_AMOUNT_COARSE);
        }

        private void nudgeStartForwards2sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeStart(NUDGE_AMOUNT_FINE);
        }

        private void nudgeStartBackwards2sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeStart(-NUDGE_AMOUNT_FINE);
        }

        //

        private void nudgePlayheadForwards20sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeCurrent(NUDGE_AMOUNT_COARSE);
        }

        private void nudgePlayheadBackwards20sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeCurrent(-NUDGE_AMOUNT_COARSE);
        }

        private void nudgePlayheadForwards2sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeCurrent(NUDGE_AMOUNT_FINE);
        }

        private void nudgePlayheadBackwards2sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeCurrent(-NUDGE_AMOUNT_FINE);
        }

        //

        private void nudgeEndForwards20sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeEnd(NUDGE_AMOUNT_COARSE);
        }

        private void nudgeEndBackwards20sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeEnd(-NUDGE_AMOUNT_COARSE);
        }

        private void nudgeEndForwards2sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeEnd(NUDGE_AMOUNT_FINE);
        }

        private void nudgeEndBackwards2sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transportControls.NudgeEnd(-NUDGE_AMOUNT_FINE);
        }

        private void typeBtnSong_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEntryLayout();
        }

        private void typeBtnLiner_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEntryLayout();
        }

        private void rdsRtLabel_Click(object sender, EventArgs e)
        {
            //Toggle forcing raw
            forceRawRtDisplay = !forceRawRtDisplay;

            //Force update RDS display
            UpdateRdsDisplay();
        }

        private void rawPSFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportRdsFrames(rds.PsFrames);
        }

        private void rawRTFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportRdsFrames(rds.ParsedRtFrames);
        }

        private void parsedRTFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportRdsFrames(rds.RawRtFrames);
        }

        private void ExportRdsFrames(IReadOnlyList<RdsValue<string>> frames)
        {
            //Prompt for filename
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "CSV Files (*.csv)|*.csv";
            if (fd.ShowDialog() != DialogResult.OK)
                return;

            //Open file and write
            using (FileStream fs = new FileStream(fd.FileName, FileMode.Create))
            using (TextWriter writer = new StreamWriter(fs))
            {
                CsvWriter.Write(
                    writer,
                    new string[] { "Start", "End", "Data" },
                    frames.Select(x => new string[] { x.first.ToString(), x.last.ToString(), x.value })
                    );
            }
        }
    }
}
