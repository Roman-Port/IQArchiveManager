using IQArchiveManager.Client.Components;
using IQArchiveManager.Client.Db;
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

namespace IQArchiveManager.Client
{
    public partial class EnviornmentPicker : Form
    {
        public EnviornmentPicker(IQDirectories env)
        {
            this.env = env;
            InitializeComponent();
        }

        private readonly IQDirectories env;

        private void EnviornmentPicker_Load(object sender, EventArgs e)
        {
            //Initialize lists
            iqaPicker.Directories = env.IqaDirs.ToArray();
            editPicker.Directories = env.EditDirs.ToArray();

        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            //Set
            env.IqaDirs = iqaPicker.Directories.ToList();
            env.EditDirs = editPicker.Directories.ToList();

            //Exit
            Close();
        }
    }
}
