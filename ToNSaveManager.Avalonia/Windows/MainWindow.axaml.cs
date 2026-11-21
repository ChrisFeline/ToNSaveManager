using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using ToNSaveManager.Utils;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Utils.JSPlugins;
using ToNSaveManager.Utils.JSPlugins.API;
using ToNSaveManager.Utils.LogParser;

namespace ToNSaveManager.Windows {
    public partial class MainWindow : Window {
        #region Initialization
        internal static SaveData SaveData => AppCore.SaveData;
        internal static MainWindow? Instance;

        readonly ObservableCollection<ListRow> Keys = new();
        readonly ObservableCollection<ListRow> Entries = new();

        public MainWindow() {
            InitializeComponent();

            listBoxKeys.ItemsSource = Keys;
            listBoxEntries.ItemsSource = Entries;

            // ListBox handles these itself
            listBoxKeys.AddHandler(PointerPressedEvent, listBoxKeys_MouseDown, RoutingStrategies.Tunnel);
            listBoxKeys.AddHandler(PointerReleasedEvent, listBoxKeys_MouseUp, RoutingStrategies.Bubble, true);
            listBoxKeys.AddHandler(KeyDownEvent, listBoxKeys_KeyDown, RoutingStrategies.Tunnel);
            listBoxEntries.AddHandler(PointerReleasedEvent, listBoxEntries_MouseUp, RoutingStrategies.Bubble, true);

            CreateContextMenus();

            JustCopiedTimer.Interval = TimeSpan.FromMilliseconds(1000);
            JustCopiedTimer.Tick += JustCopiedTimer_Tick;

            Instance = this;
            mainWindow_Loaded();
        }
        #endregion

        #region Form Events

        #region Main Window
        private string OriginalTitle = string.Empty;
        public void SetTitle(string? title) {
            Title = string.IsNullOrEmpty(title) ? OriginalTitle : OriginalTitle + " | " + title;
        }

        private void MainWindow_FormClosing(object? sender, WindowClosingEventArgs e) {
            Logger.Debug("Main Window is closing.");
            WinSettings.Get.LastWindowWidth = (int)Width;
            WinSettings.Get.LastWindowHeight = (int)Height;
            WinSettings.Get.LastWindowSplit = (int)splitContainer1.ColumnDefinitions[0].ActualWidth;
            WinSettings.Export();
        }

        private void mainWindow_Loaded() {
            if (WinSettings.Get.LastWindowWidth > Width)
                Width = WinSettings.Get.LastWindowWidth;

            if (WinSettings.Get.LastWindowHeight > Height)
                Height = WinSettings.Get.LastWindowHeight;

            if (WinSettings.Get.LastWindowSplit > 0)
                splitContainer1.ColumnDefinitions[0].Width = new GridLength(WinSettings.Get.LastWindowSplit);

            OriginalTitle = Title ?? string.Empty;
            Title = "Loading, please wait...";

            SetBackupButton(Settings.Get.DiscordWebhookEnabled && !string.IsNullOrWhiteSpace(Settings.Get.DiscordWebhookURL));
            ToolTip.SetTip(linkSupport, "Buy Me A Coffee ♥");
        }

        private void mainWindow_Opened(object? sender, EventArgs e) {
            // Let it draw first
            Dispatcher.UIThread.Post(mainWindow_Shown, DispatcherPriority.Background);
        }

        private void mainWindow_Shown() {
            LocalizeContent();

            if (AppCore.Started) return;
            JSEngine.Initialize();

            FirstImport();

            ToNLogHandler.HistoryAdded += (collection, index) => Keys.Insert(index, new ListRow(collection));
            ToNLogHandler.EntryAdded += (collection, entry, index) => {
                if (SelectedKey == collection)
                    InsertSafe(Entries, index, new ListRow(entry));
            };
            ToNLogHandler.CopyRequested += (entry, playSound) => {
                CopyToClipboard(entry);
                if (playSound) NotificationManager.PlayCopy();
            };
            ToNLogHandler.SaveStored += entry => PlaySaveNotification();
            ToNLogHandler.Start();

            AppCore.Started = true;
            SetTitle(null);

            if (Settings.Get.AutoCopy && Settings.Get.CopyOnOpen)
                ToNLogHandler.CopyRecentData();

            Integrations.Start();

            if (Args.ContainsArg("--emulator") || Args.ContainsArg("--emu") || Args.ContainsArg("-e"))
                EmulatorWindow.Open(this);

            _ = CheckForUpdate(this, false);
        }

