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
            label1 = new Label();
            SuspendLayout();
            // 
            // listBoxKeys
            // 
            listBoxKeys.BackColor = Color.FromArgb(59, 66, 82);
            listBoxKeys.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxKeys.FormattingEnabled = true;
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
            listBoxEntries.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            listBoxEntries.BackColor = Color.FromArgb(59, 66, 82);
            listBoxEntries.ForeColor = Color.FromArgb(236, 239, 244);
            listBoxEntries.FormattingEnabled = true;
            listBoxEntries.ItemHeight = 15;
            listBoxEntries.Location = new Point(210, 12);
            listBoxEntries.Name = "listBoxEntries";
            listBoxEntries.Size = new Size(307, 229);
            listBoxEntries.TabIndex = 1;
            listBoxEntries.SelectedIndexChanged += listBoxEntries_SelectedIndexChanged;
            // 
            // checkBoxAutoCopy
            // 
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
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.DimGray;
            label1.Location = new Point(391, 248);
            label1.Name = "label1";
            label1.Size = new Size(126, 15);
            label1.TabIndex = 3;
            label1.Text = "Coded by Kittenji";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(529, 270);
            Controls.Add(label1);
            Controls.Add(checkBoxAutoCopy);
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
            PerformLayout();
        }

        #endregion

        private ListBox listBoxKeys;
        private ListBox listBoxEntries;
        private CheckBox checkBoxAutoCopy;
        private Label label1;
    }
}