using System.Diagnostics;
using ToNSaveManager.Extensions;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using ToNSaveManager.Utils;
using ToNSaveManager.Utils.Discord;
using Timer = System.Windows.Forms.Timer;

namespace ToNSaveManager.Windows
{
    public partial class SettingsWindow : Form {
        #region Initialization
        internal static SettingsWindow? Instance;

        readonly Timer ClickTimer = new Timer() { Interval = 200 };
        readonly Stopwatch Stopwatch = new Stopwatch();

        public SettingsWindow() {
            InitializeComponent();
            ClickTimer.Tick += ClickTimer_Tick;
        }

        public static void Open(Form parent) {
            if (Instance == null || Instance.IsDisposed) Instance = new();

            if (Instance.Visible) {
                Instance.BringToFront();
                return;
            }

            Instance.StartPosition = FormStartPosition.Manual;
            Instance.Location = new Point(
                parent.Location.X + (parent.Width - Instance.Width) / 2,
                Math.Max(parent.Location.Y + (parent.Height - Instance.Height) / 2, 0)
            );
            Instance.Show(parent);
        }
        #endregion

        #region Form Events
        private Dictionary<string, Control> LocalizedControlCache = new Dictionary<string, Control>();

        internal void LocalizeContent() {
            LANG.C(this, "MAIN.SETTINGS");

            foreach (KeyValuePair<string, Control> pair in LocalizedControlCache) {
                LANG.C(pair.Value, pair.Key, toolTip);
                if (pair.Key == "SETTINGS.PLAYAUDIO") PostAudioLocationSet();
            }

            LANG.C(groupBoxGeneral, "SETTINGS.GROUP.GENERAL", toolTip);
            LANG.C(groupBoxNotifications, "SETTINGS.GROUP.NOTIFICATIONS", toolTip);
            LANG.C(groupBoxTime, "SETTINGS.GROUP.TIME_FORMAT", toolTip);
            LANG.C(groupBoxStyle, "SETTINGS.GROUP.STYLE", toolTip);

            LANG.C(btnCheckForUpdates, "SETTINGS.CHECK_UPDATE", toolTip);
            LANG.C(btnOpenData, "SETTINGS.OPEN_DATA_BTN", toolTip);

            LANG.C(setDataLocationToolStripMenuItem, "SETTINGS.CUSTOM_DATA_FOLDER");
            LANG.C(ctxItemPickFolder, "SETTINGS.CUSTOM_DATA_PICK_FOLDER");
            LANG.C(ctxItemResetToDefault, "SETTINGS.CUSTOM_DATA_RESET_DEFAULT");

            string? versionString = Program.GetVersion()?.ToString();
            if (!string.IsNullOrEmpty(versionString))
                toolTip.SetToolTip(btnCheckForUpdates, LANG.S("SETTINGS.VERSION", versionString) ?? $"Current Version {versionString}");

            RightToLeft = LANG.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }

        // Subscribe to events on load
        private void SettingsWindow_Load(object sender, EventArgs e) {
            BindControlsRecursive(Controls);
            LocalizeContent();

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

            // Round info
            checkShowWinLose.CheckedChanged += TimeFormat_CheckedChanged;
            checkSaveTerrors.CheckedChanged += checkSaveTerrors_CheckedChanged;
            checkSaveTerrors_CheckedChanged(checkSaveTerrors, e);

            // Discord Backups
            checkDiscordBackup.CheckedChanged += CheckDiscordBackup_CheckedChanged;

            // OSC
            checkOSCEnabled.CheckedChanged += checkOSCEnabled_CheckedChanged;

            FillLanguageBox();
        }

        private void SettingsWindow_FormClosed(object sender, FormClosedEventArgs e) {
            ClickTimer.Dispose();

            MainWindow.RefreshLists();
            MainWindow.ResetNotification();
        }

