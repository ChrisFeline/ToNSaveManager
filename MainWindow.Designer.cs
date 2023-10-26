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
            listBoxKeys = new ListBox();
            listBoxEntries = new ListBox();
            checkBoxAutoCopy = new CheckBox();
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
            ctxMenuEntries.SuspendLayout();
            ctxMenuKeys.SuspendLayout();
            SuspendLayout();
            // 
            // listBoxKeys
            // 
            listBoxKeys.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBoxKeys.BackColor = Color.FromArgb(59, 66, 82);
            listBoxKeys.DrawMode = DrawMode.OwnerDrawVariable;
            listBoxKeys.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxKeys.FormattingEnabled = true;
            listBoxKeys.IntegralHeight = false;
            listBoxKeys.ItemHeight = 15;
            listBoxKeys.Location = new Point(12, 12);
            listBoxKeys.Name = "listBoxKeys";
            listBoxKeys.Size = new Size(192, 229);
            listBoxKeys.TabIndex = 0;
            listBoxKeys.DrawItem += listBoxEntries_DrawItem;
            listBoxKeys.SelectedIndexChanged += listBoxKeys_SelectedIndexChanged;
            listBoxKeys.MouseUp += listBoxKeys_MouseUp;
            // 
            // listBoxEntries
            // 
            listBoxEntries.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxEntries.BackColor = Color.FromArgb(59, 66, 82);
            listBoxEntries.DrawMode = DrawMode.OwnerDrawVariable;
            listBoxEntries.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxEntries.FormattingEnabled = true;
            listBoxEntries.IntegralHeight = false;
            listBoxEntries.ItemHeight = 15;
            listBoxEntries.Location = new Point(210, 12);
            listBoxEntries.Name = "listBoxEntries";
            listBoxEntries.Size = new Size(307, 229);
            listBoxEntries.TabIndex = 1;
            listBoxEntries.DrawItem += listBoxEntries_DrawItem;
            listBoxEntries.SelectedIndexChanged += listBoxEntries_SelectedIndexChanged;
            listBoxEntries.MouseUp += listBoxEntries_MouseUp;
            listBoxEntries.Resize += listBoxEntries_Resize;
            // 
            // checkBoxAutoCopy
            // 
            checkBoxAutoCopy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBoxAutoCopy.AutoSize = true;
            checkBoxAutoCopy.Checked = true;
            checkBoxAutoCopy.CheckState = CheckState.Checked;
            checkBoxAutoCopy.ForeColor = Color.White;
            checkBoxAutoCopy.Location = new Point(12, 247);
            checkBoxAutoCopy.Name = "checkBoxAutoCopy";
            checkBoxAutoCopy.Size = new Size(180, 19);
            checkBoxAutoCopy.TabIndex = 2;
            checkBoxAutoCopy.Text = "Auto Save to Clipboard";
            checkBoxAutoCopy.UseVisualStyleBackColor = true;
            checkBoxAutoCopy.CheckedChanged += checkBoxAutoCopy_CheckedChanged;
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = Color.DodgerBlue;
            linkLabel1.Location = new Point(433, 248);
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
            ctxMenuEntries.Size = new Size(181, 98);
            ctxMenuEntries.Closed += ctxMenuEntries_Closed;
            ctxMenuEntries.Opened += ctxMenuEntries_Opened;
            // 
            // ctxMenuEntriesCopyTo
            // 
            ctxMenuEntriesCopyTo.DropDownItems.AddRange(new ToolStripItem[] { ctxMenuEntriesNew });
            ctxMenuEntriesCopyTo.Name = "ctxMenuEntriesCopyTo";
            ctxMenuEntriesCopyTo.Size = new Size(180, 22);
            ctxMenuEntriesCopyTo.Text = "Add to";
            // 
            // ctxMenuEntriesNew
            // 
            ctxMenuEntriesNew.Name = "ctxMenuEntriesNew";
            ctxMenuEntriesNew.Size = new Size(180, 22);
            ctxMenuEntriesNew.Text = "New Collection";
            ctxMenuEntriesNew.ToolTipText = "Add this entry to a new collection.";
            ctxMenuEntriesNew.Click += ctxMenuEntriesNew_Click;
            // 
            // ctxMenuEntriesNote
            // 
            ctxMenuEntriesNote.Name = "ctxMenuEntriesNote";
            ctxMenuEntriesNote.Size = new Size(180, 22);
            ctxMenuEntriesNote.Text = "Edit Note";
            ctxMenuEntriesNote.Click += ctxMenuEntriesNote_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(177, 6);
            // 
            // ctxMenuEntriesDelete
            // 
            ctxMenuEntriesDelete.Name = "ctxMenuEntriesDelete";
            ctxMenuEntriesDelete.Size = new Size(180, 22);
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
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(529, 270);
            Controls.Add(linkLabel1);
            Controls.Add(checkBoxAutoCopy);
            Controls.Add(listBoxEntries);
            Controls.Add(listBoxKeys);
            MaximizeBox = false;
            MinimumSize = new Size(412, 256);
            Name = "MainWindow";
            ShowIcon = false;
            Text = "ToN Save Manager";
            Load += mainWindow_Loaded;
            Shown += mainWindow_Shown;
            ctxMenuEntries.ResumeLayout(false);
            ctxMenuKeys.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxKeys;
        private ListBox listBoxEntries;
        private CheckBox checkBoxAutoCopy;
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
    }
}