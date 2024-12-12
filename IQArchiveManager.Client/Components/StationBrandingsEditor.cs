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
    public partial class StationBrandingsEditor : Form
    {
        public StationBrandingsEditor()
        {
            InitializeComponent();
        }

        public string[] Brandings
        {
            get
            {
                string[] entries = editBox.Text.Split('\n');
                for (int i = 0; i < entries.Length; i++)
                    entries[i] = entries[i].Trim('\r');
                return entries;
            }
            set
            {
                string text = "";
                string[] lines = value;
                for (int i = 0; i< lines.Length; i++)
                {
                    text += lines[i];
                    if (i + 1 != lines.Length)
                        text += "\r\n";
                }
                editBox.Text = text;
            }
        }
    }
}
