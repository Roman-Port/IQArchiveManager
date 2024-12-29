using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IQArchiveManager.Client.RDS.Modes.Csv
{
    public partial class CsvProfileConfigForm : Form
    {
        public CsvProfileConfigForm(CsvProfile[] profiles)
        {
            initialProfiles = profiles;
            InitializeComponent();
        }

        public CsvProfileConfigForm()
        {
            initialProfiles = new CsvProfile[0];
            InitializeComponent();
        }

        private readonly CsvProfile[] initialProfiles;

        public CsvProfile[] Profiles
        {
            get
            {
                List<CsvProfile> result = new List<CsvProfile>();
                for (int i = configPanel.Controls.Count - 1; i >= 0; i--)
                {
                    if (configPanel.Controls[i] is CsvProfileEditor e && e.IsValid)
                        result.Add(e.Data);
                }
                return result.ToArray();
            }
        }

        private void CsvProfileConfigForm_Load(object sender, EventArgs e)
        {
            //Add all profiles (in reverse order)
            foreach (var p in initialProfiles.Reverse())
            {
                CsvProfileEditor edit = new CsvProfileEditor();
                edit.Data = p;
                edit.Dock = DockStyle.Top;
                configPanel.Controls.Add(edit);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CsvProfileEditor edit = new CsvProfileEditor();
            edit.Dock = DockStyle.Top;
            configPanel.Controls.Add(edit);
        }
    }
}
