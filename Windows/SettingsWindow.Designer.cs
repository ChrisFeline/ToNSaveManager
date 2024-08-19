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
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            groupBoxGeneral = new GroupBox();
            checkDiscordBackup = new CheckBox();
            checkShowWinLose = new CheckBox();
            checkSaveTerrorsNote = new CheckBox();
            checkSaveTerrors = new CheckBox();
            checkPlayerNames = new CheckBox();
            checkAutoCopy = new CheckBox();
            checkSkipParsedLogs = new CheckBox();
            checkOSCEnabled = new CheckBox();
            groupBoxNotifications = new GroupBox();
            checkPlayAudio = new CheckBox();
            checkXSOverlay = new CheckBox();
            groupBoxTime = new GroupBox();
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
            groupBoxStyle = new GroupBox();
            checkColorObjectives = new CheckBox();
            languageSelectBox = new ComboBox();
            panel1 = new Panel();
            groupBoxOSC = new GroupBox();
            checkOSCSendColor = new CheckBox();
            checkSendChatbox = new CheckBox();
            groupBoxGeneral.SuspendLayout();
            groupBoxNotifications.SuspendLayout();
            groupBoxTime.SuspendLayout();
            ctxData.SuspendLayout();
            groupBoxStyle.SuspendLayout();
            panel1.SuspendLayout();
            groupBoxOSC.SuspendLayout();
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
            groupBoxGeneral.Location = new Point(8, 31);
            groupBoxGeneral.Name = "groupBoxGeneral";
            groupBoxGeneral.Size = new Size(338, 155);
            groupBoxGeneral.TabIndex = 0;
            groupBoxGeneral.TabStop = false;
            groupBoxGeneral.Text = "General";
            // 
            // checkDiscordBackup
            // 
            checkDiscordBackup.AutoSize = true;
            checkDiscordBackup.Dock = DockStyle.Top;
            checkDiscordBackup.Location = new Point(3, 133);
            checkDiscordBackup.Name = "checkDiscordBackup";
            checkDiscordBackup.Padding = new Padding(3, 0, 3, 0);
            checkDiscordBackup.Size = new Size(332, 19);
            checkDiscordBackup.TabIndex = 6;
            checkDiscordBackup.Tag = "DiscordWebhookEnabled|Automatically saves your new codes to a Discord channel using a webhook integration.";
            checkDiscordBackup.Text = "Auto Discord Backup (Webhook)";
            checkDiscordBackup.UseVisualStyleBackColor = true;
            // 
            // checkShowWinLose
            // 
            checkShowWinLose.AutoSize = true;
            checkShowWinLose.Dock = DockStyle.Top;
            checkShowWinLose.ForeColor = Color.PowderBlue;
            checkShowWinLose.Location = new Point(3, 114);
            checkShowWinLose.Name = "checkShowWinLose";
            checkShowWinLose.Padding = new Padding(21, 0, 3, 0);
            checkShowWinLose.Size = new Size(332, 19);
            checkShowWinLose.TabIndex = 5;
            checkShowWinLose.Tag = "ShowWinLose|Entries will show a [R], [W] or [D] tag based on the source that triggered the save.";
            checkShowWinLose.Text = "Show [R][W][D] Tags";
            checkShowWinLose.UseVisualStyleBackColor = true;
            // 
            // checkSaveTerrorsNote
            // 
            checkSaveTerrorsNote.AutoSize = true;
            checkSaveTerrorsNote.Dock = DockStyle.Top;
            checkSaveTerrorsNote.ForeColor = Color.PowderBlue;
            checkSaveTerrorsNote.Location = new Point(3, 95);
            checkSaveTerrorsNote.Name = "checkSaveTerrorsNote";
            checkSaveTerrorsNote.Padding = new Padding(21, 0, 3, 0);
            checkSaveTerrorsNote.Size = new Size(332, 19);
            checkSaveTerrorsNote.TabIndex = 3;
            checkSaveTerrorsNote.Tag = "SaveRoundNote|Automatically set survived terror names as note.";
            checkSaveTerrorsNote.Text = "Terror Name Notes";
            checkSaveTerrorsNote.UseVisualStyleBackColor = true;
            // 
            // checkSaveTerrors
            // 
            checkSaveTerrors.AutoSize = true;
            checkSaveTerrors.Dock = DockStyle.Top;
            checkSaveTerrors.Location = new Point(3, 76);
            checkSaveTerrors.Name = "checkSaveTerrors";
            checkSaveTerrors.Padding = new Padding(3, 0, 3, 0);
            checkSaveTerrors.Size = new Size(332, 19);
            checkSaveTerrors.TabIndex = 2;
            checkSaveTerrors.Tag = "SaveRoundInfo|Save codes will display the last round type and terror names.";
            checkSaveTerrors.Text = "Save Round Info";
            checkSaveTerrors.UseVisualStyleBackColor = true;
            // 
            // checkPlayerNames
            // 
            checkPlayerNames.AutoSize = true;
            checkPlayerNames.Dock = DockStyle.Top;
            checkPlayerNames.Location = new Point(3, 57);
            checkPlayerNames.Name = "checkPlayerNames";
            checkPlayerNames.Padding = new Padding(3, 0, 3, 0);
            checkPlayerNames.Size = new Size(332, 19);
            checkPlayerNames.TabIndex = 1;
            checkPlayerNames.Tag = "SaveNames|Save codes will show players in the instance at the time of saving.";
            checkPlayerNames.Text = "Collect Player Names";
            checkPlayerNames.UseVisualStyleBackColor = true;
            // 
            // checkAutoCopy
            // 
            checkAutoCopy.AutoSize = true;
            checkAutoCopy.Dock = DockStyle.Top;
            checkAutoCopy.Location = new Point(3, 38);
            checkAutoCopy.Name = "checkAutoCopy";
            checkAutoCopy.Padding = new Padding(3, 0, 3, 0);
            checkAutoCopy.Size = new Size(332, 19);
            checkAutoCopy.TabIndex = 0;
            checkAutoCopy.Tag = "AutoCopy|Automatically copy new save codes to clipboard.";
            checkAutoCopy.Text = "Auto Clipboard Copy";
            checkAutoCopy.UseVisualStyleBackColor = true;
            // 
            // checkSkipParsedLogs
            // 
            checkSkipParsedLogs.AutoSize = true;
            checkSkipParsedLogs.Dock = DockStyle.Top;
            checkSkipParsedLogs.Location = new Point(3, 19);
            checkSkipParsedLogs.Name = "checkSkipParsedLogs";
            checkSkipParsedLogs.Padding = new Padding(3, 0, 3, 0);
            checkSkipParsedLogs.RightToLeft = RightToLeft.No;
            checkSkipParsedLogs.Size = new Size(332, 19);
            checkSkipParsedLogs.TabIndex = 4;
            checkSkipParsedLogs.Tag = "SkipParsedLogs|Skip old parsed log files that were already processed and saved.\\nOnly disable this if you accidentally deleted a save code.";
            checkSkipParsedLogs.Text = "Skip Parsed Logs (!)";
            checkSkipParsedLogs.UseVisualStyleBackColor = true;
            // 
            // checkOSCEnabled
            // 
            checkOSCEnabled.AutoSize = true;
            checkOSCEnabled.Dock = DockStyle.Top;
            checkOSCEnabled.Location = new Point(3, 19);
            checkOSCEnabled.Name = "checkOSCEnabled";
            checkOSCEnabled.Padding = new Padding(3, 0, 3, 0);
            checkOSCEnabled.Size = new Size(332, 19);
            checkOSCEnabled.TabIndex = 7;
            checkOSCEnabled.Tag = "OSCEnabled|Sends avatar parameters to VRChat using OSC. Right click this entry to open documentation about parameter names and types.";
            checkOSCEnabled.Text = "Send OSC Parameters";
            checkOSCEnabled.UseVisualStyleBackColor = true;
            checkOSCEnabled.MouseUp += checkOSCEnabled_MouseUp;
            // 
            // groupBoxNotifications
            // 
            groupBoxNotifications.AutoSize = true;
            groupBoxNotifications.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxNotifications.Controls.Add(checkPlayAudio);
            groupBoxNotifications.Controls.Add(checkXSOverlay);
            groupBoxNotifications.Dock = DockStyle.Top;
            groupBoxNotifications.ForeColor = Color.White;
            groupBoxNotifications.Location = new Point(8, 265);
            groupBoxNotifications.Name = "groupBoxNotifications";
            groupBoxNotifications.Size = new Size(338, 60);
            groupBoxNotifications.TabIndex = 2;
            groupBoxNotifications.TabStop = false;
            groupBoxNotifications.Text = "Notifications";
            // 
            // checkPlayAudio
            // 
            checkPlayAudio.AutoCheck = false;
            checkPlayAudio.AutoSize = true;
            checkPlayAudio.Dock = DockStyle.Top;
            checkPlayAudio.Location = new Point(3, 38);
            checkPlayAudio.Name = "checkPlayAudio";
            checkPlayAudio.Padding = new Padding(3, 0, 3, 0);
            checkPlayAudio.Size = new Size(332, 19);
            checkPlayAudio.TabIndex = 1;
            checkPlayAudio.Tag = "PlayAudio|Double click to select custom audio file.\\nRight click to reset back to 'default.wav'";
            checkPlayAudio.Text = "Play Sound (default.wav)";
            checkPlayAudio.UseVisualStyleBackColor = true;
            checkPlayAudio.MouseDown += checkPlayAudio_MouseDown;
            checkPlayAudio.MouseUp += checkPlayAudio_MouseUp;
            // 
            // checkXSOverlay
            // 
            checkXSOverlay.AutoSize = true;
            checkXSOverlay.Dock = DockStyle.Top;
            checkXSOverlay.Location = new Point(3, 19);
            checkXSOverlay.Name = "checkXSOverlay";
            checkXSOverlay.Padding = new Padding(3, 0, 3, 0);
            checkXSOverlay.Size = new Size(332, 19);
            checkXSOverlay.TabIndex = 0;
            checkXSOverlay.Tag = "XSOverlay|XSOverlay popup notifications when saving.";
            checkXSOverlay.Text = "XSOverlay Popup";
            checkXSOverlay.UseVisualStyleBackColor = true;
            // 
            // groupBoxTime
            // 
            groupBoxTime.AutoSize = true;
            groupBoxTime.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxTime.Controls.Add(checkShowDate);
            groupBoxTime.Controls.Add(checkInvertMD);
            groupBoxTime.Controls.Add(checkShowSeconds);
            groupBoxTime.Controls.Add(check24Hour);
            groupBoxTime.Dock = DockStyle.Top;
            groupBoxTime.ForeColor = Color.White;
            groupBoxTime.Location = new Point(8, 325);
            groupBoxTime.Name = "groupBoxTime";
            groupBoxTime.Size = new Size(338, 98);
            groupBoxTime.TabIndex = 3;
            groupBoxTime.TabStop = false;
            groupBoxTime.Text = "Time Formatting";
            // 
            // checkShowDate
            // 
            checkShowDate.AutoSize = true;
            checkShowDate.Dock = DockStyle.Top;
            checkShowDate.Location = new Point(3, 76);
            checkShowDate.Name = "checkShowDate";
            checkShowDate.Padding = new Padding(3, 0, 3, 0);
            checkShowDate.Size = new Size(332, 19);
            checkShowDate.TabIndex = 3;
            checkShowDate.Tag = "ShowDate|Entries on the right panel will display a full date.";
            checkShowDate.Text = "Right Panel Date";
            checkShowDate.UseVisualStyleBackColor = true;
            // 
            // checkInvertMD
            // 
            checkInvertMD.AutoSize = true;
            checkInvertMD.Dock = DockStyle.Top;
            checkInvertMD.Location = new Point(3, 57);
            checkInvertMD.Name = "checkInvertMD";
            checkInvertMD.Padding = new Padding(3, 0, 3, 0);
            checkInvertMD.Size = new Size(332, 19);
            checkInvertMD.TabIndex = 2;
            checkInvertMD.Tag = "InvertMD";
            checkInvertMD.Text = "Invert Month/Day";
            checkInvertMD.UseVisualStyleBackColor = true;
            // 
            // checkShowSeconds
            // 
            checkShowSeconds.AutoSize = true;
            checkShowSeconds.Dock = DockStyle.Top;
            checkShowSeconds.Location = new Point(3, 38);
            checkShowSeconds.Name = "checkShowSeconds";
            checkShowSeconds.Padding = new Padding(3, 0, 3, 0);
            checkShowSeconds.Size = new Size(332, 19);
            checkShowSeconds.TabIndex = 1;
            checkShowSeconds.Tag = "ShowSeconds";
            checkShowSeconds.Text = "Show Seconds";
            checkShowSeconds.UseVisualStyleBackColor = true;
            // 
            // check24Hour
            // 
            check24Hour.AutoSize = true;
            check24Hour.Dock = DockStyle.Top;
            check24Hour.Location = new Point(3, 19);
            check24Hour.Name = "check24Hour";
            check24Hour.Padding = new Padding(3, 0, 3, 0);
            check24Hour.Size = new Size(332, 19);
            check24Hour.TabIndex = 0;
            check24Hour.Tag = "Use24Hour";
            check24Hour.Text = "24 Hour Time";
            check24Hour.UseVisualStyleBackColor = true;
            // 
            // btnCheckForUpdates
            // 
            btnCheckForUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCheckForUpdates.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            btnCheckForUpdates.FlatStyle = FlatStyle.Flat;
            btnCheckForUpdates.ForeColor = Color.White;
            btnCheckForUpdates.Location = new Point(0, 4);
            btnCheckForUpdates.Name = "btnCheckForUpdates";
            btnCheckForUpdates.Size = new Size(279, 24);
            btnCheckForUpdates.TabIndex = 4;
            btnCheckForUpdates.Text = "Check For Updates";
            btnCheckForUpdates.UseVisualStyleBackColor = true;
            btnCheckForUpdates.Click += btnCheckForUpdates_Click;
            // 
            // btnOpenData
            // 
            btnOpenData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            btnOpenData.ContextMenuStrip = ctxData;
            btnOpenData.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            btnOpenData.FlatStyle = FlatStyle.Flat;
            btnOpenData.ForeColor = Color.White;
            btnOpenData.Location = new Point(285, 4);
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
            // groupBoxStyle
            // 
            groupBoxStyle.AutoSize = true;
            groupBoxStyle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxStyle.Controls.Add(checkColorObjectives);
            groupBoxStyle.Dock = DockStyle.Top;
            groupBoxStyle.ForeColor = Color.White;
            groupBoxStyle.Location = new Point(8, 423);
            groupBoxStyle.Name = "groupBoxStyle";
            groupBoxStyle.Size = new Size(338, 41);
            groupBoxStyle.TabIndex = 6;
            groupBoxStyle.TabStop = false;
            groupBoxStyle.Text = "Style";
            // 
            // checkColorObjectives
            // 
            checkColorObjectives.AutoSize = true;
            checkColorObjectives.Dock = DockStyle.Top;
            checkColorObjectives.Location = new Point(3, 19);
            checkColorObjectives.Name = "checkColorObjectives";
            checkColorObjectives.Padding = new Padding(3, 0, 3, 0);
            checkColorObjectives.Size = new Size(332, 19);
            checkColorObjectives.TabIndex = 0;
            checkColorObjectives.Tag = "ColorfulObjectives|Items in the 'Objectives' window will show colors that correspond to those of the items in the game.";
            checkColorObjectives.Text = "Colorful Objectives";
            checkColorObjectives.UseVisualStyleBackColor = true;
            // 
            // languageSelectBox
            // 
            languageSelectBox.AllowDrop = true;
            languageSelectBox.BackColor = SystemColors.Window;
            languageSelectBox.Dock = DockStyle.Top;
            languageSelectBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageSelectBox.FlatStyle = FlatStyle.Flat;
            languageSelectBox.FormattingEnabled = true;
            languageSelectBox.Location = new Point(8, 8);
            languageSelectBox.Name = "languageSelectBox";
            languageSelectBox.Size = new Size(338, 23);
            languageSelectBox.TabIndex = 7;
            languageSelectBox.TabStop = false;
            languageSelectBox.SelectedIndexChanged += languageSelectBox_SelectedIndexChanged;
            languageSelectBox.DragDrop += languageSelect_DragDrop;
            languageSelectBox.DragEnter += languageSelect_DragEnter;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnCheckForUpdates);
            panel1.Controls.Add(btnOpenData);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(8, 464);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(0, 4, 0, 0);
            panel1.Size = new Size(338, 28);
            panel1.TabIndex = 8;
            // 
            // groupBoxOSC
            // 
            groupBoxOSC.AutoSize = true;
            groupBoxOSC.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxOSC.Controls.Add(checkSendChatbox);
            groupBoxOSC.Controls.Add(checkOSCSendColor);
            groupBoxOSC.Controls.Add(checkOSCEnabled);
            groupBoxOSC.Dock = DockStyle.Top;
            groupBoxOSC.ForeColor = Color.White;
            groupBoxOSC.Location = new Point(8, 186);
            groupBoxOSC.Name = "groupBoxOSC";
            groupBoxOSC.Size = new Size(338, 79);
            groupBoxOSC.TabIndex = 7;
            groupBoxOSC.TabStop = false;
            groupBoxOSC.Text = "OSC";
            // 
            // checkOSCSendColor
            // 
            checkOSCSendColor.AutoSize = true;
            checkOSCSendColor.Dock = DockStyle.Top;
            checkOSCSendColor.ForeColor = Color.PowderBlue;
            checkOSCSendColor.Location = new Point(3, 38);
            checkOSCSendColor.Name = "checkOSCSendColor";
            checkOSCSendColor.Padding = new Padding(21, 0, 3, 0);
            checkOSCSendColor.Size = new Size(332, 19);
            checkOSCSendColor.TabIndex = 9;
            checkOSCSendColor.Tag = "OSCSendColor|Sends the current Terror color as HSV parameters.";
            checkOSCSendColor.Text = "Send Terror Color (HSV)";
            checkOSCSendColor.UseVisualStyleBackColor = true;
            // 
            // checkSendChatbox
            // 
            checkSendChatbox.AutoSize = true;
            checkSendChatbox.Dock = DockStyle.Top;
            checkSendChatbox.Location = new Point(3, 57);
            checkSendChatbox.Name = "checkSendChatbox";
            checkSendChatbox.Padding = new Padding(3, 0, 3, 0);
            checkSendChatbox.Size = new Size(332, 19);
            checkSendChatbox.TabIndex = 8;
            checkSendChatbox.Tag = "OSCSendChatbox|Sends ToN information to the VRChat chatbox.\\nRight click to customize the message template.";
            checkSendChatbox.Text = "Send Chatbox Message";
            checkSendChatbox.UseVisualStyleBackColor = true;
            checkSendChatbox.MouseDown += checkSendChatbox_MouseDown;
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(354, 499);
            Controls.Add(panel1);
            Controls.Add(groupBoxStyle);
            Controls.Add(groupBoxTime);
            Controls.Add(groupBoxNotifications);
            Controls.Add(groupBoxOSC);
            Controls.Add(groupBoxGeneral);
            Controls.Add(languageSelectBox);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(370, 10);
            Name = "SettingsWindow";
            Padding = new Padding(8);
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Settings";
            FormClosed += SettingsWindow_FormClosed;
            Load += SettingsWindow_Load;
            groupBoxGeneral.ResumeLayout(false);
            groupBoxGeneral.PerformLayout();
            groupBoxNotifications.ResumeLayout(false);
            groupBoxNotifications.PerformLayout();
            groupBoxTime.ResumeLayout(false);
            groupBoxTime.PerformLayout();
            ctxData.ResumeLayout(false);
            groupBoxStyle.ResumeLayout(false);
            groupBoxStyle.PerformLayout();
            panel1.ResumeLayout(false);
            groupBoxOSC.ResumeLayout(false);
            groupBoxOSC.PerformLayout();
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
        private GroupBox groupBoxTime;
        private CheckBox checkInvertMD;
        private CheckBox checkShowSeconds;
        private CheckBox check24Hour;
        private Button btnOpenData;
        private ToolTip toolTip;
        private GroupBox groupBoxStyle;
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
        private CheckBox checkOSCEnabled;
        private ComboBox languageSelectBox;
        private Panel panel1;
        private GroupBox groupBoxOSC;
        private CheckBox checkSendChatbox;
        private CheckBox checkOSCSendColor;
    }
}