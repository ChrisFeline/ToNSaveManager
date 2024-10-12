using System;
using System.Diagnostics;
using System.Security.Policy;
using ToNSaveManager.Extensions;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Stats;
using ToNSaveManager.Utils;
using ToNSaveManager.Utils.API;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Utils.LogParser;
using ToNSaveManager.Utils.OpenRGB;
using Timer = System.Windows.Forms.Timer;

namespace ToNSaveManager.Windows
{
    public partial class SettingsWindow : Form {
        #region Initialization
        internal static SettingsWindow? Instance;

        public SettingsWindow() {
            InitializeComponent();
            languageSelectBox.FixItemHeight(true);

#if DEBUG
            linkCopyLogs.Visible = true;
            linkOpenLogs.Visible = true;

            var ev = (object? e, LinkLabelLinkClickedEventArgs a) => {
                bool isOpen = e == linkOpenLogs;
                if (ToNLogContext.Instance == null) return;

                string fullPath = isOpen ? ToNLogContext.Instance.FilePath : Path.GetFullPath("instance_logs.log");

                if (!isOpen) {
                    string instanceLogs = ToNLogContext.Instance.GetRoomLogs();
                    File.WriteAllText(fullPath, instanceLogs);

                    System.Collections.Specialized.StringCollection paths = [fullPath];
                    Clipboard.SetFileDropList(paths);

                    MessageBox.Show("Instance logs file created and copied to the clipboard.\nUse PASTE on Discord to send the log file.", "Instance Logs Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                using (Process.Start("explorer.exe", "/select, \"" + fullPath + "\"")) {
                    Logger.Debug("Opened file in explorer: " + fullPath);
                }
            };
            var hd = new LinkLabelLinkClickedEventHandler(ev);
            linkCopyLogs.LinkClicked += hd;
            linkOpenLogs.LinkClicked += hd;
#endif
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
        private readonly string[] ColorFormatLabels = [ "HSV", "RGB", "HSL", "RGB32" ];
        private static string ColorFormatTooltip = "Sends the current Terror color represented as {3}.\nColor will be sent as 3 FLOAT parameters:\n- {0}\n- {1}\n- {2}";

        internal void LocalizeContent() {
            LANG.C(this, "MAIN.SETTINGS");

            foreach (KeyValuePair<string, Control> pair in LocalizedControlCache) {
                LANG.C(pair.Value, pair.Key, toolTip);
                // if (pair.Key == "SETTINGS.PLAYAUDIO") PostAudioLocationSet();
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
            LANG.C(linkOpenRGB, "SETTINGS.OPENRGB_JSONFILE", toolTip);
            LANG.C(linkLogUpdateRate, "SETTINGS.LOGUPDATERATE", toolTip);
            LANG.C(linkAutoNoteEdit, "SETTINGS.SAVEROUNDNOTE_EDIT", toolTip);

            LANG.C(linkEditDiscordDetails, "SETTINGS.DISCORDRICHPRESENCE_EDIT", toolTip);
            LANG.C(linkEditDiscordState, "SETTINGS.DISCORDRICHPRESENCE_EDIT", toolTip);
            LANG.C(linkEditDiscordImage, "SETTINGS.DISCORDRICHPRESENCE_EDIT", toolTip);
            LANG.C(linkEditDiscordIcon, "SETTINGS.DISCORDRICHPRESENCE_EDIT", toolTip);
            LANG.C(linkEditDiscordStart, "SETTINGS.DISCORDRICHPRESENCE_EDIT", toolTip);

            LANG.C(linkSelectAudioSave, "SETTINGS.PLAYAUDIO_SELECT", toolTip);
            LANG.C(linkSelectAudioCopy, "SETTINGS.PLAYAUDIO_SELECT", toolTip);
            UpdatePlayLink();

            LANG.C(linkOSCFormat, "SETTINGS.OSCSENDCOLORFORMAT", toolTip);
            string? tt = LANG.S("SETTINGS.OSCSENDCOLOR.TT");
            if (tt != null) ColorFormatTooltip = tt;
            LinkOSCFormat_LinkClicked(null, null);

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

        private void UpdateAudioLink(LinkLabel audioLinkLabel, string? location) {
            LANG.C(audioLinkLabel, "SETTINGS.PLAYAUDIO_SELECT", toolTip);
            if (!string.IsNullOrEmpty(location)) {
                toolTip.SetToolTip(audioLinkLabel, LANG.S("SETTINGS.PLAYAUDIO_SELECT.ALT"));
                string fileName = Path.GetFileName(location);
                if (!string.IsNullOrEmpty(fileName)) audioLinkLabel.Text = $"({fileName})";
            }
        }
        private void UpdatePlayLink() {
            UpdateAudioLink(linkSelectAudioSave, Settings.Get.AudioLocation);
            UpdateAudioLink(linkSelectAudioCopy, Settings.Get.AudioCopyLocation);
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

            // Check font size for group labels
            labelGroupGeneral.Font = GroupLabelFont;
            labelGroupDiscord.Font = GroupLabelFont;
            labelGroupNotifications.Font = GroupLabelFont;
            labelGroupFormat.Font = GroupLabelFont;
            labelGroupStyle.Font = GroupLabelFont;
            labelGroupOSC.Font = GroupLabelFont;

            // Custom audio handling
            checkPlayAudioSave.CheckedChanged += CheckPlayAudio_CheckedChanged;
            checkPlayAudioCopy.CheckedChanged += CheckPlayAudio_CheckedChanged;
            CheckPlayAudio_CheckedChanged(null, EventArgs.Empty);

            linkSelectAudioSave.LinkClicked += LinkSelectAudio_LinkClicked;
            linkSelectAudioCopy.LinkClicked += LinkSelectAudio_LinkClicked;

            // Other notifications
            checkXSOverlay.CheckedChanged += CheckXSOverlay_CheckedChanged;
            // Refresh lists when time format changes
            check24Hour.CheckedChanged += TimeFormat_CheckedChanged;
            checkInvertMD.CheckedChanged += TimeFormat_CheckedChanged;
            checkShowSeconds.CheckedChanged += TimeFormat_CheckedChanged;
            checkShowDate.CheckedChanged += TimeFormat_CheckedChanged;
            checkShowTime.CheckedChanged += TimeFormat_CheckedChanged;
            // Refresh list when style is changed
            checkColorObjectives.CheckedChanged += CheckColorObjectives_CheckedChanged;

            // Round info
            checkShowWinLose.CheckedChanged += TimeFormat_CheckedChanged;
            checkSaveTerrors.CheckedChanged += checkSaveTerrors_CheckedChanged;
            checkSaveTerrors_CheckedChanged(checkSaveTerrors, e);

            // Discord Backups
            checkDiscordBackup.CheckedChanged += CheckDiscordBackup_CheckedChanged;
            checkDiscordPresence.CheckedChanged += CheckDiscordBackup_CheckedChanged;
            CheckDiscordBackup_CheckedChanged(null, EventArgs.Empty);

            // Register all the events
            checkDiscordCustomDetails.CheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            checkDiscordCustomState.CheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            checkDiscordCustomIcon.CheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            checkDiscordCustomImageText.CheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            checkDiscordCustomStart.CheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            // Events for edit buttons
            linkEditDiscordDetails.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            linkEditDiscordState.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            linkEditDiscordIcon.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            linkEditDiscordImage.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            linkEditDiscordStart.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            // Call all the events just in case
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomDetails, EventArgs.Empty);
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomState, EventArgs.Empty);
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomIcon, EventArgs.Empty);
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomImageText, EventArgs.Empty);
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomStart, EventArgs.Empty);


