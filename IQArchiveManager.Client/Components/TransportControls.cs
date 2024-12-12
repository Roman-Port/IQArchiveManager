using IQArchiveManager.Client.Pre;
using RomanPort.LibSDR.Components;
using RomanPort.LibSDR.UI.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.Components
{
    public unsafe partial class TransportControls : UserControl
    {
        public TransportControls()
        {
            InitializeComponent();
        }

        private int bufferWidth;
        private int bufferHeight;
        private UnsafeBuffer imageBuffer;
        private UnsafeColor* imageBufferPtr;

        private int[] audioPixelsMax;
        private int[] audioPixelsMin;
        private int[] audioPixelsToken;
        private bool[] audioPixelsLoaded;
        private bool[] audioPixelsQueued;

        private static readonly UnsafeColor COLOR_BLACK = new UnsafeColor(0, 0, 0, 255);
        private static readonly UnsafeColor COLOR_WHITE = new UnsafeColor(255, 255, 255, 255);
        private static readonly UnsafeColor COLOR_TRANSPARENT = new UnsafeColor(0, 0, 0, 0);
        private static readonly UnsafeColor COLOR_BLUE = new UnsafeColor(56, 130, 220);
        private static readonly UnsafeColor COLOR_BLUE_DARK = new UnsafeColor(47, 96, 156);
        private static readonly UnsafeColor COLOR_BLUE_DARK_DARK = new UnsafeColor(29, 61, 101);

        private PreProcessorFileStreamReader thumbStream;
        private PreProcessorFileStreamReader audioStream;
        private PreProcessorFileStreamReader fftStream;

        private long leftSample;
        private long rightSample;
        private long zoomCenter;
        private long zoomLevel;
        private long selectionStartSample = 0;
        private long selectionStopSample = 0;

        private bool active = true;
        private byte[] audioBuffer = new byte[2048];
        private ConcurrentQueue<QueuedThumbnailRead> readQueue = new ConcurrentQueue<QueuedThumbnailRead>();
        private Thread workerThread;
        private DateTime sourceBegin;
        private DateTime sourceEnd;

        private int lastMouseCursorX = -1;

        public double EditStartSeconds { get => (double)selectionStartSample / 20000; }
        public double EditEndSeconds { get => (double)selectionStopSample / 20000; }
        public event PreProcessorFileStreamReader_Event TimeChanged;

        public PreProcessorFileStreamReader StreamThumb => thumbStream;
        public PreProcessorFileStreamReader StreamAudio => audioStream;
        public PreProcessorFileStreamReader StreamFft => fftStream;

        private const long SCROLLBAR_SCALE_FACTOR = 1;
        private const int THUMB_SKIP = 3;

        public void SetStreams(DateTime sourceLastModified, PreProcessorFileStreamReader thumbStream, PreProcessorFileStreamReader audioStream, PreProcessorFileStreamReader fftStream)
        {
            //Set
            this.thumbStream = thumbStream;
            this.audioStream = audioStream;
            this.fftStream = fftStream;

            //Compute timestamps
            sourceBegin = sourceLastModified.AddSeconds(audioStream.Length / -20000.0);
            sourceEnd = sourceLastModified;

            //Add event
            audioStream.OnSegmentChanged += AudioStream_OnSegmentChanged;

            //Reset zoom
            zoomCenter = thumbStream.Length / 2;
            zoomLevel = 1;
            ApplyNewZoom();

            //Update image
            ResizeImage();
        }

        private void AudioStream_OnSegmentChanged(PreProcessorFileStreamReader reader)
        {
            //Set time in UI
            timeSpanChooserCurrent.Value = TimeSpan.FromSeconds((double)reader.Position / 20000);

            //Update image
            UpdateImage();

            //Sync FFT (at least close enough)
            fftStream.CurrentSegment = reader.CurrentSegment;

            //Send
            TimeChanged?.Invoke(reader);
        }

        public void ChangeZoom(int direction)
        {
            //Update
            zoomLevel = Math.Max(1, zoomLevel + direction);

            //Apply
            ApplyNewZoom();
            UpdateImage();
        }

        private void ApplyNewZoom()
        {
            //Calculate the scaled zoom level
            long scaledZoomLevel = (long)(thumbStream.Length / 2.0 / Math.Pow(zoomLevel, 0.5));
            
            //Constrain
            zoomCenter = Math.Max(zoomCenter, scaledZoomLevel);
            zoomCenter = Math.Min(zoomCenter, thumbStream.Length - scaledZoomLevel);

            //Calculate values
            long newLeft = zoomCenter - scaledZoomLevel;
            long newRight = zoomCenter + scaledZoomLevel;

            //Apply to scrollbar
            hScrollBar.Minimum = (int)(scaledZoomLevel / SCROLLBAR_SCALE_FACTOR);
            hScrollBar.Maximum = (int)((thumbStream.Length - scaledZoomLevel) / SCROLLBAR_SCALE_FACTOR);
            hScrollBar.Value = (int)(zoomCenter / SCROLLBAR_SCALE_FACTOR);

            //Apply
            leftSample = newLeft;
            rightSample = newRight;
            InvalidateAllSegments();
        }

        private void TransportControls_Load(object sender, EventArgs e)
        {
            //Create worker
            workerThread = new Thread(ThumbnailLoaderWorker);
            workerThread.Name = "Thumbnail Loader Worker";
            workerThread.Start();
        }

        private void ResizeImage()
        {
            //Validate
            if (Width <= 0 || audioStream == null || thumbStream == null)
                return;
            
            //Create arrays
            audioPixelsMax = new int[Width];
            audioPixelsMin = new int[Width];
            audioPixelsToken = new int[Width];
            audioPixelsLoaded = new bool[Width];
            audioPixelsQueued = new bool[Width];

            //Set
            bufferWidth = Width;
            bufferHeight = mainView.Height;

            //Clean up
            mainView.Image?.Dispose();
            imageBuffer?.Dispose();

            //Create image
            imageBuffer = UnsafeBuffer.Create(bufferWidth * bufferHeight, out imageBufferPtr);

            //Apply
            mainView.Image = new Bitmap(bufferWidth, bufferHeight, bufferWidth * sizeof(UnsafeColor), System.Drawing.Imaging.PixelFormat.Format32bppArgb, (IntPtr)imageBufferPtr);
            mainView.Width = bufferWidth;
            mainView.Height = bufferHeight;

            //Prepare
            UpdateImage();
        }

        private void InvalidateAllSegments()
        {
            for (int i = 0; i < bufferWidth; i++)
            {
                audioPixelsToken[i]++;
                audioPixelsQueued[i] = false;
                audioPixelsLoaded[i] = false;
            }
        }

        private void ReadMissingSegments()
        {
            for (int i = 0; i < Width; i += THUMB_SKIP)
            {
                if (audioPixelsQueued[i])
                    continue;
                ReadSegment(i);
            }
        }

        private void ReadSegment(int x)
        {
            //Mark
            audioPixelsLoaded[x] = false;
            audioPixelsQueued[x] = true;
            audioPixelsToken[x]++;

            //Queue
            readQueue.Enqueue(new QueuedThumbnailRead
            {
                index = x,
                token = audioPixelsToken[x]
            });
        }

        public void UpdateImage()
        {
            //Read semgents
            ReadMissingSegments();
            
            //Draw
            UnsafeColor* ptr;
            bool hasCursor = false;
            long pos = audioStream.Position;
            for (int x = 0; x < bufferWidth; x++)
            {
                //Set
                ptr = imageBufferPtr + x;

                //Check cursor
                bool isCursor = (PixelToSample(x) > pos || x == (bufferWidth - 1)) && !hasCursor;
                hasCursor = hasCursor || isCursor;

                //Check if this is selected
                bool isSelected = PixelToSample(x) > selectionStartSample && PixelToSample(x) < selectionStopSample;

                //Draw
                for (int y = 0; y < bufferHeight; y++)
                {
                    if (isCursor)
                        *ptr = COLOR_BLACK;
                    else if (y < audioPixelsMin[x] || !audioPixelsLoaded[x])
                        *ptr = COLOR_WHITE;
                    else if (y < audioPixelsMax[x])
                        *ptr = isSelected ? COLOR_BLUE : COLOR_BLUE_DARK;
                    else
                        *ptr = COLOR_WHITE;
                    ptr += bufferWidth;
                }
            }

            //Update
            mainView.Invalidate();
        }

        private long PixelToSample(double pixel)
        {
            return (long)((rightSample - leftSample) * (pixel / bufferWidth)) + leftSample;
        }

        private void mainView_MouseMove(object sender, MouseEventArgs e)
        {
            //Make sure the mouse has actually moved
            if (e.X == lastMouseCursorX)
                return;

            //Set
            lastMouseCursorX = e.X;
            audioStream.Position = PixelToSample(e.X);
        }

        private void mainView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectionStartSample = PixelToSample(e.X);
                timeSpanChooserStart.Value = TimeSpan.FromSeconds((double)selectionStartSample / 20000);
                UpdateImage();
            }
            if (e.Button == MouseButtons.Right)
            {
                selectionStopSample = PixelToSample(e.X);
                timeSpanChooserEnd.Value = TimeSpan.FromSeconds((double)selectionStopSample / 20000);
                UpdateImage();
            }
        }

        private void mainView_Resize(object sender, EventArgs e)
        {
            
        }

        private void TransportControls_Resize(object sender, EventArgs e)
        {
            ResizeImage();
        }

        private void timeSpanChooserCurrent_OnValueChanged(object sender, EventArgs e)
        {
            audioStream.Position = Math.Min(audioStream.Length - 1, (long)(timeSpanChooserCurrent.Value.TotalSeconds * 20000));
            UpdateImage();
        }

        private void timeSpanChooserStart_OnValueChanged(object sender, EventArgs e)
        {
            selectionStartSample = (long)(timeSpanChooserStart.Value.TotalSeconds * 20000);
            UpdateImage();
        }

        private void timeSpanChooserEnd_OnValueChanged(object sender, EventArgs e)
        {
            selectionStopSample = (long)(timeSpanChooserEnd.Value.TotalSeconds * 20000);
            UpdateImage();
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            zoomCenter = hScrollBar.Value * SCROLLBAR_SCALE_FACTOR;
            ApplyNewZoom();
            UpdateImage();
        }

        private void ThumbnailLoaderWorker()
        {
            QueuedThumbnailRead item;
            Stopwatch tss = new Stopwatch();
            tss.Start();
            while (active)
            {
                //Attempt to load item
                if (!readQueue.TryDequeue(out item) || thumbStream == null)
                {
                    Thread.Sleep(100);
                    continue;
                }

                //Make sure tokens match
                if (item.token != audioPixelsToken[item.index])
                    continue;

                //Get the sample to draw
                long drawSample = PixelToSample(item.index);

                //Seek and read
                thumbStream.Position = drawSample;
                thumbStream.Read(audioBuffer, 0, audioBuffer.Length);

                //Get the max/min sample
                float max = 0;
                float min = 0;
                float sample;
                for (int i = 0; i < audioBuffer.Length; i++)
                {
                    sample = GeneralUtils.ConvertAudioSample(audioBuffer[i]);
                    max = Math.Max(max, sample);
                    min = Math.Min(min, sample);
                }

                //Convert
                for (int i = 0; i < THUMB_SKIP && item.index + i < bufferWidth; i++)
                {
                    audioPixelsMax[item.index + i] = (mainView.Height / 2) + (int)((mainView.Height / 2f) * max);
                    audioPixelsMin[item.index + i] = (mainView.Height / 2) + (int)((mainView.Height / 2f) * min);
                    audioPixelsLoaded[item.index + i] = true;
                }

                //Refresh
                if(readQueue.IsEmpty || tss.ElapsedMilliseconds > 10)
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        UpdateImage();
                    });
                    tss.Restart();
                }
            }
        }

        public bool AutoDetectRange(RdsReader rds, out string rt)
        {
            //Get
            rt = rds.GetRtAtSample(audioStream.Position, out long start, out long end);

            //Expand range to give us some leeway
            start = Math.Max(0, start - (15 * 20000));
            end = Math.Min(audioStream.Length, end + (10 + 20000));

            //Update
            SetSelectionRegion(start, end);

            return rt != null;
        }

        public void SetSelectionRegion(long startSample, long endSample)
        {
            //Bounds check
            startSample = Math.Max(0, startSample);
            endSample = Math.Min(audioStream.Length, endSample);

            //Set start
            selectionStartSample = startSample;
            timeSpanChooserStart.Value = TimeSpan.FromSeconds(startSample / 20000);

            //Set end
            selectionStopSample = endSample;
            timeSpanChooserEnd.Value = TimeSpan.FromSeconds(endSample / 20000);

            //Set current
            audioStream.Position = startSample;

            //Refresh
            UpdateImage();
        }

        public void SetSelectionRegion(DateTime start, DateTime end)
        {
            SetSelectionRegion(TimeToSample(start), TimeToSample(end));
        }

        /// <summary>
        /// Converts a timestamp to a sample within the recording.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public long TimeToSample(DateTime time)
        {
            return (long)((time - sourceBegin).TotalSeconds * 20000.0);
        }

        /// <summary>
        /// Converts a sample within the recording to a timestamp.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public DateTime SampleToTime(long time)
        {
            return sourceBegin.AddSeconds(time / 20000.0);
        }

        public void Close()
        {
            active = false;
        }

        private void Nudge(TimeSpanChooser target, TimeSpan nudge, TimeSpan playbackOffset)
        {
            //Add the nudge amount
            TimeSpan v = target.Value + nudge;
            target.Value = v;
            target.SendUpdateEvent();

            //Change playback position
            v += playbackOffset;
            timeSpanChooserCurrent.Value = v;
            timeSpanChooserCurrent.SendUpdateEvent();
        }

        struct QueuedThumbnailRead
        {
            public int index;
            public int token;
        }

        private static readonly TimeSpan PLAYHEAD_OFFSET = TimeSpan.FromSeconds(8);

        public void SetStartToCurrent()
        {
            timeSpanChooserStart.Value = timeSpanChooserCurrent.Value - PLAYHEAD_OFFSET;
            timeSpanChooserStart.SendUpdateEvent();
        }

        public void SetEndToCurrent()
        {
            timeSpanChooserEnd.Value = timeSpanChooserCurrent.Value + PLAYHEAD_OFFSET;
            timeSpanChooserEnd.SendUpdateEvent();
        }

        public void NudgeStart(TimeSpan amount)
        {
            Nudge(timeSpanChooserStart, amount, PLAYHEAD_OFFSET);
        }

        public void NudgeCurrent(TimeSpan amount)
        {
            timeSpanChooserCurrent.Value += amount;
            timeSpanChooserCurrent.SendUpdateEvent();
        }

        public void NudgeEnd(TimeSpan amount)
        {
            Nudge(timeSpanChooserEnd, amount, -PLAYHEAD_OFFSET);
        }

        public void MovePlayheadToStart()
        {
            timeSpanChooserCurrent.Value = timeSpanChooserStart.Value;
            timeSpanChooserCurrent.SendUpdateEvent();
        }

        public void MovePlayheadToEnd()
        {
            timeSpanChooserCurrent.Value = timeSpanChooserEnd.Value;
            timeSpanChooserCurrent.SendUpdateEvent();
        }
    }
}
