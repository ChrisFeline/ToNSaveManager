namespace ToNSaveManager.Windows
{
    partial class SettingsWindow
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
            components = new System.ComponentModel.Container();
            groupBoxGeneral = new GroupBox();
            checkDiscordBackup = new CheckBox();
            checkShowWinLose = new CheckBox();
            checkSaveTerrorsNote = new CheckBox();
            checkSaveTerrors = new CheckBox();
            checkPlayerNames = new CheckBox();
            checkAutoCopy = new CheckBox();
            checkSkipParsedLogs = new CheckBox();
            groupBoxNotifications = new GroupBox();
            checkPlayAudio = new CheckBox();
            checkXSOverlay = new CheckBox();
            groupBox1 = new GroupBox();
            checkShowDate = new CheckBox();
            checkInvertMD = new CheckBox();
            checkShowSeconds = new CheckBox();
            check24Hour = new CheckBox();
            btnCheckForUpdates = new Button();
            btnOpenData = new Button();
            ctxData = new ContextMenuStrip(components);
            setDataLocationToolStripMenuItem = new ToolStripMenuItem();
            ctxItemPickFolder = new ToolStripMenuItem();
            ctxItemResetToDefault = new ToolStripMenuItem();
            toolTip = new ToolTip(components);
            groupBox2 = new GroupBox();
            checkColorObjectives = new CheckBox();
            groupBoxGeneral.SuspendLayout();
            groupBoxNotifications.SuspendLayout();
            groupBox1.SuspendLayout();
            ctxData.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxGeneral
            // 
            groupBoxGeneral.AutoSize = true;
            groupBoxGeneral.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxGeneral.Controls.Add(checkDiscordBackup);
            groupBoxGeneral.Controls.Add(checkShowWinLose);
            groupBoxGeneral.Controls.Add(checkSaveTerrorsNote);
            groupBoxGeneral.Controls.Add(checkSaveTerrors);
            groupBoxGeneral.Controls.Add(checkPlayerNames);
            groupBoxGeneral.Controls.Add(checkAutoCopy);
            groupBoxGeneral.Controls.Add(checkSkipParsedLogs);
            groupBoxGeneral.Dock = DockStyle.Top;
            groupBoxGeneral.ForeColor = Color.White;
            groupBoxGeneral.Location = new Point(8, 8);
            groupBoxGeneral.Name = "groupBoxGeneral";
            groupBoxGeneral.Size = new Size(268, 148);
            groupBoxGeneral.TabIndex = 0;
            groupBoxGeneral.TabStop = false;
            groupBoxGeneral.Text = "General";
            // 
            // checkDiscordBackup
            // 
            checkDiscordBackup.Dock = DockStyle.Top;
            checkDiscordBackup.Location = new Point(3, 127);
            checkDiscordBackup.Name = "checkDiscordBackup";
            checkDiscordBackup.Padding = new Padding(3, 0, 3, 0);
            checkDiscordBackup.Size = new Size(262, 18);
            checkDiscordBackup.TabIndex = 6;
            checkDiscordBackup.Tag = "DiscordWebhookEnabled|Automatically saves your new codes to a Discord channel using a webhook integration.";
            checkDiscordBackup.Text = "Auto Discord Backup (Webhook)";
            checkDiscordBackup.UseVisualStyleBackColor = true;
            // 
            // checkShowWinLose
            // 
            checkShowWinLose.Dock = DockStyle.Top;
            checkShowWinLose.ForeColor = Color.PowderBlue;
            checkShowWinLose.Location = new Point(3, 109);
            checkShowWinLose.Name = "checkShowWinLose";
            checkShowWinLose.Padding = new Padding(21, 0, 3, 0);
            checkShowWinLose.Size = new Size(262, 18);
            checkShowWinLose.TabIndex = 5;
            checkShowWinLose.Tag = "ShowWinLose|Entries will show a [R], [W] or [D] tag based on the source that triggered the save.";
            checkShowWinLose.Text = "Show [R][W][D] Tags";
            checkShowWinLose.UseVisualStyleBackColor = true;
            // 
            // checkSaveTerrorsNote
            // 
            checkSaveTerrorsNote.Dock = DockStyle.Top;
            checkSaveTerrorsNote.ForeColor = Color.PowderBlue;
            checkSaveTerrorsNote.Location = new Point(3, 91);
            checkSaveTerrorsNote.Name = "checkSaveTerrorsNote";
            checkSaveTerrorsNote.Padding = new Padding(21, 0, 3, 0);
            checkSaveTerrorsNote.Size = new Size(262, 18);
            checkSaveTerrorsNote.TabIndex = 3;
            checkSaveTerrorsNote.Tag = "SaveRoundNote|Automatically set survived terror names as note.";
            checkSaveTerrorsNote.Text = "Terror Name Notes";
            checkSaveTerrorsNote.UseVisualStyleBackColor = true;
            // 
            // checkSaveTerrors
            // 
            checkSaveTerrors.Dock = DockStyle.Top;
            checkSaveTerrors.Location = new Point(3, 73);
            checkSaveTerrors.Name = "checkSaveTerrors";
            checkSaveTerrors.Padding = new Padding(3, 0, 3, 0);
            checkSaveTerrors.Size = new Size(262, 18);
            checkSaveTerrors.TabIndex = 2;
            checkSaveTerrors.Tag = "SaveRoundInfo|Save codes will display the last round type and terror names.";
            checkSaveTerrors.Text = "Save Round Info";
            checkSaveTerrors.UseVisualStyleBackColor = true;
            // 
            // checkPlayerNames
            // 
            checkPlayerNames.Dock = DockStyle.Top;
            checkPlayerNames.Location = new Point(3, 55);
            checkPlayerNames.Name = "checkPlayerNames";
            checkPlayerNames.Padding = new Padding(3, 0, 3, 0);
            checkPlayerNames.Size = new Size(262, 18);
            checkPlayerNames.TabIndex = 1;
            checkPlayerNames.Tag = "SaveNames|Save codes will show players in the instance at the time of saving.";
            checkPlayerNames.Text = "Collect Player Names";
            checkPlayerNames.UseVisualStyleBackColor = true;
            // 
            // checkAutoCopy
            // 
            checkAutoCopy.Dock = DockStyle.Top;
            checkAutoCopy.Location = new Point(3, 37);
            checkAutoCopy.Name = "checkAutoCopy";
            checkAutoCopy.Padding = new Padding(3, 0, 3, 0);
            checkAutoCopy.Size = new Size(262, 18);
            checkAutoCopy.TabIndex = 0;
            checkAutoCopy.Tag = "AutoCopy|Automatically copy new save codes to clipboard.";
            checkAutoCopy.Text = "Auto Clipboard Copy";
            checkAutoCopy.UseVisualStyleBackColor = true;
            // 
            // checkSkipParsedLogs
            // 
            checkSkipParsedLogs.Dock = DockStyle.Top;
            checkSkipParsedLogs.Location = new Point(3, 19);
            checkSkipParsedLogs.Name = "checkSkipParsedLogs";
            checkSkipParsedLogs.Padding = new Padding(3, 0, 3, 0);
            checkSkipParsedLogs.Size = new Size(262, 18);
            checkSkipParsedLogs.TabIndex = 4;
            checkSkipParsedLogs.Tag = "SkipParsedLogs|Skip old parsed log files that were already processed and saved.\\nOnly disable this if you accidentally deleted a save code.";
            checkSkipParsedLogs.Text = "Skip Parsed Logs (!)";
            checkSkipParsedLogs.UseVisualStyleBackColor = true;
            // 
            // groupBoxNotifications
            // 
            groupBoxNotifications.AutoSize = true;
            groupBoxNotifications.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxNotifications.Controls.Add(checkPlayAudio);
            groupBoxNotifications.Controls.Add(checkXSOverlay);
            groupBoxNotifications.Dock = DockStyle.Top;
            groupBoxNotifications.ForeColor = Color.White;
            groupBoxNotifications.Location = new Point(8, 156);
            groupBoxNotifications.Name = "groupBoxNotifications";
            groupBoxNotifications.Size = new Size(268, 58);
            groupBoxNotifications.TabIndex = 2;
            groupBoxNotifications.TabStop = false;
            groupBoxNotifications.Text = "Notifications";
            // 
            // checkPlayAudio
            // 
            checkPlayAudio.AutoCheck = false;
            checkPlayAudio.Dock = DockStyle.Top;
            checkPlayAudio.Location = new Point(3, 37);
            checkPlayAudio.Name = "checkPlayAudio";
            checkPlayAudio.Padding = new Padding(3, 0, 3, 0);
            checkPlayAudio.Size = new Size(262, 18);
            checkPlayAudio.TabIndex = 1;
            checkPlayAudio.Tag = "PlayAudio|Double click to select custom audio file.\\nRight click to reset back to 'default.wav'";
            checkPlayAudio.Text = "Play Sound (default.wav)";
            checkPlayAudio.UseVisualStyleBackColor = true;
            checkPlayAudio.MouseDown += checkPlayAudio_MouseDown;
            checkPlayAudio.MouseUp += checkPlayAudio_MouseUp;
            // 
            // checkXSOverlay
            // 
            checkXSOverlay.Dock = DockStyle.Top;
            checkXSOverlay.Location = new Point(3, 19);
            checkXSOverlay.Name = "checkXSOverlay";
            checkXSOverlay.Padding = new Padding(3, 0, 3, 0);
            checkXSOverlay.Size = new Size(262, 18);
            checkXSOverlay.TabIndex = 0;
            checkXSOverlay.Tag = "XSOverlay|XSOverlay popup notifications when saving.";
            checkXSOverlay.Text = "XSOverlay Popup";
            checkXSOverlay.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(checkShowDate);
            groupBox1.Controls.Add(checkInvertMD);
            groupBox1.Controls.Add(checkShowSeconds);
            groupBox1.Controls.Add(check24Hour);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(8, 214);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(268, 94);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Time Formatting";
            // 
            // checkShowDate
            // 
            checkShowDate.Dock = DockStyle.Top;
            checkShowDate.Location = new Point(3, 73);
            checkShowDate.Name = "checkShowDate";
            checkShowDate.Padding = new Padding(3, 0, 3, 0);
            checkShowDate.Size = new Size(262, 18);
            checkShowDate.TabIndex = 3;
            checkShowDate.Tag = "ShowDate|Entries on the right panel will display a full date.";
            checkShowDate.Text = "Right Panel Date";
            checkShowDate.UseVisualStyleBackColor = true;
            // 
            // checkInvertMD
            // 
            checkInvertMD.Dock = DockStyle.Top;
            checkInvertMD.Location = new Point(3, 55);
            checkInvertMD.Name = "checkInvertMD";
            checkInvertMD.Padding = new Padding(3, 0, 3, 0);
            checkInvertMD.Size = new Size(262, 18);
            checkInvertMD.TabIndex = 2;
            checkInvertMD.Tag = "InvertMD";
            checkInvertMD.Text = "Invert Month/Day";
            checkInvertMD.UseVisualStyleBackColor = true;
            // 
            // checkShowSeconds
            // 
            checkShowSeconds.Dock = DockStyle.Top;
            checkShowSeconds.Location = new Point(3, 37);
            checkShowSeconds.Name = "checkShowSeconds";
            checkShowSeconds.Padding = new Padding(3, 0, 3, 0);
            checkShowSeconds.Size = new Size(262, 18);
            checkShowSeconds.TabIndex = 1;
            checkShowSeconds.Tag = "ShowSeconds";
            checkShowSeconds.Text = "Show Seconds";
            checkShowSeconds.UseVisualStyleBackColor = true;
            // 
            // check24Hour
            // 
            check24Hour.Dock = DockStyle.Top;
            check24Hour.Location = new Point(3, 19);
            check24Hour.Name = "check24Hour";
            check24Hour.Padding = new Padding(3, 0, 3, 0);
            check24Hour.Size = new Size(262, 18);
            check24Hour.TabIndex = 0;
            check24Hour.Tag = "Use24Hour";
            check24Hour.Text = "24 Hour Time";
            check24Hour.UseVisualStyleBackColor = true;
            // 
            // btnCheckForUpdates
            // 
            btnCheckForUpdates.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCheckForUpdates.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            btnCheckForUpdates.FlatStyle = FlatStyle.Flat;
            btnCheckForUpdates.ForeColor = Color.White;
            btnCheckForUpdates.Location = new Point(8, 356);
            btnCheckForUpdates.Name = "btnCheckForUpdates";
            btnCheckForUpdates.Size = new Size(209, 24);
            btnCheckForUpdates.TabIndex = 4;
            btnCheckForUpdates.Text = "Check For Updates";
            btnCheckForUpdates.UseVisualStyleBackColor = true;
            btnCheckForUpdates.Click += btnCheckForUpdates_Click;
            // 
            // btnOpenData
            // 
            btnOpenData.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOpenData.ContextMenuStrip = ctxData;
            btnOpenData.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            btnOpenData.FlatStyle = FlatStyle.Flat;
            btnOpenData.ForeColor = Color.White;
            btnOpenData.Location = new Point(223, 356);
            btnOpenData.Name = "btnOpenData";
            btnOpenData.Size = new Size(53, 24);
            btnOpenData.TabIndex = 5;
            btnOpenData.Tag = "|Open local app data folder.";
            btnOpenData.Text = "Data";
            btnOpenData.UseVisualStyleBackColor = true;
            btnOpenData.Click += btnOpenData_Click;
            // 
            // ctxData
            // 
            ctxData.Items.AddRange(new ToolStripItem[] { setDataLocationToolStripMenuItem });
            ctxData.Name = "ctxData";
            ctxData.Size = new Size(193, 26);
            // 
            // setDataLocationToolStripMenuItem
            // 
            setDataLocationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ctxItemPickFolder, ctxItemResetToDefault });
            setDataLocationToolStripMenuItem.Name = "setDataLocationToolStripMenuItem";
            setDataLocationToolStripMenuItem.Size = new Size(192, 22);
            setDataLocationToolStripMenuItem.Text = "Custom Data Location";
            // 
            // ctxItemPickFolder
            // 
            ctxItemPickFolder.Name = "ctxItemPickFolder";
            ctxItemPickFolder.Size = new Size(157, 22);
            ctxItemPickFolder.Text = "Pick Folder";
            ctxItemPickFolder.Click += ctxItemPickFolder_Click;
            // 
            // ctxItemResetToDefault
            // 
            ctxItemResetToDefault.Name = "ctxItemResetToDefault";
            ctxItemResetToDefault.Size = new Size(157, 22);
            ctxItemResetToDefault.Text = "Reset to Default";
            ctxItemResetToDefault.Click += ctxItemResetToDefault_Click;
            // 
            // toolTip
            // 
            toolTip.AutoPopDelay = 15000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 100;
            // 
            // groupBox2
            // 
            groupBox2.AutoSize = true;
            groupBox2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox2.Controls.Add(checkColorObjectives);
            groupBox2.Dock = DockStyle.Top;
            groupBox2.ForeColor = Color.White;
            groupBox2.Location = new Point(8, 308);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(268, 40);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Style";
            // 
            // checkColorObjectives
            // 
            checkColorObjectives.Dock = DockStyle.Top;
            checkColorObjectives.Location = new Point(3, 19);
            checkColorObjectives.Name = "checkColorObjectives";
            checkColorObjectives.Padding = new Padding(3, 0, 3, 0);
            checkColorObjectives.Size = new Size(262, 18);
            checkColorObjectives.TabIndex = 0;
            checkColorObjectives.Tag = "ColorfulObjectives|Items in the 'Objectives' window will show colors that correspond to those of the items in the game.";
            checkColorObjectives.Text = "Colorful Objectives";
            checkColorObjectives.UseVisualStyleBackColor = true;
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(284, 388);
            Controls.Add(groupBox2);
            Controls.Add(btnOpenData);
            Controls.Add(btnCheckForUpdates);
            Controls.Add(groupBox1);
            Controls.Add(groupBoxNotifications);
            Controls.Add(groupBoxGeneral);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(258, 200);
            Name = "SettingsWindow";
            Padding = new Padding(8);
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Settings";
            FormClosed += SettingsWindow_FormClosed;
            Load += SettingsWindow_Load;
            groupBoxGeneral.ResumeLayout(false);
            groupBoxNotifications.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            ctxData.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox groupBoxGeneral;
        private CheckBox checkAutoCopy;
        private CheckBox checkPlayerNames;
        private Button btnCheckForUpdates;
        private GroupBox groupBoxNotifications;
        private CheckBox checkPlayAudio;
        private CheckBox checkXSOverlay;
        private GroupBox groupBox1;
        private CheckBox checkInvertMD;
        private CheckBox checkShowSeconds;
        private CheckBox check24Hour;
        private Button btnOpenData;
        private ToolTip toolTip;
        private GroupBox groupBox2;
        private CheckBox checkColorObjectives;
        private CheckBox checkSaveTerrorsNote;
        private CheckBox checkSaveTerrors;
        private CheckBox checkSkipParsedLogs;
        private CheckBox checkShowDate;
        private CheckBox checkShowWinLose;
        private ContextMenuStrip ctxData;
        private ToolStripMenuItem setDataLocationToolStripMenuItem;
        private ToolStripMenuItem ctxItemPickFolder;
        private ToolStripMenuItem ctxItemResetToDefault;
        private CheckBox checkDiscordBackup;
    }
}