namespace IQArchiveManager.Client.Components
{
    partial class FolderListPicker
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
            this.dirListPanel = new System.Windows.Forms.Panel();
            this.addFolderSection = new IQArchiveManager.Client.Components.FolderListPickerItem();
            this.SuspendLayout();
            // 
            // dirListPanel
            // 
            this.dirListPanel.AutoScroll = true;
            this.dirListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dirListPanel.Location = new System.Drawing.Point(0, 0);
            this.dirListPanel.Name = "dirListPanel";
            this.dirListPanel.Size = new System.Drawing.Size(438, 226);
            this.dirListPanel.TabIndex = 0;
            // 
            // addFolderSection
            // 
            this.addFolderSection.AutoSize = true;
            this.addFolderSection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addFolderSection.BtnText = "+";
            this.addFolderSection.DisableControlOnEmpty = true;
            this.addFolderSection.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addFolderSection.EditBoxText = "";
            this.addFolderSection.Location = new System.Drawing.Point(0, 226);
            this.addFolderSection.Name = "addFolderSection";
            this.addFolderSection.Size = new System.Drawing.Size(438, 20);
            this.addFolderSection.TabIndex = 1;
            this.addFolderSection.ControlButtonClicked += new System.Action<IQArchiveManager.Client.Components.FolderListPickerItem>(this.addFolderSection_ControlButtonClicked);
            // 
            // FolderListPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dirListPanel);
            this.Controls.Add(this.addFolderSection);
            this.Name = "FolderListPicker";
            this.Size = new System.Drawing.Size(438, 246);
            this.Load += new System.EventHandler(this.FolderListPicker_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel dirListPanel;
        private FolderListPickerItem addFolderSection;
    }
}
