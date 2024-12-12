
namespace IQArchiveManager.Client.Components
{
    partial class TransportControls
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainView = new System.Windows.Forms.PictureBox();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.timeSpanChooserCurrent = new IQArchiveManager.Client.Components.TimeSpanChooser();
            this.timeSpanChooserEnd = new IQArchiveManager.Client.Components.TimeSpanChooser();
            this.timeSpanChooserStart = new IQArchiveManager.Client.Components.TimeSpanChooser();
            ((System.ComponentModel.ISupportInitialize)(this.mainView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainView
            // 
            this.mainView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainView.Location = new System.Drawing.Point(0, 26);
            this.mainView.Name = "mainView";
            this.mainView.Size = new System.Drawing.Size(613, 250);
            this.mainView.TabIndex = 0;
            this.mainView.TabStop = false;
            this.mainView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainView_MouseDown);
            this.mainView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mainView_MouseMove);
            this.mainView.Resize += new System.EventHandler(this.mainView_Resize);
            // 
            // hScrollBar
            // 
            this.hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar.Location = new System.Drawing.Point(0, 279);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(613, 17);
            this.hScrollBar.TabIndex = 1;
            this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
            // 
            // timeSpanChooserCurrent
            // 
            this.timeSpanChooserCurrent.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.timeSpanChooserCurrent.Location = new System.Drawing.Point(246, 0);
            this.timeSpanChooserCurrent.MaxValue = System.TimeSpan.Parse("10675199.02:48:05.4775807");
            this.timeSpanChooserCurrent.MinValue = System.TimeSpan.Parse("00:00:00");
            this.timeSpanChooserCurrent.Name = "timeSpanChooserCurrent";
            this.timeSpanChooserCurrent.ReadOnly = false;
            this.timeSpanChooserCurrent.Size = new System.Drawing.Size(119, 20);
            this.timeSpanChooserCurrent.TabIndex = 4;
            this.timeSpanChooserCurrent.Value = System.TimeSpan.Parse("00:00:00");
            this.timeSpanChooserCurrent.OnValueChanged += new System.EventHandler(this.timeSpanChooserCurrent_OnValueChanged);
            // 
            // timeSpanChooserEnd
            // 
            this.timeSpanChooserEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timeSpanChooserEnd.Location = new System.Drawing.Point(494, 0);
            this.timeSpanChooserEnd.MaxValue = System.TimeSpan.Parse("10675199.02:48:05.4775807");
            this.timeSpanChooserEnd.MinValue = System.TimeSpan.Parse("00:00:00");
            this.timeSpanChooserEnd.Name = "timeSpanChooserEnd";
            this.timeSpanChooserEnd.ReadOnly = false;
            this.timeSpanChooserEnd.Size = new System.Drawing.Size(119, 20);
            this.timeSpanChooserEnd.TabIndex = 3;
            this.timeSpanChooserEnd.Value = System.TimeSpan.Parse("00:00:00");
            this.timeSpanChooserEnd.OnValueChanged += new System.EventHandler(this.timeSpanChooserEnd_OnValueChanged);
            // 
            // timeSpanChooserStart
            // 
            this.timeSpanChooserStart.Location = new System.Drawing.Point(0, 0);
            this.timeSpanChooserStart.MaxValue = System.TimeSpan.Parse("10675199.02:48:05.4775807");
            this.timeSpanChooserStart.MinValue = System.TimeSpan.Parse("00:00:00");
            this.timeSpanChooserStart.Name = "timeSpanChooserStart";
            this.timeSpanChooserStart.ReadOnly = false;
            this.timeSpanChooserStart.Size = new System.Drawing.Size(119, 20);
            this.timeSpanChooserStart.TabIndex = 2;
            this.timeSpanChooserStart.Value = System.TimeSpan.Parse("00:00:00");
            this.timeSpanChooserStart.OnValueChanged += new System.EventHandler(this.timeSpanChooserStart_OnValueChanged);
            // 
            // TransportControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.timeSpanChooserCurrent);
            this.Controls.Add(this.timeSpanChooserEnd);
            this.Controls.Add(this.timeSpanChooserStart);
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.mainView);
            this.Name = "TransportControls";
            this.Size = new System.Drawing.Size(613, 296);
            this.Load += new System.EventHandler(this.TransportControls_Load);
            this.Resize += new System.EventHandler(this.TransportControls_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.mainView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox mainView;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private TimeSpanChooser timeSpanChooserStart;
        private TimeSpanChooser timeSpanChooserEnd;
        private TimeSpanChooser timeSpanChooserCurrent;
    }
}
