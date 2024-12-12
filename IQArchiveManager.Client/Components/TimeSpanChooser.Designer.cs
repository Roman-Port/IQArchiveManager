
namespace IQArchiveManager.Client.Components
{
    partial class TimeSpanChooser
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
            this.valueH = new IQArchiveManager.Client.Components.TimeSpanChooser.CustomNumericUpDown();
            this.valueM = new IQArchiveManager.Client.Components.TimeSpanChooser.CustomNumericUpDown();
            this.valueS = new IQArchiveManager.Client.Components.TimeSpanChooser.CustomNumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.valueH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueS)).BeginInit();
            this.SuspendLayout();
            // 
            // valueH
            // 
            this.valueH.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueH.Location = new System.Drawing.Point(0, 0);
            this.valueH.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.valueH.Minimum = new decimal(new int[] {
            2147483647,
            0,
            0,
            -2147483648});
            this.valueH.Name = "valueH";
            this.valueH.Size = new System.Drawing.Size(35, 20);
            this.valueH.TabIndex = 0;
            this.valueH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.valueH.ValueChanged += new System.EventHandler(this.UiValueChanged);
            // 
            // valueM
            // 
            this.valueM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueM.Location = new System.Drawing.Point(44, 0);
            this.valueM.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.valueM.Minimum = new decimal(new int[] {
            2147483647,
            0,
            0,
            -2147483648});
            this.valueM.Name = "valueM";
            this.valueM.Size = new System.Drawing.Size(34, 20);
            this.valueM.TabIndex = 1;
            this.valueM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.valueM.ValueChanged += new System.EventHandler(this.UiValueChanged);
            // 
            // valueS
            // 
            this.valueS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueS.Location = new System.Drawing.Point(85, 0);
            this.valueS.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.valueS.Minimum = new decimal(new int[] {
            2147483647,
            0,
            0,
            -2147483648});
            this.valueS.Name = "valueS";
            this.valueS.Size = new System.Drawing.Size(34, 20);
            this.valueS.TabIndex = 2;
            this.valueS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.valueS.ValueChanged += new System.EventHandler(this.UiValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(77, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = ":";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = ":";
            // 
            // TimeSpanChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.valueS);
            this.Controls.Add(this.valueM);
            this.Controls.Add(this.valueH);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "TimeSpanChooser";
            this.Size = new System.Drawing.Size(119, 20);
            this.Load += new System.EventHandler(this.TimeSpanChooser_Load);
            ((System.ComponentModel.ISupportInitialize)(this.valueH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomNumericUpDown valueH;
        private CustomNumericUpDown valueM;
        private CustomNumericUpDown valueS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