        private void TimeFormat_CheckedChanged(object? sender, EventArgs e) => MainWindow.RefreshLists();
        private void CheckXSOverlay_CheckedChanged(object? sender, EventArgs e) => MainWindow.SendXSNotification(true);
        private void CheckPlayAudio_CheckedChanged(object? sender, EventArgs e) {
            MainWindow.PlayNotification();
            if (!checkPlayAudio.Checked) MainWindow.ResetNotification();
        }

        private void checkSaveTerrors_CheckedChanged(object? sender, EventArgs e) {
            checkSaveTerrorsNote.ForeColor = checkSaveTerrors.Checked ? Color.White : Color.Gray;
            checkShowWinLose.ForeColor = checkSaveTerrorsNote.ForeColor;
            TimeFormat_CheckedChanged(sender, e);
        }

        private void CheckDiscordBackup_CheckedChanged(object? sender, EventArgs e) {
            if (checkDiscordBackup.Checked) {
                string url = Settings.Get.DiscordWebhookURL ?? string.Empty;
                EditResult edit = EditWindow.Show(Settings.Get.DiscordWebhookURL ?? string.Empty, LANG.S("SETTINGS.DISCORDWEBHOOK.TITLE") ?? "Discord Webhook URL", this);
                if (edit.Accept && !edit.Text.Equals(url, StringComparison.Ordinal)) {
                    url = edit.Text.Trim();

                    if (!string.IsNullOrWhiteSpace(url)) {
                        bool valid = DSWebHook.ValidateURL(url);

                        if (valid) {
                            Settings.Get.DiscordWebhookURL = url;
                            Settings.Export();
                        } else {
                            MessageBox.Show(LANG.S("SETTINGS.DISCORDWEBHOOKINVALID") ?? "The URL your provided does not match a discord webhook url.\n\nMake sure you created your webhook and copied the url correctly.", LANG.S("SETTINGS.DISCORDWEBHOOKINVALID.TITLE") ?? "Invalid Webhook URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } else {
                        Settings.Get.DiscordWebhookURL = null;
                    }
                }

                if (string.IsNullOrEmpty(Settings.Get.DiscordWebhookURL)) checkDiscordBackup.Checked = false;
            }

            MainWindow.Instance?.SetBackupButton(checkDiscordBackup.Checked);
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e) {
            if (Program.StartCheckForUpdate(true)) this.Close();
        }

        private void btnOpenData_Click(object sender, EventArgs e) {
            SaveData.OpenDataLocation();
        }

        private void checkPlayAudio_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) return;
            Stopwatch.Start();
            CancelNext = false;
        }

        private void checkPlayAudio_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right && !string.IsNullOrEmpty(Settings.Get.AudioLocation)) {
                Settings.Get.AudioLocation = null;
                Settings.Export();
                PostAudioLocationSet();
                return;
            }

            if (e.Button != MouseButtons.Left) return;

            Stopwatch.Stop();
            long elapsed = Stopwatch.ElapsedMilliseconds;
            Stopwatch.Reset();

            if (CancelNext) {
                CancelNext = false;
                return;
            }

            if (elapsed > 210) {
                TogglePlayAudio();
                return;
            }

            if (!DoubleClickCheck) {
                DoubleClickCheck = true;
                ClickTimer.Stop();
                ClickTimer.Start();
                return;
            }

            DoubleClickCheck = false;
            ClickTimer.Stop();

