namespace ToNSaveManager
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            listBoxKeys = new ListBox();
            listBoxEntries = new ListBox();
            linkLabel1 = new LinkLabel();
            ctxMenuEntries = new ContextMenuStrip(components);
            ctxMenuEntriesCopyTo = new ToolStripMenuItem();
            ctxMenuEntriesNew = new ToolStripMenuItem();
            ctxMenuEntriesNote = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            ctxMenuEntriesDelete = new ToolStripMenuItem();
            ctxMenuKeys = new ContextMenuStrip(components);
            importToolStripMenuItem = new ToolStripMenuItem();
            renameToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            optionsLabel = new LinkLabel();
            ctxMenuSettings = new ContextMenuStrip(components);
            ctxMenuSettingsAutoCopy = new ToolStripMenuItem();
            ctxMenuSettingsNotifSounds = new ToolStripMenuItem();
            ctxMenuSettingsSelectSound = new ToolStripMenuItem();
            ctxMenuSettingsClearSound = new ToolStripMenuItem();
            ctxMenuSettingsCollectNames = new ToolStripMenuItem();
            ctxMenuSettingsXSOverlay = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripSeparator();
            ctxMenuSettings24Hour = new ToolStripMenuItem();
            ctxMenuSettingsInvertMD = new ToolStripMenuItem();
            ctxMenuSettingsShowSeconds = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripSeparator();
            ctxMenuSettingsUpdate = new ToolStripMenuItem();
            ctxMenuSettingsClose = new ToolStripMenuItem();
            objectivesLabel = new LinkLabel();
            ctxMenuEntries.SuspendLayout();
            ctxMenuKeys.SuspendLayout();
            ctxMenuSettings.SuspendLayout();
            SuspendLayout();
            // 
            // listBoxKeys
            // 
            listBoxKeys.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBoxKeys.BackColor = Color.FromArgb(59, 66, 82);
            listBoxKeys.BorderStyle = BorderStyle.FixedSingle;
            listBoxKeys.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxKeys.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxKeys.FormattingEnabled = true;
            listBoxKeys.IntegralHeight = false;
            listBoxKeys.ItemHeight = 15;
            listBoxKeys.Location = new Point(12, 12);
            listBoxKeys.Name = "listBoxKeys";
            listBoxKeys.Size = new Size(192, 230);
            listBoxKeys.TabIndex = 0;
            listBoxKeys.TabStop = false;
            listBoxKeys.DrawItem += listBoxEntries_DrawItem;
            listBoxKeys.MouseDown += listBoxKeys_MouseDown;
            listBoxKeys.MouseUp += listBoxKeys_MouseUp;
            // 
            // listBoxEntries
            // 
            listBoxEntries.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxEntries.BackColor = Color.FromArgb(59, 66, 82);
            listBoxEntries.BorderStyle = BorderStyle.FixedSingle;
            listBoxEntries.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxEntries.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxEntries.FormattingEnabled = true;
            listBoxEntries.IntegralHeight = false;
            listBoxEntries.ItemHeight = 15;
            listBoxEntries.Location = new Point(210, 12);
            listBoxEntries.Name = "listBoxEntries";
            listBoxEntries.Size = new Size(307, 230);
            listBoxEntries.TabIndex = 1;
            listBoxEntries.TabStop = false;
            listBoxEntries.DrawItem += listBoxEntries_DrawItem;
            listBoxEntries.MouseDown += listBoxEntries_MouseDown;
            listBoxEntries.MouseLeave += listBoxEntries_MouseLeave;
            listBoxEntries.MouseMove += listBoxEntries_MouseMove;
            listBoxEntries.MouseUp += listBoxEntries_MouseUp;
            listBoxEntries.Resize += listBoxEntries_Resize;
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = Color.PowderBlue;
            linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLabel1.AutoSize = true;
            linkLabel1.BackColor = Color.Transparent;
            linkLabel1.LinkColor = Color.DodgerBlue;
            linkLabel1.Location = new Point(433, 247);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(84, 15);
            linkLabel1.TabIndex = 4;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Source Code";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // ctxMenuEntries
            // 
            ctxMenuEntries.Items.AddRange(new ToolStripItem[] { ctxMenuEntriesCopyTo, ctxMenuEntriesNote, toolStripMenuItem2, ctxMenuEntriesDelete });
            ctxMenuEntries.Name = "ctxMenuEntries";
            ctxMenuEntries.Size = new Size(138, 76);
            ctxMenuEntries.Closed += ctxMenuEntries_Closed;
            ctxMenuEntries.Opened += ctxMenuEntries_Opened;
            // 
            // ctxMenuEntriesCopyTo
            // 
            ctxMenuEntriesCopyTo.DropDownItems.AddRange(new ToolStripItem[] { ctxMenuEntriesNew });
            ctxMenuEntriesCopyTo.Name = "ctxMenuEntriesCopyTo";
            ctxMenuEntriesCopyTo.Size = new Size(137, 22);
            ctxMenuEntriesCopyTo.Text = "Add to";
            // 
            // ctxMenuEntriesNew
            // 
            ctxMenuEntriesNew.Name = "ctxMenuEntriesNew";
            ctxMenuEntriesNew.Size = new Size(172, 22);
            ctxMenuEntriesNew.Text = "New Collection";
            ctxMenuEntriesNew.ToolTipText = "Add this entry to a new collection.";
            ctxMenuEntriesNew.Click += ctxMenuEntriesNew_Click;
            // 
            // ctxMenuEntriesNote
            // 
            ctxMenuEntriesNote.Name = "ctxMenuEntriesNote";
            ctxMenuEntriesNote.Size = new Size(137, 22);
            ctxMenuEntriesNote.Text = "Edit Note";
            ctxMenuEntriesNote.Click += ctxMenuEntriesNote_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(134, 6);
            // 
            // ctxMenuEntriesDelete
            // 
            ctxMenuEntriesDelete.Name = "ctxMenuEntriesDelete";
            ctxMenuEntriesDelete.Size = new Size(137, 22);
            ctxMenuEntriesDelete.Text = "Delete";
            ctxMenuEntriesDelete.Click += ctxMenuEntriesDelete_Click;
            // 
            // ctxMenuKeys
            // 
            ctxMenuKeys.Items.AddRange(new ToolStripItem[] { importToolStripMenuItem, renameToolStripMenuItem, toolStripMenuItem1, deleteToolStripMenuItem });
            ctxMenuKeys.Name = "ctxMenuKeys";
            ctxMenuKeys.Size = new Size(117, 76);
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Size = new Size(116, 22);
            importToolStripMenuItem.Text = "Import";
            importToolStripMenuItem.Click += ctxMenuKeysImport_Click;
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new Size(116, 22);
            renameToolStripMenuItem.Text = "Rename";
            renameToolStripMenuItem.Click += ctxMenuKeysRename_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(113, 6);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(116, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += ctxMenuKeysDelete_Click;
            // 
            // optionsLabel
            // 
            optionsLabel.ActiveLinkColor = Color.Red;
            optionsLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            optionsLabel.AutoSize = true;
            optionsLabel.LinkColor = Color.Gainsboro;
            optionsLabel.Location = new Point(12, 247);
            optionsLabel.Name = "optionsLabel";
            optionsLabel.Size = new Size(63, 15);
            optionsLabel.TabIndex = 5;
            optionsLabel.TabStop = true;
            optionsLabel.Text = "Settings";
            optionsLabel.LinkClicked += optionsLabel_LinkClicked;
            // 
            // ctxMenuSettings
            // 
            ctxMenuSettings.Items.AddRange(new ToolStripItem[] { ctxMenuSettingsAutoCopy, ctxMenuSettingsNotifSounds, ctxMenuSettingsCollectNames, ctxMenuSettingsXSOverlay, toolStripMenuItem3, ctxMenuSettings24Hour, ctxMenuSettingsShowSeconds, ctxMenuSettingsInvertMD, toolStripMenuItem4, ctxMenuSettingsUpdate, ctxMenuSettingsClose });
            ctxMenuSettings.Name = "ctxMenuSettings";
            ctxMenuSettings.Size = new Size(215, 214);
            ctxMenuSettings.Closing += ctxMenuSettings_Closing;
            // 
            // ctxMenuSettingsAutoCopy
            // 
            ctxMenuSettingsAutoCopy.Checked = true;
            ctxMenuSettingsAutoCopy.CheckState = CheckState.Checked;
            ctxMenuSettingsAutoCopy.Name = "ctxMenuSettingsAutoCopy";
            ctxMenuSettingsAutoCopy.Size = new Size(214, 22);
            ctxMenuSettingsAutoCopy.Text = "Auto Clipboard Copy";
            ctxMenuSettingsAutoCopy.ToolTipText = "Automatically copy to clipboard new saves found while you play Terrors.";
            ctxMenuSettingsAutoCopy.Click += ctxMenuSettingsAutoCopy_Click;
            // 
            // ctxMenuSettingsNotifSounds
            // 
            ctxMenuSettingsNotifSounds.Checked = true;
            ctxMenuSettingsNotifSounds.CheckState = CheckState.Checked;
            ctxMenuSettingsNotifSounds.DropDownItems.AddRange(new ToolStripItem[] { ctxMenuSettingsSelectSound, ctxMenuSettingsClearSound });
            ctxMenuSettingsNotifSounds.Name = "ctxMenuSettingsNotifSounds";
            ctxMenuSettingsNotifSounds.Size = new Size(214, 22);
            ctxMenuSettingsNotifSounds.Text = "Notification Sounds";
            ctxMenuSettingsNotifSounds.ToolTipText = "Plays an audio when a new save is found. You can select a custom sound too.";
            ctxMenuSettingsNotifSounds.Click += ctxMenuSettingsNotifSounds_Click;
            // 
            // ctxMenuSettingsSelectSound
            // 
            ctxMenuSettingsSelectSound.Name = "ctxMenuSettingsSelectSound";
            ctxMenuSettingsSelectSound.Size = new Size(207, 22);
            ctxMenuSettingsSelectSound.Text = "Select Custom Sound";
            ctxMenuSettingsSelectSound.Click += ctxMenuSettingsSelectSound_Click;
            // 
            // ctxMenuSettingsClearSound
            // 
            ctxMenuSettingsClearSound.Name = "ctxMenuSettingsClearSound";
            ctxMenuSettingsClearSound.Size = new Size(207, 22);
            ctxMenuSettingsClearSound.Text = "Clear Custom Sound";
            ctxMenuSettingsClearSound.Click += ctxMenuSettingsClearSound_Click;
            // 
            // ctxMenuSettingsCollectNames
            // 
            ctxMenuSettingsCollectNames.Checked = true;
            ctxMenuSettingsCollectNames.CheckState = CheckState.Checked;
            ctxMenuSettingsCollectNames.Name = "ctxMenuSettingsCollectNames";
            ctxMenuSettingsCollectNames.Size = new Size(214, 22);
            ctxMenuSettingsCollectNames.Text = "Collect Player Names";
            ctxMenuSettingsCollectNames.ToolTipText = "Stores the names of players that were with you at the time of saving.";
            ctxMenuSettingsCollectNames.Click += ctxMenuSettingsCollectNames_Click;
            // 
            // ctxMenuSettingsXSOverlay
            // 
            ctxMenuSettingsXSOverlay.Checked = true;
            ctxMenuSettingsXSOverlay.CheckState = CheckState.Checked;
            ctxMenuSettingsXSOverlay.Name = "ctxMenuSettingsXSOverlay";
            ctxMenuSettingsXSOverlay.Size = new Size(214, 22);
            ctxMenuSettingsXSOverlay.Text = "XSOverlay Popup";
            ctxMenuSettingsXSOverlay.Click += ctxMenuSettingsXSOverlay_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(211, 6);
            // 
            // ctxMenuSettings24Hour
            // 
            ctxMenuSettings24Hour.Checked = true;
            ctxMenuSettings24Hour.CheckOnClick = true;
            ctxMenuSettings24Hour.CheckState = CheckState.Checked;
            ctxMenuSettings24Hour.Name = "ctxMenuSettings24Hour";
            ctxMenuSettings24Hour.Size = new Size(214, 22);
            ctxMenuSettings24Hour.Text = "24 Hour Time";
            ctxMenuSettings24Hour.CheckedChanged += ctxMenuSettings24Hour_CheckedChanged;
            // 
            // ctxMenuSettingsInvertMD
            // 
            ctxMenuSettingsInvertMD.CheckOnClick = true;
            ctxMenuSettingsInvertMD.Name = "ctxMenuSettingsInvertMD";
            ctxMenuSettingsInvertMD.Size = new Size(214, 22);
            ctxMenuSettingsInvertMD.Text = "Invert Month/Day";
            ctxMenuSettingsInvertMD.CheckedChanged += ctxMenuSettingsInvertMD_CheckedChanged;
            // 
            // ctxMenuSettingsShowSeconds
            // 
            ctxMenuSettingsShowSeconds.CheckOnClick = true;
            ctxMenuSettingsShowSeconds.Name = "ctxMenuSettingsShowSeconds";
            ctxMenuSettingsShowSeconds.Size = new Size(214, 22);
            ctxMenuSettingsShowSeconds.Text = "Show Seconds";
            ctxMenuSettingsShowSeconds.CheckedChanged += ctxMenuSettingsShowSeconds_CheckedChanged;
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new Size(211, 6);
            // 
            // ctxMenuSettingsUpdate
            // 
            ctxMenuSettingsUpdate.Name = "ctxMenuSettingsUpdate";
            ctxMenuSettingsUpdate.Size = new Size(214, 22);
            ctxMenuSettingsUpdate.Text = "Check For Updates";
            ctxMenuSettingsUpdate.Click += ctxMenuSettingsUpdate_Click;
            // 
            // ctxMenuSettingsClose
            // 
            ctxMenuSettingsClose.Name = "ctxMenuSettingsClose";
            ctxMenuSettingsClose.Size = new Size(214, 22);
            ctxMenuSettingsClose.Text = "Close";
            ctxMenuSettingsClose.Click += ctxMenuSettingsClose_Click;
            // 
            // objectivesLabel
            // 
            objectivesLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            objectivesLabel.AutoSize = true;
            objectivesLabel.LinkColor = Color.Gainsboro;
            objectivesLabel.Location = new Point(81, 247);
            objectivesLabel.Name = "objectivesLabel";
            objectivesLabel.Size = new Size(77, 15);
            objectivesLabel.TabIndex = 6;
            objectivesLabel.TabStop = true;
            objectivesLabel.Text = "Objectives";
            objectivesLabel.LinkClicked += objectivesLabel_LinkClicked;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(529, 271);
            Controls.Add(objectivesLabel);
            Controls.Add(optionsLabel);
            Controls.Add(linkLabel1);
            Controls.Add(listBoxEntries);
            Controls.Add(listBoxKeys);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(412, 256);
            Name = "MainWindow";
            Text = "ToN Save Manager";
            Load += mainWindow_Loaded;
            Shown += mainWindow_Shown;
            ctxMenuEntries.ResumeLayout(false);
            ctxMenuKeys.ResumeLayout(false);
            ctxMenuSettings.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxKeys;
        private ListBox listBoxEntries;
        private LinkLabel linkLabel1;
        private ContextMenuStrip ctxMenuEntries;
        private ToolStripMenuItem ctxMenuEntriesCopyTo;
        private ToolStripMenuItem ctxMenuEntriesNew;
        private ToolStripMenuItem ctxMenuEntriesNote;
        private ContextMenuStrip ctxMenuKeys;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem ctxMenuEntriesDelete;
        private ToolStripMenuItem importToolStripMenuItem;
        private LinkLabel optionsLabel;
        private ContextMenuStrip ctxMenuSettings;
        private ToolStripMenuItem ctxMenuSettingsAutoCopy;
        private ToolStripMenuItem ctxMenuSettingsNotifSounds;
        private ToolStripMenuItem ctxMenuSettingsCollectNames;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem ctxMenuSettingsClose;
        private ToolStripMenuItem ctxMenuSettingsSelectSound;
        private ToolStripMenuItem ctxMenuSettingsClearSound;
        private ToolStripMenuItem ctxMenuSettingsUpdate;
        private LinkLabel objectivesLabel;
        private ToolStripMenuItem ctxMenuSettingsXSOverlay;
        private ToolStripMenuItem dateFormatToolStripMenuItem;
        private ToolStripMenuItem ctxMenuSettings24Hour;
        private ToolStripMenuItem ctxMenuSettingsInvertMD;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem ctxMenuSettingsShowSeconds;
    }
}