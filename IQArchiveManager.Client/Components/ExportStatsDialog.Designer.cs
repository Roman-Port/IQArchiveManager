
namespace IQArchiveManager.Client.Components
{
    partial class ExportStatsDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.daysPerLine = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.minRecordingsPerStation = new System.Windows.Forms.NumericUpDown();
            this.btnExport = new System.Windows.Forms.Button();
            this.optionIncludeOther = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.daysPerLine)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minRecordingsPerStation)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Days/Line";
            // 
            // daysPerLine
            // 
            this.daysPerLine.Location = new System.Drawing.Point(6, 32);
            this.daysPerLine.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.daysPerLine.Name = "daysPerLine";
            this.daysPerLine.Size = new System.Drawing.Size(141, 20);
            this.daysPerLine.TabIndex = 1;
            this.daysPerLine.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.optionIncludeOther);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.minRecordingsPerStation);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.daysPerLine);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(487, 83);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Export Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Min Recordings/Station";
            // 
            // minRecordingsPerStation
            // 
            this.minRecordingsPerStation.Location = new System.Drawing.Point(153, 32);
            this.minRecordingsPerStation.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.minRecordingsPerStation.Name = "minRecordingsPerStation";
            this.minRecordingsPerStation.Size = new System.Drawing.Size(141, 20);
            this.minRecordingsPerStation.TabIndex = 3;
            this.minRecordingsPerStation.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(369, 125);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(130, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export as CSV...";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // checkBox1
            // 
            this.optionIncludeOther.AutoSize = true;
            this.optionIncludeOther.Location = new System.Drawing.Point(9, 58);
            this.optionIncludeOther.Name = "checkBox1";
            this.optionIncludeOther.Size = new System.Drawing.Size(145, 17);
            this.optionIncludeOther.TabIndex = 4;
            this.optionIncludeOther.Text = "Include \"Other\" Category";
            this.optionIncludeOther.UseVisualStyleBackColor = true;
            // 
            // ExportStatsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 160);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.groupBox1);
            this.Name = "ExportStatsDialog";
            this.Text = "ExportStatsDialog";
            ((System.ComponentModel.ISupportInitialize)(this.daysPerLine)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minRecordingsPerStation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown daysPerLine;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown minRecordingsPerStation;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox optionIncludeOther;
    }
}