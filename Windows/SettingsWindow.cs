﻿using System.Diagnostics;
using ToNSaveManager.Extensions;
using ToNSaveManager.Models;
using ToNSaveManager.Utils.Discord;
using Timer = System.Windows.Forms.Timer;

namespace ToNSaveManager.Windows
{
    public partial class SettingsWindow : Form
    {
        #region Initialization
        static SettingsWindow? Instance;

        readonly Timer ClickTimer = new Timer() { Interval = 200 };
        readonly Stopwatch Stopwatch = new Stopwatch();

        public SettingsWindow()
        {
            InitializeComponent();
            ClickTimer.Tick += ClickTimer_Tick;
        }

        public static void Open(Form parent)
        {
            if (Instance == null || Instance.IsDisposed) Instance = new();

            if (Instance.Visible)
            {
                Instance.BringToFront();
                return;
            }

            Instance.StartPosition = FormStartPosition.Manual;
            Instance.Location = new Point(
                parent.Location.X + (parent.Width - Instance.Width) / 2,
                parent.Location.Y + (parent.Height - Instance.Height) / 2
            );
            Instance.Show(parent);
        }
        #endregion

        #region Form Events
        // Subscribe to events on load
        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            BindControlsRecursive(Controls);
            // Custom audio handling
            PostAudioLocationSet();
            checkPlayAudio.CheckedChanged += CheckPlayAudio_CheckedChanged;
            checkXSOverlay.CheckedChanged += CheckXSOverlay_CheckedChanged;
            // Refresh lists when time format changes
            check24Hour.CheckedChanged += TimeFormat_CheckedChanged;
            checkInvertMD.CheckedChanged += TimeFormat_CheckedChanged;
            checkShowSeconds.CheckedChanged += TimeFormat_CheckedChanged;
            checkShowDate.CheckedChanged += TimeFormat_CheckedChanged;
            // Refresh list when style is changed
            checkColorObjectives.CheckedChanged += CheckColorObjectives_CheckedChanged;

            // Tooltips
            toolTip.SetToolTip(btnCheckForUpdates, "Current Version: " + Program.GetVersion());

            // Round info
            checkShowWinLose.CheckedChanged += TimeFormat_CheckedChanged;
            checkSaveTerrors.CheckedChanged += checkSaveTerrors_CheckedChanged;
            checkSaveTerrors_CheckedChanged(checkSaveTerrors, e);

            // Discord Backups
            checkDiscordBackup.CheckedChanged += CheckDiscordBackup_CheckedChanged;
        }

        private void SettingsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClickTimer.Dispose();

