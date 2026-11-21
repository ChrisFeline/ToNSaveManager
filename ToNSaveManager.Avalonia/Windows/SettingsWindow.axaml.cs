using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Stats;
using ToNSaveManager.Utils;
using ToNSaveManager.Utils.API;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Utils.JSPlugins.API;
using ToNSaveManager.Utils.LogParser;
using ToNSaveManager.Utils.OpenRGB;
using ToNSaveManager.Windows.Controls;

namespace ToNSaveManager.Windows {
    public partial class SettingsWindow : Window {
        #region Initialization
        internal static SettingsWindow? Instance;

        public SettingsWindow() {
            InitializeComponent();

#if DEBUG
            linkOpenLogs.IsVisible = true;
            linkCopyLogs.IsVisible = true;

            Action<LinkLabel, MouseButton> ev = async (link, button) => {
                bool isOpen = link == linkOpenLogs;
                if (ToNLogContext.Instance == null) return;

                string fullPath = isOpen ? ToNLogContext.Instance.FilePath : Path.GetFullPath("instance_logs.log");

                if (!isOpen) {
                    string instanceLogs = ToNLogContext.Instance.GetInstanceLogs();
                    File.WriteAllText(fullPath, instanceLogs);

                    // Can't copy files to the clipboard
                    await MessageBox.Show(this, "Instance logs file created:\n" + fullPath, "Instance Logs Debug", AlertButtons.OK, AlertIcon.Information);
                }

                MainWindow.OpenExternalLink(Path.GetDirectoryName(fullPath));
                Logger.Debug("Opened folder of: " + fullPath);
            };
            linkCopyLogs.LinkClicked += ev;
            linkOpenLogs.LinkClicked += ev;
#endif

            SettingsWindow_Load();
        }

        public static void Open(Window parent) {
            // Can't reopen a closed window
            if (Instance == null || Instance.IsClosed) Instance = new();
            Instance.ShowCentered(parent, true);
        }

        internal bool IsClosed { get; private set; }
        protected override void OnClosed(EventArgs e) {
            IsClosed = true;
            base.OnClosed(e);
            SettingsWindow_FormClosed();
        }
        #endregion

        #region Form Events
        private Dictionary<string, CheckBox> LocalizedControlCache = new Dictionary<string, CheckBox>();
        private readonly string[] ColorFormatLabels = ["HSV", "RGB", "HSL", "RGB32"];
        private static string ColorFormatTooltip = "Sends the current Terror color represented as {3}.\nColor will be sent as 3 FLOAT parameters:\n- {0}\n- {1}\n- {2}";

