
namespace IQArchiveManager.Client
{
    partial class MainMenu
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
            this.searchBox = new System.Windows.Forms.TextBox();
            this.btnClipExtract = new System.Windows.Forms.Button();
            this.clipGrid = new IQArchiveManager.Client.Components.ClipGrid();
            this.btnRebuildDb = new System.Windows.Forms.Button();
            this.btnOpenEditor = new System.Windows.Forms.Button();
            this.btnExportStats = new System.Windows.Forms.Button();
            this.btnExportResults = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Location = new System.Drawing.Point(12, 12);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(1121, 20);
            this.searchBox.TabIndex = 1;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // btnClipExtract
            // 
            this.btnClipExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClipExtract.Enabled = false;
            this.btnClipExtract.Location = new System.Drawing.Point(1003, 415);
            this.btnClipExtract.Name = "btnClipExtract";
            this.btnClipExtract.Size = new System.Drawing.Size(130, 23);
            this.btnClipExtract.TabIndex = 2;
            this.btnClipExtract.Text = "Extract...";
            this.btnClipExtract.UseVisualStyleBackColor = true;
            this.btnClipExtract.Click += new System.EventHandler(this.btnClipExtract_Click);
            // 
            // clipGrid
            // 
            this.clipGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clipGrid.IsMini = false;
            this.clipGrid.Location = new System.Drawing.Point(12, 38);
            this.clipGrid.Name = "clipGrid";
            this.clipGrid.Size = new System.Drawing.Size(1121, 371);
            this.clipGrid.TabIndex = 0;
            this.clipGrid.OnSelectionChanged += new IQArchiveManager.Client.Components.ClipGrid_SelectedItemChangedEventArgs(this.clipGrid_OnSelectionChanged);
            // 
            // btnRebuildDb
            // 
            this.btnRebuildDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRebuildDb.Location = new System.Drawing.Point(12, 415);
            this.btnRebuildDb.Name = "btnRebuildDb";
            this.btnRebuildDb.Size = new System.Drawing.Size(114, 23);
            this.btnRebuildDb.TabIndex = 3;
            this.btnRebuildDb.Text = "Rebuild Database";
            this.btnRebuildDb.UseVisualStyleBackColor = true;
            this.btnRebuildDb.Click += new System.EventHandler(this.btnRebuildDb_Click);
            // 
            // btnOpenEditor
            // 
            this.btnOpenEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenEditor.Location = new System.Drawing.Point(132, 415);
            this.btnOpenEditor.Name = "btnOpenEditor";
            this.btnOpenEditor.Size = new System.Drawing.Size(114, 23);
            this.btnOpenEditor.TabIndex = 4;
            this.btnOpenEditor.Text = "Open Editor";
            this.btnOpenEditor.UseVisualStyleBackColor = true;
            this.btnOpenEditor.Click += new System.EventHandler(this.btnOpenEditor_Click);
            // 
            // btnExportStats
            // 
            this.btnExportStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExportStats.Location = new System.Drawing.Point(252, 415);
            this.btnExportStats.Name = "btnExportStats";
            this.btnExportStats.Size = new System.Drawing.Size(114, 23);
            this.btnExportStats.TabIndex = 5;
            this.btnExportStats.Text = "Export Stats";
            this.btnExportStats.UseVisualStyleBackColor = true;
            this.btnExportStats.Click += new System.EventHandler(this.btnExportStats_Click);
            // 
            // btnExportResults
            // 
            this.btnExportResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExportResults.Location = new System.Drawing.Point(372, 415);
            this.btnExportResults.Name = "btnExportResults";
            this.btnExportResults.Size = new System.Drawing.Size(114, 23);
            this.btnExportResults.TabIndex = 6;
            this.btnExportResults.Text = "Export Results";
            this.btnExportResults.UseVisualStyleBackColor = true;
            this.btnExportResults.Click += new System.EventHandler(this.btnExportResults_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1145, 450);
            this.Controls.Add(this.btnExportResults);
            this.Controls.Add(this.btnExportStats);
            this.Controls.Add(this.btnOpenEditor);
            this.Controls.Add(this.btnRebuildDb);
            this.Controls.Add(this.btnClipExtract);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.clipGrid);
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.ClipGrid clipGrid;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Button btnClipExtract;
        private System.Windows.Forms.Button btnRebuildDb;
        private System.Windows.Forms.Button btnOpenEditor;
        private System.Windows.Forms.Button btnExportStats;
        private System.Windows.Forms.Button btnExportResults;
    }
}