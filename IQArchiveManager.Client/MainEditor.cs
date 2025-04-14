using Csv;
using IQArchiveManager.Client.Components;
using IQArchiveManager.Client.Pre;
using IQArchiveManager.Client.RDS;
using IQArchiveManager.Client.RDS.Parser;
using IQArchiveManager.Client.Util;
using IQArchiveManager.Common;
using IQArchiveManager.Common.IO.Editor.Post;
using IQArchiveManager.Common.IO.RDS;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace IQArchiveManager.Client
{
    public unsafe partial class MainEditor : Form, IRdsPatchContext
    {
        public MainEditor(ClipDatabase db, RdsParserStore parser, List<string> files)
        {
            this.db = db;
            this.files = files;
            rds = new RdsReader(parser);
            InitializeComponent();
        }

        public const int AUDIO_SAMPLE_RATE = 20000;

        private ClipDatabase db;
        private List<string> files;

        private FileInfo wavFile;
        private FileInfo infoFile;

        private PreProcessorFileReader inputReader;
        private PostFileWriter outputWriter;

        private WaveOut audioOutput;
        private AudioPlaybackProvider audioPlayer;

        private System.Windows.Forms.Timer fftTimer;
        private byte[] fftRawBuffer;
        private UnsafeBuffer fftBuffer;
        private float* fftBufferPtr;
        private PreProcessorFileStreamReader fftStream;

        private RdsReader rds;
        private System.Windows.Forms.Timer rdsTimer;

        private TuneGenieCache tuneGenie;

        private string matchedRds = null; // This and the following are written to the file when it was matched via RDS and may be used for future anaylsis
        private int matchedRdsParser = -1;

        private int lastSelectFromRdsIndex = 0; // The last RT index used in SelectFromRds

        private bool forceRawRtDisplay = false;

        private bool lockCall; // If true, do not automatically change callsign on auto detect

        private string fileUserLocation; // The user-specified string the server includes to identify itself. May be null.

        private void MainEditor_Load(object sender, EventArgs e)
        {
            //Bind
            MouseWheel += (object mSender, MouseEventArgs mE) => transportControls.ChangeZoom(Math.Max(-1, Math.Min(1, mE.Delta)));

            //Prepare output audio
            audioPlayer = new AudioPlaybackProvider();
            audioOutput = new WaveOut();
            audioOutput.NumberOfBuffers = 4;
            audioOutput.Init(audioPlayer);
            audioOutput.Play();

            //Set up FFT
            fftRawBuffer = new byte[1024];
            fftBuffer = UnsafeBuffer.Create(1024, out fftBufferPtr);
            fftTimer = new System.Windows.Forms.Timer();
            fftTimer.Interval = 1000 / 25;
            fftTimer.Tick += FftTimer_Tick;

            //Set up RDS
            rdsTimer = new System.Windows.Forms.Timer();
            rdsTimer.Interval = 50;
            rdsTimer.Tick += RdsTimer_Tick;
            rds.OnPatcherUpdated += Rds_OnPatcherUpdated;

            //Add all files to file menu
            foreach (var f in files.OrderBy(x => x))
            {
                //Get shorter filename
                int lastSlash = f.LastIndexOf(Path.DirectorySeparatorChar);
                string filename = f;
                if (lastSlash != -1)
                    filename = filename.Substring(lastSlash + 1);

                //Add
                ToolStripMenuItem item = new ToolStripMenuItem
                {
                    Text = filename,
                    Tag = f
                };
                item.Click += FileItemClicked;
                filesToolStripMenuItem.DropDownItems.Add(item);
            }

            //Set up project
            OpenNextFile();
            UpdateEntryLayout();
        }

        private void FileItemClicked(object sender, EventArgs e)
        {
            ChangeFile((ToolStripMenuItem)sender);
        }

        private void MainEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Signal
            transportControls.Close();

            //Stop audio
            audioOutput.Stop();
            audioOutput.Dispose();

            //Close file
            CloseFile();
        }

        /// <summary>
        /// Opens a new file. Assumes the active file is closed.
        /// </summary>
        /// <param name="wavPath"></param>
        private void OpenFile(string wavPath)
        {
            //Get files
            wavFile = new FileInfo(wavPath);
            infoFile = new FileInfo(wavPath + ".iqpre");
            FileInfo postFile = new FileInfo(wavPath + ".iqpost");
            FileInfo postFinalFile = new FileInfo(wavPath + ".iqedit");

            //Set UI
            editedClipsList.Items.Clear();
            btnAddClip.Enabled = false;
            inputFlagHd.Checked = true;
            inputFlagOk.Checked = true;
            inputFlagRds.Checked = true;
            inputNotes.Text = "";

            //Initialize the post file
            outputWriter = new PostFileWriter(postFile.FullName, postFinalFile.FullName);

            //Add any existing items in the post file to the list
            foreach (var e in outputWriter.Edits)
                editedClipsList.Items.Add(e.Data.ToString());

            //Set buttons
            btnFileSave.Enabled = editedClipsList.Items.Count != 0;
            btnFileDelete.Enabled = editedClipsList.Items.Count == 0;

            //Show duplicate clips in the grid
            RefreshClipsGrid();

            //Open
            inputReader = new PreProcessorFileReader(infoFile.FullName);
            inputReader.Open();

            //Attempt to read the file location
            if (inputReader.TryGetStreamByTag("LOCATION", out PreProcessorFileStreamReader stream))
            {
                //Read data
                byte[] locBuffer = new byte[1024];
                int locLen = stream.Read(locBuffer, 0, locBuffer.Length);

                //Convert to location
                fileUserLocation = Encoding.UTF8.GetString(locBuffer, 0, locLen);
            } else
            {
                //Default
                fileUserLocation = null;
            }

            //Start audio
            audioPlayer.Stream = inputReader.GetStreamByTag("AUDIO");

            //Set up FFT
            fftStream = inputReader.GetStreamByTag("SPECTRUM_MAIN");

            //Set up transport controls
            transportControls.SetStreams(wavFile.LastWriteTime, inputReader.GetStreamByTag("AUDIO"), audioPlayer.Stream, fftStream);

            //Load RDS
            InitRds();

            //Create TuneGenie instance for fetching metadata
            tuneGenie = new TuneGenieCache(CalculateOffsetTime(0), CalculateOffsetTime(transportControls.StreamAudio.Length));

            //Reset state
            lastSelectFromRdsIndex = 0;

            //Start timers
            fftTimer.Start();
            rdsTimer.Start();

            //Reset time display
            UpdateRecordingTimeLabel();

            //Set title
            Text = $"Editing {wavFile.Name}" + (fileUserLocation == null ? "" : $" ({fileUserLocation})") + "...";
        }

        /// <summary>
        /// Closes the active file.
        /// </summary>
        private void CloseFile()
        {
            //Stop timers
            fftTimer.Stop();
            rdsTimer.Stop();

            //Close all associated streams
            inputReader.Dispose();
        }

        #region RDS

        private void InitRds()
        {
            //Generate backwards list of DSP IDs
            RdsDspId[] ids = ((RdsDspId[])Enum.GetValues(typeof(RdsDspId))).Reverse().ToArray();

            //Find in-file DSP -- By default, load the existing DSP with the highest ID
            foreach (RdsDspId dspId in ids)
            {
                if (inputReader.TryGetStreamByTag(RdsDspStore.GetPreFileDspTag(dspId), out PreProcessorFileStreamReader localStream))
                {
                    //Create dummy DSP that loads from the file
                    rds.Load(new InFileRdsDsp().Load(localStream), this);

                    //Configure menu
                    SetupRdsMenu(dspId, false);
                    localStream.Close();
                    return;
                }
            }

            //Fall back to local DSPs if we have the raw bitstream
            if (inputReader.TryGetStreamByTag(RdsDspStore.PRE_FILE_RAW_BITSTREAM, out PreProcessorFileStreamReader rdsStream))
            {
                foreach (RdsDspId dspId in ids)
                {
                    //Create DSP
                    rds.Load(RdsDspStore.CreateDsp(dspId).Load(rdsStream), this);

                    //Configure menu
                    SetupRdsMenu(dspId, true);
                    rdsStream.Close();
                    return;
                }
                rdsStream.Close();
            }

            //Abort
            SetupRdsMenu(null, true);
            rds.Reset();
            MessageBox.Show("No valid RDS stream found. Outdated client?", "No RDS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SetupRdsMenu(RdsDspId? currentSelection, bool currentSelectionLocal)
        {
            //Clear existing items
            rDSDSPToolStripMenuItem.DropDownItems.Clear();

            //Create the two submenus
            List<ToolStripMenuItem> menuServer = new List<ToolStripMenuItem>();
            List<ToolStripMenuItem> menuClient = new List<ToolStripMenuItem>();
            bool hasRawBitstream = inputReader.HasStream(RdsDspStore.PRE_FILE_RAW_BITSTREAM);
            foreach (RdsDspId dspId in Enum.GetValues(typeof(RdsDspId)))
            {
                //Create server button
                ToolStripMenuItem itemServer = new ToolStripMenuItem($"{dspId} (IN-FILE)");
                itemServer.Enabled = inputReader.HasStream(RdsDspStore.GetPreFileDspTag(dspId));
                itemServer.Checked = !currentSelectionLocal && currentSelection != null && currentSelection.Value == dspId;
                itemServer.Tag = dspId;
                itemServer.Click += ChangeRdsServer;
                menuServer.Add(itemServer);

                //Create client button
                ToolStripMenuItem itemClient = new ToolStripMenuItem($"{dspId} (LOCAL)");
                itemClient.Enabled = hasRawBitstream;
                itemClient.Checked = currentSelectionLocal && currentSelection != null && currentSelection.Value == dspId;
                itemClient.Tag = dspId;
                itemClient.Click += ChangeRdsClient;
                menuClient.Add(itemClient);
            }

            //Set up menu
            foreach (var i in menuServer)
                rDSDSPToolStripMenuItem.DropDownItems.Add(i);
            rDSDSPToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            foreach (var i in menuClient)
                rDSDSPToolStripMenuItem.DropDownItems.Add(i);
        }

        private void ChangeRdsServer(object sender, EventArgs e)
        {
            //Get ID
            RdsDspId dspId = (RdsDspId)(sender as ToolStripMenuItem).Tag;

            //Create dummy DSP that loads from the file
            using (PreProcessorFileStreamReader rdsStream = inputReader.GetStreamByTag(RdsDspStore.GetPreFileDspTag(dspId)))
                rds.Load(new InFileRdsDsp().Load(rdsStream), this);

            //Configure menu
            SetupRdsMenu(dspId, false);
        }

        private void ChangeRdsClient(object sender, EventArgs e)
        {
            //Get ID
            RdsDspId dspId = (RdsDspId)(sender as ToolStripMenuItem).Tag;

            //Create DSP
            using (PreProcessorFileStreamReader rdsStream = inputReader.GetStreamByTag(RdsDspStore.PRE_FILE_RAW_BITSTREAM))
                rds.Load(RdsDspStore.CreateDsp(dspId).Load(rdsStream), this);

            //Configure menu
            SetupRdsMenu(dspId, true);
        }

        private void UpdateRdsDisplay()
        {
            //Validate
            if (audioPlayer.Stream == null)
                return;

            //Get values
            RdsValue<string> psDat = rds.GetPsAtSample(audioPlayer.Stream.Position);
            string ps = psDat == null ? null : psDat.value;
            RdsValue<string> rtDat = rds.GetRtAtSample(audioPlayer.Stream.Position);
            string rt = rtDat == null ? null : rtDat.value;

            //Update PS
            rdsPsLabel.Text = ps == null ? "" : ps;

            //Process RT
            try
            {
                //Attempt to format RT if enabled
                bool showingRaw = true;
                string formattedRt = rt == null ? "" : rt;
                if (!forceRawRtDisplay && rt != null && rds.rdsModes[rdsPatchMethod.SelectedIndex].TryParse(rtDat, out string trackTitle, out string trackArtist, out string stationName, true))
                {
                    formattedRt = trackArtist + " - " + trackTitle;
                    showingRaw = false;
                }

                //Apply
                rdsRtLabel.Text = formattedRt;
                rdsRtLabel.BackColor = showingRaw ? SystemColors.ControlLight : Color.FromArgb(192, 192, 255);
            }
            catch
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

                //Reset state
                lastSelectFromRdsIndex = 0;
            });
        }

        #endregion RDS

        #region FFT

        private void FftTimer_Tick(object sender, EventArgs e)
        {
            //If there is no stream, wait
            if (fftStream == null)
                return;

            //Read
            fftStream.Read(fftRawBuffer, 0, 1024);

            //Convert
            for (int i = 0; i < 1024; i++)
                fftBufferPtr[i] = -fftRawBuffer[i];

            //Set
            spectrumView1.WritePowerSamples(fftBufferPtr, 1024);
            waterfallView1.WritePowerSamples(fftBufferPtr, 1024);
            spectrumView1.DrawFrame();
            waterfallView1.DrawFrame();
        }

        #endregion FFT

        private void btnAddClip_Click(object sender, EventArgs e)
        {
            //Get the current edit times
            double timeIn = transportControls.EditStartSeconds;
            double timeOut = transportControls.EditEndSeconds;
            double timeToEnd = ((double)audioPlayer.Stream.Length / AUDIO_SAMPLE_RATE) - timeOut;
            double length = timeOut - timeIn;

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
                Location = fileUserLocation,
                Sha256 = null,
                Time = time,
                Id = db.GetNewId(inputCall.Text, time),
                OriginalFileSize = wavFile.Length,
                SampleRate = header.sampleRate,
                RdsParsed = matchedRds == null ? null : Convert.ToBase64String(Encoding.ASCII.GetBytes(matchedRds)),
                RdsParser = matchedRds == null ? -1 : matchedRdsParser,
                EditedAt = DateTime.UtcNow,
                EditorVersion = Constants.CURRENT_EDITOR_VERSION,
                Length = length,
                OriginalFileTime = wavFile.LastWriteTime
            };

            //Add to output file
            outputWriter.AddEdit(new TrackEditInfo
            {
                Data = clip,
                Start = timeIn,
                End = timeOut
            });

            //Add to clip database
            db.AddClip(clip);

            //Partially reset metadata
            inputArtist.Text = "";
            inputTitle.Text = "";
            matchedRds = null;
            matchedRdsParser = -1;

            //Add name to list just for notetaking
            editedClipsList.Items.Add(clip.ToString());
            btnFileSave.Enabled = editedClipsList.Items.Count != 0;
            btnFileDelete.Enabled = editedClipsList.Items.Count == 0;
            btnAddClip.Enabled = false;
        }

        private void SelectFromRds(RdsValue<string> rt)
        {
            //Determine the index of this
            lastSelectFromRdsIndex = rds.ParsedRtFrames.IndexOf(rt);
            if (lastSelectFromRdsIndex == -1)
                throw new Exception("SelectFromRds value is not in the list of parsed RDS frames.");

            //Expand range to give us some leeway
            long start = Math.Max(0, rt.first - (15 * AUDIO_SAMPLE_RATE));
            long end = Math.Min(transportControls.StreamAudio.Length, rt.last + (30 + AUDIO_SAMPLE_RATE));

            //Select region
            transportControls.SetSelectionRegion(start, end, (end - start) / 4);

            //Autodetect callsign
            string call;
            RdsValue<ushort> piFrame = rds.GetPiAtSample(rt.first);
            if (piFrame == null || !RDSClient.TryGetCallsign(piFrame.value, out call))
            {
                MessageBox.Show("Failed to auto-detect PI to get callsign.", "Automatic Recognization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Attempt to parse
            string artist = "SPOTS";
            string title = "SPOTS";
            bool matched = false;
            if (rds.rdsModes[rdsPatchMethod.SelectedIndex].TryParse(rt, out string trackTitle, out string trackArtist, out string stationName, false))
            {
                //Update fields
                artist = trackArtist;
                title = trackTitle;
                matched = true;

                //Update metadata
                matchedRds = rt.value;
                matchedRdsParser = (int)rds.rdsModes[rdsPatchMethod.SelectedIndex].Id;
            }

            //If matched to a song, set it as song. Otherwise set it as spot
            if (matched)
            {
                typeBtnSong.Checked = true;
                typeBtnLiner.Checked = false;
            }
            else
            {
                typeBtnLiner.Checked = true;
                typeBtnSong.Checked = false;
            }

            //Apply to fields
            if (!lockCall)
                inputCall.Text = call + "-FM";
            inputArtist.Text = artist;
            inputTitle.Text = title;

            //Update the button
            UpdateAddBtnStatus();
        }

        private void SelectFromRdsOffset(int offset)
        {
            //Get new index
            int newIndex = lastSelectFromRdsIndex + offset;

            //Search until we find one of substancial length
            while (true)
            {
                //Check if it is in bounds
                if (newIndex < 0)
                {
                    MessageBox.Show("This is the first RDS frame.", "Select From RDS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newIndex >= rds.ParsedRtFrames.Count)
                {
                    MessageBox.Show("This is the last RDS frame.", "Select From RDS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Get frame
                RdsValue<string> frame = rds.ParsedRtFrames[newIndex];

                //Check that it is at least a few seconds long
                if (frame.last - frame.first >= AUDIO_SAMPLE_RATE * 20)
                {
                    //Jump
                    SelectFromRds(rds.ParsedRtFrames[newIndex]);
                    break;
                } else
                {
                    //Add to the index and try again
                    newIndex += offset;
                }
            }
        }

        private void btnAutoRds_Click(object sender, EventArgs e)
        {
            //Autodetect
            RdsValue<string> rt = rds.GetRtAtSample(transportControls.StreamAudio.Position);
            if (rt == null)
            {
                MessageBox.Show("Failed to auto-detect RDS.", "Automatic Recognization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Select
            SelectFromRds(rt);
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
            RefreshClipsGrid();
        }

        public Color ClipListSummaryColorShort { get; set; }
        public Color ClipListSummaryColorMedium { get; set; }
        public Color ClipListSummaryColorLong { get; set; }

        public const int MAX_CLIP_GRID_ITEMS = 100;

        private void RefreshClipsGrid()
        {
            //Get query info and check if it's even worth searching.
            string currentCall = inputCall.Text.ToUpper();
            string currentArtist = GetEntryArtist().ToUpper();
            string currentTitle = GetEntryTitle().ToUpper();
            bool searchPerformed = (currentArtist.Length > 0 || currentTitle.Length > 0) && currentCall.Length > 0;
            TrackClipInfo[] clips = new TrackClipInfo[0];
            if (searchPerformed)
            {
                //Find all clips
                clips = db.Clips.Where(
                        x => x.Station.ToUpper() == currentCall &&
                        x.Artist.ToUpper() == currentArtist &&
                        x.Title.ToUpper() == currentTitle
                    ).OrderByDescending(x => x.Time).ToArray();
            }

            //Set summary
            if (clips.Length > 0)
            {
                //Find date closest to the current playhead position
                DateTime playheadTime = CalculateOffsetTime(transportControls.StreamAudio.Position);
                DateTime nearestEventTime = clips.Select(x => x.Time).FindNearest(playheadTime);
                TimeSpan distance = playheadTime - nearestEventTime;
                int distanceDays = (int)distance.Abs().TotalDays;
                string distanceString = distanceDays.ToString() + " day" + (distanceDays == 1 ? "" : "s");

                //Update text
                itemsListSummaryLabel.Text = $"{clips.Length.ToStringPlural("clip")} found, nearest on {nearestEventTime.ToShortDateString()} ({distanceDays.ToStringPlural("day")} {(distance > TimeSpan.Zero ? "ago" : "ahead")}).";

                //Determine color
                Color textColor;
                if (distanceDays > 1000)
                    textColor = Color.FromArgb(245, 66, 66); //red
                else if (distanceDays > 400)
                    textColor = Color.FromArgb(227, 110, 7); //orange
                else if (distanceDays > 190)
                    textColor = Color.FromArgb(40, 89, 250); //blue
                else
                    textColor = Color.Black;

                //Apply color
                itemsListSummaryLabel.ForeColor = textColor;
            } else if (searchPerformed)
            {
                itemsListSummaryLabel.ForeColor = Color.FromArgb(245, 66, 66);
                itemsListSummaryLabel.Text = "No matching items found.";
            } else
            {
                itemsListSummaryLabel.Text = "";
            }

            //Show in grid if within a reasonable number
            if (clips.Length < MAX_CLIP_GRID_ITEMS)
            {
                //Show
                clipGrid1.AddClips(clips, true);
                if (!clipGrid1.Visible)
                    clipGrid1.Visible = true;
            } else
            {
                //Clear grid and hide it
                clipGrid1.AddClips(new TrackClipInfo[0], true);
                if (clipGrid1.Visible)
                    clipGrid1.Visible = false;
                itemsListSummaryLabel.Text += $" Clip count >= {MAX_CLIP_GRID_ITEMS}, list disabled for performance.";
            }
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

        /// <summary>
        /// Removes the active file from the list and goes to the next one.
        /// </summary>
        public void OpenNextFile()
        {
            //Pop current file from queue
            if (lastFileItem != null)
                filesToolStripMenuItem.DropDownItems.Remove(lastFileItem);

            //Check if empty
            if (filesToolStripMenuItem.DropDownItems.Count == 0)
            {
                //Done
                MessageBox.Show("All files have been edited!");
                Close();
                return;
            }

            //Open
            ChangeFile(filesToolStripMenuItem.DropDownItems[0] as ToolStripMenuItem);
        }

        private ToolStripMenuItem lastFileItem; // The item in the menu for the last file opened

        /// <summary>
        /// Changes files without removing them from the list.
        /// </summary>
        /// <param name="item"></param>
        public void ChangeFile(ToolStripMenuItem item)
        {
            //Get filename
            string filename = (string)item.Tag;

            //Uncheck old
            if (lastFileItem != null)
                lastFileItem.Checked = false;

            //Set as checked
            item.Checked = true;
            lastFileItem = item;

            //Open
            OpenFile(filename);
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
            UpdateRecordingTimeLabel();
        }

        private void UpdateRecordingTimeLabel()
        {
            DateTime time = CalculateOffsetTime(transportControls.StreamAudio.Position);
            recordingTimeLabel.Text = showDate ? time.ToShortDateString() : time.ToLongTimeString();
        }

        private void recordingTimeLabel_Click(object sender, EventArgs e)
        {
            showDate = !showDate;
            UpdateRecordingTimeLabel();
        }

        private void rdsPatchMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            rds.SwitchPatcher(rds.rdsModes[rdsPatchMethod.SelectedIndex], this);
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

        //

        private void nextRDSItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Change selection
            SelectFromRdsOffset(1);

            //Shift suffix -> prefix
            inputPrefix.Text = inputSuffix.Text;
            inputSuffix.Text = "";

            //Validate
            UpdateAddBtnStatus();
        }

        private void previousRDSItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Change selection
            SelectFromRdsOffset(-1);

            //Shift prefix -> suffix
            inputSuffix.Text = inputPrefix.Text;
            inputPrefix.Text = "";

            //Validate
            UpdateAddBtnStatus();
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

        private void rawPIFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportRdsFrames(rds.RawPiFrames.Select(x => new RdsValue<string>(x.first, x.last, x.value.ToString())));
        }

        private void ExportRdsFrames(IEnumerable<RdsValue<string>> frames)
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

        public DateTime GetTimeOfFrameStart<T>(RdsValue<T> value)
        {
            return CalculateOffsetTime(value.first);
        }

        public DateTime GetTimeOfFrameEnd<T>(RdsValue<T> value)
        {
            return CalculateOffsetTime(value.last);
        }

        public long GetSampleFromTime(DateTime time)
        {
            return transportControls.TimeToSample(time);
        }

        private void btnFileDelete_Click(object sender, EventArgs e)
        {
            //Check if the user really wants to delete the file
            if (MessageBox.Show($"Are you sure you want to delete {wavFile.Name}?", "Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            //Close file
            CloseFile();

            //Wait a sec for threads to catch up
            Thread.Sleep(500);

            //Delete the metadata file
            infoFile.Delete();

            //Delete WAV file
            wavFile.Delete();

            //Finally, open the next file
            OpenNextFile();
        }

        private void btnFileSave_Click(object sender, EventArgs e)
        {
            //Prompt if the user wants to delete the original
            bool delete;
            switch (MessageBox.Show("Would you like to delete the original file after clips are saved?", "Finish Edit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
            {
                case DialogResult.Yes:
                    delete = true;
                    break;
                case DialogResult.No:
                    delete = false;
                    break;
                default:
                    return;
            }

            //Close file
            CloseFile();

            //Wait a sec for threads to catch up
            Thread.Sleep(500);

            //Delete the metadata file
            infoFile.Delete();

            //Finalize the post file
            outputWriter.Finalize(delete);

            //Finally, open the next file
            OpenNextFile();
        }

        private void btnLockCall_Click(object sender, EventArgs e)
        {
            lockCall = !lockCall;
            btnLockCall.Text = lockCall ? "Unlock" : "Lock";
        }

        private void tipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EditorTipsForm().ShowDialog();
        }
    }
}
