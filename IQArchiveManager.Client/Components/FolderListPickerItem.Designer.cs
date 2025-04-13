namespace IQArchiveManager.Client.Components
{
    partial class FolderListPickerItem
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.controlButton = new System.Windows.Forms.Button();
            this.dirButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(0, 20);
            this.textBox.TabIndex = 0;
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // controlButton
            // 
            this.controlButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.controlButton.Location = new System.Drawing.Point(37, 0);
            this.controlButton.Name = "controlButton";
            this.controlButton.Size = new System.Drawing.Size(37, 20);
            this.controlButton.TabIndex = 1;
            this.controlButton.UseVisualStyleBackColor = true;
            this.controlButton.Click += new System.EventHandler(this.controlButton_Click);
            // 
            // dirButton
            // 
            this.dirButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.dirButton.Location = new System.Drawing.Point(0, 0);
            this.dirButton.Name = "dirButton";
            this.dirButton.Size = new System.Drawing.Size(37, 20);
            this.dirButton.TabIndex = 2;
            this.dirButton.Text = "...";
            this.dirButton.UseVisualStyleBackColor = true;
            this.dirButton.Click += new System.EventHandler(this.dirButton_Click);
            // 
            // FolderListPickerItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.dirButton);
            this.Controls.Add(this.controlButton);
            this.Name = "FolderListPickerItem";
            this.Size = new System.Drawing.Size(74, 20);
            this.Load += new System.EventHandler(this.FolderListPickerItem_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button controlButton;
        private System.Windows.Forms.Button dirButton;
    }
}