            // OSC
            checkOSCEnabled.CheckedChanged += checkOSCEnabled_CheckedChanged;
            checkOSCEnabled_CheckedChanged(null, EventArgs.Empty);
            checkSendChatbox.CheckedChanged += checkSendChatbox_CheckedChanged;
            checkOSCSendColor.CheckedChanged += CheckOSCSendColor_CheckedChanged;
            linkOSCFormat.LinkClicked += LinkOSCFormat_LinkClicked;
            LinkOSCFormat_LinkClicked(null, null);

            // OBS
            checkRoundToFile.CheckedChanged += CheckRoundToFile_CheckedChanged;
            CheckRoundToFile_CheckedChanged(null, EventArgs.Empty);

            // Open RGB
            checkOpenRGBEnabled.CheckedChanged += CheckOpenRGBEnabled_CheckedChanged;
            // CheckOpenRGBEnabled_CheckedChanged(null, EventArgs.Empty);

            checkAutoCopy.CheckedChanged += checkAutoCopy_CheckedChanged;
            checkAutoCopy_CheckedChanged(null, EventArgs.Empty);

            checkWebSocketServer.CheckedChanged += CheckWebSocketServer_CheckedChanged;
            CheckWebSocketServer_CheckedChanged(null, EventArgs.Empty);

