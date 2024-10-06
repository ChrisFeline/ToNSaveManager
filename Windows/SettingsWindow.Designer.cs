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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
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
            flowLayoutPanel5 = new FlowLayoutPanel();
            linkLogUpdateRate = new LinkLabel();
            linkOpenLogs = new LinkLabel();
            linkCopyLogs = new LinkLabel();
            checkWebSocketServer = new CheckBox();
            checkWebTrackerComp = new CheckBox();
            checkCopyOnOpen = new CheckBox();
            checkCopyOnJoin = new CheckBox();
            checkCopyOnSave = new CheckBox();
            flowLayoutPanel6 = new FlowLayoutPanel();
            linkAutoNoteEdit = new LinkLabel();
            flowRoundInfoFilePanel = new FlowLayoutPanel();
            checkRoundToFile = new CheckBox();
            flowRoundInfoFiles = new FlowLayoutPanel();
            linkAddInfoFile = new LinkLabel();
            labelGroupDiscord = new Label();
            checkDiscordPresence = new CheckBox();
            flowDiscordStateText = new FlowLayoutPanel();
            checkDiscordCustomState = new CheckBox();
            linkEditDiscordState = new LinkLabel();
            flowDiscordDetailsText = new FlowLayoutPanel();
            checkDiscordCustomDetails = new CheckBox();
            linkEditDiscordDetails = new LinkLabel();
            flowDiscordImageText = new FlowLayoutPanel();
            checkDiscordCustomImageText = new CheckBox();
            linkEditDiscordImage = new LinkLabel();
            flowDiscordIconText = new FlowLayoutPanel();
            checkDiscordCustomIcon = new CheckBox();
            linkEditDiscordIcon = new LinkLabel();
            flowDiscordIconStart = new FlowLayoutPanel();
            checkDiscordCustomStart = new CheckBox();
            linkEditDiscordStart = new LinkLabel();
            labelGroupNotifications = new Label();
            labelGroupOSC = new Label();
            flowLayoutPanel7 = new FlowLayoutPanel();
            linkOSCFormat = new LinkLabel();
            labelOSCFormat = new Label();
            flowLayoutPanel3 = new FlowLayoutPanel();
            checkOSCSendDamage = new CheckBox();
            linkSetDamageInterval = new LinkLabel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            linkEditChatbox = new LinkLabel();
            labelGroupFormat = new Label();
            labelGroupStyle = new Label();
            flowLayoutPanel4 = new FlowLayoutPanel();
            checkOpenRGBEnabled = new CheckBox();
            linkOpenRGB = new LinkLabel();
            checkShowTime = new CheckBox();
            ctxData.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel5.SuspendLayout();
            flowLayoutPanel6.SuspendLayout();
            flowRoundInfoFilePanel.SuspendLayout();
            flowDiscordStateText.SuspendLayout();
            flowDiscordDetailsText.SuspendLayout();
            flowDiscordImageText.SuspendLayout();
            flowDiscordIconText.SuspendLayout();
            flowDiscordIconStart.SuspendLayout();
            flowLayoutPanel7.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            flowLayoutPanel4.SuspendLayout();
            SuspendLayout();
            // 
            // checkDiscordBackup
            // 
            checkDiscordBackup.AutoSize = true;
            checkDiscordBackup.Location = new Point(3, 349);
            checkDiscordBackup.Margin = new Padding(3, 1, 3, 1);
            checkDiscordBackup.Name = "checkDiscordBackup";
            checkDiscordBackup.Padding = new Padding(20, 0, 3, 0);
            checkDiscordBackup.Size = new Size(209, 19);
            checkDiscordBackup.TabIndex = 6;
            checkDiscordBackup.Tag = "DiscordWebhookEnabled|Automatically saves your new codes to a Discord channel using a webhook integration.";
            checkDiscordBackup.Text = "Automatic Backup (Webhook)";
            checkDiscordBackup.UseVisualStyleBackColor = true;
            // 
            // checkShowWinLose
            // 
            checkShowWinLose.AutoSize = true;
            checkShowWinLose.ForeColor = Color.PowderBlue;
            checkShowWinLose.Location = new Point(3, 239);
            checkShowWinLose.Margin = new Padding(3, 1, 3, 1);
            checkShowWinLose.Name = "checkShowWinLose";
            checkShowWinLose.Padding = new Padding(50, 0, 3, 0);
            checkShowWinLose.Size = new Size(183, 19);
            checkShowWinLose.TabIndex = 5;
            checkShowWinLose.Tag = "ShowWinLose|Entries will show a [R], [W] or [D] tag based on the source that triggered the save.";
            checkShowWinLose.Text = "Show Save Symbols";
            checkShowWinLose.UseVisualStyleBackColor = true;
            // 
            // checkSaveTerrorsNote
            // 
            checkSaveTerrorsNote.AutoSize = true;
            checkSaveTerrorsNote.ForeColor = Color.PowderBlue;
            checkSaveTerrorsNote.Location = new Point(3, 0);
            checkSaveTerrorsNote.Margin = new Padding(0);
            checkSaveTerrorsNote.Name = "checkSaveTerrorsNote";
            checkSaveTerrorsNote.Padding = new Padding(50, 0, 3, 0);
            checkSaveTerrorsNote.Size = new Size(164, 19);
            checkSaveTerrorsNote.TabIndex = 3;
            checkSaveTerrorsNote.Tag = "SaveRoundNote|Automatically set survived terror names as note.";
            checkSaveTerrorsNote.Text = "Automatic Note";
            checkSaveTerrorsNote.UseVisualStyleBackColor = true;
            // 
            // checkSaveTerrors
            // 
            checkSaveTerrors.AutoSize = true;
            checkSaveTerrors.Location = new Point(3, 218);
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
            checkPlayerNames.Location = new Point(3, 197);
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
            checkAutoCopy.Location = new Point(3, 113);
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
            checkSkipParsedLogs.Location = new Point(3, 0);
            checkSkipParsedLogs.Margin = new Padding(0);
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
            checkOSCEnabled.Location = new Point(3, 598);
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
            checkPlayAudio.Location = new Point(3, 526);
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
            checkXSOverlay.Location = new Point(3, 547);
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
            checkShowDate.Location = new Point(3, 775);
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
            checkInvertMD.Location = new Point(3, 733);
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
            checkShowSeconds.Location = new Point(3, 754);
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
            check24Hour.Location = new Point(3, 712);
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
            btnCheckForUpdates.Size = new Size(510, 24);
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
            btnOpenData.Location = new Point(516, 4);
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
            checkColorObjectives.Location = new Point(3, 847);
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
            languageSelectBox.Size = new Size(569, 23);
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
            panel1.Size = new Size(569, 28);
            panel1.TabIndex = 8;
            // 
            // checkSendChatbox
            // 
            checkSendChatbox.AutoSize = true;
            checkSendChatbox.Location = new Point(3, 0);
            checkSendChatbox.Margin = new Padding(0);
            checkSendChatbox.Name = "checkSendChatbox";
            checkSendChatbox.Padding = new Padding(20, 0, 3, 0);
            checkSendChatbox.Size = new Size(172, 19);
            checkSendChatbox.TabIndex = 8;
            checkSendChatbox.Tag = "OSCSendChatbox|Sends ToN information to the VRChat chatbox.\\nRight click to customize the message template.";
            checkSendChatbox.Text = "Send Chatbox Message";
            checkSendChatbox.UseVisualStyleBackColor = true;
            // 
            // checkOSCSendColor
            // 
            checkOSCSendColor.AutoSize = true;
            checkOSCSendColor.ForeColor = Color.PowderBlue;
            checkOSCSendColor.Location = new Point(3, 0);
            checkOSCSendColor.Margin = new Padding(0);
            checkOSCSendColor.Name = "checkOSCSendColor";
            checkOSCSendColor.Padding = new Padding(50, 0, 3, 0);
            checkOSCSendColor.Size = new Size(170, 19);
            checkOSCSendColor.TabIndex = 9;
            checkOSCSendColor.Tag = "OSCSendColor|Sends the current Terror color as HSV parameters.";
            checkOSCSendColor.Text = "Send Terror Color";
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
            panel2.Size = new Size(569, 536);
            panel2.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.Controls.Add(labelGroupGeneral);
            flowLayoutPanel1.Controls.Add(flowLayoutPanel5);
            flowLayoutPanel1.Controls.Add(linkOpenLogs);
            flowLayoutPanel1.Controls.Add(linkCopyLogs);
            flowLayoutPanel1.Controls.Add(checkWebSocketServer);
            flowLayoutPanel1.Controls.Add(checkWebTrackerComp);
            flowLayoutPanel1.Controls.Add(checkAutoCopy);
            flowLayoutPanel1.Controls.Add(checkCopyOnOpen);
            flowLayoutPanel1.Controls.Add(checkCopyOnJoin);
            flowLayoutPanel1.Controls.Add(checkCopyOnSave);
            flowLayoutPanel1.Controls.Add(checkPlayerNames);
            flowLayoutPanel1.Controls.Add(checkSaveTerrors);
            flowLayoutPanel1.Controls.Add(checkShowWinLose);
            flowLayoutPanel1.Controls.Add(flowLayoutPanel6);
            flowLayoutPanel1.Controls.Add(flowRoundInfoFilePanel);
            flowLayoutPanel1.Controls.Add(labelGroupDiscord);
            flowLayoutPanel1.Controls.Add(checkDiscordBackup);
            flowLayoutPanel1.Controls.Add(checkDiscordPresence);
            flowLayoutPanel1.Controls.Add(flowDiscordStateText);
            flowLayoutPanel1.Controls.Add(flowDiscordDetailsText);
            flowLayoutPanel1.Controls.Add(flowDiscordImageText);
            flowLayoutPanel1.Controls.Add(flowDiscordIconText);
            flowLayoutPanel1.Controls.Add(flowDiscordIconStart);
            flowLayoutPanel1.Controls.Add(labelGroupNotifications);
            flowLayoutPanel1.Controls.Add(checkPlayAudio);
            flowLayoutPanel1.Controls.Add(checkXSOverlay);
            flowLayoutPanel1.Controls.Add(labelGroupOSC);
            flowLayoutPanel1.Controls.Add(checkOSCEnabled);
            flowLayoutPanel1.Controls.Add(flowLayoutPanel7);
            flowLayoutPanel1.Controls.Add(flowLayoutPanel3);
            flowLayoutPanel1.Controls.Add(flowLayoutPanel2);
            flowLayoutPanel1.Controls.Add(labelGroupFormat);
            flowLayoutPanel1.Controls.Add(check24Hour);
            flowLayoutPanel1.Controls.Add(checkInvertMD);
            flowLayoutPanel1.Controls.Add(checkShowSeconds);
            flowLayoutPanel1.Controls.Add(checkShowDate);
            flowLayoutPanel1.Controls.Add(checkShowTime);
            flowLayoutPanel1.Controls.Add(labelGroupStyle);
            flowLayoutPanel1.Controls.Add(checkColorObjectives);
            flowLayoutPanel1.Controls.Add(flowLayoutPanel4);
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(544, 888);
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
            // flowLayoutPanel5
            // 
            flowLayoutPanel5.AutoSize = true;
            flowLayoutPanel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel5.Controls.Add(checkSkipParsedLogs);
            flowLayoutPanel5.Controls.Add(linkLogUpdateRate);
            flowLayoutPanel5.Location = new Point(0, 16);
            flowLayoutPanel5.Margin = new Padding(0, 1, 0, 1);
            flowLayoutPanel5.Name = "flowLayoutPanel5";
            flowLayoutPanel5.Padding = new Padding(3, 0, 0, 0);
            flowLayoutPanel5.Size = new Size(252, 19);
            flowLayoutPanel5.TabIndex = 17;
            flowLayoutPanel5.WrapContents = false;
            // 
            // linkLogUpdateRate
            // 
            linkLogUpdateRate.ActiveLinkColor = Color.White;
            linkLogUpdateRate.AutoSize = true;
            linkLogUpdateRate.LinkBehavior = LinkBehavior.HoverUnderline;
            linkLogUpdateRate.LinkColor = Color.Gray;
            linkLogUpdateRate.Location = new Point(154, 0);
            linkLogUpdateRate.Margin = new Padding(0);
            linkLogUpdateRate.Name = "linkLogUpdateRate";
            linkLogUpdateRate.Size = new Size(98, 15);
            linkLogUpdateRate.TabIndex = 11;
            linkLogUpdateRate.TabStop = true;
            linkLogUpdateRate.Text = "(Set Update Rate)";
            linkLogUpdateRate.TextAlign = ContentAlignment.MiddleLeft;
            linkLogUpdateRate.VisitedLinkColor = Color.Gray;
            linkLogUpdateRate.LinkClicked += linkLogUpdateRate_LinkClicked;
            // 
            // linkOpenLogs
            // 
            linkOpenLogs.ActiveLinkColor = Color.White;
            linkOpenLogs.AutoSize = true;
            linkOpenLogs.BackColor = Color.LightBlue;
            linkOpenLogs.LinkBehavior = LinkBehavior.HoverUnderline;
            linkOpenLogs.LinkColor = SystemColors.Desktop;
            linkOpenLogs.Location = new Point(53, 37);
            linkOpenLogs.Margin = new Padding(53, 1, 3, 1);
            linkOpenLogs.Name = "linkOpenLogs";
            linkOpenLogs.Padding = new Padding(3, 0, 3, 0);
            linkOpenLogs.Size = new Size(151, 15);
            linkOpenLogs.TabIndex = 27;
            linkOpenLogs.TabStop = true;
            linkOpenLogs.Text = "> FIND LATEST LOG FILE <";
            linkOpenLogs.TextAlign = ContentAlignment.MiddleLeft;
            linkOpenLogs.Visible = false;
            linkOpenLogs.VisitedLinkColor = Color.Gray;
            // 
            // linkCopyLogs
            // 
            linkCopyLogs.ActiveLinkColor = Color.White;
            linkCopyLogs.AutoSize = true;
            linkCopyLogs.BackColor = Color.LightBlue;
            linkCopyLogs.LinkBehavior = LinkBehavior.HoverUnderline;
            linkCopyLogs.LinkColor = SystemColors.Desktop;
            linkCopyLogs.Location = new Point(53, 54);
            linkCopyLogs.Margin = new Padding(53, 1, 3, 1);
            linkCopyLogs.Name = "linkCopyLogs";
            linkCopyLogs.Padding = new Padding(3, 0, 3, 0);
            linkCopyLogs.Size = new Size(155, 15);
            linkCopyLogs.TabIndex = 12;
            linkCopyLogs.TabStop = true;
            linkCopyLogs.Text = "> COPY INSTANCE LOGS <";
            linkCopyLogs.TextAlign = ContentAlignment.MiddleLeft;
            linkCopyLogs.Visible = false;
            linkCopyLogs.VisitedLinkColor = Color.Gray;
            // 
            // checkWebSocketServer
            // 
            checkWebSocketServer.AutoSize = true;
            checkWebSocketServer.Location = new Point(3, 71);
            checkWebSocketServer.Margin = new Padding(3, 1, 3, 1);
            checkWebSocketServer.Name = "checkWebSocketServer";
            checkWebSocketServer.Padding = new Padding(20, 0, 3, 0);
            checkWebSocketServer.Size = new Size(164, 19);
            checkWebSocketServer.TabIndex = 25;
            checkWebSocketServer.Tag = "WebSocketEnabled";
            checkWebSocketServer.Text = "WebSocket API Server";
            checkWebSocketServer.UseVisualStyleBackColor = true;
            // 
            // checkWebTrackerComp
            // 
            checkWebTrackerComp.AutoSize = true;
            checkWebTrackerComp.ForeColor = Color.White;
            checkWebTrackerComp.Location = new Point(3, 92);
            checkWebTrackerComp.Margin = new Padding(3, 1, 3, 1);
            checkWebTrackerComp.Name = "checkWebTrackerComp";
            checkWebTrackerComp.Padding = new Padding(50, 0, 3, 0);
            checkWebTrackerComp.Size = new Size(215, 19);
            checkWebTrackerComp.TabIndex = 26;
            checkWebTrackerComp.Tag = "WebTrackerCompatibility";
            checkWebTrackerComp.Text = "Live Tracker Compatibility";
            checkWebTrackerComp.UseVisualStyleBackColor = true;
            // 
            // checkCopyOnOpen
            // 
            checkCopyOnOpen.AutoSize = true;
            checkCopyOnOpen.ForeColor = Color.PowderBlue;
            checkCopyOnOpen.Location = new Point(3, 134);
            checkCopyOnOpen.Margin = new Padding(3, 1, 3, 1);
            checkCopyOnOpen.Name = "checkCopyOnOpen";
            checkCopyOnOpen.Padding = new Padding(50, 0, 3, 0);
            checkCopyOnOpen.Size = new Size(158, 19);
            checkCopyOnOpen.TabIndex = 23;
            checkCopyOnOpen.Tag = "CopyOnOpen";
            checkCopyOnOpen.Text = "Copy On Open";
            checkCopyOnOpen.UseVisualStyleBackColor = true;
            // 
            // checkCopyOnJoin
            // 
            checkCopyOnJoin.AutoSize = true;
            checkCopyOnJoin.ForeColor = Color.PowderBlue;
            checkCopyOnJoin.Location = new Point(3, 155);
            checkCopyOnJoin.Margin = new Padding(3, 1, 3, 1);
            checkCopyOnJoin.Name = "checkCopyOnJoin";
            checkCopyOnJoin.Padding = new Padding(50, 0, 3, 0);
            checkCopyOnJoin.Size = new Size(150, 19);
            checkCopyOnJoin.TabIndex = 24;
            checkCopyOnJoin.Tag = "CopyOnJoin";
            checkCopyOnJoin.Text = "Copy On Join";
            checkCopyOnJoin.UseVisualStyleBackColor = true;
            // 
            // checkCopyOnSave
            // 
            checkCopyOnSave.AutoSize = true;
            checkCopyOnSave.ForeColor = Color.PowderBlue;
            checkCopyOnSave.Location = new Point(3, 176);
            checkCopyOnSave.Margin = new Padding(3, 1, 3, 1);
            checkCopyOnSave.Name = "checkCopyOnSave";
            checkCopyOnSave.Padding = new Padding(50, 0, 3, 0);
            checkCopyOnSave.Size = new Size(153, 19);
            checkCopyOnSave.TabIndex = 22;
            checkCopyOnSave.Tag = "CopyOnSave";
            checkCopyOnSave.Text = "Copy On Save";
            checkCopyOnSave.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel6
            // 
            flowLayoutPanel6.AutoSize = true;
            flowLayoutPanel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel6.Controls.Add(checkSaveTerrorsNote);
            flowLayoutPanel6.Controls.Add(linkAutoNoteEdit);
            flowLayoutPanel6.Location = new Point(0, 260);
            flowLayoutPanel6.Margin = new Padding(0, 1, 0, 1);
            flowLayoutPanel6.Name = "flowLayoutPanel6";
            flowLayoutPanel6.Padding = new Padding(3, 0, 0, 0);
            flowLayoutPanel6.Size = new Size(202, 19);
            flowLayoutPanel6.TabIndex = 17;
            flowLayoutPanel6.WrapContents = false;
            // 
            // linkAutoNoteEdit
            // 
            linkAutoNoteEdit.ActiveLinkColor = Color.White;
            linkAutoNoteEdit.AutoSize = true;
            linkAutoNoteEdit.LinkBehavior = LinkBehavior.HoverUnderline;
            linkAutoNoteEdit.LinkColor = Color.Gray;
            linkAutoNoteEdit.Location = new Point(167, 0);
            linkAutoNoteEdit.Margin = new Padding(0);
            linkAutoNoteEdit.Name = "linkAutoNoteEdit";
            linkAutoNoteEdit.Size = new Size(35, 15);
            linkAutoNoteEdit.TabIndex = 11;
            linkAutoNoteEdit.TabStop = true;
            linkAutoNoteEdit.Text = "(Edit)";
            linkAutoNoteEdit.TextAlign = ContentAlignment.MiddleLeft;
            linkAutoNoteEdit.VisitedLinkColor = Color.Gray;
            linkAutoNoteEdit.LinkClicked += linkAutoNoteEdit_LinkClicked;
            // 
            // flowRoundInfoFilePanel
            // 
            flowRoundInfoFilePanel.AutoSize = true;
            flowRoundInfoFilePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowRoundInfoFilePanel.BorderStyle = BorderStyle.FixedSingle;
            flowRoundInfoFilePanel.Controls.Add(checkRoundToFile);
            flowRoundInfoFilePanel.Controls.Add(flowRoundInfoFiles);
            flowRoundInfoFilePanel.Controls.Add(linkAddInfoFile);
            flowRoundInfoFilePanel.FlowDirection = FlowDirection.TopDown;
            flowRoundInfoFilePanel.Location = new Point(3, 281);
            flowRoundInfoFilePanel.Margin = new Padding(3, 1, 3, 1);
            flowRoundInfoFilePanel.Name = "flowRoundInfoFilePanel";
            flowRoundInfoFilePanel.Size = new Size(145, 36);
            flowRoundInfoFilePanel.TabIndex = 17;
            flowRoundInfoFilePanel.WrapContents = false;
            // 
            // checkRoundToFile
            // 
            checkRoundToFile.AutoSize = true;
            checkRoundToFile.Location = new Point(0, 0);
            checkRoundToFile.Margin = new Padding(0);
            checkRoundToFile.Name = "checkRoundToFile";
            checkRoundToFile.Padding = new Padding(20, 0, 3, 0);
            checkRoundToFile.Size = new Size(143, 19);
            checkRoundToFile.TabIndex = 15;
            checkRoundToFile.Tag = resources.GetString("checkRoundToFile.Tag");
            checkRoundToFile.Text = "Round Info to File";
            checkRoundToFile.UseVisualStyleBackColor = true;
            // 
            // flowRoundInfoFiles
            // 
            flowRoundInfoFiles.AutoSize = true;
            flowRoundInfoFiles.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowRoundInfoFiles.FlowDirection = FlowDirection.TopDown;
            flowRoundInfoFiles.Location = new Point(50, 19);
            flowRoundInfoFiles.Margin = new Padding(50, 0, 0, 0);
            flowRoundInfoFiles.Name = "flowRoundInfoFiles";
            flowRoundInfoFiles.Size = new Size(0, 0);
            flowRoundInfoFiles.TabIndex = 17;
            flowRoundInfoFiles.WrapContents = false;
            // 
            // linkAddInfoFile
            // 
            linkAddInfoFile.ActiveLinkColor = Color.White;
            linkAddInfoFile.AutoSize = true;
            linkAddInfoFile.LinkBehavior = LinkBehavior.HoverUnderline;
            linkAddInfoFile.LinkColor = Color.Gray;
            linkAddInfoFile.Location = new Point(0, 19);
            linkAddInfoFile.Margin = new Padding(0);
            linkAddInfoFile.Name = "linkAddInfoFile";
            linkAddInfoFile.Padding = new Padding(50, 0, 0, 0);
            linkAddInfoFile.Size = new Size(108, 15);
            linkAddInfoFile.TabIndex = 16;
            linkAddInfoFile.TabStop = true;
            linkAddInfoFile.Text = "(Add File)";
            linkAddInfoFile.TextAlign = ContentAlignment.MiddleLeft;
            linkAddInfoFile.VisitedLinkColor = Color.Gray;
            linkAddInfoFile.LinkClicked += linkAddInfoFile_LinkClicked;
            // 
            // labelGroupDiscord
            // 
            labelGroupDiscord.AutoSize = true;
            labelGroupDiscord.Location = new Point(3, 333);
            labelGroupDiscord.Margin = new Padding(3, 15, 3, 0);
            labelGroupDiscord.Name = "labelGroupDiscord";
            labelGroupDiscord.Size = new Size(94, 15);
            labelGroupDiscord.TabIndex = 19;
            labelGroupDiscord.Text = "Discord Features";
            // 
            // checkDiscordPresence
            // 
            checkDiscordPresence.AutoSize = true;
            checkDiscordPresence.Location = new Point(3, 370);
            checkDiscordPresence.Margin = new Padding(3, 1, 3, 1);
            checkDiscordPresence.Name = "checkDiscordPresence";
            checkDiscordPresence.Padding = new Padding(20, 0, 3, 0);
            checkDiscordPresence.Size = new Size(160, 19);
            checkDiscordPresence.TabIndex = 20;
            checkDiscordPresence.Tag = "DiscordRichPresence";
            checkDiscordPresence.Text = "Enable Rich Presence";
            checkDiscordPresence.UseVisualStyleBackColor = true;
            // 
            // flowDiscordStateText
            // 
            flowDiscordStateText.AutoSize = true;
            flowDiscordStateText.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowDiscordStateText.Controls.Add(checkDiscordCustomState);
            flowDiscordStateText.Controls.Add(linkEditDiscordState);
            flowDiscordStateText.Location = new Point(0, 391);
            flowDiscordStateText.Margin = new Padding(0, 1, 0, 1);
            flowDiscordStateText.Name = "flowDiscordStateText";
            flowDiscordStateText.Padding = new Padding(3, 0, 0, 0);
            flowDiscordStateText.Size = new Size(212, 19);
            flowDiscordStateText.TabIndex = 18;
            flowDiscordStateText.WrapContents = false;
            // 
            // checkDiscordCustomState
            // 
            checkDiscordCustomState.AutoSize = true;
            checkDiscordCustomState.ForeColor = SystemColors.ScrollBar;
            checkDiscordCustomState.Location = new Point(3, 0);
            checkDiscordCustomState.Margin = new Padding(0);
            checkDiscordCustomState.Name = "checkDiscordCustomState";
            checkDiscordCustomState.Padding = new Padding(50, 0, 3, 0);
            checkDiscordCustomState.Size = new Size(174, 19);
            checkDiscordCustomState.TabIndex = 3;
            checkDiscordCustomState.Tag = "DiscordCustomState";
            checkDiscordCustomState.Text = "Custom State Text";
            checkDiscordCustomState.UseVisualStyleBackColor = true;
            // 
            // linkEditDiscordState
            // 
            linkEditDiscordState.ActiveLinkColor = Color.White;
            linkEditDiscordState.AutoSize = true;
            linkEditDiscordState.LinkBehavior = LinkBehavior.HoverUnderline;
            linkEditDiscordState.LinkColor = Color.Gray;
            linkEditDiscordState.Location = new Point(177, 0);
            linkEditDiscordState.Margin = new Padding(0);
            linkEditDiscordState.Name = "linkEditDiscordState";
            linkEditDiscordState.Size = new Size(35, 15);
            linkEditDiscordState.TabIndex = 11;
            linkEditDiscordState.TabStop = true;
            linkEditDiscordState.Text = "(Edit)";
            linkEditDiscordState.TextAlign = ContentAlignment.MiddleLeft;
            linkEditDiscordState.VisitedLinkColor = Color.Gray;
            // 
            // flowDiscordDetailsText
            // 
            flowDiscordDetailsText.AutoSize = true;
            flowDiscordDetailsText.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowDiscordDetailsText.Controls.Add(checkDiscordCustomDetails);
            flowDiscordDetailsText.Controls.Add(linkEditDiscordDetails);
            flowDiscordDetailsText.Location = new Point(0, 412);
            flowDiscordDetailsText.Margin = new Padding(0, 1, 0, 1);
            flowDiscordDetailsText.Name = "flowDiscordDetailsText";
            flowDiscordDetailsText.Padding = new Padding(3, 0, 0, 0);
            flowDiscordDetailsText.Size = new Size(221, 19);
            flowDiscordDetailsText.TabIndex = 19;
            flowDiscordDetailsText.WrapContents = false;
            // 
            // checkDiscordCustomDetails
            // 
            checkDiscordCustomDetails.AutoSize = true;
            checkDiscordCustomDetails.ForeColor = SystemColors.ScrollBar;
            checkDiscordCustomDetails.Location = new Point(3, 0);
            checkDiscordCustomDetails.Margin = new Padding(0);
            checkDiscordCustomDetails.Name = "checkDiscordCustomDetails";
            checkDiscordCustomDetails.Padding = new Padding(50, 0, 3, 0);
            checkDiscordCustomDetails.Size = new Size(183, 19);
            checkDiscordCustomDetails.TabIndex = 3;
            checkDiscordCustomDetails.Tag = "DiscordCustomDetails";
            checkDiscordCustomDetails.Text = "Custom Details Text";
            checkDiscordCustomDetails.UseVisualStyleBackColor = true;
            // 
            // linkEditDiscordDetails
            // 
            linkEditDiscordDetails.ActiveLinkColor = Color.White;
            linkEditDiscordDetails.AutoSize = true;
            linkEditDiscordDetails.LinkBehavior = LinkBehavior.HoverUnderline;
            linkEditDiscordDetails.LinkColor = Color.Gray;
            linkEditDiscordDetails.Location = new Point(186, 0);
            linkEditDiscordDetails.Margin = new Padding(0);
            linkEditDiscordDetails.Name = "linkEditDiscordDetails";
            linkEditDiscordDetails.Size = new Size(35, 15);
            linkEditDiscordDetails.TabIndex = 11;
            linkEditDiscordDetails.TabStop = true;
            linkEditDiscordDetails.Text = "(Edit)";
            linkEditDiscordDetails.TextAlign = ContentAlignment.MiddleLeft;
            linkEditDiscordDetails.VisitedLinkColor = Color.Gray;
            // 
            // flowDiscordImageText
            // 
            flowDiscordImageText.AutoSize = true;
            flowDiscordImageText.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowDiscordImageText.Controls.Add(checkDiscordCustomImageText);
            flowDiscordImageText.Controls.Add(linkEditDiscordImage);
            flowDiscordImageText.Location = new Point(0, 433);
            flowDiscordImageText.Margin = new Padding(0, 1, 0, 1);
            flowDiscordImageText.Name = "flowDiscordImageText";
            flowDiscordImageText.Padding = new Padding(3, 0, 0, 0);
            flowDiscordImageText.Size = new Size(219, 19);
            flowDiscordImageText.TabIndex = 20;
            flowDiscordImageText.WrapContents = false;
            // 
            // checkDiscordCustomImageText
            // 
            checkDiscordCustomImageText.AutoSize = true;
            checkDiscordCustomImageText.ForeColor = SystemColors.ScrollBar;
            checkDiscordCustomImageText.Location = new Point(3, 0);
            checkDiscordCustomImageText.Margin = new Padding(0);
            checkDiscordCustomImageText.Name = "checkDiscordCustomImageText";
            checkDiscordCustomImageText.Padding = new Padding(50, 0, 3, 0);
            checkDiscordCustomImageText.Size = new Size(181, 19);
            checkDiscordCustomImageText.TabIndex = 3;
            checkDiscordCustomImageText.Tag = "DiscordCustomImageText";
            checkDiscordCustomImageText.Text = "Custom Image Text";
            checkDiscordCustomImageText.UseVisualStyleBackColor = true;
            // 
            // linkEditDiscordImage
            // 
            linkEditDiscordImage.ActiveLinkColor = Color.White;
            linkEditDiscordImage.AutoSize = true;
            linkEditDiscordImage.LinkBehavior = LinkBehavior.HoverUnderline;
            linkEditDiscordImage.LinkColor = Color.Gray;
            linkEditDiscordImage.Location = new Point(184, 0);
            linkEditDiscordImage.Margin = new Padding(0);
            linkEditDiscordImage.Name = "linkEditDiscordImage";
            linkEditDiscordImage.Size = new Size(35, 15);
            linkEditDiscordImage.TabIndex = 11;
            linkEditDiscordImage.TabStop = true;
            linkEditDiscordImage.Text = "(Edit)";
            linkEditDiscordImage.TextAlign = ContentAlignment.MiddleLeft;
            linkEditDiscordImage.VisitedLinkColor = Color.Gray;
            // 
            // flowDiscordIconText
            // 
            flowDiscordIconText.AutoSize = true;
            flowDiscordIconText.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowDiscordIconText.Controls.Add(checkDiscordCustomIcon);
            flowDiscordIconText.Controls.Add(linkEditDiscordIcon);
            flowDiscordIconText.Location = new Point(0, 454);
            flowDiscordIconText.Margin = new Padding(0, 1, 0, 1);
            flowDiscordIconText.Name = "flowDiscordIconText";
            flowDiscordIconText.Padding = new Padding(3, 0, 0, 0);
            flowDiscordIconText.Size = new Size(209, 19);
            flowDiscordIconText.TabIndex = 21;
            flowDiscordIconText.WrapContents = false;
            // 
            // checkDiscordCustomIcon
            // 
            checkDiscordCustomIcon.AutoSize = true;
            checkDiscordCustomIcon.ForeColor = SystemColors.ScrollBar;
            checkDiscordCustomIcon.Location = new Point(3, 0);
            checkDiscordCustomIcon.Margin = new Padding(0);
            checkDiscordCustomIcon.Name = "checkDiscordCustomIcon";
            checkDiscordCustomIcon.Padding = new Padding(50, 0, 3, 0);
            checkDiscordCustomIcon.Size = new Size(171, 19);
            checkDiscordCustomIcon.TabIndex = 3;
            checkDiscordCustomIcon.Tag = "DiscordCustomIconText";
            checkDiscordCustomIcon.Text = "Custom Icon Text";
            checkDiscordCustomIcon.UseVisualStyleBackColor = true;
            // 
            // linkEditDiscordIcon
            // 
            linkEditDiscordIcon.ActiveLinkColor = Color.White;
            linkEditDiscordIcon.AutoSize = true;
            linkEditDiscordIcon.LinkBehavior = LinkBehavior.HoverUnderline;
            linkEditDiscordIcon.LinkColor = Color.Gray;
            linkEditDiscordIcon.Location = new Point(174, 0);
            linkEditDiscordIcon.Margin = new Padding(0);
            linkEditDiscordIcon.Name = "linkEditDiscordIcon";
            linkEditDiscordIcon.Size = new Size(35, 15);
            linkEditDiscordIcon.TabIndex = 11;
            linkEditDiscordIcon.TabStop = true;
            linkEditDiscordIcon.Text = "(Edit)";
            linkEditDiscordIcon.TextAlign = ContentAlignment.MiddleLeft;
            linkEditDiscordIcon.VisitedLinkColor = Color.Gray;
            // 
            // flowDiscordIconStart
            // 
            flowDiscordIconStart.AutoSize = true;
            flowDiscordIconStart.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowDiscordIconStart.Controls.Add(checkDiscordCustomStart);
            flowDiscordIconStart.Controls.Add(linkEditDiscordStart);
            flowDiscordIconStart.Location = new Point(0, 475);
            flowDiscordIconStart.Margin = new Padding(0, 1, 0, 1);
            flowDiscordIconStart.Name = "flowDiscordIconStart";
            flowDiscordIconStart.Padding = new Padding(3, 0, 0, 0);
            flowDiscordIconStart.Size = new Size(210, 19);
            flowDiscordIconStart.TabIndex = 22;
            flowDiscordIconStart.WrapContents = false;
            // 
            // checkDiscordCustomStart
            // 
            checkDiscordCustomStart.AutoSize = true;
            checkDiscordCustomStart.ForeColor = SystemColors.ScrollBar;
            checkDiscordCustomStart.Location = new Point(3, 0);
            checkDiscordCustomStart.Margin = new Padding(0);
            checkDiscordCustomStart.Name = "checkDiscordCustomStart";
            checkDiscordCustomStart.Padding = new Padding(50, 0, 3, 0);
            checkDiscordCustomStart.Size = new Size(172, 19);
            checkDiscordCustomStart.TabIndex = 3;
            checkDiscordCustomStart.Tag = "DiscordCustomStartText";
            checkDiscordCustomStart.Text = "Custom Start Text";
            checkDiscordCustomStart.UseVisualStyleBackColor = true;
            // 
            // linkEditDiscordStart
            // 
            linkEditDiscordStart.ActiveLinkColor = Color.White;
            linkEditDiscordStart.AutoSize = true;
            linkEditDiscordStart.LinkBehavior = LinkBehavior.HoverUnderline;
            linkEditDiscordStart.LinkColor = Color.Gray;
            linkEditDiscordStart.Location = new Point(175, 0);
            linkEditDiscordStart.Margin = new Padding(0);
            linkEditDiscordStart.Name = "linkEditDiscordStart";
            linkEditDiscordStart.Size = new Size(35, 15);
            linkEditDiscordStart.TabIndex = 11;
            linkEditDiscordStart.TabStop = true;
            linkEditDiscordStart.Text = "(Edit)";
            linkEditDiscordStart.TextAlign = ContentAlignment.MiddleLeft;
            linkEditDiscordStart.VisitedLinkColor = Color.Gray;
            // 
            // labelGroupNotifications
            // 
            labelGroupNotifications.AutoSize = true;
            labelGroupNotifications.Location = new Point(3, 510);
            labelGroupNotifications.Margin = new Padding(3, 15, 3, 0);
            labelGroupNotifications.Name = "labelGroupNotifications";
            labelGroupNotifications.Size = new Size(75, 15);
            labelGroupNotifications.TabIndex = 12;
            labelGroupNotifications.Text = "Notifications";
            // 
            // labelGroupOSC
            // 
            labelGroupOSC.AutoSize = true;
            labelGroupOSC.Location = new Point(3, 582);
            labelGroupOSC.Margin = new Padding(3, 15, 3, 0);
            labelGroupOSC.Name = "labelGroupOSC";
            labelGroupOSC.Size = new Size(30, 15);
            labelGroupOSC.TabIndex = 11;
            labelGroupOSC.Text = "OSC";
            // 
            // flowLayoutPanel7
            // 
            flowLayoutPanel7.AutoSize = true;
            flowLayoutPanel7.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel7.Controls.Add(checkOSCSendColor);
            flowLayoutPanel7.Controls.Add(linkOSCFormat);
            flowLayoutPanel7.Controls.Add(labelOSCFormat);
            flowLayoutPanel7.Location = new Point(0, 619);
            flowLayoutPanel7.Margin = new Padding(0, 1, 0, 1);
            flowLayoutPanel7.Name = "flowLayoutPanel7";
            flowLayoutPanel7.Padding = new Padding(3, 0, 0, 0);
            flowLayoutPanel7.Size = new Size(307, 19);
            flowLayoutPanel7.TabIndex = 18;
            flowLayoutPanel7.WrapContents = false;
            // 
            // linkOSCFormat
            // 
            linkOSCFormat.ActiveLinkColor = Color.Black;
            linkOSCFormat.AutoSize = true;
            linkOSCFormat.BackColor = Color.LightGray;
            linkOSCFormat.ForeColor = Color.Black;
            linkOSCFormat.LinkBehavior = LinkBehavior.HoverUnderline;
            linkOSCFormat.LinkColor = Color.Black;
            linkOSCFormat.Location = new Point(173, 0);
            linkOSCFormat.Margin = new Padding(0);
            linkOSCFormat.Name = "linkOSCFormat";
            linkOSCFormat.Size = new Size(97, 15);
            linkOSCFormat.TabIndex = 11;
            linkOSCFormat.TabStop = true;
            linkOSCFormat.Text = "(Change Format)";
            linkOSCFormat.TextAlign = ContentAlignment.MiddleCenter;
            linkOSCFormat.VisitedLinkColor = Color.Gray;
            // 
            // labelOSCFormat
            // 
            labelOSCFormat.AutoSize = true;
            labelOSCFormat.BackColor = Color.LightBlue;
            labelOSCFormat.ForeColor = Color.Black;
            labelOSCFormat.Location = new Point(270, 0);
            labelOSCFormat.Margin = new Padding(0);
            labelOSCFormat.Name = "labelOSCFormat";
            labelOSCFormat.Padding = new Padding(4, 0, 4, 0);
            labelOSCFormat.Size = new Size(37, 15);
            labelOSCFormat.TabIndex = 12;
            labelOSCFormat.Text = "HSV";
            labelOSCFormat.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.AutoSize = true;
            flowLayoutPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel3.Controls.Add(checkOSCSendDamage);
            flowLayoutPanel3.Controls.Add(linkSetDamageInterval);
            flowLayoutPanel3.Location = new Point(3, 640);
            flowLayoutPanel3.Margin = new Padding(3, 1, 3, 1);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(286, 19);
            flowLayoutPanel3.TabIndex = 18;
            flowLayoutPanel3.WrapContents = false;
            // 
            // checkOSCSendDamage
            // 
            checkOSCSendDamage.AutoSize = true;
            checkOSCSendDamage.ForeColor = Color.PowderBlue;
            checkOSCSendDamage.Location = new Point(0, 0);
            checkOSCSendDamage.Margin = new Padding(0);
            checkOSCSendDamage.Name = "checkOSCSendDamage";
            checkOSCSendDamage.Padding = new Padding(50, 0, 3, 0);
            checkOSCSendDamage.Size = new Size(213, 19);
            checkOSCSendDamage.TabIndex = 12;
            checkOSCSendDamage.Tag = "OSCDamagedEvent";
            checkOSCSendDamage.Text = "Send Damage Event (INT)";
            checkOSCSendDamage.UseVisualStyleBackColor = true;
            // 
            // linkSetDamageInterval
            // 
            linkSetDamageInterval.ActiveLinkColor = Color.White;
            linkSetDamageInterval.AutoSize = true;
            linkSetDamageInterval.LinkBehavior = LinkBehavior.HoverUnderline;
            linkSetDamageInterval.LinkColor = Color.Gray;
            linkSetDamageInterval.Location = new Point(213, 0);
            linkSetDamageInterval.Margin = new Padding(0);
            linkSetDamageInterval.Name = "linkSetDamageInterval";
            linkSetDamageInterval.Size = new Size(73, 15);
            linkSetDamageInterval.TabIndex = 11;
            linkSetDamageInterval.TabStop = true;
            linkSetDamageInterval.Text = "(Set Interval)";
            linkSetDamageInterval.TextAlign = ContentAlignment.MiddleLeft;
            linkSetDamageInterval.VisitedLinkColor = Color.Gray;
            linkSetDamageInterval.LinkClicked += linkSetDamageInterval_LinkClicked;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel2.Controls.Add(checkSendChatbox);
            flowLayoutPanel2.Controls.Add(linkEditChatbox);
            flowLayoutPanel2.Location = new Point(0, 661);
            flowLayoutPanel2.Margin = new Padding(0, 1, 0, 1);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Padding = new Padding(3, 0, 0, 0);
            flowLayoutPanel2.Size = new Size(210, 19);
            flowLayoutPanel2.TabIndex = 16;
            flowLayoutPanel2.WrapContents = false;
            // 
            // linkEditChatbox
            // 
            linkEditChatbox.ActiveLinkColor = Color.White;
            linkEditChatbox.AutoSize = true;
            linkEditChatbox.LinkBehavior = LinkBehavior.HoverUnderline;
            linkEditChatbox.LinkColor = Color.Gray;
            linkEditChatbox.Location = new Point(175, 0);
            linkEditChatbox.Margin = new Padding(0);
            linkEditChatbox.Name = "linkEditChatbox";
            linkEditChatbox.Size = new Size(35, 15);
            linkEditChatbox.TabIndex = 11;
            linkEditChatbox.TabStop = true;
            linkEditChatbox.Text = "(Edit)";
            linkEditChatbox.TextAlign = ContentAlignment.MiddleLeft;
            linkEditChatbox.VisitedLinkColor = Color.Gray;
            linkEditChatbox.LinkClicked += linkEditChatbox_Click;
            // 
            // labelGroupFormat
            // 
            labelGroupFormat.AutoSize = true;
            labelGroupFormat.Location = new Point(3, 696);
            labelGroupFormat.Margin = new Padding(3, 15, 3, 0);
            labelGroupFormat.Name = "labelGroupFormat";
            labelGroupFormat.Size = new Size(74, 15);
            labelGroupFormat.TabIndex = 13;
            labelGroupFormat.Text = "Time Format";
            // 
            // labelGroupStyle
            // 
            labelGroupStyle.AutoSize = true;
            labelGroupStyle.Location = new Point(3, 831);
            labelGroupStyle.Margin = new Padding(3, 15, 3, 0);
            labelGroupStyle.Name = "labelGroupStyle";
            labelGroupStyle.Size = new Size(32, 15);
            labelGroupStyle.TabIndex = 14;
            labelGroupStyle.Text = "Style";
            // 
            // flowLayoutPanel4
            // 
            flowLayoutPanel4.AutoSize = true;
            flowLayoutPanel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel4.Controls.Add(checkOpenRGBEnabled);
            flowLayoutPanel4.Controls.Add(linkOpenRGB);
            flowLayoutPanel4.Location = new Point(0, 868);
            flowLayoutPanel4.Margin = new Padding(0, 1, 0, 1);
            flowLayoutPanel4.Name = "flowLayoutPanel4";
            flowLayoutPanel4.Padding = new Padding(3, 0, 0, 0);
            flowLayoutPanel4.Size = new Size(223, 19);
            flowLayoutPanel4.TabIndex = 21;
            flowLayoutPanel4.WrapContents = false;
            // 
            // checkOpenRGBEnabled
            // 
            checkOpenRGBEnabled.AutoSize = true;
            checkOpenRGBEnabled.Location = new Point(3, 0);
            checkOpenRGBEnabled.Margin = new Padding(0);
            checkOpenRGBEnabled.Name = "checkOpenRGBEnabled";
            checkOpenRGBEnabled.Padding = new Padding(20, 0, 3, 0);
            checkOpenRGBEnabled.Size = new Size(145, 19);
            checkOpenRGBEnabled.TabIndex = 8;
            checkOpenRGBEnabled.Tag = "OpenRGBEnabled";
            checkOpenRGBEnabled.Text = "OpenRGB Support";
            checkOpenRGBEnabled.UseVisualStyleBackColor = true;
            // 
            // linkOpenRGB
            // 
            linkOpenRGB.ActiveLinkColor = Color.White;
            linkOpenRGB.AutoSize = true;
            linkOpenRGB.LinkBehavior = LinkBehavior.HoverUnderline;
            linkOpenRGB.LinkColor = Color.Gray;
            linkOpenRGB.Location = new Point(148, 0);
            linkOpenRGB.Margin = new Padding(0);
            linkOpenRGB.Name = "linkOpenRGB";
            linkOpenRGB.Size = new Size(75, 15);
            linkOpenRGB.TabIndex = 11;
            linkOpenRGB.TabStop = true;
            linkOpenRGB.Text = "(Open JSON)";
            linkOpenRGB.TextAlign = ContentAlignment.MiddleLeft;
            linkOpenRGB.VisitedLinkColor = Color.Gray;
            linkOpenRGB.LinkClicked += linkOpenRGB_LinkClicked;
            // 
            // checkShowTime
            // 
            checkShowTime.AutoSize = true;
            checkShowTime.Location = new Point(3, 796);
            checkShowTime.Margin = new Padding(3, 1, 3, 1);
            checkShowTime.Name = "checkShowTime";
            checkShowTime.Padding = new Padding(20, 0, 3, 0);
            checkShowTime.Size = new Size(138, 19);
            checkShowTime.TabIndex = 28;
            checkShowTime.Tag = "ShowTime";
            checkShowTime.Text = "Right Panel Time";
            checkShowTime.UseVisualStyleBackColor = true;
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(585, 614);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(languageSelectBox);
            ForeColor = Color.White;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(200, 200);
            Name = "SettingsWindow";
            Padding = new Padding(8);
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
            flowLayoutPanel5.ResumeLayout(false);
            flowLayoutPanel5.PerformLayout();
            flowLayoutPanel6.ResumeLayout(false);
            flowLayoutPanel6.PerformLayout();
            flowRoundInfoFilePanel.ResumeLayout(false);
            flowRoundInfoFilePanel.PerformLayout();
            flowDiscordStateText.ResumeLayout(false);
            flowDiscordStateText.PerformLayout();
            flowDiscordDetailsText.ResumeLayout(false);
            flowDiscordDetailsText.PerformLayout();
            flowDiscordImageText.ResumeLayout(false);
            flowDiscordImageText.PerformLayout();
            flowDiscordIconText.ResumeLayout(false);
            flowDiscordIconText.PerformLayout();
            flowDiscordIconStart.ResumeLayout(false);
            flowDiscordIconStart.PerformLayout();
            flowLayoutPanel7.ResumeLayout(false);
            flowLayoutPanel7.PerformLayout();
            flowLayoutPanel3.ResumeLayout(false);
            flowLayoutPanel3.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            flowLayoutPanel4.ResumeLayout(false);
            flowLayoutPanel4.PerformLayout();
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
        private CheckBox checkRoundToFile;
        private FlowLayoutPanel flowLayoutPanel2;
        private LinkLabel linkEditChatbox;
        private FlowLayoutPanel flowRoundInfoFilePanel;
        private LinkLabel linkAddInfoFile;
        private FlowLayoutPanel flowRoundInfoFiles;
        private FlowLayoutPanel flowLayoutPanel3;
        private CheckBox checkOSCSendDamage;
        private LinkLabel linkSetDamageInterval;
        private Label labelGroupDiscord;
        private CheckBox checkDiscordPresence;
        private FlowLayoutPanel flowLayoutPanel4;
        private CheckBox checkOpenRGBEnabled;
        private LinkLabel linkOpenRGB;
        private FlowLayoutPanel flowLayoutPanel5;
        private LinkLabel linkLogUpdateRate;
        private CheckBox checkCopyOnSave;
        private CheckBox checkCopyOnOpen;
        private CheckBox checkCopyOnJoin;
        private FlowLayoutPanel flowLayoutPanel6;
        private LinkLabel linkAutoNoteEdit;
        private CheckBox checkWebSocketServer;
        private CheckBox checkWebTrackerComp;
        private FlowLayoutPanel flowDiscordStateText;
        private CheckBox checkDiscordCustomState;
        private LinkLabel linkEditDiscordState;
        private FlowLayoutPanel flowDiscordDetailsText;
        private CheckBox checkDiscordCustomDetails;
        private LinkLabel linkEditDiscordDetails;
        private FlowLayoutPanel flowDiscordImageText;
        private CheckBox checkDiscordCustomImageText;
        private LinkLabel linkEditDiscordImage;
        private FlowLayoutPanel flowDiscordIconText;
        private CheckBox checkDiscordCustomIcon;
        private LinkLabel linkEditDiscordIcon;
        private FlowLayoutPanel flowDiscordIconStart;
        private CheckBox checkDiscordCustomStart;
        private LinkLabel linkEditDiscordStart;
        private LinkLabel linkCopyLogs;
        private LinkLabel linkOpenLogs;
        private FlowLayoutPanel flowLayoutPanel7;
        private LinkLabel linkOSCFormat;
        private Label labelOSCFormat;
        private CheckBox checkShowTime;
    }
}