            using (OpenFileDialog fileDialog = new OpenFileDialog()) {
                fileDialog.InitialDirectory = "./";
                fileDialog.Title = LANG.S("SETTINGS.PLAYAUDIO.TITLE") ?? "Select Custom Sound";
                fileDialog.Filter = "Waveform (*.wav)|*.wav";

                if (fileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName)) {
                    Settings.Get.AudioLocation = fileDialog.FileName;
                    Settings.Export();
                    PostAudioLocationSet();
                }
            }
        }

        private void checkOSCEnabled_CheckedChanged(object? sender, EventArgs e) {
            if (checkOSCEnabled.Checked) LilOSC.SendData(true);
        }

        private void checkOSCEnabled_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                MainWindow.OpenExternalLink("https://github.com/ChrisFeline/ToNSaveManager/?tab=readme-ov-file#osc-documentation");
        }

        private void ctxItemPickFolder_Click(object sender, EventArgs e) {
            SaveData.SetDataLocation(false);
        }

        private void ctxItemResetToDefault_Click(object sender, EventArgs e) {
            SaveData.SetDataLocation(true);
        }

        private void CheckColorObjectives_CheckedChanged(object? sender, EventArgs e) {
            ObjectivesWindow.RefreshLists();
        }

        // Double click
        private bool DoubleClickCheck = false;
        private bool CancelNext = false;
        private void ClickTimer_Tick(object? sender, EventArgs e) {
            ClickTimer.Stop();
            if (DoubleClickCheck) {
                DoubleClickCheck = false;
                CancelNext = true;
                TogglePlayAudio();
            }
        }

        private void languageSelectBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!FilledLanguages || languageSelectBox.SelectedIndex < 0 || languageSelectBox.SelectedItem == null) return;
            LANG.LangKey langKey = (LANG.LangKey)languageSelectBox.SelectedItem;

            if (LANG.SelectedKey != langKey.Key) {
                Debug.WriteLine("Changing language to: " + langKey);
                LANG.Select(langKey.Key);
                LANG.ReloadAll();
                Settings.Get.SelectedLanguage = langKey.Key;
                Settings.Export();
            }
        }

        private void languageSelect_DragEnter(object sender, DragEventArgs e) {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void languageSelect_DragDrop(object sender, DragEventArgs e) {
            string[]? files = (string[]?)e.Data?.GetData(DataFormats.FileDrop);
            if (files == null) return;

            foreach (string file in files) {
                if (file.EndsWith(".json"))
                    LANG.AddFromFile(file);
            }
        }

        private bool FilledLanguages;
        internal void FillLanguageBox() {
            FilledLanguages = false;
            languageSelectBox.Items.Clear();
            foreach (var lang in LANG.AvailableLang) {
                int index = languageSelectBox.Items.Count;
                languageSelectBox.Items.Add(lang);
                if (lang.Key == LANG.SelectedKey) languageSelectBox.SelectedIndex = index;
            }
            FilledLanguages = true;
        }
        #endregion

        #region Utils
        private void TogglePlayAudio() {
            checkPlayAudio.Checked = !checkPlayAudio.Checked;
        }

        private void BindControlsRecursive(Control.ControlCollection controls) {
            foreach (Control c in controls) {
                string? tag = c.Tag?.ToString();
                if (!string.IsNullOrEmpty(tag)) {
                    int index = tag.IndexOf('|', StringComparison.InvariantCulture);
                    if (index > -1) {
                        string tooltip = tag.Substring(index + 1).Replace("\\n", Environment.NewLine, StringComparison.Ordinal);
                        tag = tag.Substring(0, index);
                        // toolTip.SetToolTip(c, tooltip);
                    }
                }

                switch (c) {
                    case GroupBox g:
                        BindControlsRecursive(g.Controls);
                        break;
                    case CheckBox b:
                        if (!string.IsNullOrEmpty(tag)) {
                            LocalizedControlCache.Add("SETTINGS." + tag.ToUpperInvariant(), c);
                            b.BindSettings(tag);
                        }
                        break;
                    default: break;
                }
            }
        }

        private void PostAudioLocationSet() {
            bool hasLocation = string.IsNullOrEmpty(Settings.Get.AudioLocation);
            string? name = (hasLocation ? "default.wav" : (Path.GetFileName(Settings.Get.AudioLocation) ?? "custom.wav"));
            checkPlayAudio.Text = LANG.S("SETTINGS.PLAYAUDIO", name) ?? $"Play Audio ({name})";
        }
        #endregion
    }
}
