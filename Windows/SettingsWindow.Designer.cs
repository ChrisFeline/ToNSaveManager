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
            checkPlayerNames = new CheckBox();
            checkAutoCopy = new CheckBox();
            groupBoxNotifications = new GroupBox();
            checkPlayAudio = new CheckBox();
            checkXSOverlay = new CheckBox();
            groupBox1 = new GroupBox();
            checkInvertMD = new CheckBox();
            checkShowSeconds = new CheckBox();
            check24Hour = new CheckBox();
            btnCheckForUpdates = new Button();
            btnOpenData = new Button();
            toolTip = new ToolTip(components);
            groupBoxGeneral.SuspendLayout();
            groupBoxNotifications.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxGeneral
            // 
            groupBoxGeneral.AutoSize = true;
            groupBoxGeneral.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxGeneral.Controls.Add(checkPlayerNames);
            groupBoxGeneral.Controls.Add(checkAutoCopy);
            groupBoxGeneral.Dock = DockStyle.Top;
            groupBoxGeneral.ForeColor = Color.White;
            groupBoxGeneral.Location = new Point(8, 8);
            groupBoxGeneral.Name = "groupBoxGeneral";
            groupBoxGeneral.Size = new Size(268, 57);
            groupBoxGeneral.TabIndex = 0;
            groupBoxGeneral.TabStop = false;
            groupBoxGeneral.Text = "General";
            // 
            // checkPlayerNames
            // 
            checkPlayerNames.Dock = DockStyle.Top;
            checkPlayerNames.Location = new Point(3, 36);
            checkPlayerNames.Name = "checkPlayerNames";
            checkPlayerNames.Padding = new Padding(3, 0, 3, 0);
            checkPlayerNames.Size = new Size(262, 18);
            checkPlayerNames.TabIndex = 1;
            checkPlayerNames.Tag = "SaveNames";
            checkPlayerNames.Text = "Collect Player Names";
            checkPlayerNames.UseVisualStyleBackColor = true;
            // 
            // checkAutoCopy
            // 
            checkAutoCopy.Dock = DockStyle.Top;
            checkAutoCopy.Location = new Point(3, 18);
            checkAutoCopy.Name = "checkAutoCopy";
            checkAutoCopy.Padding = new Padding(3, 0, 3, 0);
            checkAutoCopy.Size = new Size(262, 18);
            checkAutoCopy.TabIndex = 0;
            checkAutoCopy.Tag = "AutoCopy";
            checkAutoCopy.Text = "Auto Clipboard Copy";
            checkAutoCopy.UseVisualStyleBackColor = true;
            // 
            // groupBoxNotifications
            // 
            groupBoxNotifications.AutoSize = true;
            groupBoxNotifications.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxNotifications.Controls.Add(checkPlayAudio);
            groupBoxNotifications.Controls.Add(checkXSOverlay);
            groupBoxNotifications.Dock = DockStyle.Top;
            groupBoxNotifications.ForeColor = Color.White;
            groupBoxNotifications.Location = new Point(8, 65);
            groupBoxNotifications.Name = "groupBoxNotifications";
            groupBoxNotifications.Size = new Size(268, 57);
            groupBoxNotifications.TabIndex = 2;
            groupBoxNotifications.TabStop = false;
            groupBoxNotifications.Text = "Notifications";
            // 
            // checkPlayAudio
            // 
            checkPlayAudio.Dock = DockStyle.Top;
            checkPlayAudio.Location = new Point(3, 36);
            checkPlayAudio.Name = "checkPlayAudio";
            checkPlayAudio.Padding = new Padding(3, 0, 3, 0);
            checkPlayAudio.Size = new Size(262, 18);
            checkPlayAudio.TabIndex = 1;
            checkPlayAudio.Tag = "PlayAudio";
            checkPlayAudio.Text = "Play Sound (default.wav)";
            checkPlayAudio.UseVisualStyleBackColor = true;
            checkPlayAudio.MouseUp += checkPlayAudio_MouseUp;
            // 
            // checkXSOverlay
            // 
            checkXSOverlay.Dock = DockStyle.Top;
            checkXSOverlay.Location = new Point(3, 18);
            checkXSOverlay.Name = "checkXSOverlay";
            checkXSOverlay.Padding = new Padding(3, 0, 3, 0);
            checkXSOverlay.Size = new Size(262, 18);
            checkXSOverlay.TabIndex = 0;
            checkXSOverlay.Tag = "XSOverlay";
            checkXSOverlay.Text = "XSOverlay Popup";
            checkXSOverlay.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(checkInvertMD);
            groupBox1.Controls.Add(checkShowSeconds);
            groupBox1.Controls.Add(check24Hour);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(8, 122);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(268, 75);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Time Formatting";
            // 
            // checkInvertMD
            // 
            checkInvertMD.Dock = DockStyle.Top;
            checkInvertMD.Location = new Point(3, 54);
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
            checkShowSeconds.Location = new Point(3, 36);
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
            check24Hour.Location = new Point(3, 18);
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
            btnCheckForUpdates.Location = new Point(8, 208);
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
            btnOpenData.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            btnOpenData.FlatStyle = FlatStyle.Flat;
            btnOpenData.ForeColor = Color.White;
            btnOpenData.Location = new Point(223, 208);
            btnOpenData.Name = "btnOpenData";
            btnOpenData.Size = new Size(53, 24);
            btnOpenData.TabIndex = 5;
            btnOpenData.Text = "Data";
            btnOpenData.UseVisualStyleBackColor = true;
            btnOpenData.Click += btnOpenData_Click;
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(284, 243);
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
    }
}