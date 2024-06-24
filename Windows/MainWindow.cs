using System.Diagnostics;
using System.Media;
using ToNSaveManager.Extensions;
using ToNSaveManager.Models;
using ToNSaveManager.Utils;
using ToNSaveManager.Windows;

using OnLineArgs = ToNSaveManager.Utils.LogWatcher.OnLineArgs;
using LogContext = ToNSaveManager.Utils.LogWatcher.LogContext;

namespace ToNSaveManager
{
    public partial class MainWindow : Form
    {
        #region Initialization
        internal static readonly LogWatcher LogWatcher = new LogWatcher();
        // internal static readonly AppSettings Settings = AppSettings.Import();
        internal static readonly SaveData SaveData = SaveData.Import();
        internal static MainWindow? Instance;
        private static bool Started;

        public MainWindow()
        {
            InitializeComponent();
            listBoxKeys.FixItemHeight();
            listBoxEntries.FixItemHeight();
            Instance = this;
        }
        #endregion

        #region Form Events

        #region Main Window
        private string OriginalTitle = string.Empty;
        public void SetTitle(string? title)
        {
            this.Text = string.IsNullOrEmpty(title) ? OriginalTitle : OriginalTitle + " | " + title;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            WinSettings.Get.LastWindowWidth = this.Width;
            WinSettings.Get.LastWindowHeight = this.Height;
            WinSettings.Get.LastWindowSplit = splitContainer1.SplitterDistance;
            WinSettings.Export();
        }

        private void mainWindow_Loaded(object sender, EventArgs e)
        {
            if (WinSettings.Get.LastWindowWidth > this.Width)
                this.Width = WinSettings.Get.LastWindowWidth;

            if (WinSettings.Get.LastWindowHeight > this.Height)
                this.Height = WinSettings.Get.LastWindowHeight;

            if (WinSettings.Get.LastWindowSplit > 0)
                splitContainer1.SplitterDistance = WinSettings.Get.LastWindowSplit;

            OriginalTitle = this.Text;
            this.Text = "Loading, please wait...";

            XSOverlay.SetPort(Settings.Get.XSOverlayPort);
        }

        private void mainWindow_Shown(object sender, EventArgs e)
        {
            if (Started) return;

            FirstImport();

            LogWatcher.OnLine += LogWatcher_OnLine;
            LogWatcher.OnTick += LogWatcher_OnTick;
            LogWatcher.Start();

            Started = true;
            SetTitle(null);
        }
        #endregion

