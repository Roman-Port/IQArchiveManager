
namespace IQArchiveManager.Client
{
    partial class MainEditor
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
            this.spectrumView1 = new RomanPort.LibSDR.UI.SpectrumView();
            this.waterfallView1 = new RomanPort.LibSDR.UI.WaterfallView();
            this.rdsPsLabel = new System.Windows.Forms.Label();
            this.rdsRtLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSwapTitleArtist = new System.Windows.Forms.Button();
            this.btnLockCall = new System.Windows.Forms.Button();
            this.autoTgCall = new System.Windows.Forms.TextBox();
            this.autoTgBtn = new System.Windows.Forms.Button();
            this.typeBtnLiner = new System.Windows.Forms.RadioButton();
            this.typeBtnSong = new System.Windows.Forms.RadioButton();
            this.rdsPatchMethod = new System.Windows.Forms.ComboBox();
            this.btnAutoRds = new System.Windows.Forms.Button();
            this.btnAddClip = new System.Windows.Forms.Button();
            this.inputNotes = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.prefixSuffixPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.inputPrefix = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.inputSuffix = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnShiftSuffix = new System.Windows.Forms.Button();
            this.inputTitle = new System.Windows.Forms.TextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.inputArtist = new System.Windows.Forms.TextBox();
            this.labelArtist = new System.Windows.Forms.Label();
            this.inputFlagOk = new System.Windows.Forms.CheckBox();
            this.inputFlagHd = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inputFlagRds = new System.Windows.Forms.CheckBox();
            this.inputCall = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.editedClipsList = new System.Windows.Forms.ListBox();
            this.btnFileDelete = new System.Windows.Forms.Button();
            this.btnFileSave = new System.Windows.Forms.Button();
            this.recordingTimeLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navigateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgeStartForward20sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgeStartBackwards20sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgeStartForwards2sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgeStartBackwards2sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.nudgePlayheadForwards20sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgePlayheadBackwards20sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgePlayheadForwards2sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgePlayheadBackwards2sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.nudgeEndForwards20sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgeEndBackwards20sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgeEndForwards2sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nudgeEndBackwards2sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToEndToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveStartToPlayheadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveEndToPlayheadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.findViaRDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.nextRDSItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousRDSItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportRDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawPSFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawRTFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parsedRTFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawPIFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rDSDSPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.blockCurrentRTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemsListSummaryLabel = new System.Windows.Forms.Label();
            this.recordingDateLabel = new System.Windows.Forms.Label();
            this.clipGrid1 = new IQArchiveManager.Client.Components.ClipGrid();
            this.transportControls = new IQArchiveManager.Client.Components.TransportControls();
            this.groupBox1.SuspendLayout();
            this.prefixSuffixPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // spectrumView1
            // 
            this.spectrumView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spectrumView1.BackColor = System.Drawing.Color.Black;
            this.spectrumView1.FftOffset = 0F;
            this.spectrumView1.FftRange = 100F;
            this.spectrumView1.Location = new System.Drawing.Point(631, 55);
            this.spectrumView1.Name = "spectrumView1";
            this.spectrumView1.Size = new System.Drawing.Size(742, 197);
            this.spectrumView1.TabIndex = 1;
            // 
            // waterfallView1
            // 
            this.waterfallView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waterfallView1.BackColor = System.Drawing.Color.Black;
            this.waterfallView1.FftOffset = 0F;
            this.waterfallView1.FftRange = 100F;
            this.waterfallView1.Location = new System.Drawing.Point(631, 258);
            this.waterfallView1.Name = "waterfallView1";
            this.waterfallView1.Size = new System.Drawing.Size(742, 273);
            this.waterfallView1.TabIndex = 2;
            // 
            // rdsPsLabel
            // 
            this.rdsPsLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rdsPsLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdsPsLabel.Location = new System.Drawing.Point(631, 27);
            this.rdsPsLabel.Name = "rdsPsLabel";
            this.rdsPsLabel.Padding = new System.Windows.Forms.Padding(5);
            this.rdsPsLabel.Size = new System.Drawing.Size(73, 25);
            this.rdsPsLabel.TabIndex = 3;
            // 
            // rdsRtLabel
            // 
            this.rdsRtLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rdsRtLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rdsRtLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdsRtLabel.Location = new System.Drawing.Point(710, 27);
            this.rdsRtLabel.Name = "rdsRtLabel";
            this.rdsRtLabel.Padding = new System.Windows.Forms.Padding(5);
            this.rdsRtLabel.Size = new System.Drawing.Size(440, 25);
            this.rdsRtLabel.TabIndex = 4;
            this.rdsRtLabel.Click += new System.EventHandler(this.rdsRtLabel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSwapTitleArtist);
            this.groupBox1.Controls.Add(this.btnLockCall);
            this.groupBox1.Controls.Add(this.autoTgCall);
            this.groupBox1.Controls.Add(this.autoTgBtn);
            this.groupBox1.Controls.Add(this.typeBtnLiner);
            this.groupBox1.Controls.Add(this.typeBtnSong);
            this.groupBox1.Controls.Add(this.rdsPatchMethod);
            this.groupBox1.Controls.Add(this.btnAutoRds);
            this.groupBox1.Controls.Add(this.btnAddClip);
            this.groupBox1.Controls.Add(this.inputNotes);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.prefixSuffixPanel);
            this.groupBox1.Controls.Add(this.inputTitle);
            this.groupBox1.Controls.Add(this.labelTitle);
            this.groupBox1.Controls.Add(this.inputArtist);
            this.groupBox1.Controls.Add(this.labelArtist);
            this.groupBox1.Controls.Add(this.inputFlagOk);
            this.groupBox1.Controls.Add(this.inputFlagHd);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.inputFlagRds);
            this.groupBox1.Controls.Add(this.inputCall);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 333);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Clip Settings";
            // 
            // btnSwapTitleArtist
            // 
            this.btnSwapTitleArtist.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSwapTitleArtist.Location = new System.Drawing.Point(247, 82);
            this.btnSwapTitleArtist.Name = "btnSwapTitleArtist";
            this.btnSwapTitleArtist.Size = new System.Drawing.Size(31, 75);
            this.btnSwapTitleArtist.TabIndex = 24;
            this.btnSwapTitleArtist.Text = "↕";
            this.btnSwapTitleArtist.UseVisualStyleBackColor = true;
            this.btnSwapTitleArtist.Click += new System.EventHandler(this.btnSwapTitleArtist_Click);
            // 
            // btnLockCall
            // 
            this.btnLockCall.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLockCall.Location = new System.Drawing.Point(142, 32);
            this.btnLockCall.Name = "btnLockCall";
            this.btnLockCall.Size = new System.Drawing.Size(49, 20);
            this.btnLockCall.TabIndex = 23;
            this.btnLockCall.Text = "Lock";
            this.btnLockCall.UseVisualStyleBackColor = true;
            this.btnLockCall.Click += new System.EventHandler(this.btnLockCall_Click);
            // 
            // autoTgCall
            // 
            this.autoTgCall.Location = new System.Drawing.Point(8, 277);
            this.autoTgCall.Name = "autoTgCall";
            this.autoTgCall.Size = new System.Drawing.Size(128, 20);
            this.autoTgCall.TabIndex = 22;
            this.autoTgCall.TextChanged += new System.EventHandler(this.autoTgCall_TextChanged);
            // 
            // autoTgBtn
            // 
            this.autoTgBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.autoTgBtn.Enabled = false;
            this.autoTgBtn.Location = new System.Drawing.Point(142, 275);
            this.autoTgBtn.Name = "autoTgBtn";
            this.autoTgBtn.Size = new System.Drawing.Size(130, 23);
            this.autoTgBtn.TabIndex = 21;
            this.autoTgBtn.Text = "Auto-TuneGenie";
            this.autoTgBtn.UseVisualStyleBackColor = true;
            this.autoTgBtn.Click += new System.EventHandler(this.autoTgBtn_Click);
            // 
            // typeBtnLiner
            // 
            this.typeBtnLiner.Appearance = System.Windows.Forms.Appearance.Button;
            this.typeBtnLiner.AutoSize = true;
            this.typeBtnLiner.Location = new System.Drawing.Point(56, 56);
            this.typeBtnLiner.Name = "typeBtnLiner";
            this.typeBtnLiner.Size = new System.Drawing.Size(40, 23);
            this.typeBtnLiner.TabIndex = 20;
            this.typeBtnLiner.Text = "Liner";
            this.typeBtnLiner.UseVisualStyleBackColor = true;
            this.typeBtnLiner.CheckedChanged += new System.EventHandler(this.typeBtnLiner_CheckedChanged);
            // 
            // typeBtnSong
            // 
            this.typeBtnSong.Appearance = System.Windows.Forms.Appearance.Button;
            this.typeBtnSong.AutoSize = true;
            this.typeBtnSong.Checked = true;
            this.typeBtnSong.Location = new System.Drawing.Point(8, 56);
            this.typeBtnSong.Name = "typeBtnSong";
            this.typeBtnSong.Size = new System.Drawing.Size(42, 23);
            this.typeBtnSong.TabIndex = 18;
            this.typeBtnSong.TabStop = true;
            this.typeBtnSong.Text = "Song";
            this.typeBtnSong.UseVisualStyleBackColor = true;
            this.typeBtnSong.CheckedChanged += new System.EventHandler(this.typeBtnSong_CheckedChanged);
            // 
            // rdsPatchMethod
            // 
            this.rdsPatchMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rdsPatchMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rdsPatchMethod.FormattingEnabled = true;
            this.rdsPatchMethod.Location = new System.Drawing.Point(7, 248);
            this.rdsPatchMethod.Name = "rdsPatchMethod";
            this.rdsPatchMethod.Size = new System.Drawing.Size(129, 21);
            this.rdsPatchMethod.TabIndex = 19;
            this.rdsPatchMethod.SelectedIndexChanged += new System.EventHandler(this.rdsPatchMethod_SelectedIndexChanged);
            // 
            // btnAutoRds
            // 
            this.btnAutoRds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoRds.Location = new System.Drawing.Point(142, 246);
            this.btnAutoRds.Name = "btnAutoRds";
            this.btnAutoRds.Size = new System.Drawing.Size(128, 23);
            this.btnAutoRds.TabIndex = 18;
            this.btnAutoRds.Text = "Auto-RDS";
            this.btnAutoRds.UseVisualStyleBackColor = true;
            this.btnAutoRds.Click += new System.EventHandler(this.btnAutoRds_Click);
            // 
            // btnAddClip
            // 
            this.btnAddClip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddClip.Location = new System.Drawing.Point(9, 304);
            this.btnAddClip.Name = "btnAddClip";
            this.btnAddClip.Size = new System.Drawing.Size(263, 23);
            this.btnAddClip.TabIndex = 16;
            this.btnAddClip.Text = "Add Clip";
            this.btnAddClip.UseVisualStyleBackColor = true;
            this.btnAddClip.Click += new System.EventHandler(this.btnAddClip_Click);
            // 
            // inputNotes
            // 
            this.inputNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputNotes.Location = new System.Drawing.Point(9, 216);
            this.inputNotes.Name = "inputNotes";
            this.inputNotes.Size = new System.Drawing.Size(263, 20);
            this.inputNotes.TabIndex = 15;
            this.inputNotes.TextChanged += new System.EventHandler(this.InputUpdated);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 200);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Notes";
            // 
            // prefixSuffixPanel
            // 
            this.prefixSuffixPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prefixSuffixPanel.ColumnCount = 3;
            this.prefixSuffixPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.prefixSuffixPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.prefixSuffixPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.prefixSuffixPanel.Controls.Add(this.panel1, 0, 0);
            this.prefixSuffixPanel.Controls.Add(this.panel2, 2, 0);
            this.prefixSuffixPanel.Controls.Add(this.panel3, 1, 0);
            this.prefixSuffixPanel.Location = new System.Drawing.Point(4, 157);
            this.prefixSuffixPanel.Name = "prefixSuffixPanel";
            this.prefixSuffixPanel.RowCount = 1;
            this.prefixSuffixPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.prefixSuffixPanel.Size = new System.Drawing.Size(272, 44);
            this.prefixSuffixPanel.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.inputPrefix);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(110, 38);
            this.panel1.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(-1, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Prefix";
            // 
            // inputPrefix
            // 
            this.inputPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputPrefix.FormattingEnabled = true;
            this.inputPrefix.Items.AddRange(new object[] {
            "Call Letters",
            "Ad/Song",
            "Station Slogan",
            "Station Promo",
            "DJ, No Clip",
            "DJ, Clip"});
            this.inputPrefix.Location = new System.Drawing.Point(2, 16);
            this.inputPrefix.Name = "inputPrefix";
            this.inputPrefix.Size = new System.Drawing.Size(108, 21);
            this.inputPrefix.TabIndex = 12;
            this.inputPrefix.SelectedIndexChanged += new System.EventHandler(this.InputUpdated);
            this.inputPrefix.TextUpdate += new System.EventHandler(this.InputUpdated);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.inputSuffix);
            this.panel2.Location = new System.Drawing.Point(159, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(110, 38);
            this.panel2.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(-3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Suffix";
            // 
            // inputSuffix
            // 
            this.inputSuffix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputSuffix.FormattingEnabled = true;
            this.inputSuffix.Items.AddRange(new object[] {
            "Call Letters",
            "Ad/Song",
            "Station Slogan",
            "Station Promo",
            "DJ, No Clip",
            "DJ, Clip"});
            this.inputSuffix.Location = new System.Drawing.Point(0, 16);
            this.inputSuffix.Name = "inputSuffix";
            this.inputSuffix.Size = new System.Drawing.Size(109, 21);
            this.inputSuffix.TabIndex = 14;
            this.inputSuffix.SelectedIndexChanged += new System.EventHandler(this.InputUpdated);
            this.inputSuffix.TextUpdate += new System.EventHandler(this.InputUpdated);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnShiftSuffix);
            this.panel3.Location = new System.Drawing.Point(119, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(34, 38);
            this.panel3.TabIndex = 2;
            // 
            // btnShiftSuffix
            // 
            this.btnShiftSuffix.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShiftSuffix.Location = new System.Drawing.Point(3, 16);
            this.btnShiftSuffix.Name = "btnShiftSuffix";
            this.btnShiftSuffix.Size = new System.Drawing.Size(28, 19);
            this.btnShiftSuffix.TabIndex = 0;
            this.btnShiftSuffix.Text = "<-";
            this.btnShiftSuffix.UseVisualStyleBackColor = true;
            this.btnShiftSuffix.Click += new System.EventHandler(this.btnShiftSuffix_Click);
            // 
            // inputTitle
            // 
            this.inputTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTitle.Location = new System.Drawing.Point(9, 137);
            this.inputTitle.Name = "inputTitle";
            this.inputTitle.Size = new System.Drawing.Size(232, 20);
            this.inputTitle.TabIndex = 11;
            this.inputTitle.TextChanged += new System.EventHandler(this.InputUpdated);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(6, 121);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(27, 13);
            this.labelTitle.TabIndex = 10;
            this.labelTitle.Text = "Title";
            // 
            // inputArtist
            // 
            this.inputArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputArtist.Location = new System.Drawing.Point(9, 98);
            this.inputArtist.Name = "inputArtist";
            this.inputArtist.Size = new System.Drawing.Size(232, 20);
            this.inputArtist.TabIndex = 9;
            this.inputArtist.TextChanged += new System.EventHandler(this.InputUpdated);
            // 
            // labelArtist
            // 
            this.labelArtist.AutoSize = true;
            this.labelArtist.Location = new System.Drawing.Point(6, 82);
            this.labelArtist.Name = "labelArtist";
            this.labelArtist.Size = new System.Drawing.Size(30, 13);
            this.labelArtist.TabIndex = 8;
            this.labelArtist.Text = "Artist";
            // 
            // inputFlagOk
            // 
            this.inputFlagOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputFlagOk.Location = new System.Drawing.Point(253, 32);
            this.inputFlagOk.Name = "inputFlagOk";
            this.inputFlagOk.Size = new System.Drawing.Size(15, 20);
            this.inputFlagOk.TabIndex = 7;
            this.inputFlagOk.UseVisualStyleBackColor = true;
            this.inputFlagOk.TextChanged += new System.EventHandler(this.InputUpdated);
            // 
            // inputFlagHd
            // 
            this.inputFlagHd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputFlagHd.Location = new System.Drawing.Point(226, 32);
            this.inputFlagHd.Name = "inputFlagHd";
            this.inputFlagHd.Size = new System.Drawing.Size(15, 20);
            this.inputFlagHd.TabIndex = 6;
            this.inputFlagHd.UseVisualStyleBackColor = true;
            this.inputFlagHd.TextChanged += new System.EventHandler(this.InputUpdated);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(250, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "OK";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(223, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "HD";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(194, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "RDS";
            // 
            // inputFlagRds
            // 
            this.inputFlagRds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputFlagRds.Location = new System.Drawing.Point(197, 32);
            this.inputFlagRds.Name = "inputFlagRds";
            this.inputFlagRds.Size = new System.Drawing.Size(15, 20);
            this.inputFlagRds.TabIndex = 2;
            this.inputFlagRds.UseVisualStyleBackColor = true;
            this.inputFlagRds.TextChanged += new System.EventHandler(this.InputUpdated);
            // 
            // inputCall
            // 
            this.inputCall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputCall.Location = new System.Drawing.Point(9, 32);
            this.inputCall.Name = "inputCall";
            this.inputCall.Size = new System.Drawing.Size(127, 20);
            this.inputCall.TabIndex = 1;
            this.inputCall.TextChanged += new System.EventHandler(this.InputUpdated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Station";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.editedClipsList);
            this.groupBox2.Controls.Add(this.btnFileDelete);
            this.groupBox2.Controls.Add(this.btnFileSave);
            this.groupBox2.Location = new System.Drawing.Point(296, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(329, 333);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "File Settings";
            // 
            // editedClipsList
            // 
            this.editedClipsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editedClipsList.FormattingEnabled = true;
            this.editedClipsList.IntegralHeight = false;
            this.editedClipsList.Location = new System.Drawing.Point(6, 19);
            this.editedClipsList.Name = "editedClipsList";
            this.editedClipsList.Size = new System.Drawing.Size(317, 250);
            this.editedClipsList.TabIndex = 21;
            this.editedClipsList.DoubleClick += new System.EventHandler(this.editedClipsList_DoubleClick);
            // 
            // btnFileDelete
            // 
            this.btnFileDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileDelete.Location = new System.Drawing.Point(6, 275);
            this.btnFileDelete.Name = "btnFileDelete";
            this.btnFileDelete.Size = new System.Drawing.Size(317, 23);
            this.btnFileDelete.TabIndex = 20;
            this.btnFileDelete.Text = "DELETE";
            this.btnFileDelete.UseVisualStyleBackColor = true;
            this.btnFileDelete.Click += new System.EventHandler(this.btnFileDelete_Click);
            // 
            // btnFileSave
            // 
            this.btnFileSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileSave.Location = new System.Drawing.Point(6, 304);
            this.btnFileSave.Name = "btnFileSave";
            this.btnFileSave.Size = new System.Drawing.Size(317, 23);
            this.btnFileSave.TabIndex = 19;
            this.btnFileSave.Text = "SAVE";
            this.btnFileSave.UseVisualStyleBackColor = true;
            this.btnFileSave.Click += new System.EventHandler(this.btnFileSave_Click);
            // 
            // recordingTimeLabel
            // 
            this.recordingTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recordingTimeLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.recordingTimeLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recordingTimeLabel.Location = new System.Drawing.Point(1276, 27);
            this.recordingTimeLabel.Name = "recordingTimeLabel";
            this.recordingTimeLabel.Padding = new System.Windows.Forms.Padding(5);
            this.recordingTimeLabel.Size = new System.Drawing.Size(97, 25);
            this.recordingTimeLabel.TabIndex = 16;
            this.recordingTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1385, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.filesToolStripMenuItem.Text = "Files";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigateToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.tipsToolStripMenuItem,
            this.toolStripSeparator5,
            this.blockCurrentRTToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // navigateToolStripMenuItem
            // 
            this.navigateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transportToolStripMenuItem,
            this.jumpToolStripMenuItem,
            this.moveToolStripMenuItem});
            this.navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            this.navigateToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.navigateToolStripMenuItem.Text = "Navigate";
            this.navigateToolStripMenuItem.Visible = false;
            // 
            // transportToolStripMenuItem
            // 
            this.transportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nudgeStartForward20sToolStripMenuItem,
            this.nudgeStartBackwards20sToolStripMenuItem,
            this.nudgeStartForwards2sToolStripMenuItem,
            this.nudgeStartBackwards2sToolStripMenuItem,
            this.toolStripSeparator1,
            this.nudgePlayheadForwards20sToolStripMenuItem,
            this.nudgePlayheadBackwards20sToolStripMenuItem,
            this.nudgePlayheadForwards2sToolStripMenuItem,
            this.nudgePlayheadBackwards2sToolStripMenuItem,
            this.toolStripSeparator2,
            this.nudgeEndForwards20sToolStripMenuItem,
            this.nudgeEndBackwards20sToolStripMenuItem,
            this.nudgeEndForwards2sToolStripMenuItem,
            this.nudgeEndBackwards2sToolStripMenuItem});
            this.transportToolStripMenuItem.Name = "transportToolStripMenuItem";
            this.transportToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.transportToolStripMenuItem.Text = "Nudge...";
            // 
            // nudgeStartForward20sToolStripMenuItem
            // 
            this.nudgeStartForward20sToolStripMenuItem.Name = "nudgeStartForward20sToolStripMenuItem";
            this.nudgeStartForward20sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Q)));
            this.nudgeStartForward20sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgeStartForward20sToolStripMenuItem.Text = "Nudge Start Forwards 20s";
            this.nudgeStartForward20sToolStripMenuItem.Click += new System.EventHandler(this.nudgeStartForward20sToolStripMenuItem_Click);
            // 
            // nudgeStartBackwards20sToolStripMenuItem
            // 
            this.nudgeStartBackwards20sToolStripMenuItem.Name = "nudgeStartBackwards20sToolStripMenuItem";
            this.nudgeStartBackwards20sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.nudgeStartBackwards20sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgeStartBackwards20sToolStripMenuItem.Text = "Nudge Start Backwards 20s";
            this.nudgeStartBackwards20sToolStripMenuItem.Click += new System.EventHandler(this.nudgeStartBackwards20sToolStripMenuItem_Click);
            // 
            // nudgeStartForwards2sToolStripMenuItem
            // 
            this.nudgeStartForwards2sToolStripMenuItem.Name = "nudgeStartForwards2sToolStripMenuItem";
            this.nudgeStartForwards2sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Q)));
            this.nudgeStartForwards2sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgeStartForwards2sToolStripMenuItem.Text = "Nudge Start Forwards 2s";
            this.nudgeStartForwards2sToolStripMenuItem.Click += new System.EventHandler(this.nudgeStartForwards2sToolStripMenuItem_Click);
            // 
            // nudgeStartBackwards2sToolStripMenuItem
            // 
            this.nudgeStartBackwards2sToolStripMenuItem.Name = "nudgeStartBackwards2sToolStripMenuItem";
            this.nudgeStartBackwards2sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.nudgeStartBackwards2sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgeStartBackwards2sToolStripMenuItem.Text = "Nudge Start Backwards 2s";
            this.nudgeStartBackwards2sToolStripMenuItem.Click += new System.EventHandler(this.nudgeStartBackwards2sToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(299, 6);
            // 
            // nudgePlayheadForwards20sToolStripMenuItem
            // 
            this.nudgePlayheadForwards20sToolStripMenuItem.Name = "nudgePlayheadForwards20sToolStripMenuItem";
            this.nudgePlayheadForwards20sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.W)));
            this.nudgePlayheadForwards20sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgePlayheadForwards20sToolStripMenuItem.Text = "Nudge Playhead Forwards 20s";
            this.nudgePlayheadForwards20sToolStripMenuItem.Click += new System.EventHandler(this.nudgePlayheadForwards20sToolStripMenuItem_Click);
            // 
            // nudgePlayheadBackwards20sToolStripMenuItem
            // 
            this.nudgePlayheadBackwards20sToolStripMenuItem.Name = "nudgePlayheadBackwards20sToolStripMenuItem";
            this.nudgePlayheadBackwards20sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.nudgePlayheadBackwards20sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgePlayheadBackwards20sToolStripMenuItem.Text = "Nudge Playhead Backwards 20s";
            this.nudgePlayheadBackwards20sToolStripMenuItem.Click += new System.EventHandler(this.nudgePlayheadBackwards20sToolStripMenuItem_Click);
            // 
            // nudgePlayheadForwards2sToolStripMenuItem
            // 
            this.nudgePlayheadForwards2sToolStripMenuItem.Name = "nudgePlayheadForwards2sToolStripMenuItem";
            this.nudgePlayheadForwards2sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.nudgePlayheadForwards2sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgePlayheadForwards2sToolStripMenuItem.Text = "Nudge Playhead Forwards 2s";
            this.nudgePlayheadForwards2sToolStripMenuItem.Click += new System.EventHandler(this.nudgePlayheadForwards2sToolStripMenuItem_Click);
            // 
            // nudgePlayheadBackwards2sToolStripMenuItem
            // 
            this.nudgePlayheadBackwards2sToolStripMenuItem.Name = "nudgePlayheadBackwards2sToolStripMenuItem";
            this.nudgePlayheadBackwards2sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.nudgePlayheadBackwards2sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgePlayheadBackwards2sToolStripMenuItem.Text = "Nudge Playhead Backwards 2s";
            this.nudgePlayheadBackwards2sToolStripMenuItem.Click += new System.EventHandler(this.nudgePlayheadBackwards2sToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(299, 6);
            // 
            // nudgeEndForwards20sToolStripMenuItem
            // 
            this.nudgeEndForwards20sToolStripMenuItem.Name = "nudgeEndForwards20sToolStripMenuItem";
            this.nudgeEndForwards20sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.nudgeEndForwards20sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgeEndForwards20sToolStripMenuItem.Text = "Nudge End Forwards 20s";
            this.nudgeEndForwards20sToolStripMenuItem.Click += new System.EventHandler(this.nudgeEndForwards20sToolStripMenuItem_Click);
            // 
            // nudgeEndBackwards20sToolStripMenuItem
            // 
            this.nudgeEndBackwards20sToolStripMenuItem.Name = "nudgeEndBackwards20sToolStripMenuItem";
            this.nudgeEndBackwards20sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D)));
            this.nudgeEndBackwards20sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgeEndBackwards20sToolStripMenuItem.Text = "Nudge End Backwards 20s";
            this.nudgeEndBackwards20sToolStripMenuItem.Click += new System.EventHandler(this.nudgeEndBackwards20sToolStripMenuItem_Click);
            // 
            // nudgeEndForwards2sToolStripMenuItem
            // 
            this.nudgeEndForwards2sToolStripMenuItem.Name = "nudgeEndForwards2sToolStripMenuItem";
            this.nudgeEndForwards2sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.nudgeEndForwards2sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgeEndForwards2sToolStripMenuItem.Text = "Nudge End Forwards 2s";
            this.nudgeEndForwards2sToolStripMenuItem.Click += new System.EventHandler(this.nudgeEndForwards2sToolStripMenuItem_Click);
            // 
            // nudgeEndBackwards2sToolStripMenuItem
            // 
            this.nudgeEndBackwards2sToolStripMenuItem.Name = "nudgeEndBackwards2sToolStripMenuItem";
            this.nudgeEndBackwards2sToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
            this.nudgeEndBackwards2sToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.nudgeEndBackwards2sToolStripMenuItem.Text = "Nudge End Backwards 2s";
            this.nudgeEndBackwards2sToolStripMenuItem.Click += new System.EventHandler(this.nudgeEndBackwards2sToolStripMenuItem_Click);
            // 
            // jumpToolStripMenuItem
            // 
            this.jumpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jumpToStartToolStripMenuItem,
            this.jumpToEndToolStripMenuItem});
            this.jumpToolStripMenuItem.Name = "jumpToolStripMenuItem";
            this.jumpToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.jumpToolStripMenuItem.Text = "Jump...";
            // 
            // jumpToStartToolStripMenuItem
            // 
            this.jumpToStartToolStripMenuItem.Name = "jumpToStartToolStripMenuItem";
            this.jumpToStartToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D1)));
            this.jumpToStartToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.jumpToStartToolStripMenuItem.Text = "Jump To Start";
            this.jumpToStartToolStripMenuItem.Click += new System.EventHandler(this.jumpToStartToolStripMenuItem_Click);
            // 
            // jumpToEndToolStripMenuItem
            // 
            this.jumpToEndToolStripMenuItem.Name = "jumpToEndToolStripMenuItem";
            this.jumpToEndToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D3)));
            this.jumpToEndToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.jumpToEndToolStripMenuItem.Text = "Jump To End";
            this.jumpToEndToolStripMenuItem.Click += new System.EventHandler(this.jumpToEndToolStripMenuItem_Click);
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveStartToPlayheadToolStripMenuItem,
            this.moveEndToPlayheadToolStripMenuItem,
            this.toolStripSeparator3,
            this.findViaRDSToolStripMenuItem,
            this.toolStripSeparator4,
            this.nextRDSItemToolStripMenuItem,
            this.previousRDSItemToolStripMenuItem});
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.moveToolStripMenuItem.Text = "Move...";
            // 
            // moveStartToPlayheadToolStripMenuItem
            // 
            this.moveStartToPlayheadToolStripMenuItem.Name = "moveStartToPlayheadToolStripMenuItem";
            this.moveStartToPlayheadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Z)));
            this.moveStartToPlayheadToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.moveStartToPlayheadToolStripMenuItem.Text = "Move Start to Playhead";
            this.moveStartToPlayheadToolStripMenuItem.Click += new System.EventHandler(this.moveStartToPlayheadToolStripMenuItem_Click);
            // 
            // moveEndToPlayheadToolStripMenuItem
            // 
            this.moveEndToPlayheadToolStripMenuItem.Name = "moveEndToPlayheadToolStripMenuItem";
            this.moveEndToPlayheadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.moveEndToPlayheadToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.moveEndToPlayheadToolStripMenuItem.Text = "Move End to Playhead";
            this.moveEndToPlayheadToolStripMenuItem.Click += new System.EventHandler(this.moveEndToPlayheadToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(230, 6);
            // 
            // findViaRDSToolStripMenuItem
            // 
            this.findViaRDSToolStripMenuItem.Name = "findViaRDSToolStripMenuItem";
            this.findViaRDSToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.findViaRDSToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.findViaRDSToolStripMenuItem.Text = "Find via RDS";
            this.findViaRDSToolStripMenuItem.Click += new System.EventHandler(this.btnAutoRds_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(230, 6);
            // 
            // nextRDSItemToolStripMenuItem
            // 
            this.nextRDSItemToolStripMenuItem.Name = "nextRDSItemToolStripMenuItem";
            this.nextRDSItemToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.nextRDSItemToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.nextRDSItemToolStripMenuItem.Text = "Next RDS Item";
            this.nextRDSItemToolStripMenuItem.Click += new System.EventHandler(this.nextRDSItemToolStripMenuItem_Click);
            // 
            // previousRDSItemToolStripMenuItem
            // 
            this.previousRDSItemToolStripMenuItem.Name = "previousRDSItemToolStripMenuItem";
            this.previousRDSItemToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.previousRDSItemToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.previousRDSItemToolStripMenuItem.Text = "Previous RDS Item";
            this.previousRDSItemToolStripMenuItem.Click += new System.EventHandler(this.previousRDSItemToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportRDSToolStripMenuItem,
            this.rDSDSPToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // exportRDSToolStripMenuItem
            // 
            this.exportRDSToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rawPSFramesToolStripMenuItem,
            this.rawRTFramesToolStripMenuItem,
            this.parsedRTFramesToolStripMenuItem,
            this.rawPIFramesToolStripMenuItem});
            this.exportRDSToolStripMenuItem.Name = "exportRDSToolStripMenuItem";
            this.exportRDSToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.exportRDSToolStripMenuItem.Text = "Export RDS";
            // 
            // rawPSFramesToolStripMenuItem
            // 
            this.rawPSFramesToolStripMenuItem.Name = "rawPSFramesToolStripMenuItem";
            this.rawPSFramesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.rawPSFramesToolStripMenuItem.Text = "Raw PS Frames...";
            this.rawPSFramesToolStripMenuItem.Click += new System.EventHandler(this.rawPSFramesToolStripMenuItem_Click);
            // 
            // rawRTFramesToolStripMenuItem
            // 
            this.rawRTFramesToolStripMenuItem.Name = "rawRTFramesToolStripMenuItem";
            this.rawRTFramesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.rawRTFramesToolStripMenuItem.Text = "Raw RT Frames...";
            this.rawRTFramesToolStripMenuItem.Click += new System.EventHandler(this.rawRTFramesToolStripMenuItem_Click);
            // 
            // parsedRTFramesToolStripMenuItem
            // 
            this.parsedRTFramesToolStripMenuItem.Name = "parsedRTFramesToolStripMenuItem";
            this.parsedRTFramesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.parsedRTFramesToolStripMenuItem.Text = "Parsed RT Frames...";
            this.parsedRTFramesToolStripMenuItem.Click += new System.EventHandler(this.parsedRTFramesToolStripMenuItem_Click);
            // 
            // rawPIFramesToolStripMenuItem
            // 
            this.rawPIFramesToolStripMenuItem.Name = "rawPIFramesToolStripMenuItem";
            this.rawPIFramesToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.rawPIFramesToolStripMenuItem.Text = "Raw PI Frames...";
            this.rawPIFramesToolStripMenuItem.Click += new System.EventHandler(this.rawPIFramesToolStripMenuItem_Click);
            // 
            // rDSDSPToolStripMenuItem
            // 
            this.rDSDSPToolStripMenuItem.Name = "rDSDSPToolStripMenuItem";
            this.rDSDSPToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.rDSDSPToolStripMenuItem.Text = "RDS DSP";
            // 
            // tipsToolStripMenuItem
            // 
            this.tipsToolStripMenuItem.Name = "tipsToolStripMenuItem";
            this.tipsToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.tipsToolStripMenuItem.Text = "Tips...";
            this.tipsToolStripMenuItem.Click += new System.EventHandler(this.tipsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(207, 6);
            // 
            // blockCurrentRTToolStripMenuItem
            // 
            this.blockCurrentRTToolStripMenuItem.Name = "blockCurrentRTToolStripMenuItem";
            this.blockCurrentRTToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.blockCurrentRTToolStripMenuItem.Text = "Block/Unblock Current RT";
            this.blockCurrentRTToolStripMenuItem.Click += new System.EventHandler(this.blockCurrentRTToolStripMenuItem_Click);
            // 
            // itemsListSummaryLabel
            // 
            this.itemsListSummaryLabel.Location = new System.Drawing.Point(9, 518);
            this.itemsListSummaryLabel.Name = "itemsListSummaryLabel";
            this.itemsListSummaryLabel.Size = new System.Drawing.Size(616, 16);
            this.itemsListSummaryLabel.TabIndex = 18;
            this.itemsListSummaryLabel.Text = "Items";
            // 
            // recordingDateLabel
            // 
            this.recordingDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recordingDateLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.recordingDateLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recordingDateLabel.Location = new System.Drawing.Point(1156, 27);
            this.recordingDateLabel.Name = "recordingDateLabel";
            this.recordingDateLabel.Padding = new System.Windows.Forms.Padding(5);
            this.recordingDateLabel.Size = new System.Drawing.Size(114, 25);
            this.recordingDateLabel.TabIndex = 19;
            this.recordingDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // clipGrid1
            // 
            this.clipGrid1.IsMini = true;
            this.clipGrid1.Location = new System.Drawing.Point(12, 366);
            this.clipGrid1.Name = "clipGrid1";
            this.clipGrid1.Size = new System.Drawing.Size(613, 149);
            this.clipGrid1.TabIndex = 14;
            // 
            // transportControls
            // 
            this.transportControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.transportControls.Location = new System.Drawing.Point(12, 537);
            this.transportControls.Name = "transportControls";
            this.transportControls.Size = new System.Drawing.Size(1361, 240);
            this.transportControls.TabIndex = 0;
            this.transportControls.TimeChanged += new IQArchiveManager.Client.Pre.PreProcessorFileStreamReader_Event(this.transportControls_TimeChanged);
            // 
            // MainEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1385, 789);
            this.Controls.Add(this.recordingDateLabel);
            this.Controls.Add(this.itemsListSummaryLabel);
            this.Controls.Add(this.recordingTimeLabel);
            this.Controls.Add(this.clipGrid1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rdsRtLabel);
            this.Controls.Add(this.rdsPsLabel);
            this.Controls.Add(this.waterfallView1);
            this.Controls.Add(this.spectrumView1);
            this.Controls.Add(this.transportControls);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainEditor";
            this.Text = "MainEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainEditor_FormClosing);
            this.Load += new System.EventHandler(this.MainEditor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.prefixSuffixPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.TransportControls transportControls;
        private RomanPort.LibSDR.UI.SpectrumView spectrumView1;
        private RomanPort.LibSDR.UI.WaterfallView waterfallView1;
        private System.Windows.Forms.Label rdsPsLabel;
        private System.Windows.Forms.Label rdsRtLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAutoRds;
        private System.Windows.Forms.Button btnAddClip;
        private System.Windows.Forms.TextBox inputNotes;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TableLayoutPanel prefixSuffixPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox inputPrefix;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox inputSuffix;
        private System.Windows.Forms.TextBox inputTitle;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox inputArtist;
        private System.Windows.Forms.Label labelArtist;
        private System.Windows.Forms.CheckBox inputFlagOk;
        private System.Windows.Forms.CheckBox inputFlagHd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox inputFlagRds;
        private System.Windows.Forms.TextBox inputCall;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox editedClipsList;
        private System.Windows.Forms.Button btnFileDelete;
        private System.Windows.Forms.Button btnFileSave;
        private Components.ClipGrid clipGrid1;
        private System.Windows.Forms.Label recordingTimeLabel;
        private System.Windows.Forms.ComboBox rdsPatchMethod;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnShiftSuffix;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem transportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgeStartForward20sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgeStartBackwards20sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgeStartForwards2sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgeStartBackwards2sToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem nudgePlayheadForwards20sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgePlayheadBackwards20sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgePlayheadForwards2sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgePlayheadBackwards2sToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem nudgeEndForwards20sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgeEndBackwards20sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgeEndForwards2sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nudgeEndBackwards2sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jumpToStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jumpToEndToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveStartToPlayheadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveEndToPlayheadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem findViaRDSToolStripMenuItem;
        private System.Windows.Forms.RadioButton typeBtnLiner;
        private System.Windows.Forms.RadioButton typeBtnSong;
        private System.Windows.Forms.TextBox autoTgCall;
        private System.Windows.Forms.Button autoTgBtn;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportRDSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawPSFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawRTFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parsedRTFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rDSDSPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem navigateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem nextRDSItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousRDSItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawPIFramesToolStripMenuItem;
        private System.Windows.Forms.Button btnLockCall;
        private System.Windows.Forms.Label itemsListSummaryLabel;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tipsToolStripMenuItem;
        private System.Windows.Forms.Label recordingDateLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem blockCurrentRTToolStripMenuItem;
        private System.Windows.Forms.Button btnSwapTitleArtist;
    }
}