using System.Diagnostics;

namespace ToNSaveManager
{
    public partial class MainWindow : Form
    {
        static readonly LogWatcher LogWatcher = new LogWatcher();
        static readonly AppSettings Settings = AppSettings.Import();
        private bool Started;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // I know that SaveData won't be accessed before is not null
        public MainWindow() => InitializeComponent();
#pragma warning restore CS8618

        #region Form Events

        #region Main Window
        private void mainWindow_Loaded(object sender, EventArgs e)
        {
            checkBoxAutoCopy.Checked = Settings.AutoCopy;
            TooltipUtil.Set(checkBoxAutoCopy, "Automatically copy new Save Codes to clipboard while you play.");
            TooltipUtil.Set(linkLabel1, "Source Code and Documentation for this tool can be found in my GitHub.\n" + SourceLink + "\n\nYou can also find me in discord as Kittenji.");
        }

        private void mainWindow_Shown(object sender, EventArgs e)
        {
            if (Started) return;

            Import();

            LogWatcher.OnLine += LogWatcher_OnLine;
            LogWatcher.OnTick += LogWatcher_OnTick;
            LogWatcher.Start();

            Started = true;
        }

        #endregion
        #region ListBox Entries
        bool IsMouseUp;
        private void listBoxEntries_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listBoxEntries.IndexFromPoint(e.Location);
                if (!IsValidIndex(index)) return;

                IsMouseUp = true;
                listBoxEntries.SelectedIndex = index;
                ctxMenuEntries.Show(listBoxEntries, listBoxEntries.PointToClient(Cursor.Position));
                IsMouseUp = false;
            }
        }


        private void listBoxEntries_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            ListBox listBox = (ListBox)sender;
            string itemText = listBox.Items[e.Index].ToString() ?? string.Empty;
            e.DrawBackground();

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
            listBoxEntries.ClearSelected();
        }

        private void ctxMenuEntries_Opened(object sender, EventArgs e)
        {
            listBoxEntries.Enabled = false;
            ctxMenuEntriesCopyTo.DropDownItems.Clear();

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
                listBoxEntries.Refresh();
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
                    UpdateEntries();
                    Export(true);
                }
            }
        }

        private void listBoxEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEntries.SelectedItem == null || IsMouseUp) return;
            Entry entry = (Entry)listBoxEntries.SelectedItem;
            entry.CopyToClipboard();
            MessageBox.Show("Copied to clipboard!\n\nYou can now paste the code in game.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            listBoxEntries.ClearSelected();
        }
        #endregion

        // Helper Methods for ListBox Entries
        private bool IsValidIndex(int index)
        {
            return index > -1 && index < listBoxEntries.Items.Count;
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

        #region ListBox Keys
        private void listBoxKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEntries();
        }

        private void listBoxKeys_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                listBoxKeys.Focus();
                int index = listBoxKeys.IndexFromPoint(e.Location);
                if (index < 0 || index >= SaveData.Count) return;
                listBoxKeys.SelectedIndex = index;
                ctxMenuKeys.Show(listBoxKeys, listBoxKeys.PointToClient(Cursor.Position));
            }
        }

        #region Context Menu | Keys
        private void ctxMenuKeysImport_Click(object sender, EventArgs e)
        {
            if (listBoxKeys.SelectedItem == null) return;
            History h = (History)listBoxKeys.SelectedItem;
            if (!h.IsCustom) return;

            EditResult edit = EditWindow.Show(h.Name, "Import Code", this);
            if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text))
            {
                string content = edit.Text.Trim();
                AddCustomEntry(new Entry(content, DateTime.Now) { Note = "Imported" }, h);
                UpdateEntries();
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
                SetDataSourceSilent(listBoxKeys, SaveData.Collection);
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
                    SetDataSource(listBoxKeys, SaveData.Collection);
                    listBoxEntries.Items.Clear();
                    listBoxKeys.ClearSelected();
                    Export(true);
                }
            }
        }
        #endregion
        #endregion

        #region Options & Settings
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

        #region Form Methods
        private void UpdateEntries()
        {
            if (listBoxKeys.SelectedItem == null) return;

            History selected = (History)listBoxKeys.SelectedItem;
            listBoxEntries.Items.Clear();
            foreach (Entry entry in selected.Entries)
                listBoxEntries.Items.Add(entry);
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
                string.IsNullOrEmpty(context.RecentWorld) ||
                !context.RecentWorld.Contains(WorldNameKeyword))
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
        private SaveData SaveData;
        private void Import()
        {
            SaveData = SaveData.Import();
            for (int i = 0; i < SaveData.Count; i++)
            {
                if (SaveData[i].IsCustom) continue;

                Entry? first = SaveData[i].Entries.FirstOrDefault(); // First should always be the most recent
                if (first != null) SetRecent(first);
            }

            CopyRecent();
            SetDataSourceSilent(listBoxKeys, SaveData.Collection);
        }

        private void Export(bool force = false) => SaveData.Export(force);

        private void AddCustomEntry(Entry entry, History? collection)
        {
            if (collection == null)
            {
                EditResult edit = EditWindow.Show(string.Empty, "Set Collection Name", this);
                if (edit.Accept && !string.IsNullOrWhiteSpace(edit.Text))
                {
                    string title = edit.Text.Trim();
                    collection = new History(title, DateTime.Now);

                    int index = listBoxKeys.SelectedIndex;
                    int i = SaveData.Add(collection);
                    SetDataSource(listBoxKeys, SaveData.Collection);
                    if (i <= index) listBoxKeys.SelectedIndex = index + 1;
                }
                else return;
            }
            else collection.Timestamp = DateTime.Now;

            collection.Add(entry);
            Export(true);
        }
        private void AddLogEntry(string dateKey, string content, DateTime timestamp)
        {
            History? history = SaveData[dateKey];
            if (history == null)
            {
                history = new History(dateKey);
                int index = listBoxKeys.SelectedIndex;
                int i = SaveData.Add(history);
                SetDataSourceSilent(listBoxKeys, SaveData.Collection);
                if (i <= index) listBoxKeys.SelectedIndex = index + 1;
            }


            if (!history.Add(content, timestamp, out Entry? entry)) return;
            if (history == listBoxKeys.SelectedItem) UpdateEntries();

#pragma warning disable CS8604 // Nullability is handled along with the return value of <History>.Add
            SetRecent(entry);
#pragma warning restore CS8604
            SaveData.SetDirty();
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

        private void SetDataSource<T>(ListBox list, IList<T> source)
        {
            list.DataSource = null;
            list.DataSource = source;
        }

        private void SetDataSourceSilent<T>(ListBox list, IList<T> source)
        {
            Debug.WriteLine("TEST");
            list.SelectionMode = SelectionMode.None;
            SetDataSource(list, source);
            list.SelectionMode = SelectionMode.One;
        }
        #endregion
    }
}