        #region ListBox Keys
        private void listBoxKeys_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int index = listBoxKeys.IndexFromPoint(e.Location);
                if (index < 0 || index >= SaveData.Count) return;
                listBoxKeys.SelectedIndex = index;
            }
        }

        private void listBoxKeys_MouseUp(object sender, MouseEventArgs e)
        {
            bool isRight = e.Button == MouseButtons.Right;
            if (e.Button == MouseButtons.Left || isRight)
            {
                int index = listBoxKeys.SelectedIndex;
                if (index < 0)
                    return;

                if (isRight && index == listBoxKeys.IndexFromPoint(e.Location))
                    ctxMenuKeys.Show((ListBox)sender, e.Location);

                UpdateEntries();
            }
        }

        #region Context Menu | Keys
        private void ctxMenuKeysImport_Click(object sender, EventArgs e)
        {
            if (listBoxKeys.SelectedItem == null) return;
            History h = (History)listBoxKeys.SelectedItem;
            if (!h.IsCustom) return;

            EditResult edit = EditWindow.Show(string.Empty, "Import Code", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text))
            {
                string content = edit.Text.Trim();
                AddCustomEntry(new Entry(content, DateTime.Now) { Note = "Imported" }, h);
                Export(true);
            }
        }

        private void ctxMenuKeysRename_Click(object sender, EventArgs e)
        {
            if (listBoxKeys.SelectedItem == null) return;
            History h = (History)listBoxKeys.SelectedItem;

            EditResult edit = EditWindow.Show(h.Name, "Set Collection Name", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text))
            {
                string title = edit.Text.Trim();
                if (title == h.Name) return;

                h.Name = title;
                listBoxKeys.Refresh();
                SetTitle(title);
                Export(true);
            }
        }

        private void ctxMenuKeysDelete_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBoxKeys.SelectedIndex;
            if (selectedIndex != -1 && listBoxKeys.SelectedItem != null)
            {
                History h = (History)listBoxKeys.SelectedItem;
                DialogResult result = MessageBox.Show($"Are you SURE that you want to delete this entry?\n\nEvery code from '{h}' will be permanently deleted.\nThis operation is not reversible!", "Deleting Entry: " + h.ToString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    listBoxKeys.SelectedIndex = -1;
                    listBoxKeys.Items.Remove(h);
                    SaveData.Remove(h);
                    UpdateEntries();
                    SetTitle(null);
                    Export(true);
                }
            }
        }
        #endregion
        #endregion

        #region ListBox Entries
        private void listBoxEntries_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int index = listBoxEntries.IndexFromPoint(e.Location);
                if (index < 0) return;
                listBoxEntries.SelectedIndex = index;
            }
        }

        private void listBoxEntries_MouseUp(object sender, MouseEventArgs e)
        {
            bool isRight = e.Button == MouseButtons.Right;
            if (e.Button == MouseButtons.Left || isRight)
            {
                int index = listBoxEntries.SelectedIndex;
                if (index < 0) return;

                if (isRight && index == listBoxEntries.IndexFromPoint(e.Location))
                {
                    ctxMenuEntries.Show((ListBox)sender, e.Location);
                    return;
                }

                if (listBoxEntries.SelectedItem != null)
                {
                    Entry entry = (Entry)listBoxEntries.SelectedItem;
                    entry.CopyToClipboard();
                    MessageBox.Show("Copied to clipboard!\n\nYou can now paste the code in game.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                listBoxEntries.SelectedIndex = -1;
            }
        }

        private void listBoxEntries_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();

            ListBox listBox = (ListBox)sender;
            string itemText = listBox.Items[e.Index].ToString() ?? string.Empty;

            int maxWidth = e.Bounds.Width;
            TextRenderer.DrawText(e.Graphics, GetTruncatedText(itemText, listBox.Font, maxWidth), listBox.Font, e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter);

            e.DrawFocusRectangle();
        }

        private void listBoxEntries_Resize(object sender, EventArgs e)
        {
            listBoxEntries.Refresh();
        }

        // Tooltips
        int PreviousTooltipIndex = -1;
        private void listBoxEntries_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the index of the item under the mouse pointer
            int index = listBoxEntries.IndexFromPoint(e.Location);

            if (PreviousTooltipIndex != index)
            {
                PreviousTooltipIndex = index;

                if (index < 0)
                {
                    TooltipUtil.Set(listBoxEntries, null);
                    return;
                }

                Entry entry = (Entry)listBoxEntries.Items[index];
                TooltipUtil.Set(listBoxEntries, entry.GetTooltip(Settings.Get.SaveNames, Settings.Get.SaveRoundInfo));
            }
        }

        // Reset tooltip when mouse leaves the control.
        // This prevents accidental tooltip display when doing ALT+TAB.
        private void listBoxEntries_MouseLeave(object sender, EventArgs e)
        {
            if (PreviousTooltipIndex < 0) return;
            PreviousTooltipIndex = -1;
            TooltipUtil.Set(listBoxEntries, null);
        }

        #region Context Menu | Entries
        private Entry? ContextEntry;

        private void ctxMenuEntries_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            listBoxEntries.Enabled = true;
            if (e.CloseReason != ToolStripDropDownCloseReason.ItemClicked)
                listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntries_Opened(object sender, EventArgs e)
        {
            listBoxEntries.Enabled = false;
            ctxMenuEntriesCopyTo.DropDownItems.Clear();

            // Might not be the most efficient way of doing this
            foreach (History h in SaveData.Collection)
            {
                if (!h.IsCustom) continue;

                ToolStripMenuItem item = new ToolStripMenuItem(h.Name);
                ctxMenuEntriesCopyTo.DropDownItems.Insert(0, item);
                item.Click += (o, e) =>
                {
                    if (ContextEntry != null)
                        AddCustomEntry(ContextEntry, h);
                };
            }

            ctxMenuEntriesCopyTo.DropDownItems.Add(ctxMenuEntriesNew);

            if (listBoxEntries.SelectedItem == null) ctxMenuEntries.Close();
            else ContextEntry = (Entry)listBoxEntries.SelectedItem;
        }

        private void ctxMenuEntriesNew_Click(object sender, EventArgs e)
        {
            if (ContextEntry != null)
                AddCustomEntry(ContextEntry, null);

            listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntriesNote_Click(object sender, EventArgs e)
        {
            if (ContextEntry != null)
            {
                EditResult edit = EditWindow.Show(ContextEntry.Note, "Note Editor", this);
                if (edit.Accept && !edit.Text.Equals(ContextEntry.Note, StringComparison.Ordinal))
                {
                    ContextEntry.Note = edit.Text.Trim();
                    listBoxEntries.Refresh();
                    Export(true);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }

        private void ctxMenuEntriesDelete_Click(object sender, EventArgs e)
        {
            if (listBoxKeys.SelectedItem == null)
            {
                listBoxEntries.SelectedIndex = -1;
                return;
            }

            History h = (History)listBoxKeys.SelectedItem;
            if (ContextEntry != null)
            {
                DialogResult result = MessageBox.Show($"Are you SURE that you want to delete this entry?\n\nDate: {ContextEntry.Timestamp}\nNote: {ContextEntry.Note}\n\nThis operation is not reversible!", "Deleting Entry: " + h.ToString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    h.Entries.Remove(ContextEntry);
                    listBoxEntries.Items.Remove(ContextEntry);
                    Export(true);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }
        #endregion
        #endregion

        #region Settings & Info
        private void btnSettings_Click(object? sender, EventArgs e)
        {
            SettingsWindow.Open(this);
        }

        private void linkSource_Clicked(object sender, EventArgs ev)
        {
            const string link = "https://github.com/ChrisFeline/ToNSaveManager/";
            OpenExternalLink(link);
        }

        private void linkWiki_Clicked(object sender, EventArgs e)
        {
            const string wiki = "https://terror.moe/";
            OpenExternalLink(wiki);
        }

        private void btnObjectives_Click(object sender, EventArgs e)
        {
            ObjectivesWindow.Open(this);
        }

        internal static void OpenExternalLink(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            ProcessStartInfo psInfo = new ProcessStartInfo { FileName = url, UseShellExecute = true };
            using (Process.Start(psInfo))
            {
                Debug.WriteLine("Opening external link: " + url);
            }
        }
        #endregion

        #region Split Container
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (splitContainer1.CanFocus)
                splitContainer1.ActiveControl = listBoxEntries;

            listBoxKeys.Refresh();
            listBoxEntries.Refresh();
        }
        #endregion
        #endregion

        #region Form Methods
        #region Notifications
        static readonly XSOverlay XSOverlay = new XSOverlay();
        static readonly SoundPlayer CustomNotificationPlayer = new SoundPlayer();
        static readonly SoundPlayer DefaultNotificationPlayer = new SoundPlayer();
        static readonly Stream? DefaultAudioStream = // Get default notification in the embeded resources
            Program.GetEmbededResource("notification.wav");

        internal static void ResetNotification()
        {
            CustomNotificationPlayer.Stop();
            DefaultNotificationPlayer.Stop();
        }
        internal static void PlayNotification(bool forceDefault = false)
        {
            if ((!Started || !Settings.Get.PlayAudio) && !forceDefault) return;

            try
            {
                if (!forceDefault && !string.IsNullOrEmpty(Settings.Get.AudioLocation) && File.Exists(Settings.Get.AudioLocation))
                {
                    CustomNotificationPlayer.SoundLocation = Settings.Get.AudioLocation;
                    CustomNotificationPlayer.Play();
                    return;
                }

                DefaultNotificationPlayer.Stream = DefaultAudioStream;
                DefaultNotificationPlayer.Play();
            }
            catch { }
        }
        internal static void SendXSNotification(bool test = false)
        {
            if (!Started || !Settings.Get.XSOverlay) return;
            const string message = "<color=#ff9999><b>ToN</b></color><color=grey>:</color> <color=#adff2f>Save Data Stored</color>";
            const string msgtest = "<color=#ff9999><b>ToN</b></color><color=grey>:</color> <color=#adff2f>Notifications Enabled</color>";

            if (test) XSOverlay.Send(msgtest, 1);
            else XSOverlay.Send(message);
        }
        #endregion

        internal static void RefreshLists()
        {
            Instance?.listBoxKeys.Refresh();
            Instance?.listBoxEntries.Refresh();
        }

        private void UpdateEntries()
        {
            listBoxEntries.Items.Clear();

            if (listBoxKeys.SelectedItem == null)
                return;

            History selected = (History)listBoxKeys.SelectedItem;
            SetTitle(selected.Name);

            foreach (Entry entry in selected.Entries)
                listBoxEntries.Items.Add(entry);
        }

        private static void InsertSafe(ListBox list, int i, object value) =>
            list.Items.Insert(Math.Min(Math.Max(i, 0), list.Items.Count), value);

        internal static string GetTruncatedText(string text, Font font, int maxWidth)
        {
            Size textSize = TextRenderer.MeasureText(text, font);
            if (textSize.Width <= maxWidth) return text;

            int ellipsisWidth = TextRenderer.MeasureText("...", font).Width;
            while (textSize.Width + ellipsisWidth > maxWidth && text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
                textSize = TextRenderer.MeasureText(text, font);
            }

            return text + "...";
        }
        #endregion

        #region Log Handling
        const string SaveInitKey = "saveInit";
        const string SaveStartKeyword = "[START]";
        const string SaveEndKeyword = "[END]";
        const string SaveInitKeyword = "[TERRORS SAVE CODE CREATED";

        const string ROUND_PARTICIPATION_KEY = "optedIn";
        const string ROUND_OPTIN_KEYWORD = "opted in";
        const string ROUND_OPTOUT_KEYWORD = "Player respawned";

        const string ROUND_RESULT_KEY = "rResult";
        const string ROUND_KILLERS_KEY = "rKillers";
        const string ROUND_WON_KEYWORD = "Player Won";
        const string ROUND_LOST_KEYWORD = "Player lost,";

        const string KILLER_MATRIX_KEYWORD = "Killers have been set - ";
        const string KILLER_ROUND_TYPE_KEYWORD = " // Round type is ";

        private void LogWatcher_OnLine(object? sender, OnLineArgs e)
        {
            DateTime timestamp = e.Timestamp;
            LogContext context = e.Context;
            string line = e.Content.Substring(34);

            if (HandleSaveCode(line, timestamp, context) ||
                (Settings.Get.SaveRoundInfo && HandleTerrorIndex(line, timestamp, context))) { }
        }

        private void LogWatcher_OnTick(object? sender, EventArgs e)
        {
            CopyRecent();
            Export();
        }

        private bool HandleSaveCode(string line, DateTime timestamp, LogContext context)
        {
            int index = line.IndexOf(SaveInitKeyword);
            if (index > -1)
            {
                context.Set(SaveInitKey, true);
                return true;
            }

            if (!context.Get<bool>(SaveInitKey)) return false;

            index = line.IndexOf(SaveStartKeyword);
            if (index < 0) return false;

            index += SaveStartKeyword.Length;

            int end = line.IndexOf(SaveEndKeyword, index);
            if (end < 0) return false;
            end -= index;

            string save = line.Substring(index, end);
            if (string.IsNullOrEmpty(save)) {
                context.Set(SaveInitKey, false);
                return false;
            }

            AddLogEntry(context.DateKey, save, timestamp, context);
            context.Set(SaveInitKey, false);
            return true;
        }

        private bool HandleTerrorIndex(string line, DateTime timestamp, LogContext context)
        {
            // Handle participation
            bool isOptedIn = line.StartsWith(ROUND_OPTIN_KEYWORD);
            if (isOptedIn || line.StartsWith(ROUND_OPTOUT_KEYWORD))
            {
                context.Set(ROUND_PARTICIPATION_KEY, isOptedIn);
                if (!isOptedIn)
                {
                    context.Rem(ROUND_KILLERS_KEY);
                    context.Rem(ROUND_RESULT_KEY);
                }
                return true;
            }
            else
            {
                isOptedIn = context.Get<bool>(ROUND_PARTICIPATION_KEY);
            }

            if (!isOptedIn) return false;

            // Track round participation results
            isOptedIn = line.StartsWith(ROUND_WON_KEYWORD);
            if (isOptedIn || line.StartsWith(ROUND_LOST_KEYWORD))
            {
                context.Set(ROUND_RESULT_KEY, isOptedIn ? ToNRoundResult.W : ToNRoundResult.L);
                return true;
            }

            if (line.StartsWith(KILLER_MATRIX_KEYWORD))
            {
                int index = KILLER_MATRIX_KEYWORD.Length;
                int rndInd = line.IndexOf(KILLER_ROUND_TYPE_KEYWORD, index);
                if (rndInd < 0) return true;

                string roundType = line.Substring(rndInd + KILLER_ROUND_TYPE_KEYWORD.Length).Trim();
                string[] kMatrixRaw = line.Substring(index, rndInd - index).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                int[] killerMatrix = new int[kMatrixRaw.Length];

                for (int i = 0; i < kMatrixRaw.Length; i++)
                {
                    killerMatrix[i] = int.TryParse(kMatrixRaw[i], out index) ? index : -1;
                }

                TerrorMatrix terrorMatrix = new TerrorMatrix(roundType, killerMatrix);
                context.Set(ROUND_KILLERS_KEY, terrorMatrix);
                return true;
            }

            return true;
        }
        #endregion

        #region Data
        private Entry? RecentData;

        private void Export(bool force = false) =>
            SaveData.Export(force);

        private void FirstImport()
        {
            for (int i = 0; i < SaveData.Count; i++)
            {
                AddKey(SaveData[i], i);
                if (SaveData[i].IsCustom) continue;

                // First should always be the most recent, hopefully
                Entry? first = SaveData[i].Entries.FirstOrDefault();
                if (first != null) SetRecent(first);
            }

            CopyRecent();
        }

        private void AddCustomEntry(Entry entry, History? collection)
        {
            if (collection == null)
            {
                EditResult edit = EditWindow.Show(string.Empty, "Set Collection Name", this);
                if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text))
                {
                    string title = edit.Text.Trim();
                    collection = new History(title, DateTime.Now);
                    AddKey(collection);
                }
                else return;
            }
            else collection.Timestamp = DateTime.Now; // Update edited timestamp

            int ind = collection.Add(entry);
            if (listBoxKeys.SelectedItem == collection)
                InsertSafe(listBoxEntries, ind, entry);

            Export(true);
        }
        private void AddLogEntry(string dateKey, string content, DateTime timestamp, LogContext context)
        {
            History? collection = SaveData[dateKey];
            if (collection == null)
            {
                collection = new History(dateKey);
                AddKey(collection);
            }

            int ind = collection.Add(content, timestamp, out Entry? entry);
            if (ind < 0) return; // Not added, duplicate

#pragma warning disable CS8604, CS8602 // Nullability is handled along with the return value of <History>.Add
            if (Settings.Get.SaveNames) entry.Players = context.GetRoomString();
            if (Settings.Get.SaveRoundInfo)
            {
                if (context.HasKey(ROUND_RESULT_KEY))
                {
                    ToNRoundResult result = context.Get<ToNRoundResult>(ROUND_RESULT_KEY);
                    entry.RResult = result;
                    context.Rem(ROUND_RESULT_KEY);
                }

                if (context.HasKey(ROUND_KILLERS_KEY))
                {
                    TerrorMatrix killers = context.Get<TerrorMatrix>(ROUND_KILLERS_KEY);

                    if (killers.TerrorNames.Length > 0)
                    {
                        entry.RTerrors = killers.TerrorNames;
                        entry.RType = killers.RoundTypeRaw;

                        if (Settings.Get.SaveRoundNote)
                            entry.Note = string.Join(", ", killers.TerrorNames);
                    }

                    context.Rem(ROUND_KILLERS_KEY);
                }
            }

            if (listBoxKeys.SelectedItem == collection)
                InsertSafe(listBoxEntries, ind, entry);

            SetRecent(entry);
#pragma warning restore CS8604, CS8602
            SaveData.SetDirty();

            if (context.Initialized)
            {
                PlayNotification();
                SendXSNotification();
            }
        }

        private void AddKey(History collection, int i = -1)
        {
            if (i == -1) i = SaveData.Add(collection);
            listBoxKeys.Items.Insert(i, collection);
        }

        private void SetRecent(Entry entry)
        {
            if (entry == null) return;
            if (RecentData == null || RecentData.Timestamp < entry.Timestamp)
            {
                entry.Fresh = true;
                RecentData = entry;
            }
        }

        private void CopyRecent()
        {
            if (!Settings.Get.AutoCopy || RecentData == null || !RecentData.Fresh) return;

            RecentData.CopyToClipboard();
            RecentData.Fresh = false;
        }

        #endregion
    }
}