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
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            listBoxKeys = new ListBox();
            listBoxEntries = new ListBox();
            ctxMenuEntries = new ContextMenuStrip(components);
            ctxMenuEntriesCopyTo = new ToolStripMenuItem();
            ctxMenuEntriesNew = new ToolStripMenuItem();
            ctxMenuEntriesNote = new ToolStripMenuItem();
            ctxMenuEntriesBackup = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            ctxMenuEntriesDelete = new ToolStripMenuItem();
            ctxMenuKeys = new ContextMenuStrip(components);
            importToolStripMenuItem = new ToolStripMenuItem();
            renameToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            btnSettings = new Button();
            btnObjectives = new Button();
            linkWiki = new Button();
            splitContainer1 = new SplitContainer();
            btnStats = new Button();
            linkSupport = new Button();
            ctxMenuEntries.SuspendLayout();
            ctxMenuKeys.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // listBoxKeys
            // 
            listBoxKeys.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxKeys.BackColor = Color.FromArgb(59, 66, 82);
            listBoxKeys.BorderStyle = BorderStyle.FixedSingle;
            listBoxKeys.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxKeys.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxKeys.FormattingEnabled = true;
            listBoxKeys.IntegralHeight = false;
            listBoxKeys.ItemHeight = 15;
            listBoxKeys.Location = new Point(0, 0);
            listBoxKeys.Name = "listBoxKeys";
            listBoxKeys.Size = new Size(178, 217);
            listBoxKeys.TabIndex = 0;
            listBoxKeys.TabStop = false;
            listBoxKeys.DrawItem += listBoxEntries_DrawItem;
            listBoxKeys.KeyPress += listBoxKeys_KeyPress;
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
            listBoxEntries.Location = new Point(0, 0);
            listBoxEntries.Name = "listBoxEntries";
            listBoxEntries.Size = new Size(323, 217);
            listBoxEntries.TabIndex = 1;
            listBoxEntries.TabStop = false;
            listBoxEntries.DrawItem += listBoxEntries_DrawItem;
            listBoxEntries.MouseDown += listBoxEntries_MouseDown;
            listBoxEntries.MouseLeave += listBoxEntries_MouseLeave;
            listBoxEntries.MouseMove += listBoxEntries_MouseMove;
            listBoxEntries.MouseUp += listBoxEntries_MouseUp;
            listBoxEntries.Resize += listBoxEntries_Resize;
            // 
            // ctxMenuEntries
            // 
            ctxMenuEntries.Items.AddRange(new ToolStripItem[] { ctxMenuEntriesCopyTo, ctxMenuEntriesNote, ctxMenuEntriesBackup, toolStripMenuItem2, ctxMenuEntriesDelete });
            ctxMenuEntries.Name = "ctxMenuEntries";
            ctxMenuEntries.Size = new Size(124, 98);
            ctxMenuEntries.Closed += ctxMenuEntries_Closed;
            ctxMenuEntries.Opened += ctxMenuEntries_Opened;
            // 
            // ctxMenuEntriesCopyTo
            // 
            ctxMenuEntriesCopyTo.DropDownItems.AddRange(new ToolStripItem[] { ctxMenuEntriesNew });
            ctxMenuEntriesCopyTo.Name = "ctxMenuEntriesCopyTo";
            ctxMenuEntriesCopyTo.Size = new Size(123, 22);
            ctxMenuEntriesCopyTo.Text = "Add to";
            // 
            // ctxMenuEntriesNew
            // 
            ctxMenuEntriesNew.Name = "ctxMenuEntriesNew";
            ctxMenuEntriesNew.Size = new Size(155, 22);
            ctxMenuEntriesNew.Text = "New Collection";
            ctxMenuEntriesNew.ToolTipText = "Add this entry to a new collection.";
            ctxMenuEntriesNew.Click += ctxMenuEntriesNew_Click;
            // 
            // ctxMenuEntriesNote
            // 
            ctxMenuEntriesNote.Name = "ctxMenuEntriesNote";
            ctxMenuEntriesNote.Size = new Size(123, 22);
            ctxMenuEntriesNote.Text = "Edit Note";
            ctxMenuEntriesNote.Click += ctxMenuEntriesNote_Click;
            // 
            // ctxMenuEntriesBackup
            // 
            ctxMenuEntriesBackup.Enabled = false;
            ctxMenuEntriesBackup.Name = "ctxMenuEntriesBackup";
            ctxMenuEntriesBackup.Size = new Size(123, 22);
            ctxMenuEntriesBackup.Text = "Backup";
            ctxMenuEntriesBackup.ToolTipText = "Force upload a backup of this code to Discord as a file, requires Auto Discord Backup to be enabled in settings.";
            ctxMenuEntriesBackup.Click += ctxMenuEntriesBackup_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(120, 6);
            // 
            // ctxMenuEntriesDelete
            // 
            ctxMenuEntriesDelete.Name = "ctxMenuEntriesDelete";
            ctxMenuEntriesDelete.Size = new Size(123, 22);
            ctxMenuEntriesDelete.Text = "Delete";
            ctxMenuEntriesDelete.Click += ctxMenuEntriesDelete_Click;
            // 
            // ctxMenuKeys
            // 
            ctxMenuKeys.Items.AddRange(new ToolStripItem[] { importToolStripMenuItem, renameToolStripMenuItem, toolStripMenuItem1, deleteToolStripMenuItem });
            ctxMenuKeys.Name = "ctxMenuKeys";
            ctxMenuKeys.Size = new Size(118, 76);
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Size = new Size(117, 22);
            importToolStripMenuItem.Text = "Import";
            importToolStripMenuItem.Click += ctxMenuKeysImport_Click;
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new Size(117, 22);
            renameToolStripMenuItem.Text = "Rename";
            renameToolStripMenuItem.Click += ctxMenuKeysRename_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(114, 6);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(117, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += ctxMenuKeysDelete_Click;
            // 
            // btnSettings
            // 
            btnSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSettings.BackColor = Color.FromArgb(46, 52, 64);
            btnSettings.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            btnSettings.FlatStyle = FlatStyle.Flat;
            btnSettings.ForeColor = Color.White;
            btnSettings.Location = new Point(0, 223);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(178, 24);
            btnSettings.TabIndex = 0;
            btnSettings.TabStop = false;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = false;
            btnSettings.Click += btnSettings_Click;
            // 
            // btnObjectives
            // 
            btnObjectives.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnObjectives.BackColor = Color.FromArgb(46, 52, 64);
            btnObjectives.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            btnObjectives.FlatStyle = FlatStyle.Flat;
            btnObjectives.ForeColor = Color.White;
            btnObjectives.Location = new Point(0, 223);
            btnObjectives.Name = "btnObjectives";
            btnObjectives.Size = new Size(165, 24);
            btnObjectives.TabIndex = 0;
            btnObjectives.TabStop = false;
            btnObjectives.Text = "Objectives";
            btnObjectives.UseVisualStyleBackColor = false;
            btnObjectives.Click += btnObjectives_Click;
            // 
            // linkWiki
            // 
            linkWiki.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkWiki.BackColor = Color.FromArgb(46, 52, 64);
            linkWiki.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            linkWiki.FlatStyle = FlatStyle.Flat;
            linkWiki.ForeColor = Color.White;
            linkWiki.Location = new Point(235, 223);
            linkWiki.Name = "linkWiki";
            linkWiki.Size = new Size(58, 24);
            linkWiki.TabIndex = 3;
            linkWiki.TabStop = false;
            linkWiki.Text = "Wiki";
            linkWiki.UseVisualStyleBackColor = false;
            linkWiki.Click += linkWiki_Clicked;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(12, 12);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(listBoxKeys);
            splitContainer1.Panel1.Controls.Add(btnSettings);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(btnStats);
            splitContainer1.Panel2.Controls.Add(linkSupport);
            splitContainer1.Panel2.Controls.Add(listBoxEntries);
            splitContainer1.Panel2.Controls.Add(btnObjectives);
            splitContainer1.Panel2.Controls.Add(linkWiki);
            splitContainer1.Size = new Size(505, 247);
            splitContainer1.SplitterDistance = 178;
            splitContainer1.TabIndex = 0;
            splitContainer1.TabStop = false;
            splitContainer1.SplitterMoved += splitContainer1_SplitterMoved;
            // 
            // btnStats
            // 
            btnStats.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnStats.BackColor = Color.FromArgb(46, 52, 64);
            btnStats.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            btnStats.FlatStyle = FlatStyle.Flat;
            btnStats.ForeColor = Color.White;
            btnStats.Location = new Point(171, 223);
            btnStats.Name = "btnStats";
            btnStats.Size = new Size(58, 24);
            btnStats.TabIndex = 5;
            btnStats.TabStop = false;
            btnStats.Text = "Stats";
            btnStats.UseVisualStyleBackColor = false;
            btnStats.Click += btnStats_Click;
            // 
            // linkSupport
            // 
            linkSupport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkSupport.BackColor = Color.FromArgb(46, 52, 64);
            linkSupport.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            linkSupport.FlatStyle = FlatStyle.Flat;
            linkSupport.ForeColor = Color.White;
            linkSupport.Image = (Image)resources.GetObject("linkSupport.Image");
            linkSupport.Location = new Point(299, 223);
            linkSupport.Name = "linkSupport";
            linkSupport.Size = new Size(24, 24);
            linkSupport.TabIndex = 4;
            linkSupport.TabStop = false;
            linkSupport.UseVisualStyleBackColor = false;
            linkSupport.Click += linkSupport_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(529, 271);
            Controls.Add(splitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(412, 256);
            Name = "MainWindow";
            Text = "ToN Save Manager";
            Activated += MainWindow_Activated;
            FormClosing += MainWindow_FormClosing;
            Load += mainWindow_Loaded;
            Shown += mainWindow_Shown;
            ctxMenuEntries.ResumeLayout(false);
            ctxMenuKeys.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBoxKeys;
        private ListBox listBoxEntries;
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
        private Button btnSettings;
        private Button btnObjectives;
        private Button linkWiki;
        private SplitContainer splitContainer1;
        private ToolStripMenuItem ctxMenuEntriesBackup;
        private Button linkSupport;
        private Button btnStats;
    }
}