            FillLanguageBox();
        }

        private void LinkSelectAudio_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) {
            bool isLeft = e.Button == MouseButtons.Left;

            string? file = null;
            if (isLeft) {
                file = SelectAudioFile();
                if (string.IsNullOrEmpty(file)) return;
            }

            bool isSave = sender == linkSelectAudioSave;
            if (isSave) {
                Settings.Get.AudioLocation = file;
                NotificationManager.PlaySave();
            } else {
                Settings.Get.AudioCopyLocation = file;
                NotificationManager.PlayCopy();
            }

            Settings.Export();
            UpdatePlayLink();
        }

        private void LinkOSCFormat_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs? e) {
            int index = Settings.Get.OSCSendColorFormat;
            if (sender != null && e != null) {
                if (e.Button == MouseButtons.Left) {
                    index = (index + 1) % ColorFormatLabels.Length;
                } else if (e.Button == MouseButtons.Right) {
                    index = index - 1;
                    if (index < 0) index = ColorFormatLabels.Length - 1;
                }
                Settings.Get.OSCSendColorFormat = (byte)index;
                Settings.Export();
            }

            string label = ColorFormatLabels[index];
            labelOSCFormat.Text = label;

            string paramX, paramY, paramZ;
            switch (index) {
                default: // HSV
                    paramX = LilOSC.ParamTerrorColorH;
                    paramY = LilOSC.ParamTerrorColorS;
                    paramZ = LilOSC.ParamTerrorColorV;
                    break;
                case 1: // RGB
                    paramX = LilOSC.ParamTerrorColorR;
                    paramY = LilOSC.ParamTerrorColorG;
                    paramZ = LilOSC.ParamTerrorColorB;
                    break;
                case 2: // HSL
                    paramX = LilOSC.ParamTerrorColorH;
                    paramY = LilOSC.ParamTerrorColorS;
                    paramZ = LilOSC.ParamTerrorColorL;
                    break;
                case 3: // RGB32
                    paramX = LilOSC.ParamTerrorColorR;
                    paramY = LilOSC.ParamTerrorColorG;
                    paramZ = LilOSC.ParamTerrorColorB;
                    break;
            }

            string tag = index == 3 ? " (INT)" : " (FLOAT)";
            paramX += tag;
            paramY += tag;
            paramZ += tag;

            toolTip.SetToolTip(checkOSCSendColor, string.Format(ColorFormatTooltip, paramX, paramY, paramZ, label));

            flowTerrorColor.Refresh();
        }

        private void LinkEditDiscordDetails_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) {
            RoundInfoTemplate template;
            if (sender == linkEditDiscordDetails) template = Settings.Get.DiscordTemplateDetails;
            else if (sender == linkEditDiscordState) template = Settings.Get.DiscordTemplateState;
            else if (sender == linkEditDiscordImage) template = Settings.Get.DiscordTemplateImage;
            else if (sender == linkEditDiscordIcon) template = Settings.Get.DiscordTemplateIcon;
            else if (sender == linkEditDiscordStart) template = Settings.Get.DiscordTemplateStart;
            else template = Settings.Get.DiscordTemplateDetails;

            string value = template.Template;
            EditResult show = EditWindow.Show(value, LANG.S("SETTINGS.DISCORDRICHPRESENCE_EDIT.TITLE", template.FileName) ?? $"Edit Template", this, false, false, false, true);
            if (show.Accept) {
                if (string.IsNullOrWhiteSpace(show.Text))
                    template.Template = string.Empty;
                else template.Template = show.Text;

                Settings.Export();
            }
        }

        private void CheckDiscordCustomDetails_CheckedChanged(object? sender, EventArgs e) {
            CheckBox? checkBox = (CheckBox?)sender;
            Control? control = checkBox?.Parent;
            if (checkBox == null || control == null) return;

            foreach (Control c in control.Controls) {
                if (c is LinkLabel) c.Visible = checkBox.Checked;
            }
        }

        private void CheckWebSocketServer_CheckedChanged(object? sender, EventArgs e) {
            checkWebTrackerComp.Visible = checkWebSocketServer.Checked;
            WebSocketAPI.Initialize();
        }

        private void CheckOpenRGBEnabled_CheckedChanged(object? sender, EventArgs e) {
            if (sender == null) return;
            if (checkOpenRGBEnabled.Checked) OpenRGBControl.Initialize();
            else OpenRGBControl.DeInitialize();
        }

        private void linkOpenRGB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            RGBProfile.OpenFile();
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
            EditResult show = EditWindow.Show(value, LANG.S("SETTINGS.ROUNDINFOTOFILE_EDIT.TITLE", template.FileName) ?? $"Editing: {template.FileName}", this, true, false, true, true);
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
            EditResult show = EditWindow.Show(string.Empty, LANG.S("SETTINGS.ROUNDINFOTOFILE_ADD.TITLE") ?? "Create New Template", this, false, true, true);
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
            MainWindow.RefreshLists();
            MainWindow.ResetNotification();
        }

        private void TimeFormat_CheckedChanged(object? sender, EventArgs e) => MainWindow.RefreshLists();
        private void CheckXSOverlay_CheckedChanged(object? sender, EventArgs e) => MainWindow.SendXSNotification(true);
        private void CheckPlayAudio_CheckedChanged(object? sender, EventArgs e) {
            linkSelectAudioCopy.Visible = checkPlayAudioCopy.Checked;
            linkSelectAudioSave.Visible = checkPlayAudioSave.Checked;

            if (sender == null) return;

            CheckBox checkbox = (CheckBox)sender;
            bool isChecked = checkbox.Checked;

            if (!isChecked) {
                MainWindow.ResetNotification();
                return;
            }

            if (checkbox == checkPlayAudioSave)
                MainWindow.PlaySaveNotification();
            else MainWindow.PlayCopyNotification();
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
                if (sender != null) {
                    DSRichPresence.Initialize();
                }

                flowDiscordDetailsText.Visible = flowDiscordStateText.Visible =
                    flowDiscordImageText.Visible = flowDiscordIconText.Visible =
                    flowDiscordIconStart.Visible = checkDiscordPresence.Checked;
            }
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e) {
            if (Program.StartCheckForUpdate(true)) this.Close();
        }

        private void btnOpenData_Click(object sender, EventArgs e) {
            SaveData.OpenDataLocation();
        }

        private void checkSendChatbox_CheckedChanged(object? sender, EventArgs e) {
            if (checkSendChatbox.Checked) StatsWindow.UpdateChatboxContent();
            else LilOSC.SetChatboxMessage(string.Empty);
        }

        private void linkEditChatbox_Click(object sender, LinkLabelLinkClickedEventArgs e) {
            string template = Settings.Get.OSCMessageInfoTemplate.Template;
            EditResult edit = EditWindow.Show(template, LANG.S("SETTINGS.OSCSENDCHATBOX.TITLE") ?? "Chatbox Message Template", this, handleNewLine: true, insertKeyTemplate: true);
            if (edit.Accept) {
                Settings.Get.OSCMessageInfoTemplate.Template = string.IsNullOrEmpty(edit.Text) ? Settings.Default.OSCMessageInfoTemplate.Template : edit.Text;
                Settings.Export();

                StatsWindow.UpdateChatboxContent();
            }
        }

        private void linkAutoNoteEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            string template = Settings.Get.RoundNoteTemplate.Template;
            EditResult edit = EditWindow.Show(template, LANG.S("SETTINGS.SAVEROUNDNOTE.TITLE") ?? "Automatic Note Template", this, handleNewLine: true, insertKeyTemplate: true);
            if (edit.Accept) {
                Settings.Get.RoundNoteTemplate.Template = string.IsNullOrEmpty(edit.Text) ? Settings.Default.RoundNoteTemplate.Template : edit.Text;
                Settings.Export();
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

        private void linkLogUpdateRate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            int original = Settings.Get.LogUpdateRate;
            string value = original.ToString();
            EditResult show = EditWindow.Show(value, LANG.S("SETTINGS.LOGUPDATERATE.TITLE") ?? "Set Update Rate (Milliseconds)", this);

            if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result) && result != original) {
                Settings.Get.LogUpdateRate = Math.Clamp(result, 10, 5000);
                MainWindow.LogWatcher.Interval = Settings.Get.LogUpdateRate;
                Settings.Export();
            }
        }

        /// TODO: Move this function somewhere else
        private string? SelectAudioFile() {
            string? result = null;

            using (OpenFileDialog fileDialog = new OpenFileDialog()) {
                fileDialog.InitialDirectory = "./";
                fileDialog.Title = LANG.S("SETTINGS.PLAYAUDIO.TITLE") ?? "Select Custom Sound";
                fileDialog.Filter = "Waveform (*.wav)|*.wav";

                if (fileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName)) {
                    result = fileDialog.FileName;
                } else {
                    result = null;
                }
            }

            return result;
        }

        private void checkAutoCopy_CheckedChanged(object? sender, EventArgs e) {
            if (checkAutoCopy.Checked && sender != null) LilOSC.SendData(true);
            checkCopyOnOpen.ForeColor = checkCopyOnJoin.ForeColor = checkCopyOnSave.ForeColor =
                checkAutoCopy.Checked ? Color.White : Color.Gray;
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
        #endregion

    }
}
