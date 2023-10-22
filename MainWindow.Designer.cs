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
            SuspendLayout();
            // 
            // listBoxKeys
            // 
            listBoxKeys.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBoxKeys.BackColor = Color.FromArgb(59, 66, 82);
            listBoxKeys.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxKeys.FormattingEnabled = true;
            listBoxKeys.ItemHeight = 15;
            listBoxKeys.Location = new Point(12, 12);
            listBoxKeys.Name = "listBoxKeys";
            listBoxKeys.Size = new Size(192, 259);
            listBoxKeys.TabIndex = 0;
            listBoxKeys.SelectedIndexChanged += listBoxKeys_SelectedIndexChanged;
            // 
            // listBoxEntries
            // 
            listBoxEntries.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBoxEntries.BackColor = Color.FromArgb(59, 66, 82);
            listBoxEntries.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxEntries.FormattingEnabled = true;
            listBoxEntries.ItemHeight = 15;
            listBoxEntries.Location = new Point(210, 12);
            listBoxEntries.Name = "listBoxEntries";
            listBoxEntries.Size = new Size(307, 259);
            listBoxEntries.TabIndex = 1;
            listBoxEntries.SelectedIndexChanged += listBoxEntries_SelectedIndexChanged;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(529, 285);
            Controls.Add(listBoxEntries);
            Controls.Add(listBoxKeys);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainWindow";
            ShowIcon = false;
            Text = "ToN Save Manager";
            Load += mainWindow_Loaded;
            Shown += mainWindow_Shown;
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBoxKeys;
        private ListBox listBoxEntries;
    }
}