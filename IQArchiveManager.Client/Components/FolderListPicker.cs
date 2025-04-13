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
    public partial class FolderListPicker : UserControl
    {
        public FolderListPicker()
        {
            InitializeComponent();
        }

        public string[] Directories
        {
            get
            {
                List<string> dirs = new List<string>();
                for (int i = 0; i < dirListPanel.Controls.Count; i++)
                {
                    string dir = ((FolderListPickerItem)dirListPanel.Controls[i]).EditBoxText;
                    if (dir.Length > 0)
                        dirs.Add(dir);
                }
                return dirs.ToArray();
            }
            set
            {
                dirListPanel.SuspendLayout();
                dirListPanel.Controls.Clear();
                foreach (var d in value)
                {
                    if (d.Length > 0)
                        AddDirEntry(d);
                }
                dirListPanel.PerformLayout();
                dirListPanel.ResumeLayout();
            }
        }

        private void FolderListPicker_Load(object sender, EventArgs e)
        {
            
        }

        private void AddDirEntry(string filename)
        {
            FolderListPickerItem entry = new FolderListPickerItem
            {
                BtnText = "-",
                EditBoxText = filename,
                DisableControlOnEmpty = false,
                Dock = DockStyle.Top
            };
            dirListPanel.Controls.Add(entry);
            entry.ControlButtonClicked += ExitingItemControlButtonClicked;
        }

        private void ExitingItemControlButtonClicked(FolderListPickerItem obj)
        {
            dirListPanel.Controls.Remove(obj);
        }

        private void addFolderSection_ControlButtonClicked(FolderListPickerItem obj)
        {
            AddDirEntry(obj.EditBoxText);
            obj.EditBoxText = string.Empty;
        }
    }
}
