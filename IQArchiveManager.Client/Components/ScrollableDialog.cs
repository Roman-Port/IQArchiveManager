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
    public partial class ScrollableDialog : Form
    {
        public ScrollableDialog(string title, string body)
        {
            InitializeComponent();
            Text = title;
            viewBox.Text = body;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void ScrollableDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
