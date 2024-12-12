using System;
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
    public delegate DialogResult WorkerDialog_Work(IWorkerDialogControl ctx);
    
    public partial class WorkerDialog : Form, IWorkerDialogControl
    {
        public WorkerDialog(string status, WorkerDialog_Work run)
        {
            this.status = status;
            this.run = run;
            InitializeComponent();
        }

        private string status;
        private WorkerDialog_Work run;
        private Thread worker;
        private Stopwatch timer = new Stopwatch();

        private void WorkerDialog_Load(object sender, EventArgs e)
        {
            labelMain.Text = status;
            UpdateStatusBarCurrent(0);
            worker = new Thread(Work);
            worker.Start();
        }

        private void Work()
        {
            timer.Start();
            DialogResult r = run(this);
            Invoke((MethodInvoker)delegate
            {
                DialogResult = r;
                Close();
            });
        }

        public void UpdateStatusText(string status)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                labelMain.Text = status;
            });
        }
        
        public void UpdateStatusBar(double progress)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                UpdateStatusBarCurrent(progress);
            });
        }

        private void UpdateStatusBarCurrent(double progress)
        {
            //Calculate ETA
            string eta = "--:--:--";
            if (progress != 0)
            {
                long seconds = (long)((timer.Elapsed.TotalSeconds / progress) * (1 - progress));
                eta = $"{((seconds / 60 / 60) % 60).ToString().PadLeft(2, '0')}:{((seconds / 60) % 60).ToString().PadLeft(2, '0')}:{(seconds % 60).ToString().PadLeft(2, '0')}";
            }

            //Set
            progressBar.Value = (int)(progress * 1000);
            labelProgress.Text = $"(ETA {eta}) {(progress * 100).ToString("F")}%";
        }
    }

    public interface IWorkerDialogControl
    {
        void UpdateStatusText(string status);
        void UpdateStatusBar(double progress);
    }
}