            MainWindow.RefreshLists();
            MainWindow.ResetNotification();
        }

        private void TimeFormat_CheckedChanged(object? sender, EventArgs e) => MainWindow.RefreshLists();
        private void CheckXSOverlay_CheckedChanged(object? sender, EventArgs e) => MainWindow.SendXSNotification(true);
        private void CheckPlayAudio_CheckedChanged(object? sender, EventArgs e)
        {
            MainWindow.PlayNotification();
            if (!checkPlayAudio.Checked) MainWindow.ResetNotification();
        }

        private void checkSaveTerrors_CheckedChanged(object? sender, EventArgs e)
        {
            checkSaveTerrorsNote.ForeColor = checkSaveTerrors.Checked ? Color.White : Color.Gray;
            checkShowWinLose.ForeColor = checkSaveTerrorsNote.ForeColor;
            TimeFormat_CheckedChanged(sender, e);
        }

        private void CheckDiscordBackup_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkDiscordBackup.Checked)
            {
                string url = Settings.Get.DiscordWebhookURL ?? string.Empty;
                EditResult edit = EditWindow.Show(Settings.Get.DiscordWebhookURL ?? string.Empty, "Discord Webhook URL", this);
                if (edit.Accept && !edit.Text.Equals(url, StringComparison.Ordinal))
                {
                    url = edit.Text.Trim();

                    if (!string.IsNullOrWhiteSpace(url)) {
                        bool valid = DSWebHook.ValidateURL(url);

                        if (valid) {
                            Settings.Get.DiscordWebhookURL = url;
                            Settings.Export();
                        } else {
                            MessageBox.Show($"The URL your provided does not match a discord webhook url.\n\nMake sure you created your webhook and copied the url correctly.", "Invalid Webhook URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } else {
                        Settings.Get.DiscordWebhookURL = null;
                    }
                }

                if (string.IsNullOrEmpty(Settings.Get.DiscordWebhookURL)) checkDiscordBackup.Checked = false;
            }

            MainWindow.Instance?.SetBackupButton(checkDiscordBackup.Checked);
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            if (Program.StartCheckForUpdate(true)) this.Close();
        }

        private void btnOpenData_Click(object sender, EventArgs e)
        {
            SaveData.OpenDataLocation();
        }

        private void checkPlayAudio_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            Stopwatch.Start();
            CancelNext = false;
        }

        private void checkPlayAudio_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !string.IsNullOrEmpty(Settings.Get.AudioLocation))
            {
                Settings.Get.AudioLocation = null;
                Settings.Export();
                PostAudioLocationSet();
                return;
            }

            if (e.Button != MouseButtons.Left) return;

            Stopwatch.Stop();
            long elapsed = Stopwatch.ElapsedMilliseconds;
            Stopwatch.Reset();

            if (CancelNext)
            {
                CancelNext = false;
                return;
            }

            if (elapsed > 210)
            {
                TogglePlayAudio();
                return;
            }

            if (!DoubleClickCheck)
            {
                DoubleClickCheck = true;
                ClickTimer.Stop();
                ClickTimer.Start();
                return;
            }

            DoubleClickCheck = false;
            ClickTimer.Stop();

            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.InitialDirectory = "./";
                fileDialog.Title = "Select Custom Sound";
                fileDialog.Filter = "Waveform Audio (*.wav)|*.wav";

                if (fileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
                {
                    Settings.Get.AudioLocation = fileDialog.FileName;
                    Settings.Export();
                    PostAudioLocationSet();
                }
            }
        }

        private void ctxItemPickFolder_Click(object sender, EventArgs e)
        {
            SaveData.SetDataLocation(false);
        }

        private void ctxItemResetToDefault_Click(object sender, EventArgs e)
        {
            SaveData.SetDataLocation(true);
        }

        private void CheckColorObjectives_CheckedChanged(object? sender, EventArgs e)
        {
            ObjectivesWindow.RefreshLists();
        }

        // Double click
        private bool DoubleClickCheck = false;
        private bool CancelNext = false;
        private void ClickTimer_Tick(object? sender, EventArgs e)
        {
            ClickTimer.Stop();
            if (DoubleClickCheck)
            {
                DoubleClickCheck = false;
                CancelNext = true;
                TogglePlayAudio();
            }
        }
        #endregion

        #region Utils
        private void TogglePlayAudio()
        {
            checkPlayAudio.Checked = !checkPlayAudio.Checked;
        }

        private void BindControlsRecursive(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                string? tag = c.Tag?.ToString();
                if (!string.IsNullOrEmpty(tag))
                {
                    int index = tag.IndexOf('|', StringComparison.InvariantCulture);
                    if (index > -1)
                    {
                        string tooltip = tag.Substring(index + 1).Replace("\\n", Environment.NewLine, StringComparison.Ordinal);
                        tag = tag.Substring(0, index);
                        toolTip.SetToolTip(c, tooltip);
                    }
                }

                switch (c)
                {
                    case GroupBox g:
                        BindControlsRecursive(g.Controls);
                        break;
                    case CheckBox b:
                        if (!string.IsNullOrEmpty(tag))
                            b.BindSettings(tag);
                        break;
                    default: break;
                }
            }
        }

        private void PostAudioLocationSet()
        {
            bool hasLocation = string.IsNullOrEmpty(Settings.Get.AudioLocation);
            checkPlayAudio.Text = "Play Audio (" + (hasLocation ? "default.wav" : Path.GetFileName(Settings.Get.AudioLocation)) + ")";
        }

        /*
        private void WriteInstanceLogs()
        {
            if (!Settings.Get.RecordInstanceLogs) return;

            var logContext = MainWindow.LogWatcher.GetEarliestContext();
            if (logContext == null) return;

            string logs = logContext.GetRoomLogs();
            string destination = "debug";
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            string filePath = Path.Combine(destination, "output_logs_instance.log");
            File.WriteAllText(filePath, logs);

            logs = logContext.GetRoomExceptions();
            if (logs.Length > 0)
            {
                filePath = Path.Combine(destination, "output_log_exceptions.log");
                File.WriteAllText(filePath, logs);
            }

            filePath = Path.GetFullPath(destination);
            MainWindow.OpenExternalLink(filePath);
        }
        */
        #endregion

    }
}