        internal void LocalizeContent() {
            btnSettings.Localize("MAIN.SETTINGS");
            btnObjectives.Localize("MAIN.OBJECTIVES");
            linkWiki.Localize("MAIN.WIKI");
            linkSupport.Localize("MAIN.SUPPORT");
            btnStats.Localize("MAIN.STATS");

            importToolStripMenuItem.Localize("MAIN.CTX_IMPORT"); // .TITLE
            renameToolStripMenuItem.Localize("MAIN.CTX_RENAME"); // .TITLE
            deleteToolStripMenuItem.Localize("MAIN.CTX_DELETE"); // .TITLE

            ctxMenuEntriesCopyTo.Localize("MAIN.CTX_ADD_TO");
            ctxMenuEntriesNew.Localize("MAIN.CTX_ADD_TO.NEW");
            ctxMenuEntriesNote.Localize("MAIN.CTX_EDIT_NOTE");
            ctxMenuEntriesBackup.Localize("MAIN.CTX_BACKUP");
            ctxMenuEntriesDelete.Localize("MAIN.CTX_DELETE");

            Entry.LocalizeContent();
            RefreshLists();

            FlowDirection = LANG.IsRightToLeft ? Avalonia.Media.FlowDirection.RightToLeft : Avalonia.Media.FlowDirection.LeftToRight;
        }
        #endregion

        #region Context Menus
        readonly ContextMenu ctxMenuKeys = new();
        readonly MenuItem importToolStripMenuItem = new() { Header = "Import" };
        readonly MenuItem renameToolStripMenuItem = new() { Header = "Rename" };
        readonly MenuItem deleteToolStripMenuItem = new() { Header = "Delete" };

        readonly ContextMenu ctxMenuEntries = new();
        readonly MenuItem ctxMenuEntriesCopyTo = new() { Header = "Add to" };
        readonly MenuItem ctxMenuEntriesNew = new() { Header = "New Collection" };
        readonly MenuItem ctxMenuEntriesNote = new() { Header = "Edit Note" };
        readonly MenuItem ctxMenuEntriesBackup = new() { Header = "Backup", IsEnabled = false };
        readonly MenuItem ctxMenuEntriesDelete = new() { Header = "Delete" };

        private void CreateContextMenus() {
            importToolStripMenuItem.Click += ctxMenuKeysImport_Click;
            renameToolStripMenuItem.Click += ctxMenuKeysRename_Click;
            deleteToolStripMenuItem.Click += ctxMenuKeysDelete_Click;
            ctxMenuKeys.Items.Add(importToolStripMenuItem);
            ctxMenuKeys.Items.Add(renameToolStripMenuItem);
            ctxMenuKeys.Items.Add(new Separator());
            ctxMenuKeys.Items.Add(deleteToolStripMenuItem);

            ToolTip.SetTip(ctxMenuEntriesNew, "Add this entry to a new collection.");
            ToolTip.SetTip(ctxMenuEntriesBackup, "Force upload a backup of this code to Discord as a file, requires Auto Discord Backup to be enabled in settings.");
            ctxMenuEntriesNew.Click += ctxMenuEntriesNew_Click;
            ctxMenuEntriesNote.Click += ctxMenuEntriesNote_Click;
            ctxMenuEntriesBackup.Click += ctxMenuEntriesBackup_Click;
            ctxMenuEntriesDelete.Click += ctxMenuEntriesDelete_Click;
            ctxMenuEntries.Items.Add(ctxMenuEntriesCopyTo);
            ctxMenuEntries.Items.Add(ctxMenuEntriesNote);
            ctxMenuEntries.Items.Add(ctxMenuEntriesBackup);
            ctxMenuEntries.Items.Add(new Separator());
            ctxMenuEntries.Items.Add(ctxMenuEntriesDelete);
            ctxMenuEntries.Opened += ctxMenuEntries_Opened;
            ctxMenuEntries.Closed += ctxMenuEntries_Closed;
        }
        #endregion

