using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.RDS.Modes.Csv
{
    public partial class CsvProfileEditor : UserControl
    {
        public CsvProfileEditor()
        {
            InitializeComponent();
        }

        public bool IsValid => ushort.TryParse(boxPi.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out ushort value);

        public CsvProfile Data
        {
            get
            {
                return new CsvProfile
                {
                    PiCode = ushort.Parse(boxPi.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture),
                    Name = boxLabel.Text,
                    PathTemplate = boxPath.Text,
                    ColumnTime = (int)colTime.Value,
                    ColumnArtist = (int)colArtist.Value,
                    ColumnTitle = (int)colTitle.Value,
                    EomPsTriggers = boxEom.Text,
                    EomGrace = (int)(valGrace.Value * 1000),
                    Offset = (int)(valDelay.Value * 1000)
                };
            }
            set
            {
                boxPi.Text = value.PiCode.ToString("X");
                boxLabel.Text = value.Name;
                boxPath.Text = value.PathTemplate;
                colTime.Value = value.ColumnTime;
                colArtist.Value = value.ColumnArtist;
                colTitle.Value = value.ColumnTitle;
                boxEom.Text = value.EomPsTriggers;
                valGrace.Value = (decimal)value.EomGrace / 1000;
                valDelay.Value = (decimal)value.Offset / 1000;
            }
        }

        private void btnPathHelp_Click(object sender, EventArgs e)
        {
            string resolved = RdsPatchCsv.ResolveTemplate(boxPath.Text, 2024, 4, 25);
            if (resolved.Length != 0)
                resolved = "The current value would resolve to the following on April 25th, 2024:\n" + resolved;
            else
                resolved = "You can click the help button after completing the path for a preview.";
            MessageBox.Show("This is the filename that will be loaded for each day respectively. It is a filename but with the year, month, and day resolved.\nIt uses the following characters as templates:\n\n* %y - Year (4 digits)\n* %m - Month (2 digits)\n* %d - Day (2 digits)\n\n" + resolved, "Path Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
