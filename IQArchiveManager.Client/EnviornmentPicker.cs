using IQArchiveManager.Client.Components;
using IQArchiveManager.Client.Db;
using IQArchiveManager.Client.RdsModes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using static System.Net.WebRequestMethods;

namespace IQArchiveManager.Client
{
    public partial class EnviornmentPicker : Form
    {
        public EnviornmentPicker(IQEnviornment env)
        {
            this.env = env;
            InitializeComponent();
        }

        private readonly IQEnviornment env;

        private void EnviornmentPicker_Load(object sender, EventArgs e)
        {
            //Create options
            InitOption(comboBox1, env.IqaDirs);
            InitOption(comboBox2, env.EditDirs);
            InitOption(comboBox3, env.MoveDirs);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Close();
            //Apply
            //RdsPatchNoDelimiters.BRANDING_REMOVAL = Brandings;
        }

        private bool updatingOption = false;

        private void InitOptionValues(ComboBox box, EnviornmentPath paths)
        {
            //Suspend
            updatingOption = true;
            box.SuspendLayout();

            //Clear
            box.Items.Clear();

            //Add all saved paths
            foreach (var p in paths.Paths)
                box.Items.Add((string)p);

            //Add "add" button
            box.Items.Add("[Add New Path...]");

            //Set selected
            box.SelectedIndex = paths.SelectedIndex;

            //Resume
            box.ResumeLayout();
            updatingOption = false;
        }

        private void InitOption(ComboBox box, EnviornmentPath paths)
        {
            //Set up combobox
            box.Tag = paths;

            //Add events
            box.SelectedIndexChanged += Box_SelectedIndexChanged;

            //Set items
            InitOptionValues(box, paths);
        }

        private void Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Ignore if flag is set
            if (updatingOption)
                return;

            //Get components
            ComboBox box = (ComboBox)sender;
            EnviornmentPath paths = (EnviornmentPath)box.Tag;

            //Check if we're not adding a new path
            updatingOption = true;
            if (box.SelectedIndex < paths.Paths.Count)
            {
                paths.SelectedIndex = box.SelectedIndex;
            }
            else
            {
                //Open picker
                FolderBrowserDialog fd = new FolderBrowserDialog();
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    paths.Paths.Add(fd.SelectedPath + Path.DirectorySeparatorChar);
                    paths.SelectedIndex = paths.Paths.Count - 1;
                    InitOptionValues(box, paths);
                }
                else
                {
                    box.SelectedIndex = -1;
                }
            }
            updatingOption = false;
        }
    }
}
