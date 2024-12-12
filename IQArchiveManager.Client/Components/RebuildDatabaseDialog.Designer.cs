
namespace IQArchiveManager.Client.Components
{
    partial class RebuildDatabaseDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RebuildDatabaseDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.scanDirList = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAddScanDir = new System.Windows.Forms.Button();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.optRemoveLost = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(362, 96);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // scanDirList
            // 
            this.scanDirList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scanDirList.FormattingEnabled = true;
            this.scanDirList.IntegralHeight = false;
            this.scanDirList.Location = new System.Drawing.Point(6, 19);
            this.scanDirList.Name = "scanDirList";
            this.scanDirList.Size = new System.Drawing.Size(347, 91);
            this.scanDirList.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnAddScanDir);
            this.groupBox1.Controls.Add(this.scanDirList);
            this.groupBox1.Location = new System.Drawing.Point(15, 143);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 145);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scan Directories";
            // 
            // btnAddScanDir
            // 
            this.btnAddScanDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddScanDir.Location = new System.Drawing.Point(278, 116);
            this.btnAddScanDir.Name = "btnAddScanDir";
            this.btnAddScanDir.Size = new System.Drawing.Size(75, 23);
            this.btnAddScanDir.TabIndex = 2;
            this.btnAddScanDir.Text = "Add...";
            this.btnAddScanDir.UseVisualStyleBackColor = true;
            this.btnAddScanDir.Click += new System.EventHandler(this.btnAddScanDir_Click);
            // 
            // btnRebuild
            // 
            this.btnRebuild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRebuild.Location = new System.Drawing.Point(283, 294);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(91, 23);
            this.btnRebuild.TabIndex = 3;
            this.btnRebuild.Text = "Rebuild";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // optRemoveLost
            // 
            this.optRemoveLost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.optRemoveLost.AutoSize = true;
            this.optRemoveLost.Checked = true;
            this.optRemoveLost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optRemoveLost.Location = new System.Drawing.Point(12, 298);
            this.optRemoveLost.Name = "optRemoveLost";
            this.optRemoveLost.Size = new System.Drawing.Size(119, 17);
            this.optRemoveLost.TabIndex = 4;
            this.optRemoveLost.Text = "Remove lost entries";
            this.optRemoveLost.UseVisualStyleBackColor = true;
            // 
            // RebuildDatabaseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 329);
            this.Controls.Add(this.optRemoveLost);
            this.Controls.Add(this.btnRebuild);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "RebuildDatabaseDialog";
            this.Text = "RebuildDatabaseDialog";
            this.Load += new System.EventHandler(this.RebuildDatabaseDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox scanDirList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAddScanDir;
        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.CheckBox optRemoveLost;
    }
}