        #region ListBox Keys
        private History? SelectedKey => (listBoxKeys.SelectedItem as ListRow)?.Item as History;

        private static ListRow? RowFromPoint(RoutedEventArgs e) =>
            (e.Source as Avalonia.Visual)?.FindAncestorOfType<ListBoxItem>(true)?.DataContext as ListRow;

        private void listBoxKeys_MouseDown(object? sender, PointerPressedEventArgs e) {
            // Right click selects too
            ListRow? row = RowFromPoint(e);
            if (row != null) listBoxKeys.SelectedItem = row;
        }

        private void listBoxKeys_MouseUp(object? sender, PointerReleasedEventArgs e) {
            bool isRight = e.InitialPressMouseButton == MouseButton.Right;
            if (e.InitialPressMouseButton == MouseButton.Left || isRight) {
                if (listBoxKeys.SelectedItem == null) return;

                if (isRight && RowFromPoint(e) == listBoxKeys.SelectedItem) {
                    ctxMenuKeys_Show(listBoxKeys);
                }

                UpdateEntries();
            }
        }

        private void listBoxKeys_KeyDown(object? sender, KeyEventArgs e) {
            int count = Keys.Count, index;

            if (count > 0) {
                switch (e.Key) {
                    case Key.Enter:
                        if (listBoxKeys.SelectedItem != null) {
                            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift)) ctxMenuKeys_Show(listBoxKeys);
                            else UpdateEntries();
                        }
                        break;

                    case Key.Down:
                        index = listBoxKeys.SelectedIndex + 1;
                        if (index >= count) index = 0;
                        listBoxKeys.SelectedIndex = Math.Max(index, 0);
                        break;

                    case Key.Up:
                        index = listBoxKeys.SelectedIndex - 1;
                        if (index < 0) index = count - 1;
                        listBoxKeys.SelectedIndex = Math.Min(index, count - 1);
                        break;

                    default:
                        break;
                }
            }

