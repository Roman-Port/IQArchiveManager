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
    public partial class FolderListPickerItem : UserControl
    {
        public FolderListPickerItem()
        {
            InitializeComponent();
        }

        public event Action<FolderListPickerItem> ControlButtonClicked;

        public string BtnText
        {
            get => controlButton.Text;
            set => controlButton.Text = value;
        }

        public string EditBoxText
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public bool DisableControlOnEmpty { get; set; }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            RefreshTextBoxEnabledStatus();
        }

        private void controlButton_Click(object sender, EventArgs e)
        {
            ControlButtonClicked?.Invoke(this);
        }

        private void FolderListPickerItem_Load(object sender, EventArgs e)
        {
            RefreshTextBoxEnabledStatus();
        }

        private void RefreshTextBoxEnabledStatus()
        {
            controlButton.Enabled = textBox.Text.Length > 0 || !DisableControlOnEmpty;
        }

        private void dirButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
                textBox.Text = fd.SelectedPath;
        }
    }
}
