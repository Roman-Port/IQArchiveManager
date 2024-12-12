using IQArchiveManager.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.Components
{
    public delegate void ClipGrid_SelectedItemChangedEventArgs(ClipGrid grid, TrackClipInfo clip);

    public partial class ClipGrid : UserControl
    {
        public ClipGrid()
        {
            InitializeComponent();
        }

        private static readonly Color COLOR_GOOD = Color.FromArgb(82, 237, 62);
        private static readonly Color COLOR_BAD = Color.FromArgb(237, 68, 62);
        private static readonly Color COLOR_NA = Color.FromArgb(219, 217, 217);

        private const int MIN_DISPLAYED_SNR_VERSION = 1;

        private const float SNR_QUALITY_MIN = 10;
        private const float SNR_QUALITY_MAX = 50;

        private const float SNR_COLOR_HUE_RANGE = 126;

        private List<TrackClipInfo> items = new List<TrackClipInfo>();
        private bool isMini = false;

        public bool IsMini
        {
            get => isMini;
            set => ApplyMini(value);
        }

        public TrackClipInfo SelectedItem { get => dataGridView1.SelectedRows.Count == 1 ? (TrackClipInfo)dataGridView1.SelectedRows[0].Tag : null; }
        public IReadOnlyList<TrackClipInfo> CurrentItems => items;

        public event ClipGrid_SelectedItemChangedEventArgs OnSelectionChanged;

        public void Clear()
        {
            items.Clear();
            dataGridView1.Rows.Clear();
        }

        public void AddClips(IEnumerable<TrackClipInfo> clips, bool clear = false)
        {
            dataGridView1.SuspendLayout();
            if (clear)
                Clear();
            items.AddRange(clips);
            foreach (var c in clips)
                AddClip(c);
            dataGridView1.ResumeLayout();
        }

        public void AddClip(TrackClipInfo clip)
        {
            int index = dataGridView1.Rows.Add();
            dataGridView1.Rows[index].Cells["station"].Value = clip.Station;
            dataGridView1.Rows[index].Cells["title"].Value = clip.Title;
            dataGridView1.Rows[index].Cells["artist"].Value = clip.Artist;
            dataGridView1.Rows[index].Cells["prefix"].Value = clip.Prefix;
            dataGridView1.Rows[index].Cells["suffix"].Value = clip.Suffix;
            dataGridView1.Rows[index].Cells["date"].Value = clip.Time;
            dataGridView1.Rows[index].Cells["time"].Value = clip.Time;
            ConfigureSnrCell(dataGridView1.Rows[index].Cells["SNR"], clip.Snr);
            ConfigureBoolCell(dataGridView1.Rows[index].Cells["ok"], clip.FlagOk);
            ConfigureBoolCell(dataGridView1.Rows[index].Cells["hd"], clip.FlagHd);
            ConfigureBoolCell(dataGridView1.Rows[index].Cells["rds"], clip.FlagRds);
            dataGridView1.Rows[index].Cells["radio"].Value = clip.GetRadioString();
            dataGridView1.Rows[index].Cells["id"].Value = clip.Id;
            dataGridView1.Rows[index].Cells["notes"].Value = clip.Notes;
            dataGridView1.Rows[index].Tag = clip;
        }

        private void ConfigureBoolCell(DataGridViewCell cell, bool flag)
        {
            cell.Value = flag ? "✓" : "✗";
            cell.Style.BackColor = flag ? COLOR_GOOD : COLOR_BAD;
        }

        private void ConfigureSnrCell(DataGridViewCell cell, TrackClipInfoSnr info)
        {
            //Do some checks
            if (info == null)
            {
                cell.Value = "Unavailable";
                cell.Style.BackColor = COLOR_NA;
                return;
            }
            if (info.Version < MIN_DISPLAYED_SNR_VERSION)
            {
                cell.Value = "Outdated";
                cell.Style.BackColor = COLOR_NA;
                return;
            }

            //Format string
            cell.Value = info.Snr.ToString("00.0") + " dB";

            //Make background
            cell.Style.BackColor = ColorFromHSV(GetNormalizedSnrQuality(info.Snr) * SNR_COLOR_HUE_RANGE, 0.66, 0.99);
        }

        private float GetNormalizedSnrQuality(float snr)
        {
            if (float.IsInfinity(snr))
                return 1;
            float quality = (snr - SNR_QUALITY_MIN) / (SNR_QUALITY_MAX - SNR_QUALITY_MIN);
            if (quality > 1)
                quality = 1;
            if (quality < 0)
                quality = 0;
            return quality;
        }

        private void ApplyMini(bool value)
        {
            isMini = value;
            dataGridView1.Columns["station"].Visible = !value;
            dataGridView1.Columns["title"].Visible = !value;
            dataGridView1.Columns["artist"].Visible = !value;
            dataGridView1.Columns["time"].Visible = !value;
            dataGridView1.Columns["id"].Visible = !value;
        }

        private void ClipGrid_Load(object sender, EventArgs e)
        {
            Clear();
            ApplyMini(isMini);

            foreach (var c in dataGridView1.Columns)
                ((DataGridViewColumn)c).AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            OnSelectionChanged?.Invoke(this, SelectedItem);
        }

        //https://stackoverflow.com/questions/1335426/is-there-a-built-in-c-net-system-api-for-hsv-to-rgb
        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
