using System;
using System.Diagnostics;
using System.Security.Policy;
using ToNSaveManager.Extensions;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Stats;
using ToNSaveManager.Utils;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Utils.OpenRGB;
using Windows.ApplicationModel.Contacts;
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

            LANG.C(labelGroupGeneral, "SETTINGS.GROUP.GENERAL", toolTip);
            LANG.C(labelGroupDiscord, "SETTINGS.GROUP.DISCORD", toolTip);
            LANG.C(labelGroupNotifications, "SETTINGS.GROUP.NOTIFICATIONS", toolTip);
            LANG.C(labelGroupFormat, "SETTINGS.GROUP.TIME_FORMAT", toolTip);
            LANG.C(labelGroupStyle, "SETTINGS.GROUP.STYLE", toolTip);
            LANG.C(labelGroupOSC, "SETTINGS.GROUP.OSC", toolTip);

            LANG.C(linkEditChatbox, "SETTINGS.OSCSENDCHATBOX_EDIT", toolTip);
            LANG.C(linkAddInfoFile, "SETTINGS.ROUNDINFOTOFILE_ADD", toolTip);
            LANG.C(linkSetDamageInterval, "SETTINGS.OSCDAMAGEDEVENT_EDIT", toolTip);

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

        private static Font? GroupLabelFont { get; set; }
        // Subscribe to events on load
        private void SettingsWindow_Load(object sender, EventArgs e) {
            BindControlsRecursive(Controls);
            LocalizeContent();

            // Clone font for group labels
            if (GroupLabelFont == null) {
                GroupLabelFont = new Font(labelGroupGeneral.Font.FontFamily, 12, FontStyle.Bold);
            }

            Logger.Debug("FONT: " + labelGroupGeneral.Font.FontFamily.Name);

            // Check font size for group labels
            labelGroupGeneral.Font = GroupLabelFont;
            labelGroupDiscord.Font = GroupLabelFont;
            labelGroupNotifications.Font = GroupLabelFont;
            labelGroupFormat.Font = GroupLabelFont;
            labelGroupStyle.Font = GroupLabelFont;
            labelGroupOSC.Font = GroupLabelFont;

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
            checkDiscordPresence.CheckedChanged += CheckDiscordBackup_CheckedChanged;

            // OSC
            checkOSCEnabled.CheckedChanged += checkOSCEnabled_CheckedChanged;
            checkOSCEnabled_CheckedChanged(null, EventArgs.Empty);
            checkSendChatbox.CheckedChanged += checkSendChatbox_CheckedChanged;
            checkOSCSendColor.CheckedChanged += CheckOSCSendColor_CheckedChanged;

            // OBS
            checkRoundToFile.CheckedChanged += CheckRoundToFile_CheckedChanged;
            CheckRoundToFile_CheckedChanged(null, EventArgs.Empty);

            // Open RGB
            checkOpenRGBEnabled.CheckedChanged += CheckOpenRGBEnabled_CheckedChanged;
            CheckOpenRGBEnabled_CheckedChanged(null, EventArgs.Empty);

            FillLanguageBox();
        }

        private void CheckOpenRGBEnabled_CheckedChanged(object? sender, EventArgs e) {
            linkOpenRGB.Visible = checkOpenRGBEnabled.Checked;
            if (sender == null) return;

            if (checkOpenRGBEnabled.Checked) OpenRGBControl.Initialize();
            else OpenRGBControl.DeInitialize();
        }

        private void linkOpenRGB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            MainWindow.OpenExternalLink(RGBProfile.FileName);
        }

        private void RoundInfoTemplate_Control_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) {
            RoundInfoTemplate? template = (RoundInfoTemplate?)((LinkLabel?)sender)?.DataContext;
            if (template == null) return;

            if (e.Button == MouseButtons.Right) RoundInfoTemplate_OnDelete(template);
            else if (e.Button == MouseButtons.Middle) MainWindow.OpenExternalLink(Path.GetDirectoryName(template.FilePath));
            else RoundInfoTemplate_OnEdit(template);
        }
        private void RoundInfoTemplate_OnDelete(RoundInfoTemplate template) {
            DialogResult dRes = MessageBox.Show(LANG.S("SETTINGS.ROUNDINFOTOFILE_DELETE.MESSAGE", template.FileName) ?? $"Are you sure you want to delete this template file?\nFile Name: {template.FileName}", LANG.S("SETTINGS.ROUNDINFOTOFILE_DELETE.TITLE", template.FileName) ?? $"Deleting Template: {template.FileName}", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dRes == DialogResult.OK) {
                int index = Array.IndexOf(Settings.Get.RoundInfoTemplates, template);
                if (index < 0) return;

                var templates = new RoundInfoTemplate[Settings.Get.RoundInfoTemplates.Length - 1];
                for (int i = 0, j = 0; i < Settings.Get.RoundInfoTemplates.Length; i++) {
                    if (i == index) continue;

                    templates[j] = Settings.Get.RoundInfoTemplates[i];
                    j++;
                }
                Settings.Get.RoundInfoTemplates = templates;
                Settings.Export();

                CheckRoundToFile_CheckedChanged(null, EventArgs.Empty);
            }
        }
        private void RoundInfoTemplate_OnEdit(RoundInfoTemplate template) {
            string value = template.Template;
            EditResult show = EditWindow.Show(value, LANG.S("SETTINGS.ROUNDINFOTOFILE_EDIT.TITLE", template.FileName) ?? $"Editing: {template.FileName}", this, true, false, true);
            if (show.Accept) {
                if (string.IsNullOrWhiteSpace(show.Text)) {
                    DialogResult dRes = MessageBox.Show(LANG.S("SETTINGS.ROUNDINFOTOFILE_DELETE.MESSAGE", template.FileName) ?? $"Are you sure you want to delete this template file?\nFile Name: {template.FileName}", LANG.S("SETTINGS.ROUNDINFOTOFILE_DELETE.TITLE", template.FileName) ?? $"Deleting Template: {template.FileName}", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (dRes == DialogResult.OK) RoundInfoTemplate_OnDelete(template);
                } else {
                    template.Template = show.Text;
                    Settings.Export();
                }
            }

            CheckRoundToFile_CheckedChanged(null, EventArgs.Empty);
        }
        private void CheckRoundToFile_CheckedChanged(object? sender, EventArgs e) {
            int length = Settings.Get.RoundInfoTemplates.Length;
            int count = flowRoundInfoFiles.Controls.Count;
            int max = Math.Max(length, count);
            for (int i = 0; i < max; i++) {
                LinkLabel control;
                if (i < count) {
                    control = (LinkLabel)flowRoundInfoFiles.Controls[i];
                } else {
                    control = new LinkLabel() {
                        VisitedLinkColor = Color.Gainsboro,
                        ActiveLinkColor = Color.White,
                        LinkColor = Color.Gainsboro,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        UseMnemonic = false,
                        Margin = new Padding(0, 0, 5, 3),
                        LinkBehavior = LinkBehavior.HoverUnderline
                    };
                    control.LinkClicked += RoundInfoTemplate_Control_LinkClicked;
                    control.BorderStyle = BorderStyle.FixedSingle;

                    flowRoundInfoFiles.Controls.Add(control);
                }

                if (i < length) {
                    RoundInfoTemplate template = Settings.Get.RoundInfoTemplates[i];

                    if (control.Visible = checkRoundToFile.Checked && i < length) {
                        control.DataContext = template;
                        control.Text = template.FileName;
                        toolTip.SetToolTip(control, template.FilePath);
                    }
                } else control.Visible = false;
            }

            linkAddInfoFile.Visible = checkRoundToFile.Checked;
            flowRoundInfoFilePanel.BorderStyle = checkRoundToFile.Checked ? BorderStyle.FixedSingle : BorderStyle.None;

            flowRoundInfoFilePanel.Update();
        }

        private void linkAddInfoFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            EditResult show = EditWindow.Show(string.Empty, LANG.S("SETTINGS.ROUNDINFOTOFILE_ADD.TITLE") ?? "Create New Template", this, false, true);
            if (!show.Accept || string.IsNullOrEmpty(show.Text)) return;

            string template = show.Text;
            string filePath = string.Empty;

            using (SaveFileDialog saveFileDialog = new() { FileName = "ton_new_template.txt", Title = "Save Template File", Filter = "Text File|*.txt" }) {
                filePath = saveFileDialog.ShowDialog() == DialogResult.OK ? saveFileDialog.FileName : string.Empty;
            }

            if (string.IsNullOrEmpty(filePath)) return;

            if (Settings.Get.RoundInfoTemplates.Any(t => t.FilePath == filePath)) {
                _ = MessageBox.Show(LANG.S("SETTINGS.ROUNDINFOTOFILE_FILE_EXISTS.MESSAGE", Path.GetFileName(filePath)), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Debug("Selected File Path: " + filePath);

            // Add to settings file
            RoundInfoTemplate infoTemplate = new RoundInfoTemplate(filePath, template);
            if (!infoTemplate.HasKeys) {
                _ = MessageBox.Show(LANG.S("SETTINGS.ROUNDINFOTOFILE_NOKEYS.MESSAGE", Path.GetFileName(filePath)), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RoundInfoTemplate[] temp = new RoundInfoTemplate[Settings.Get.RoundInfoTemplates.Length + 1];
            int index = temp.Length - 1;
            temp[index] = infoTemplate;
            Array.Copy(Settings.Get.RoundInfoTemplates, temp, index);
            Settings.Get.RoundInfoTemplates = temp;
            Settings.Export();

            infoTemplate.WriteToFile(true);
            Logger.Debug("Added new template: " + infoTemplate.FilePath);
            CheckRoundToFile_CheckedChanged(null, EventArgs.Empty);
        }

        private void CheckOSCSendColor_CheckedChanged(object? sender, EventArgs e) {
            if (checkOSCSendColor.Checked) LilOSC.IsDirty = true;
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
            if (sender == checkDiscordBackup) {
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
            } else { // Is the rich presence one
                if (checkDiscordPresence.Checked) DSRichPresence.Initialize();
                else DSRichPresence.Deinitialize();
            }
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

        private void checkSendChatbox_CheckedChanged(object? sender, EventArgs e) {
            if (checkSendChatbox.Checked) StatsWindow.UpdateChatboxContent();
            else LilOSC.SetChatboxMessage(string.Empty);
        }

        private void linkEditChatbox_Click(object sender, LinkLabelLinkClickedEventArgs e) {
            string template = Settings.Get.OSCMessageInfoTemplate.Template;
            EditResult edit = EditWindow.Show(template, LANG.S("SETTINGS.OSCSENDCHATBOX.TITLE") ?? "Chatbox Message Template", this, handleNewLine: true);
            if (edit.Accept) {
                Settings.Get.OSCMessageInfoTemplate.Template = string.IsNullOrEmpty(edit.Text) ? Settings.Default.OSCMessageInfoTemplate.Template : edit.Text;
                Settings.Export();

                StatsWindow.UpdateChatboxContent();
            }
        }

        private void linkSetDamageInterval_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            int original = Settings.Get.OSCDamagedInterval;
            string value = original.ToString();
            EditResult show = EditWindow.Show(value, LANG.S("SETTINGS.OSCDAMAGEDEVENT.TITLE") ?? "Set Damage Interval", this);

            if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result) && result != original) {
                Settings.Get.OSCDamagedInterval = result;
                Settings.Export();
            }
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
            if (checkOSCEnabled.Checked && sender != null) LilOSC.SendData(true);
            checkOSCSendDamage.ForeColor = checkOSCSendColor.ForeColor =
                checkOSCEnabled.Checked ? Color.White : Color.Gray;
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
                Logger.Log("Changing language to: " + langKey);
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
                    case Panel p:
                        BindControlsRecursive(p.Controls);
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
