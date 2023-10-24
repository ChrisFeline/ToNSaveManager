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
            listBoxKeys = new ListBox();
            listBoxEntries = new ListBox();
            checkBoxAutoCopy = new CheckBox();
            linkLabel1 = new LinkLabel();
            SuspendLayout();
            // 
            // listBoxKeys
            // 
            listBoxKeys.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBoxKeys.BackColor = Color.FromArgb(59, 66, 82);
            listBoxKeys.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxKeys.FormattingEnabled = true;
            listBoxKeys.IntegralHeight = false;
            listBoxKeys.ItemHeight = 15;
            listBoxKeys.Location = new Point(12, 12);
            listBoxKeys.Name = "listBoxKeys";
            listBoxKeys.Size = new Size(192, 229);
            listBoxKeys.TabIndex = 0;
            listBoxKeys.SelectedIndexChanged += listBoxKeys_SelectedIndexChanged;
            listBoxKeys.KeyUp += listBoxKeys_KeyUp;
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
            listBoxEntries.MouseMove += listBoxEntries_MouseMove;
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxKeys;
        private ListBox listBoxEntries;
        private CheckBox checkBoxAutoCopy;
        private LinkLabel linkLabel1;
    }
}