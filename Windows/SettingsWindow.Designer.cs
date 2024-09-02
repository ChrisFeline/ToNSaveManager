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
            checkDiscordBackup = new CheckBox();
            checkShowWinLose = new CheckBox();
            checkSaveTerrorsNote = new CheckBox();
            checkSaveTerrors = new CheckBox();
            checkPlayerNames = new CheckBox();
            checkAutoCopy = new CheckBox();
            checkSkipParsedLogs = new CheckBox();
            checkOSCEnabled = new CheckBox();
            checkPlayAudio = new CheckBox();
            checkXSOverlay = new CheckBox();
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
            checkColorObjectives = new CheckBox();
            languageSelectBox = new ComboBox();
            panel1 = new Panel();
            checkSendChatbox = new CheckBox();
            checkOSCSendColor = new CheckBox();
            panel2 = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            labelGroupGeneral = new Label();
            labelGroupOSC = new Label();
            labelGroupNotifications = new Label();
            labelGroupFormat = new Label();
            labelGroupStyle = new Label();
            ctxData.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // checkDiscordBackup
            // 
            checkDiscordBackup.AutoSize = true;
            checkDiscordBackup.Location = new Point(3, 142);
            checkDiscordBackup.Margin = new Padding(3, 1, 3, 1);
            checkDiscordBackup.Name = "checkDiscordBackup";
            checkDiscordBackup.Padding = new Padding(20, 0, 3, 0);
            checkDiscordBackup.Size = new Size(222, 19);
            checkDiscordBackup.TabIndex = 6;
            checkDiscordBackup.Tag = "DiscordWebhookEnabled|Automatically saves your new codes to a Discord channel using a webhook integration.";
            checkDiscordBackup.Text = "Auto Discord Backup (Webhook)";
            checkDiscordBackup.UseVisualStyleBackColor = true;
            // 
            // checkShowWinLose
            // 
            checkShowWinLose.AutoSize = true;
            checkShowWinLose.ForeColor = Color.PowderBlue;
            checkShowWinLose.Location = new Point(3, 100);
            checkShowWinLose.Margin = new Padding(3, 1, 3, 1);
            checkShowWinLose.Name = "checkShowWinLose";
            checkShowWinLose.Padding = new Padding(50, 0, 3, 0);
            checkShowWinLose.Size = new Size(187, 19);
            checkShowWinLose.TabIndex = 5;
            checkShowWinLose.Tag = "ShowWinLose|Entries will show a [R], [W] or [D] tag based on the source that triggered the save.";
            checkShowWinLose.Text = "Show [R][W][D] Tags";
            checkShowWinLose.UseVisualStyleBackColor = true;
            // 
            // checkSaveTerrorsNote
            // 
            checkSaveTerrorsNote.AutoSize = true;
            checkSaveTerrorsNote.ForeColor = Color.PowderBlue;
            checkSaveTerrorsNote.Location = new Point(3, 121);
            checkSaveTerrorsNote.Margin = new Padding(3, 1, 3, 1);
            checkSaveTerrorsNote.Name = "checkSaveTerrorsNote";
            checkSaveTerrorsNote.Padding = new Padding(50, 0, 3, 0);
            checkSaveTerrorsNote.Size = new Size(178, 19);
            checkSaveTerrorsNote.TabIndex = 3;
            checkSaveTerrorsNote.Tag = "SaveRoundNote|Automatically set survived terror names as note.";
            checkSaveTerrorsNote.Text = "Terror Name Notes";
            checkSaveTerrorsNote.UseVisualStyleBackColor = true;
            // 
            // checkSaveTerrors
            // 
            checkSaveTerrors.AutoSize = true;
            checkSaveTerrors.Location = new Point(3, 79);
            checkSaveTerrors.Margin = new Padding(3, 1, 3, 1);
            checkSaveTerrors.Name = "checkSaveTerrors";
            checkSaveTerrors.Padding = new Padding(20, 0, 3, 0);
            checkSaveTerrors.Size = new Size(135, 19);
            checkSaveTerrors.TabIndex = 2;
            checkSaveTerrors.Tag = "SaveRoundInfo|Save codes will display the last round type and terror names.";
            checkSaveTerrors.Text = "Save Round Info";
            checkSaveTerrors.UseVisualStyleBackColor = true;
            // 
            // checkPlayerNames
            // 
            checkPlayerNames.AutoSize = true;
            checkPlayerNames.Location = new Point(3, 58);
            checkPlayerNames.Margin = new Padding(3, 1, 3, 1);
            checkPlayerNames.Name = "checkPlayerNames";
            checkPlayerNames.Padding = new Padding(20, 0, 3, 0);
            checkPlayerNames.Size = new Size(161, 19);
            checkPlayerNames.TabIndex = 1;
            checkPlayerNames.Tag = "SaveNames|Save codes will show players in the instance at the time of saving.";
            checkPlayerNames.Text = "Collect Player Names";
            checkPlayerNames.UseVisualStyleBackColor = true;
            // 
            // checkAutoCopy
            // 
            checkAutoCopy.AutoSize = true;
            checkAutoCopy.Location = new Point(3, 37);
            checkAutoCopy.Margin = new Padding(3, 1, 3, 1);
            checkAutoCopy.Name = "checkAutoCopy";
            checkAutoCopy.Padding = new Padding(20, 0, 3, 0);
            checkAutoCopy.Size = new Size(161, 19);
            checkAutoCopy.TabIndex = 0;
            checkAutoCopy.Tag = "AutoCopy|Automatically copy new save codes to clipboard.";
            checkAutoCopy.Text = "Auto Clipboard Copy";
            checkAutoCopy.UseVisualStyleBackColor = true;
            // 
            // checkSkipParsedLogs
            // 
            checkSkipParsedLogs.AutoSize = true;
            checkSkipParsedLogs.Location = new Point(3, 16);
            checkSkipParsedLogs.Margin = new Padding(3, 1, 3, 1);
            checkSkipParsedLogs.Name = "checkSkipParsedLogs";
            checkSkipParsedLogs.Padding = new Padding(20, 0, 3, 0);
            checkSkipParsedLogs.RightToLeft = RightToLeft.No;
            checkSkipParsedLogs.Size = new Size(151, 19);
            checkSkipParsedLogs.TabIndex = 4;
            checkSkipParsedLogs.Tag = "SkipParsedLogs|Skip old parsed log files that were already processed and saved.\\nOnly disable this if you accidentally deleted a save code.";
            checkSkipParsedLogs.Text = "Skip Parsed Logs (!)";
            checkSkipParsedLogs.UseVisualStyleBackColor = true;
            // 
            // checkOSCEnabled
            // 
            checkOSCEnabled.AutoSize = true;
            checkOSCEnabled.Location = new Point(3, 265);
            checkOSCEnabled.Margin = new Padding(3, 1, 3, 1);
            checkOSCEnabled.Name = "checkOSCEnabled";
            checkOSCEnabled.Padding = new Padding(20, 0, 3, 0);
            checkOSCEnabled.Size = new Size(163, 19);
            checkOSCEnabled.TabIndex = 7;
            checkOSCEnabled.Tag = "OSCEnabled|Sends avatar parameters to VRChat using OSC. Right click this entry to open documentation about parameter names and types.";
            checkOSCEnabled.Text = "Send OSC Parameters";
            checkOSCEnabled.UseVisualStyleBackColor = true;
            checkOSCEnabled.MouseUp += checkOSCEnabled_MouseUp;
            // 
            // checkPlayAudio
            // 
            checkPlayAudio.AutoCheck = false;
            checkPlayAudio.AutoSize = true;
            checkPlayAudio.Location = new Point(3, 193);
            checkPlayAudio.Margin = new Padding(3, 1, 3, 1);
            checkPlayAudio.Name = "checkPlayAudio";
            checkPlayAudio.Padding = new Padding(20, 0, 3, 0);
            checkPlayAudio.Size = new Size(180, 19);
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
            checkXSOverlay.Location = new Point(3, 214);
            checkXSOverlay.Margin = new Padding(3, 1, 3, 1);
            checkXSOverlay.Name = "checkXSOverlay";
            checkXSOverlay.Padding = new Padding(20, 0, 3, 0);
            checkXSOverlay.Size = new Size(140, 19);
            checkXSOverlay.TabIndex = 0;
            checkXSOverlay.Tag = "XSOverlay|XSOverlay popup notifications when saving.";
            checkXSOverlay.Text = "XSOverlay Popup";
            checkXSOverlay.UseVisualStyleBackColor = true;
            // 
            // checkShowDate
            // 
            checkShowDate.AutoSize = true;
            checkShowDate.Location = new Point(3, 421);
            checkShowDate.Margin = new Padding(3, 1, 3, 1);
            checkShowDate.Name = "checkShowDate";
            checkShowDate.Padding = new Padding(20, 0, 3, 0);
            checkShowDate.Size = new Size(136, 19);
            checkShowDate.TabIndex = 3;
            checkShowDate.Tag = "ShowDate|Entries on the right panel will display a full date.";
            checkShowDate.Text = "Right Panel Date";
            checkShowDate.UseVisualStyleBackColor = true;
            // 
            // checkInvertMD
            // 
            checkInvertMD.AutoSize = true;
            checkInvertMD.Location = new Point(3, 379);
            checkInvertMD.Margin = new Padding(3, 1, 3, 1);
            checkInvertMD.Name = "checkInvertMD";
            checkInvertMD.Padding = new Padding(20, 0, 3, 0);
            checkInvertMD.Size = new Size(143, 19);
            checkInvertMD.TabIndex = 2;
            checkInvertMD.Tag = "InvertMD";
            checkInvertMD.Text = "Invert Month/Day";
            checkInvertMD.UseVisualStyleBackColor = true;
            // 
            // checkShowSeconds
            // 
            checkShowSeconds.AutoSize = true;
            checkShowSeconds.Location = new Point(3, 400);
            checkShowSeconds.Margin = new Padding(3, 1, 3, 1);
            checkShowSeconds.Name = "checkShowSeconds";
            checkShowSeconds.Padding = new Padding(20, 0, 3, 0);
            checkShowSeconds.Size = new Size(125, 19);
            checkShowSeconds.TabIndex = 1;
            checkShowSeconds.Tag = "ShowSeconds";
            checkShowSeconds.Text = "Show Seconds";
            checkShowSeconds.UseVisualStyleBackColor = true;
            // 
            // check24Hour
            // 
            check24Hour.AutoSize = true;
            check24Hour.Location = new Point(3, 358);
            check24Hour.Margin = new Padding(3, 1, 3, 1);
            check24Hour.Name = "check24Hour";
            check24Hour.Padding = new Padding(20, 0, 3, 0);
            check24Hour.Size = new Size(120, 19);
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
            // checkColorObjectives
            // 
            checkColorObjectives.AutoSize = true;
            checkColorObjectives.Location = new Point(3, 472);
            checkColorObjectives.Margin = new Padding(3, 1, 3, 1);
            checkColorObjectives.Name = "checkColorObjectives";
            checkColorObjectives.Padding = new Padding(20, 0, 3, 0);
            checkColorObjectives.Size = new Size(150, 19);
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
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(btnCheckForUpdates);
            panel1.Controls.Add(btnOpenData);
            panel1.Location = new Point(8, 575);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(0, 4, 0, 0);
            panel1.Size = new Size(338, 28);
            panel1.TabIndex = 8;
            // 
            // checkSendChatbox
            // 
            checkSendChatbox.AutoSize = true;
            checkSendChatbox.Location = new Point(3, 307);
            checkSendChatbox.Margin = new Padding(3, 1, 3, 1);
            checkSendChatbox.Name = "checkSendChatbox";
            checkSendChatbox.Padding = new Padding(20, 0, 3, 0);
            checkSendChatbox.Size = new Size(172, 19);
            checkSendChatbox.TabIndex = 8;
            checkSendChatbox.Tag = "OSCSendChatbox|Sends ToN information to the VRChat chatbox.\\nRight click to customize the message template.";
            checkSendChatbox.Text = "Send Chatbox Message";
            checkSendChatbox.UseVisualStyleBackColor = true;
            checkSendChatbox.MouseDown += checkSendChatbox_MouseDown;
            // 
            // checkOSCSendColor
            // 
            checkOSCSendColor.AutoSize = true;
            checkOSCSendColor.ForeColor = Color.PowderBlue;
            checkOSCSendColor.Location = new Point(3, 286);
            checkOSCSendColor.Margin = new Padding(3, 1, 3, 1);
            checkOSCSendColor.Name = "checkOSCSendColor";
            checkOSCSendColor.Padding = new Padding(50, 0, 3, 0);
            checkOSCSendColor.Size = new Size(203, 19);
            checkOSCSendColor.TabIndex = 9;
            checkOSCSendColor.Tag = "OSCSendColor|Sends the current Terror color as HSV parameters.";
            checkOSCSendColor.Text = "Send Terror Color (HSV)";
            checkOSCSendColor.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.AutoScroll = true;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(flowLayoutPanel1);
            panel2.Location = new Point(8, 37);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(3);
            panel2.Size = new Size(338, 536);
            panel2.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.Controls.Add(labelGroupGeneral);
            flowLayoutPanel1.Controls.Add(checkSkipParsedLogs);
            flowLayoutPanel1.Controls.Add(checkAutoCopy);
            flowLayoutPanel1.Controls.Add(checkPlayerNames);
            flowLayoutPanel1.Controls.Add(checkSaveTerrors);
            flowLayoutPanel1.Controls.Add(checkShowWinLose);
            flowLayoutPanel1.Controls.Add(checkSaveTerrorsNote);
            flowLayoutPanel1.Controls.Add(checkDiscordBackup);
            flowLayoutPanel1.Controls.Add(labelGroupNotifications);
            flowLayoutPanel1.Controls.Add(checkPlayAudio);
            flowLayoutPanel1.Controls.Add(checkXSOverlay);
            flowLayoutPanel1.Controls.Add(labelGroupOSC);
            flowLayoutPanel1.Controls.Add(checkOSCEnabled);
            flowLayoutPanel1.Controls.Add(checkOSCSendColor);
            flowLayoutPanel1.Controls.Add(checkSendChatbox);
            flowLayoutPanel1.Controls.Add(labelGroupFormat);
            flowLayoutPanel1.Controls.Add(check24Hour);
            flowLayoutPanel1.Controls.Add(checkInvertMD);
            flowLayoutPanel1.Controls.Add(checkShowSeconds);
            flowLayoutPanel1.Controls.Add(checkShowDate);
            flowLayoutPanel1.Controls.Add(labelGroupStyle);
            flowLayoutPanel1.Controls.Add(checkColorObjectives);
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(330, 492);
            flowLayoutPanel1.TabIndex = 0;
            flowLayoutPanel1.WrapContents = false;
            // 
            // labelGroupGeneral
            // 
            labelGroupGeneral.AutoSize = true;
            labelGroupGeneral.Location = new Point(3, 0);
            labelGroupGeneral.Name = "labelGroupGeneral";
            labelGroupGeneral.Size = new Size(47, 15);
            labelGroupGeneral.TabIndex = 10;
            labelGroupGeneral.Text = "General";
            // 
            // labelGroupOSC
            // 
            labelGroupOSC.AutoSize = true;
            labelGroupOSC.Location = new Point(3, 249);
            labelGroupOSC.Margin = new Padding(3, 15, 3, 0);
            labelGroupOSC.Name = "labelGroupOSC";
            labelGroupOSC.Size = new Size(30, 15);
            labelGroupOSC.TabIndex = 11;
            labelGroupOSC.Text = "OSC";
            // 
            // labelGroupNotifications
            // 
            labelGroupNotifications.AutoSize = true;
            labelGroupNotifications.Location = new Point(3, 177);
            labelGroupNotifications.Margin = new Padding(3, 15, 3, 0);
            labelGroupNotifications.Name = "labelGroupNotifications";
            labelGroupNotifications.Size = new Size(75, 15);
            labelGroupNotifications.TabIndex = 12;
            labelGroupNotifications.Text = "Notifications";
            // 
            // labelGroupFormat
            // 
            labelGroupFormat.AutoSize = true;
            labelGroupFormat.Location = new Point(3, 342);
            labelGroupFormat.Margin = new Padding(3, 15, 3, 0);
            labelGroupFormat.Name = "labelGroupFormat";
            labelGroupFormat.Size = new Size(74, 15);
            labelGroupFormat.TabIndex = 13;
            labelGroupFormat.Text = "Time Format";
            // 
            // labelGroupStyle
            // 
            labelGroupStyle.AutoSize = true;
            labelGroupStyle.Location = new Point(3, 456);
            labelGroupStyle.Margin = new Padding(3, 15, 3, 0);
            labelGroupStyle.Name = "labelGroupStyle";
            labelGroupStyle.Size = new Size(32, 15);
            labelGroupStyle.TabIndex = 14;
            labelGroupStyle.Text = "Style";
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(354, 614);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(languageSelectBox);
            ForeColor = Color.White;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(200, 200);
            Name = "SettingsWindow";
            Padding = new Padding(8);
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Settings";
            FormClosed += SettingsWindow_FormClosed;
            Load += SettingsWindow_Load;
            ctxData.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private CheckBox checkAutoCopy;
        private CheckBox checkPlayerNames;
        private Button btnCheckForUpdates;
        private CheckBox checkPlayAudio;
        private CheckBox checkXSOverlay;
        private CheckBox checkInvertMD;
        private CheckBox checkShowSeconds;
        private CheckBox check24Hour;
        private Button btnOpenData;
        private ToolTip toolTip;
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
        private CheckBox checkSendChatbox;
        private CheckBox checkOSCSendColor;
        private Panel panel2;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label labelGroupGeneral;
        private Label labelGroupOSC;
        private Label labelGroupNotifications;
        private Label labelGroupFormat;
        private Label labelGroupStyle;
    }
}