        internal void LocalizeContent() {
            this.Localize("MAIN.SETTINGS");

            foreach (KeyValuePair<string, CheckBox> pair in LocalizedControlCache) {
                pair.Value.Localize(pair.Key);
            }

            labelGroupGeneral.Localize("SETTINGS.GROUP.GENERAL");
            labelGroupDiscord.Localize("SETTINGS.GROUP.DISCORD");
            labelGroupNotifications.Localize("SETTINGS.GROUP.NOTIFICATIONS");
            labelGroupFormat.Localize("SETTINGS.GROUP.TIME_FORMAT");
            labelGroupStyle.Localize("SETTINGS.GROUP.STYLE");
            labelGroupOSC.Localize("SETTINGS.GROUP.OSC");

            // OSC LINKS
            linkEditChatbox.Localize("SETTINGS.OSCSENDCHATBOX_EDIT");
            linkSetDamageInterval.Localize("SETTINGS.OSCDAMAGEDEVENT_EDIT");
            linkSetDamageOutput.Localize("SETTINGS.OSCDAMAGEDEVENT_OUTPUT");
            linkSetMasterInterval.Localize("SETTINGS.OSCMASTERCHANGE_EDIT");
            linkEditDeathID.Localize("SETTINGS.OSCDEATHEVENT_EDIT");
            linkSetDeathDecay.Localize("SETTINGS.OSCDEATHEVENT_DECAY");
            linkSetDeathCooldown.Localize("SETTINGS.OSCDEATHEVENT_COOLDOWN");

            linkAddInfoFile.Localize("SETTINGS.ROUNDINFOTOFILE_ADD");
            linkOpenRGB.Localize("SETTINGS.OPENRGB_JSONFILE");
            linkLogUpdateRate.Localize("SETTINGS.LOGUPDATERATE");
            linkAutoNoteEdit.Localize("SETTINGS.SAVEROUNDNOTE_EDIT");

            linkEditDiscordDetails.Localize("SETTINGS.DISCORDRICHPRESENCE_EDIT");
            linkEditDiscordState.Localize("SETTINGS.DISCORDRICHPRESENCE_EDIT");
            linkEditDiscordImage.Localize("SETTINGS.DISCORDRICHPRESENCE_EDIT");
            linkEditDiscordIcon.Localize("SETTINGS.DISCORDRICHPRESENCE_EDIT");
            linkEditDiscordStart.Localize("SETTINGS.DISCORDRICHPRESENCE_EDIT");

            linkSelectAudioSave.Localize("SETTINGS.PLAYAUDIO_SELECT");
            linkSelectAudioCopy.Localize("SETTINGS.PLAYAUDIO_SELECT");
            UpdatePlayLink();

            linkOSCFormat.Localize("SETTINGS.OSCSENDCOLORFORMAT");
            string? tt = LANG.S("SETTINGS.OSCSENDCOLOR.TT");
            if (tt != null) ColorFormatTooltip = tt;
            LinkOSCFormat_LinkClicked(null, MouseButton.None);

            btnCheckForUpdates.Localize("SETTINGS.CHECK_UPDATE");
            btnOpenData.Localize("SETTINGS.OPEN_DATA_BTN");

            setDataLocationToolStripMenuItem.Localize("SETTINGS.CUSTOM_DATA_FOLDER");
            ctxItemPickFolder.Localize("SETTINGS.CUSTOM_DATA_PICK_FOLDER");
            ctxItemResetToDefault.Localize("SETTINGS.CUSTOM_DATA_RESET_DEFAULT");

            string? versionString = AppCore.Version?.ToString();
            if (!string.IsNullOrEmpty(versionString))
                ToolTip.SetTip(btnCheckForUpdates, LANG.S("SETTINGS.VERSION", versionString) ?? $"Current Version {versionString}");

            FlowDirection = LANG.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        private void UpdateAudioLink(LinkLabel audioLinkLabel, string? location) {
            audioLinkLabel.Localize("SETTINGS.PLAYAUDIO_SELECT");
            if (!string.IsNullOrEmpty(location)) {
                ToolTip.SetTip(audioLinkLabel, LANG.S("SETTINGS.PLAYAUDIO_SELECT.ALT"));
                string fileName = Path.GetFileName(location);
                if (!string.IsNullOrEmpty(fileName)) audioLinkLabel.Text = $"({fileName})";
            }
        }
        private void UpdatePlayLink() {
            UpdateAudioLink(linkSelectAudioSave, Settings.Get.AudioLocation);
            UpdateAudioLink(linkSelectAudioCopy, Settings.Get.AudioCopyLocation);
        }

        // Subscribe to events on load
        private void SettingsWindow_Load() {
            BindControlsRecursive();
            LocalizeContent();
            btnCheckForUpdates.IsEnabled = UpdateChecker.Enabled;

            // Custom audio handling
            checkPlayAudioSave.IsCheckedChanged += CheckPlayAudio_CheckedChanged;
            checkPlayAudioCopy.IsCheckedChanged += CheckPlayAudio_CheckedChanged;
            CheckPlayAudio_CheckedChanged(null, new RoutedEventArgs());

            linkSelectAudioSave.LinkClicked += LinkSelectAudio_LinkClicked;
            linkSelectAudioCopy.LinkClicked += LinkSelectAudio_LinkClicked;

            // Other notifications
            checkXSOverlay.IsCheckedChanged += CheckXSOverlay_CheckedChanged;
            // Refresh lists when time format changes
            check24Hour.IsCheckedChanged += TimeFormat_CheckedChanged;
            checkInvertMD.IsCheckedChanged += TimeFormat_CheckedChanged;
            checkShowSeconds.IsCheckedChanged += TimeFormat_CheckedChanged;
            checkShowDate.IsCheckedChanged += TimeFormat_CheckedChanged;
            checkShowTime.IsCheckedChanged += TimeFormat_CheckedChanged;
            // Refresh list when style is changed
            checkColorObjectives.IsCheckedChanged += CheckColorObjectives_CheckedChanged;

            // Round info
            checkShowWinLose.IsCheckedChanged += TimeFormat_CheckedChanged;
            checkSaveTerrors.IsCheckedChanged += checkSaveTerrors_CheckedChanged;
            checkSaveTerrors_CheckedChanged(checkSaveTerrors, new RoutedEventArgs());

            // Discord Backups
            checkDiscordBackup.IsCheckedChanged += CheckDiscordBackup_CheckedChanged;
            checkDiscordPresence.IsCheckedChanged += CheckDiscordBackup_CheckedChanged;
            CheckDiscordBackup_CheckedChanged(null, new RoutedEventArgs());

            // Register all the events
            checkDiscordCustomDetails.IsCheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            checkDiscordCustomState.IsCheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            checkDiscordCustomIcon.IsCheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            checkDiscordCustomImageText.IsCheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            checkDiscordCustomStart.IsCheckedChanged += CheckDiscordCustomDetails_CheckedChanged;
            // Events for edit buttons
            linkEditDiscordDetails.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            linkEditDiscordState.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            linkEditDiscordIcon.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            linkEditDiscordImage.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            linkEditDiscordStart.LinkClicked += LinkEditDiscordDetails_LinkClicked;
            // Call all the events just in case
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomDetails, new RoutedEventArgs());
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomState, new RoutedEventArgs());
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomIcon, new RoutedEventArgs());
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomImageText, new RoutedEventArgs());
            CheckDiscordCustomDetails_CheckedChanged(checkDiscordCustomStart, new RoutedEventArgs());

            // OSC
            checkOSCEnabled.IsCheckedChanged += checkOSCEnabled_CheckedChanged;
            checkOSCEnabled_CheckedChanged(null, new RoutedEventArgs());
            checkOSCEnabled.AddHandler(PointerReleasedEvent, checkOSCEnabled_MouseUp, RoutingStrategies.Bubble, true);
            checkSendChatbox.IsCheckedChanged += checkSendChatbox_CheckedChanged;
            checkOSCSendColor.IsCheckedChanged += CheckOSCSendColor_CheckedChanged;
            linkOSCFormat.LinkClicked += LinkOSCFormat_LinkClicked;
            LinkOSCFormat_LinkClicked(null, MouseButton.None);
            linkEditDeathID.LinkClicked += LinkEditDeathID_LinkClicked;
            linkSetDeathDecay.LinkClicked += LinkEditDeathID_LinkClicked;
            linkSetDeathCooldown.LinkClicked += LinkEditDeathID_LinkClicked;
            checkOSCDeathEvent.IsCheckedChanged += CheckOSCDeathEvent_CheckedChanged;
            linkSetDamageOutput.LinkClicked += linkSetDamageInterval_LinkClicked;
            linkSetDamageInterval.LinkClicked += linkSetDamageInterval_LinkClicked;
            linkSetMasterInterval.LinkClicked += linkSetMasterInterval_LinkClicked;
            linkEditChatbox.LinkClicked += linkEditChatbox_Click;

            // OBS
            checkRoundToFile.IsCheckedChanged += CheckRoundToFile_CheckedChanged;
            CheckRoundToFile_CheckedChanged(null, new RoutedEventArgs());
            linkAddInfoFile.LinkClicked += linkAddInfoFile_LinkClicked;

            // Open RGB
            checkOpenRGBEnabled.IsCheckedChanged += CheckOpenRGBEnabled_CheckedChanged;
            linkOpenRGB.LinkClicked += linkOpenRGB_LinkClicked;

            checkAutoCopy.IsCheckedChanged += checkAutoCopy_CheckedChanged;
            checkAutoCopy_CheckedChanged(null, new RoutedEventArgs());

            checkWebSocketServer.IsCheckedChanged += CheckWebSocketServer_CheckedChanged;
            CheckWebSocketServer_CheckedChanged(null, new RoutedEventArgs());

            linkLogUpdateRate.LinkClicked += linkLogUpdateRate_LinkClicked;
            linkAutoNoteEdit.LinkClicked += linkAutoNoteEdit_LinkClicked;

            languageSelectBox.AddHandler(DragDrop.DragEnterEvent, languageSelect_DragEnter);
            languageSelectBox.AddHandler(DragDrop.DropEvent, languageSelect_DragDrop);

            FillLanguageBox();
        }

        private void CheckOSCDeathEvent_CheckedChanged(object? sender, RoutedEventArgs e) {
            if (checkOSCDeathEvent.IsChecked == true && (Settings.Get.OSCDeathIDs == null || Settings.Get.OSCDeathIDs.Length == 0)) {
                LinkEditDeathID_LinkClicked(linkEditDeathID, MouseButton.Left);
            }
        }

        private async void LinkEditDeathID_LinkClicked(LinkLabel? sender, MouseButton button) {
            if (sender == null) return;

            if (sender == linkEditDeathID) {
                string names = Settings.Get.OSCDeathIDs != null && Settings.Get.OSCDeathIDs.Length > 0 ? string.Join(',', Settings.Get.OSCDeathIDs) : string.Empty;
                EditResult edit = await EditWindow.Show(names, LANG.S("SETTINGS.OSCDEATHEVENT.TITLE") ?? "Set Name List", this);
                if (edit.Accept) {
                    names = edit.Text;
                    Settings.Get.OSCDeathIDs = names.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                    Settings.Export();
                }
            }

            if (sender == linkSetDeathDecay) {
                int original = Settings.Get.OSCDeathDecay;
                string value = original.ToString();
                EditResult show = await EditWindow.Show(value, LANG.S("SETTINGS.OSCDEATHEVENT_DECAY.TITLE") ?? "Set Decay Time", this);

                if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result) && result != original) {
                    Settings.Get.OSCDeathDecay = Math.Max(result, 10);
                    Settings.Export();
                } else if (show.Accept && string.IsNullOrEmpty(show.Text)) {
                    Settings.Get.OSCDeathDecay = Settings.Default.OSCDeathDecay;
                    Settings.Export();
                }
            }

            if (sender == linkSetDeathCooldown) {
                int original = Settings.Get.OSCDeathCooldown;
                string value = original.ToString();
                EditResult show = await EditWindow.Show(value, LANG.S("SETTINGS.OSCDEATHEVENT_COOLDOWN.TITLE") ?? "Set Cooldown Time", this);

                if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result) && result != original) {
                    Settings.Get.OSCDeathCooldown = Math.Max(result, 10);
                    Settings.Export();
                } else if (show.Accept && string.IsNullOrEmpty(show.Text)) {
                    Settings.Get.OSCDeathCooldown = Settings.Default.OSCDeathCooldown;
                    Settings.Export();
                }
            }
        }

        private async void LinkSelectAudio_LinkClicked(LinkLabel sender, MouseButton button) {
            bool isLeft = button == MouseButton.Left;

            string? file = null;
            if (isLeft) {
                file = await SelectAudioFile();
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

        private void LinkOSCFormat_LinkClicked(LinkLabel? sender, MouseButton button) {
            int index = Settings.Get.OSCSendColorFormat;
            if (sender != null) {
                if (button == MouseButton.Left) {
                    index = (index + 1) % ColorFormatLabels.Length;
                } else if (button == MouseButton.Right) {
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

            ToolTip.SetTip(checkOSCSendColor, string.Format(ColorFormatTooltip, paramX, paramY, paramZ, label));
        }

        private async void LinkEditDiscordDetails_LinkClicked(LinkLabel sender, MouseButton button) {
            RoundInfoTemplate template;
            if (sender == linkEditDiscordDetails) template = Settings.Get.DiscordTemplateDetails;
            else if (sender == linkEditDiscordState) template = Settings.Get.DiscordTemplateState;
            else if (sender == linkEditDiscordImage) template = Settings.Get.DiscordTemplateImage;
            else if (sender == linkEditDiscordIcon) template = Settings.Get.DiscordTemplateIcon;
            else if (sender == linkEditDiscordStart) template = Settings.Get.DiscordTemplateStart;
            else template = Settings.Get.DiscordTemplateDetails;

            string value = template.Template;
            EditResult show = await EditWindow.Show(value, LANG.S("SETTINGS.DISCORDRICHPRESENCE_EDIT.TITLE", template.FileName) ?? $"Edit Template", this, false, false, false, true);
            if (show.Accept) {
                if (string.IsNullOrWhiteSpace(show.Text))
                    template.Template = string.Empty;
                else template.Template = show.Text;

                Settings.Export();
            }
        }

        private void CheckDiscordCustomDetails_CheckedChanged(object? sender, RoutedEventArgs e) {
            CheckBox? checkBox = sender as CheckBox;
            Panel? control = checkBox?.Parent as Panel;
            if (checkBox == null || control == null) return;

            foreach (Control c in control.Children) {
                if (c is LinkLabel) c.IsVisible = checkBox.IsChecked == true;
            }
        }

        private void CheckWebSocketServer_CheckedChanged(object? sender, RoutedEventArgs e) {
            checkWebTrackerComp.IsVisible = checkWebSocketServer.IsChecked == true;
            WebSocketAPI.Initialize();
        }

        private void CheckOpenRGBEnabled_CheckedChanged(object? sender, RoutedEventArgs e) {
            if (sender == null) return;
            if (checkOpenRGBEnabled.IsChecked == true) OpenRGBControl.Initialize();
            else OpenRGBControl.DeInitialize();
        }

        private void linkOpenRGB_LinkClicked(LinkLabel sender, MouseButton button) {
            RGBProfile.EnsureFile();
            MainWindow.OpenExternalLink(RGBProfile.FilePath);
        }

        private void RoundInfoTemplate_Control_LinkClicked(LinkLabel sender, MouseButton button) {
            RoundInfoTemplate? template = sender.DataContext as RoundInfoTemplate;
            if (template == null) return;

            if (button == MouseButton.Right) RoundInfoTemplate_OnDelete(template);
            else if (button == MouseButton.Middle) MainWindow.OpenExternalLink(Path.GetDirectoryName(template.FilePath));
            else RoundInfoTemplate_OnEdit(template);
        }
        private async void RoundInfoTemplate_OnDelete(RoundInfoTemplate template) {
            AlertResult dRes = await MessageBox.Show(this, LANG.S("SETTINGS.ROUNDINFOTOFILE_DELETE.MESSAGE", template.FileName) ?? $"Are you sure you want to delete this template file?\nFile Name: {template.FileName}", LANG.S("SETTINGS.ROUNDINFOTOFILE_DELETE.TITLE", template.FileName) ?? $"Deleting Template: {template.FileName}", AlertButtons.OKCancel, AlertIcon.Warning);
            if (dRes == AlertResult.OK) {
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

                CheckRoundToFile_CheckedChanged(null, new RoutedEventArgs());
            }
        }
        private async void RoundInfoTemplate_OnEdit(RoundInfoTemplate template) {
            string value = template.Template;
            EditResult show = await EditWindow.Show(value, LANG.S("SETTINGS.ROUNDINFOTOFILE_EDIT.TITLE", template.FileName) ?? $"Editing: {template.FileName}", this, true, false, true, true);
            if (show.Accept) {
                if (string.IsNullOrWhiteSpace(show.Text)) {
                    AlertResult dRes = await MessageBox.Show(this, LANG.S("SETTINGS.ROUNDINFOTOFILE_DELETE.MESSAGE", template.FileName) ?? $"Are you sure you want to delete this template file?\nFile Name: {template.FileName}", LANG.S("SETTINGS.ROUNDINFOTOFILE_DELETE.TITLE", template.FileName) ?? $"Deleting Template: {template.FileName}", AlertButtons.OKCancel, AlertIcon.Warning);
                    if (dRes == AlertResult.OK) RoundInfoTemplate_OnDelete(template);
                } else {
                    template.Template = show.Text;
                    Settings.Export();
                }
            }

            CheckRoundToFile_CheckedChanged(null, new RoutedEventArgs());
        }
        private void CheckRoundToFile_CheckedChanged(object? sender, RoutedEventArgs e) {
            bool isChecked = checkRoundToFile.IsChecked == true;
            int length = Settings.Get.RoundInfoTemplates.Length;
            int count = flowRoundInfoFiles.Children.Count;
            int max = Math.Max(length, count);
            for (int i = 0; i < max; i++) {
                LinkLabel control;
                if (i < count) {
                    control = (LinkLabel)flowRoundInfoFiles.Children[i];
                } else {
                    control = new LinkLabel() {
                        Margin = new Avalonia.Thickness(0, 0, 5, 3),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
                    };
                    control.LinkClicked += RoundInfoTemplate_Control_LinkClicked;

                    flowRoundInfoFiles.Children.Add(control);
                }

                if (i < length) {
                    RoundInfoTemplate template = Settings.Get.RoundInfoTemplates[i];

                    if (control.IsVisible = isChecked && i < length) {
                        control.DataContext = template;
                        control.Text = template.FileName;
                        ToolTip.SetTip(control, template.FilePath);
                    }
                } else control.IsVisible = false;
            }

            linkAddInfoFile.IsVisible = isChecked;
            flowRoundInfoFilePanel.BorderThickness = new Avalonia.Thickness(isChecked ? 1 : 0);
        }

        private async void linkAddInfoFile_LinkClicked(LinkLabel sender, MouseButton button) {
            EditResult show = await EditWindow.Show(string.Empty, LANG.S("SETTINGS.ROUNDINFOTOFILE_ADD.TITLE") ?? "Create New Template", this, false, true, true);
            if (!show.Accept || string.IsNullOrEmpty(show.Text)) return;

            string template = show.Text;

            IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {
                SuggestedFileName = "ton_new_template.txt",
                Title = "Save Template File",
                FileTypeChoices = [new FilePickerFileType("Text File") { Patterns = ["*.txt"] }]
            });
            string filePath = file?.TryGetLocalPath() ?? string.Empty;

            if (string.IsNullOrEmpty(filePath)) return;

            if (Settings.Get.RoundInfoTemplates.Any(t => t.FilePath == filePath)) {
                _ = await MessageBox.Show(this, LANG.S("SETTINGS.ROUNDINFOTOFILE_FILE_EXISTS.MESSAGE", Path.GetFileName(filePath)) ?? string.Empty, string.Empty, AlertButtons.OK, AlertIcon.Error);
                return;
            }

            Logger.Debug("Selected File Path: " + filePath);

            // Add to settings file
            RoundInfoTemplate infoTemplate = new RoundInfoTemplate(filePath, template);
            if (!infoTemplate.HasKeys) {
                _ = await MessageBox.Show(this, LANG.S("SETTINGS.ROUNDINFOTOFILE_NOKEYS.MESSAGE", Path.GetFileName(filePath)) ?? string.Empty, string.Empty, AlertButtons.OK, AlertIcon.Error);
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
            CheckRoundToFile_CheckedChanged(null, new RoutedEventArgs());
        }

        private void CheckOSCSendColor_CheckedChanged(object? sender, RoutedEventArgs e) {
            if (checkOSCSendColor.IsChecked == true) LilOSC.IsDirty = true;
        }

        private void SettingsWindow_FormClosed() {
            MainWindow.RefreshLists();
            MainWindow.ResetNotification();
        }

        private void TimeFormat_CheckedChanged(object? sender, RoutedEventArgs e) => MainWindow.RefreshLists();
        private void CheckXSOverlay_CheckedChanged(object? sender, RoutedEventArgs e) => XSOverlay.SendSaveNotification(true);
        private void CheckPlayAudio_CheckedChanged(object? sender, RoutedEventArgs e) {
            linkSelectAudioCopy.IsVisible = checkPlayAudioCopy.IsChecked == true;
            linkSelectAudioSave.IsVisible = checkPlayAudioSave.IsChecked == true;

            if (sender == null) return;

            CheckBox checkbox = (CheckBox)sender;
            bool isChecked = checkbox.IsChecked == true;

            if (!isChecked) {
                MainWindow.ResetNotification();
                return;
            }

            if (checkbox == checkPlayAudioSave)
                MainWindow.PlaySaveNotification();
            else MainWindow.PlayCopyNotification();
        }

        private void checkSaveTerrors_CheckedChanged(object? sender, RoutedEventArgs e) {
            checkSaveTerrorsNote.Foreground = WindowExtensions.GetBrush(checkSaveTerrors.IsChecked == true ? "ListFore" : "Muted");
            checkShowWinLose.Foreground = checkSaveTerrorsNote.Foreground;
            TimeFormat_CheckedChanged(sender, e);
        }

        private async void CheckDiscordBackup_CheckedChanged(object? sender, RoutedEventArgs e) {
            if (sender == checkDiscordBackup) {
                if (checkDiscordBackup.IsChecked == true) {
                    string url = Settings.Get.DiscordWebhookURL ?? string.Empty;
                    EditResult edit = await EditWindow.Show(Settings.Get.DiscordWebhookURL ?? string.Empty, LANG.S("SETTINGS.DISCORDWEBHOOK.TITLE") ?? "Discord Webhook URL", this);
                    if (edit.Accept && !edit.Text.Equals(url, StringComparison.Ordinal)) {
                        url = edit.Text.Trim();

                        if (!string.IsNullOrWhiteSpace(url)) {
                            bool valid = DSWebHook.ValidateURL(url);

                            if (valid) {
                                Settings.Get.DiscordWebhookURL = url;
                                Settings.Export();
                            } else {
                                await MessageBox.Show(this, LANG.S("SETTINGS.DISCORDWEBHOOKINVALID") ?? "The URL your provided does not match a discord webhook url.\n\nMake sure you created your webhook and copied the url correctly.", LANG.S("SETTINGS.DISCORDWEBHOOKINVALID.TITLE") ?? "Invalid Webhook URL", AlertButtons.OK, AlertIcon.Error);
                            }
                        } else {
                            Settings.Get.DiscordWebhookURL = null;
                        }
                    }

                    if (string.IsNullOrEmpty(Settings.Get.DiscordWebhookURL)) checkDiscordBackup.IsChecked = false;
                }

                MainWindow.Instance?.SetBackupButton(checkDiscordBackup.IsChecked == true);
            } else { // Is the rich presence one
                if (sender != null) {
                    DSRichPresence.Initialize();
                }

                flowDiscordDetailsText.IsVisible = flowDiscordStateText.IsVisible =
                    flowDiscordImageText.IsVisible = flowDiscordIconText.IsVisible =
                    flowDiscordIconStart.IsVisible = checkDiscordPresence.IsChecked == true;
            }
        }

        private async void btnCheckForUpdates_Click(object? sender, RoutedEventArgs e) {
            btnCheckForUpdates.IsEnabled = false;
            await MainWindow.CheckForUpdate(this, true);
            btnCheckForUpdates.IsEnabled = true;
        }

        private void btnOpenData_Click(object? sender, RoutedEventArgs e) {
            MainWindow.OpenExternalLink(Path.GetDirectoryName(SaveData.Destination) ?? string.Empty);
        }

        private void checkSendChatbox_CheckedChanged(object? sender, RoutedEventArgs e) {
            if (checkSendChatbox.IsChecked == true) LilOSC.UpdateChatboxContent();
            else LilOSC.SetChatboxMessage(string.Empty, force: true);
        }

        private async void linkEditChatbox_Click(LinkLabel sender, MouseButton button) {
            string template = Settings.Get.OSCMessageInfoTemplate.Template;
            EditResult edit = await EditWindow.Show(template, LANG.S("SETTINGS.OSCSENDCHATBOX.TITLE") ?? "Chatbox Message Template", this, handleNewLine: false, insertKeyTemplate: true);
            if (edit.Accept) {
                Settings.Get.OSCMessageInfoTemplate.Template = string.IsNullOrEmpty(edit.Text) ? Settings.Default.OSCMessageInfoTemplate.Template : edit.Text;
                Settings.Export();

                LilOSC.UpdateChatboxContent();
            }
        }

        private async void linkAutoNoteEdit_LinkClicked(LinkLabel sender, MouseButton button) {
            string template = Settings.Get.RoundNoteTemplate.Template;
            EditResult edit = await EditWindow.Show(template, LANG.S("SETTINGS.SAVEROUNDNOTE.TITLE") ?? "Automatic Note Template", this, handleNewLine: false, insertKeyTemplate: true);
            if (edit.Accept) {
                Settings.Get.RoundNoteTemplate.Template = string.IsNullOrEmpty(edit.Text) ? Settings.Default.RoundNoteTemplate.Template : edit.Text;
                Settings.Export();
            }
        }

        private async void linkSetMasterInterval_LinkClicked(LinkLabel sender, MouseButton button) {
            int original = Settings.Get.OSCMasterChangeInterval;
            string value = original.ToString();
            EditResult show = await EditWindow.Show(value, LANG.S("SETTINGS.OSCMASTERCHANGE.TITLE") ?? "Set Interval", this);

            if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result) && result != original) {
                Settings.Get.OSCMasterChangeInterval = result;
                Settings.Export();
            }
        }

        private async void linkSetDamageInterval_LinkClicked(LinkLabel sender, MouseButton button) {
            if (sender == linkSetDamageOutput) {
                string value = Settings.Get.OSCDamageTemplate.Template;
                EditResult show = await EditWindow.Show(value, LANG.S("SETTINGS.OSCDAMAGEDEVENT_OUTPUT.TITLE") ?? "Set Output Code", this, true, insertKeyTemplate: true);

                if (show.Accept) {
                    Settings.Get.OSCDamageTemplate.Template = string.IsNullOrEmpty(show.Text) ? Settings.Default.OSCDamageTemplate.Template : show.Text.Trim();
                    Settings.Export();
                }
            }

            if (sender == linkSetDamageInterval) {
                string value = Settings.Get.OSCDamageIntervalTemplate.Template;
                EditResult show = await EditWindow.Show(value, LANG.S("SETTINGS.OSCDAMAGEDEVENT.TITLE") ?? "Set Damage Interval", this, true, insertKeyTemplate: true);

                if (show.Accept) {
                    Settings.Get.OSCDamageIntervalTemplate.Template = string.IsNullOrEmpty(show.Text) ? Settings.Default.OSCDamageIntervalTemplate.Template : show.Text.Trim();
                    Settings.Export();
                }
            }
        }

        private async void linkLogUpdateRate_LinkClicked(LinkLabel sender, MouseButton button) {
            int original = Settings.Get.LogUpdateRate;
            string value = original.ToString();
            EditResult show = await EditWindow.Show(value, LANG.S("SETTINGS.LOGUPDATERATE.TITLE") ?? "Set Update Rate (Milliseconds)", this);

            if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result) && result != original) {
                Settings.Get.LogUpdateRate = Math.Clamp(result, 10, 5000);
                ToNLogHandler.LogWatcher.Interval = Settings.Get.LogUpdateRate;
                Settings.Export();
            }
        }

        private async Task<string?> SelectAudioFile() {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
                Title = LANG.S("SETTINGS.PLAYAUDIO.TITLE") ?? "Select Custom Sound",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("Waveform (*.wav)") { Patterns = ["*.wav"] }]
            });

            string? result = files.Count > 0 ? files[0].TryGetLocalPath() : null;
            return string.IsNullOrEmpty(result) ? null : result;
        }

        private void checkAutoCopy_CheckedChanged(object? sender, RoutedEventArgs e) {
            if (checkAutoCopy.IsChecked == true && sender != null) LilOSC.SendData(true);
            checkCopyOnOpen.Foreground = checkCopyOnJoin.Foreground = checkCopyOnSave.Foreground =
                WindowExtensions.GetBrush(checkAutoCopy.IsChecked == true ? "ListFore" : "Muted");
        }

        private void checkOSCEnabled_CheckedChanged(object? sender, RoutedEventArgs e) {
            if (checkOSCEnabled.IsChecked == true && sender != null) LilOSC.SendData(true);
            checkOSCMasterChange.Foreground = checkOSCDeathEvent.Foreground = checkOSCSendDamage.Foreground = checkOSCSendColor.Foreground =
                WindowExtensions.GetBrush(checkOSCEnabled.IsChecked == true ? "ListFore" : "Muted");
        }

        private void checkOSCEnabled_MouseUp(object? sender, PointerReleasedEventArgs e) {
            if (e.InitialPressMouseButton == MouseButton.Right)
                MainWindow.OpenExternalLink("https://github.com/ChrisFeline/ToNSaveManager/?tab=readme-ov-file#osc-documentation");
        }

        private void ctxItemPickFolder_Click(object? sender, RoutedEventArgs e) {
            ChangeDataLocation(false);
        }

        private void ctxItemResetToDefault_Click(object? sender, RoutedEventArgs e) {
            ChangeDataLocation(true);
        }

        private async void ChangeDataLocation(bool reset) {
            string? folder = null;
            if (!reset) {
                IStorageFolder? start = await StorageProvider.TryGetFolderFromPathAsync(Path.GetDirectoryName(SaveData.Destination) ?? string.Empty);
                var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { AllowMultiple = false, SuggestedStartLocation = start });
                folder = folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
                if (string.IsNullOrEmpty(folder)) return;
            }

            try {
                if (!SaveData.SetDataLocation(folder)) return;
            } catch (Exception e) {
                string selectedFolder = folder ?? AppCore.DataLocation;
                await MessageBox.Show(this, LANG.S("MESSAGE.COPY_FILES_ERROR", selectedFolder) ?? $"An error ocurred while trying to copy your files to the selected location.\n\nMake sure that the program contains permissions to write files to the destination.\nPath: {selectedFolder}" + "\n\n" + e, "Error", AlertButtons.OK, AlertIcon.Error);
                return;
            }

            if (reset) await MessageBox.Show(this, LANG.S("MESSAGE.SAVE_LOCATION_RESET") ?? "Save data location has been reset to default.", LANG.S("MESSAGE.SAVE_LOCATION_RESET.TITLE") ?? "Reset Custom Data Location");
        }

        private void CheckColorObjectives_CheckedChanged(object? sender, RoutedEventArgs e) {
            ObjectivesWindow.RefreshLists();
        }

        private void languageSelectBox_SelectedIndexChanged(object? sender, SelectionChangedEventArgs e) {
            if (!FilledLanguages || languageSelectBox.SelectedIndex < 0 || languageSelectBox.SelectedItem == null) return;
            LANG.LangKey langKey = (LANG.LangKey)languageSelectBox.SelectedItem;

            if (LANG.SelectedKey != langKey.Key) {
                Logger.Log("Changing language to: " + langKey);
                LANG.Select(langKey.Key);
                Settings.Get.SelectedLanguage = langKey.Key;
                Settings.Export();
            }
        }

        private void languageSelect_DragEnter(object? sender, DragEventArgs e) {
            if (e.DataTransfer.Contains(DataFormat.File)) e.DragEffects = DragDropEffects.Copy;
        }

        private void languageSelect_DragDrop(object? sender, DragEventArgs e) {
            IStorageItem[]? files = e.DataTransfer.TryGetFiles();
            if (files == null) return;

            foreach (IStorageItem item in files) {
                string? file = item.TryGetLocalPath();
                if (file != null && file.EndsWith(".json"))
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
        private void BindControlsRecursive() {
            foreach (CheckBox c in this.GetLogicalDescendants().OfType<CheckBox>()) {
                string? tag = c.Tag?.ToString();
                if (string.IsNullOrEmpty(tag)) continue;

                LocalizedControlCache.Add("SETTINGS." + tag.ToUpperInvariant(), c);
                BindSettings(c, tag);
            }
        }

        private static void BindSettings(CheckBox checkBox, string memberName) {
            PropertyInfo? property = typeof(Settings).GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                throw new NullReferenceException("Member name not found: " + memberName);

            object? value = property.GetValue(Settings.Get);

            checkBox.IsChecked = value != null && (bool)value;
            checkBox.IsCheckedChanged += (o, e) => {
                property.SetValue(Settings.Get, checkBox.IsChecked == true);
                Settings.Export();
            };
        }
        #endregion
    }
}
