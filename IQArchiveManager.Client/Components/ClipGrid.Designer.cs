
namespace IQArchiveManager.Client.Components
{
    partial class ClipGrid
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.station = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.artist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prefix = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.suffix = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.radio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SNR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ok = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.station,
            this.artist,
            this.title,
            this.date,
            this.time,
            this.prefix,
            this.suffix,
            this.radio,
            this.id,
            this.SNR,
            this.ok,
            this.hd,
            this.rds,
            this.notes});
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1118, 299);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // station
            // 
            this.station.HeaderText = "Station";
            this.station.Name = "station";
            this.station.ReadOnly = true;
            this.station.Width = 70;
            // 
            // artist
            // 
            this.artist.HeaderText = "Artist";
            this.artist.Name = "artist";
            this.artist.ReadOnly = true;
            this.artist.Width = 130;
            // 
            // title
            // 
            this.title.HeaderText = "Title";
            this.title.Name = "title";
            this.title.ReadOnly = true;
            this.title.Width = 130;
            // 
            // date
            // 
            dataGridViewCellStyle1.Format = "d";
            dataGridViewCellStyle1.NullValue = null;
            this.date.DefaultCellStyle = dataGridViewCellStyle1;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 80;
            // 
            // time
            // 
            dataGridViewCellStyle2.Format = "t";
            dataGridViewCellStyle2.NullValue = null;
            this.time.DefaultCellStyle = dataGridViewCellStyle2;
            this.time.HeaderText = "Time";
            this.time.Name = "time";
            this.time.ReadOnly = true;
            this.time.Width = 80;
            // 
            // prefix
            // 
            this.prefix.HeaderText = "Prefix";
            this.prefix.Name = "prefix";
            this.prefix.ReadOnly = true;
            // 
            // suffix
            // 
            this.suffix.HeaderText = "Suffix";
            this.suffix.Name = "suffix";
            this.suffix.ReadOnly = true;
            // 
            // radio
            // 
            this.radio.HeaderText = "Radio";
            this.radio.Name = "radio";
            this.radio.ReadOnly = true;
            this.radio.Width = 70;
            // 
            // id
            // 
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Width = 60;
            // 
            // SNR
            // 
            this.SNR.FillWeight = 80F;
            this.SNR.HeaderText = "SNR";
            this.SNR.Name = "SNR";
            this.SNR.ReadOnly = true;
            this.SNR.Width = 80;
            // 
            // ok
            // 
            this.ok.HeaderText = "OK";
            this.ok.Name = "ok";
            this.ok.ReadOnly = true;
            this.ok.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ok.Width = 35;
            // 
            // hd
            // 
            this.hd.HeaderText = "HD";
            this.hd.Name = "hd";
            this.hd.ReadOnly = true;
            this.hd.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.hd.Width = 35;
            // 
            // rds
            // 
            this.rds.HeaderText = "RDS";
            this.rds.Name = "rds";
            this.rds.ReadOnly = true;
            this.rds.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.rds.Width = 35;
            // 
            // notes
            // 
            this.notes.FillWeight = 200F;
            this.notes.HeaderText = "Notes";
            this.notes.Name = "notes";
            this.notes.ReadOnly = true;
            this.notes.Width = 300;
            // 
            // ClipGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Name = "ClipGrid";
            this.Size = new System.Drawing.Size(1118, 299);
            this.Load += new System.EventHandler(this.ClipGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn station;
        private System.Windows.Forms.DataGridViewTextBoxColumn artist;
        private System.Windows.Forms.DataGridViewTextBoxColumn title;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn time;
        private System.Windows.Forms.DataGridViewTextBoxColumn prefix;
        private System.Windows.Forms.DataGridViewTextBoxColumn suffix;
        private System.Windows.Forms.DataGridViewTextBoxColumn radio;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn SNR;
        private System.Windows.Forms.DataGridViewTextBoxColumn ok;
        private System.Windows.Forms.DataGridViewTextBoxColumn hd;
        private System.Windows.Forms.DataGridViewTextBoxColumn rds;
        private System.Windows.Forms.DataGridViewTextBoxColumn notes;
    }
}