            e.Handled = true;
        }

        #region Context Menu | Keys
        private void ctxMenuKeys_Show(Control control) {
            importToolStripMenuItem.IsEnabled = renameToolStripMenuItem.IsEnabled =
                SelectedKey != null && SelectedKey.IsCustom;

            ctxMenuKeys.Open(control);
        }

        private async void ctxMenuKeysImport_Click(object? sender, RoutedEventArgs e) {
            History? h = SelectedKey;
            if (h == null || !h.IsCustom) return;

            EditResult edit = await EditWindow.Show(string.Empty, LANG.S("MAIN.CTX_IMPORT.TITLE") ?? "Import Title", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text)) {
                string content = edit.Text.Trim();
                await AddCustomEntry(new Entry(content, DateTime.Now) { Note = "Imported" }, h);
                Export();
            }
        }

        private async void ctxMenuKeysRename_Click(object? sender, RoutedEventArgs e) {
            History? h = SelectedKey;
            if (h == null || !h.IsCustom) return;

            EditResult edit = await EditWindow.Show(h.Name, LANG.S("MAIN.CTX_RENAME.TITLE") ?? "Set Collection Name", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text)) {
                string title = edit.Text.Trim();
                if (title == h.Name) return;

                h.Name = title;
                RefreshLists();
                SetTitle(title);

                Export(h);
            }
        }

        private async void ctxMenuKeysDelete_Click(object? sender, RoutedEventArgs e) {
            History? h = SelectedKey;
            if (h != null) {
                AlertResult result = await MessageBox.Show(this,
                    LANG.S("MAIN.CTX_DELETE_ALL.SUBTITLE", h.ToString()) ?? $"Are you SURE that you want to delete this entry?\n\nEvery code from '{h}' will be permanently deleted.\nThis operation is not reversible!",
                    LANG.S("MAIN.CTX_DELETE_ALL.TITLE", h.ToString()) ?? "Deleting Entry: " + h.ToString(),
                    AlertButtons.OKCancel, AlertIcon.Warning);

                if (result == AlertResult.OK) {
                    listBoxKeys.SelectedIndex = -1;
                    RemoveRow(Keys, h);
                    SaveData.Remove(h);
                    UpdateEntries();
                    SetTitle(null);
                    Export(null, true);
                }
            }
        }
        #endregion
        #endregion

        #region ListBox Entries
        int JustCopiedIndex = -1;
        readonly DispatcherTimer JustCopiedTimer = new DispatcherTimer();
        private void listBoxEntries_MouseUp(object? sender, PointerReleasedEventArgs e) {
            bool isRight = e.InitialPressMouseButton == MouseButton.Right;
            if (e.InitialPressMouseButton == MouseButton.Left || isRight) {
                int index = listBoxEntries.SelectedIndex;
                if (index < 0) return;

                if (isRight && RowFromPoint(e) == listBoxEntries.SelectedItem) {
                    ctxMenuEntries.Open(listBoxEntries);
                    return;
                }

                if (listBoxEntries.SelectedItem is ListRow row) {
                    CopyToClipboard((Entry)row.Item);

                    SetJustCopied(index);

                    if (Settings.Get.PlayAudioCopy)
                        NotificationManager.PlayCopy();
                }

                listBoxEntries.SelectedIndex = -1;
            }
        }

        private void SetJustCopied(int index) {
            JustCopiedTimer.Stop();
            if (JustCopiedIndex > -1 && JustCopiedIndex < Entries.Count) Entries[JustCopiedIndex].IsCopied = false;
            JustCopiedIndex = index;

            if (JustCopiedIndex > -1) {
                Entries[JustCopiedIndex].IsCopied = true;
                JustCopiedTimer.Start();
            }
        }

        private void JustCopiedTimer_Tick(object? sender, EventArgs e) {
            JustCopiedTimer.Stop();
            SetJustCopied(-1);
        }

        #region Context Menu | Entries
        private Entry? ContextEntry;
        private bool ContextItemClicked;

        private void ctxMenuEntries_Closed(object? sender, RoutedEventArgs e) {
            if (!ContextItemClicked)
                listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntries_Opened(object? sender, RoutedEventArgs e) {
            ContextItemClicked = false;
            ctxMenuEntriesCopyTo.Items.Clear();

            // Might not be the most efficient way of doing this
            foreach (History h in SaveData.Collection) {
                if (!h.IsCustom) continue;

                MenuItem item = new MenuItem { Header = h.Name };
                ctxMenuEntriesCopyTo.Items.Insert(0, item);
                item.Click += async (o, e) => {
                    ContextItemClicked = true;
                    if (ContextEntry != null)
                        await AddCustomEntry(ContextEntry, h);
                    listBoxEntries.SelectedIndex = -1;
                };
            }

            ctxMenuEntriesCopyTo.Items.Add(ctxMenuEntriesNew);

            if (listBoxEntries.SelectedItem is not ListRow row) ctxMenuEntries.Close();
            else {
                ContextEntry = (Entry)row.Item;
                if (ContextEntry.Parent == null)
                    ContextEntry.Parent = SelectedKey;
            }
        }

        private async void ctxMenuEntriesNew_Click(object? sender, RoutedEventArgs e) {
            ContextItemClicked = true;
            if (ContextEntry != null)
                await AddCustomEntry(ContextEntry, null);

            listBoxEntries.SelectedIndex = -1;
        }

        private async void ctxMenuEntriesNote_Click(object? sender, RoutedEventArgs e) {
            ContextItemClicked = true;
            if (ContextEntry != null) {
                EditResult edit = await EditWindow.Show(ContextEntry.Note, LANG.S("MAIN.CTX_EDIT_NOTE.TITLE") ?? "Note Editor", this);
                if (edit.Accept && !edit.Text.Equals(ContextEntry.Note, StringComparison.Ordinal)) {
                    ContextEntry.Note = edit.Text.Trim();
                    RefreshLists();
                    Export(ContextEntry.Parent);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntriesBackup_Click(object? sender, RoutedEventArgs e) {
            ContextItemClicked = true;
            if (ContextEntry != null)
                DSWebHook.Send(ContextEntry, true);

            listBoxEntries.SelectedIndex = -1;
        }

        private async void ctxMenuEntriesDelete_Click(object? sender, RoutedEventArgs e) {
            ContextItemClicked = true;
            History? h = SelectedKey;
            if (h == null) {
                listBoxEntries.SelectedIndex = -1;
                return;
            }

            if (ContextEntry != null) {
                Entry entry = ContextEntry;
                AlertResult result = await MessageBox.Show(this,
                    LANG.S("MAIN.CTX_DELETE_ENTRY.SUBTITLE", entry.ToString()) ?? $"Are you SURE that you want to delete this entry?\n\nDate: {entry.Timestamp}\nNote: {entry.Note}\n\nThis operation is not reversible!",
                    LANG.S("MAIN.CTX_DELETE_ENTRY.TITLE", entry.ToString()) ?? "Deleting Entry: " + entry.ToString(),
                    AlertButtons.OKCancel, AlertIcon.Warning);

                if (result == AlertResult.OK) {
                    h.Database.Remove(entry);
                    RemoveRow(Entries, entry);
                    Export(entry.Parent);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }
        #endregion
        #endregion

        #region Settings & Info
        private void btnSettings_Click(object? sender, RoutedEventArgs e) {
            SettingsWindow.Open(this);
        }

        private void linkWiki_Clicked(object? sender, RoutedEventArgs e) {
            const string wiki = "https://terror.moe/";
            const string wikiJP = "https://www.ton-jp.com/";
            OpenExternalLink(LANG.SelectedKey == "ja-JP" ? wikiJP : wiki);
        }

        private void linkSupport_Click(object? sender, RoutedEventArgs e) {
            const string support = "https://ko-fi.com/kittenji";
            OpenExternalLink(support);
        }

        private void btnObjectives_Click(object? sender, RoutedEventArgs e) {
            ObjectivesWindow.Open(this);
        }

        private void btnStats_Click(object? sender, RoutedEventArgs e) {
            StatsWindow.Open(this);
        }

        internal static async void OpenExternalLink(string? url) {
            Logger.Debug("Opening: " + url);
            if (string.IsNullOrEmpty(url) || Instance == null) return;

            bool opened;
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)) {
                opened = await Instance.Launcher.LaunchUriAsync(uri);
            } else if (Directory.Exists(url)) {
                opened = await Instance.Launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(url));
            } else {
                opened = await Instance.Launcher.LaunchFileInfoAsync(new FileInfo(url));
            }

            if (opened) Logger.Info("Opening external link: " + url);
            else Logger.Warning("Could not open external link: " + url);
        }

        // No self update here, only links to the releases
        internal static async Task CheckForUpdate(Window owner, bool showUpToDate) {
            (UpdateStatus status, GitHubRelease? release, string changelog) = await Task.Run(() => {
                UpdateStatus s = UpdateChecker.Check(showUpToDate, null, out GitHubRelease? r, out _, out string c);
                return (s, r, c);
            });

            if (status == UpdateStatus.Available && release != null) {
                AlertResult result = await MessageBox.Show(owner, (LANG.S("MESSAGE.UPDATE_AVAILABLE_LINK") ?? "A new update has been released on GitHub.\n\nWould you like to open the download page?") + changelog, LANG.S("MESSAGE.UPDATE_AVAILABLE.TITLE") ?? "New update available", AlertButtons.YesNo, AlertIcon.Information);
                if (result == AlertResult.Yes) OpenExternalLink(UpdateChecker.ReleasesUrl);
                else if (!showUpToDate) UpdateChecker.Ignore(release);
            } else if (status == UpdateStatus.UpToDate && showUpToDate) {
                await MessageBox.Show(owner, LANG.S("MESSAGE.UPDATE_UNAVAILABLE") ?? "No updates are currently available.", LANG.S("MESSAGE.UPDATE_UNAVAILABLE.TITLE") ?? "No updates available", AlertButtons.OK, AlertIcon.Information);
            }
        }
        #endregion

        #region Split Container
        private void splitContainer1_SplitterMoved(object? sender, VectorEventArgs e) {
            listBoxEntries.Focus();
        }
        #endregion
        #endregion

        #region Form Methods
        #region Notifications
        internal static void ResetNotification() {
            NotificationManager.Reset();
        }

        internal static void PlayCopyNotification() {
            if (!AppCore.Started || !Settings.Get.PlayAudioCopy) return;

            try {
                NotificationManager.PlayCopy();
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        internal static void PlaySaveNotification() {
            if (!AppCore.Started || !Settings.Get.PlayAudioSave) return;

            try {
                NotificationManager.PlaySave();
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }
        #endregion

        internal static void RefreshLists() {
            if (Instance == null) return;
            foreach (ListRow row in Instance.Keys) row.Refresh();
            foreach (ListRow row in Instance.Entries) row.Refresh();
        }

        private void UpdateEntries() {
            if (JustCopiedIndex > -1) SetJustCopied(-1);
            Entries.Clear();

            History? selected = SelectedKey;
            if (selected == null)
                return;

            SetTitle(selected.Name);

            foreach (Entry entry in selected.Database)
                Entries.Add(new ListRow(entry));
        }

        private static void InsertSafe(ObservableCollection<ListRow> list, int i, ListRow value) =>
            list.Insert(Math.Min(Math.Max(i, 0), list.Count), value);

        private static void RemoveRow(ObservableCollection<ListRow> list, object item) {
            ListRow? row = list.FirstOrDefault(r => r.Item == item);
            if (row != null) list.Remove(row);
        }

        internal void SetBackupButton(bool enabled) {
            ctxMenuEntriesBackup.IsEnabled = enabled;
        }

        private async void CopyToClipboard(Entry entry) {
            Logger.Log("COPYING TO CLIPBOARD: " + entry);

            try {
                if (Clipboard != null) await Clipboard.SetTextAsync(entry.Content);
            } catch (Exception e) {
                Logger.Error("Error trying to set clipboard object: " + e);
            }
        }
        #endregion

        #region Data
        private void Export(History? h = null, bool force = false) {
            if (h != null) h.SetDirty();
            SaveData.Export(force);
        }

        private void FirstImport() {
            for (int i = 0; i < SaveData.Count; i++) {
                AddKey(SaveData[i], i);
            }
        }

        private async Task AddCustomEntry(Entry entry, History? collection) {
            if (collection == null) {
                EditResult edit = await EditWindow.Show(string.Empty, LANG.S("MAIN.CTX_RENAME.TITLE") ?? "Set Collection Name", this);
                if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text)) {
                    string title = edit.Text.Trim();
                    collection = new History(title, DateTime.Now);
                    AddKey(collection);
                } else return;
            } else collection.Timestamp = DateTime.Now; // Update edited timestamp

            int ind = collection.Add(entry);
            if (SelectedKey == collection)
                InsertSafe(Entries, ind, new ListRow(entry));

            Export(collection, true);
        }

        private void AddKey(History collection, int i = -1) {
            if (i == -1) i = SaveData.Add(collection);
            Keys.Insert(i, new ListRow(collection));
        }
        #endregion
    }
}
