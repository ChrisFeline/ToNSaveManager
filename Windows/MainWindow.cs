using System.Diagnostics;
using System.Media;
using System.Text;
using ToNSaveManager.Extensions;
using ToNSaveManager.Models;
using ToNSaveManager.Utils;
using ToNSaveManager.Windows;

using OnLineArgs = ToNSaveManager.Utils.LogWatcher.OnLineArgs;
using LogContext = ToNSaveManager.Utils.LogWatcher.LogContext;
using System.Collections.Generic;

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

        private void mainWindow_Loaded(object sender, EventArgs e)
        {
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
                TooltipUtil.Set(listBoxEntries, entry.GetTooltip(Settings.Get.SaveNames, Settings.Get.SaveTerrors));
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
            ProcessStartInfo psInfo = new ProcessStartInfo { FileName = url, UseShellExecute = true };
            using (Process.Start(psInfo))
            {
                Debug.WriteLine("Opening external link: " + url);
            }
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
        const string SaveStartKeyword = "  [START]";
        const string SaveEndKeyword = "[END]";
        const string SaveInitKeyword = "  [TERRORS SAVE CODE CREATED";

        const string ROUND_PARTICIPATION_KEY = "optedIn";
        const string ROUND_OPTIN_KEYWORD = "  opted in";
        const string ROUND_OPTOUT_KEYWORD = " opted out";

        const string ROUND_KILLERS_KEY = "rKillers";
        const string ROUND_OVER_KEYWORD = "  Player Won";

        const string KILLER_MATRIX_KEY = "killerMatrix";
        const string KILLER_MATRIX_KEYWORD = " Killers have been set - ";

        const string KILLER_UNLOCK_KEYWORD_1 = " Unlocking Entry ";
        const string KILLER_UNLOCK_KEYWORD_2 = " Unlocking Alt Entry ";

        private void LogWatcher_OnLine(object? sender, OnLineArgs e)
        {
            string line = e.Content;
            DateTime timestamp = e.Timestamp;
            LogContext context = e.Context;

            if (HandleSaveCode(line, timestamp, context) ||
                (Settings.Get.SaveTerrors && HandleTerrorIndex(line, timestamp, context))) { }
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

            index = line.IndexOf(SaveStartKeyword, 32);
            if (index < 0) return false;

            index += SaveStartKeyword.Length;

            int end = line.IndexOf(SaveEndKeyword, index);
            if (end < 0) return false;
            end -= index;

            string save = line.Substring(index, end);
            string logName = context.FileName.Substring(11, 19);

            AddLogEntry(logName, save, timestamp, context);
            context.Set(SaveInitKey, false);
            return true;
        }

        static readonly ToNIndex TerrorIndex = ToNIndex.Import();
        private bool HandleTerrorIndex(string line, DateTime timestamp, LogContext context)
        {
            // Handle participation
            bool isOptedIn = line.Contains(ROUND_OPTIN_KEYWORD);
            if (isOptedIn || line.Contains(ROUND_OPTOUT_KEYWORD))
            {
                context.Set(ROUND_PARTICIPATION_KEY, isOptedIn);
                if (!isOptedIn) context.Rem(ROUND_KILLERS_KEY);
                return true;
            } else
            {
                isOptedIn = context.Get<bool>(ROUND_PARTICIPATION_KEY);
            }

            if (!isOptedIn) return false;

            int[,]? killerMatrix;
            int type;
            if (line.Contains(ROUND_OVER_KEYWORD))
            {
                killerMatrix = context.Get<int[,]?>(KILLER_MATRIX_KEY);
                if (killerMatrix == null) return true;

                // Hardcoding this to 2 entries because Beyond is not logging/registering the third entry in BloodBath/Midnight rounds.
                int l = Math.Min(killerMatrix.GetLength(0), 2);
                if (!(l > 1 && killerMatrix[0, 1] > 0 && killerMatrix[1, 1] > 0)) l = 1; // Round is single terror

                HashSet<string> killers = new HashSet<string>(l);
                for (int i = 0; i < l; i++)
                {
                    int val = killerMatrix[i, 0];
                    type = killerMatrix[i, 1];

                    string terrorName = TerrorIndex[val, type > 1] ?? "???";
                    if (!killers.Contains(terrorName)) killers.Add(terrorName);
                }

                context.Set(ROUND_KILLERS_KEY, killers.ToArray());
                context.Rem(KILLER_MATRIX_KEY);
                return true;
            }

            int index = line.IndexOf(KILLER_MATRIX_KEYWORD);
            if (index > 0)
            {
                string[] kMatrixRaw = line.Substring(index + KILLER_MATRIX_KEYWORD.Length).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                killerMatrix = new int[kMatrixRaw.Length,2];
                // 0 is undefined
                // 1 is normal
                // 2 is alternate

                for (int i = 0; i < kMatrixRaw.Length; i++)
                {
                    string raw = kMatrixRaw[i];
                    killerMatrix[i,0] = int.TryParse(raw, out index) ? index : -1;
                }

                context.Set(KILLER_MATRIX_KEY, killerMatrix);
                return true;
            }

            index = line.IndexOf(KILLER_UNLOCK_KEYWORD_1);
            if (index > 0)
            {
                index += KILLER_UNLOCK_KEYWORD_1.Length;
                type = 1;

            }
            else if ((index = line.IndexOf(KILLER_UNLOCK_KEYWORD_2)) > 0)
            {
                index += KILLER_UNLOCK_KEYWORD_2.Length;
                type = 2;
            }
            else return false;

            line = line.Substring(index).Trim();
            if (int.TryParse(line, out index))
            {
                killerMatrix = context.Get<int[,]?>(KILLER_MATRIX_KEY);
                if (killerMatrix != null)
                {
                    for (int i = 0; i < killerMatrix.GetLength(0); i++)
                    {
                        if (index == killerMatrix[i,0] && killerMatrix[i, 1] == 0)
                        {
                            killerMatrix[i, 1] = type;
                            break;
                        }
                    }
                }
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
            if (Settings.Get.SaveTerrors && context.HasKey(ROUND_KILLERS_KEY))
            {
                string[]? killers = context.Get<string[]>(ROUND_KILLERS_KEY);
                if (killers != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("- ");
                    sb.AppendJoin(Environment.NewLine + "- ", killers);

                    entry.Terrors = sb.ToString();

                    if (Settings.Get.SaveTerrorsNote)
                    {
                        sb.Clear();
                        sb.AppendJoin(", ", killers);

                        entry.Note = sb.ToString();
                    }
                }

                context.Rem(ROUND_KILLERS_KEY);
            }

            if (listBoxKeys.SelectedItem == collection)
                InsertSafe(listBoxEntries, ind, entry);

            SetRecent(entry);
#pragma warning restore CS8604, CS8602
            SaveData.SetDirty();

            PlayNotification();
            SendXSNotification();
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