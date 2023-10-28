using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace ToNSaveManager
{
    public partial class MainWindow : Form
    {
        #region Initialization
        static readonly LogWatcher LogWatcher = new LogWatcher();
        static readonly AppSettings Settings = AppSettings.Import();
        static readonly SaveData SaveData = SaveData.Import();
        private bool Started;

        public MainWindow() =>
            InitializeComponent();
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

            InitializeOptions();
            TooltipUtil.Set(linkLabel1, "Source Code and Documentation for this tool can be found in my GitHub.\n" + SourceLink + "\n\nYou can also find me in discord as Kittenji.");
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

            SetDataSourceSilent(listBoxKeys, SaveData.Collection);
        }
        #endregion

        #region ListBox Keys
        private void listBoxKeys_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int index = listBoxKeys.IndexFromPoint(e.Location);
                if (index < 0 || index >= SaveData.Count) return;
                listBoxKeys.SetSelected(index, true);
            }
        }

        private void listBoxKeys_MouseUp(object sender, MouseEventArgs e)
        {
            bool isRight = e.Button == MouseButtons.Right;
            if (e.Button == MouseButtons.Left || isRight)
            {
                int index = listBoxKeys.SelectedIndex;
                if (index < 0) return;

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
                    SaveData.Remove(h);
                    listBoxKeys.SelectedIndex = -1;
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
            TextRenderer.DrawText(e.Graphics, GetTruncatedText(itemText, listBox.Font, maxWidth), listBox.Font, e.Bounds, e.ForeColor, TextFormatFlags.Left);

            e.DrawFocusRectangle();
        }

        private void listBoxEntries_Resize(object sender, EventArgs e)
        {
            listBoxEntries.Refresh();
        }

        #region Context Menu | Entries
        private Entry? ContextEntry;
        private void ctxMenuEntries_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            listBoxEntries.Enabled = true;
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
            if (ContextEntry == null) return;
            AddCustomEntry(ContextEntry, null);
        }

        private void ctxMenuEntriesNote_Click(object sender, EventArgs e)
        {
            if (ContextEntry == null) return;

            EditResult edit = EditWindow.Show(ContextEntry.Note, "Note Editor", this);
            if (edit.Accept && !edit.Text.Equals(ContextEntry.Note, StringComparison.Ordinal))
            {
                ContextEntry.Note = edit.Text.Trim();
                Export(true);
            }
        }

        private void ctxMenuEntriesDelete_Click(object sender, EventArgs e)
        {
            if (listBoxKeys.SelectedItem == null) return;
            History h = (History)listBoxKeys.SelectedItem;

            if (ContextEntry != null)
            {
                DialogResult result = MessageBox.Show($"Are you SURE that you want to delete this entry?\n\nThe save code from '{ContextEntry.Timestamp.ToString()}' will be permanently deleted.\nThis operation is not reversible!", "Deleting Entry: " + h.ToString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    h.Entries.Remove(ContextEntry);
                    Export(true);
                }
            }

            listBoxEntries.SelectedIndex = -1;
        }
        #endregion
        #endregion

        #region Options & Settings
        private void InitializeOptions()
        {
            checkBoxAutoCopy.Checked = Settings.AutoCopy;
            TooltipUtil.Set(checkBoxAutoCopy, "Automatically copy new Save Codes to clipboard while you play.");
        }

        private void checkBoxAutoCopy_CheckedChanged(object sender, EventArgs e)
        {
            Settings.AutoCopy = checkBoxAutoCopy.Checked;
            Settings.Export();

            if (RecentData != null)
            {
                RecentData.Fresh = true;
                CopyRecent();
            }
        }
        #endregion

        #region Extras & Info
        const string SourceLink = "https://github.com/ChrisFeline/ToNSaveManager/";
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs ev)
        {
            try
            {
                using (Process.Start("explorer.exe", SourceLink))
                {
                    Debug.WriteLine("Opened source link");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error ocurred while trying to open a link to: {SourceLink}.\n\nMake sure that the program contains permissions to open processes in your machine.\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #endregion

        #region Form Methods
        private void UpdateEntries()
        {
            if (listBoxKeys.SelectedItem == null)
            {
                listBoxEntries.DataSource = null;
                return;
            }

            History selected = (History)listBoxKeys.SelectedItem;
            SetTitle(selected.Name);

            SetDataSourceSilent(listBoxEntries, selected.Entries);
        }

        private void SetDataSourceSilent<T>(ListBox list, IList<T> source)
        {
            SelectionMode sm = list.SelectionMode;
            list.SelectionMode = SelectionMode.None;
            list.DataSource = null;
            list.DataSource = source;
            list.SelectionMode = sm;
        }

        private static string GetTruncatedText(string text, Font font, int maxWidth)
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
        const string WorldNameKeyword = "Terrors of Nowhere";
        const string SaveStartKeyword = "  [START]";
        const string SaveEndKeyword = "[END]";

        private void LogWatcher_OnLine(object? sender, LogWatcher.OnLineArgs e)
        {
            string line = e.Content;
            DateTime timestamp = e.Timestamp;

            LogWatcher.LogContext context = e.Context;
            if (string.IsNullOrEmpty(context.DisplayName) ||
                string.IsNullOrEmpty(context.RoomName) ||
                !context.RoomName.Contains(WorldNameKeyword))
            {
                return;
            }

            int index = line.IndexOf(SaveStartKeyword, 32);
            if (index < 0) return;

            index += SaveStartKeyword.Length;

            int end = line.IndexOf(SaveEndKeyword, index);
            if (end < 0) return;
            end -= index;

            string save = line.Substring(index, end);
            string logName = context.FileName.Substring(11, 19);

            AddLogEntry(logName, save, timestamp);
        }

        private void LogWatcher_OnTick(object? sender, EventArgs e)
        {
            CopyRecent();
            Export();
        }
        #endregion

        #region Data
        private Entry? RecentData;

        private void Export(bool force = false) =>
            SaveData.Export(force);

        private void FirstImport()
        {
            // Don't need to run this if AutoCopy option is not enabled
            if (!Settings.AutoCopy) return;

            for (int i = 0; i < SaveData.Count; i++)
            {
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
                    AddToSaveData(collection);
                }
                else return;
            }
            else collection.Timestamp = DateTime.Now; // Update edited timestamp

            collection.Add(entry);
            if (listBoxEntries.Enabled) listBoxEntries.SelectedIndex = -1;

            Export(true);
        }
        private void AddLogEntry(string dateKey, string content, DateTime timestamp)
        {
            History? history = SaveData[dateKey];
            if (history == null)
            {
                history = new History(dateKey);
                AddToSaveData(history);
            }

            if (!history.Add(content, timestamp, out Entry? entry)) return;
            else if (listBoxEntries.Enabled) listBoxEntries.SelectedIndex = -1;

#pragma warning disable CS8604 // Nullability is handled along with the return value of <History>.Add
            SetRecent(entry);
#pragma warning restore CS8604
            SaveData.SetDirty();
        }
        private void AddToSaveData(History history)
        {
            if (listBoxKeys.SelectedIndex < 0)
                listBoxKeys.SelectionMode = SelectionMode.None;

            SaveData.Add(history);
            listBoxKeys.SelectionMode = SelectionMode.One;
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
            if (!Settings.AutoCopy || RecentData == null || !RecentData.Fresh) return;

            RecentData.CopyToClipboard();
            RecentData.Fresh = false;
        }
        #endregion
    }
}