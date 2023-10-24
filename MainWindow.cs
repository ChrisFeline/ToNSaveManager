using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace ToNSaveManager
{
    public partial class MainWindow : Form
    {
        const string Destination = "data.json";
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
        private bool IsMouseUp;

        private void listBoxEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxEntries.SelectedIndex;
            if (!IsValidIndex(index) || listBoxEntries.SelectedItem == null) return;

            Entry entry = (Entry)listBoxEntries.SelectedItem;
            if (IsMouseUp)
            {
                IsMouseUp = false;
                // Open note editor
                EditResult edit = EditWindow.Show(entry.Note, this);
                if (edit.Accept && !edit.Text.Equals(entry.Note, StringComparison.Ordinal))
                {
                    entry.Note = edit.Text;
                    listBoxEntries.Refresh();

                    Dirty = true;
                    Export();
                }
            }
            else
            {
                // Copy to clipboard
                entry.CopyToClipboard();
                MessageBox.Show("Save string have been copied to Clipboard.\n" + entry.ToString(), "Copied");
            }

            listBoxEntries.ClearSelected();
        }

        private void listBoxEntries_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listBoxEntries.IndexFromPoint(e.Location);
                if (!IsValidIndex(index)) return;

                IsMouseUp = true;
                listBoxEntries.SelectedIndex = index;
            }
        }

        int LastHoveredIndex = -1;
        private void listBoxEntries_MouseMove(object sender, MouseEventArgs e)
        {
            int index = listBoxEntries.IndexFromPoint(e.Location);

            if (LastHoveredIndex != index)
            {
                LastHoveredIndex = index;
                if (IsValidIndex(index))
                {
                    Entry entry = (Entry)listBoxEntries.Items[index];
                    TooltipUtil.Set(listBoxEntries, entry.GetTooltip());
                }
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

        private void listBoxKeys_KeyUp(object sender, KeyEventArgs e)
        {
            int selectedIndex = listBoxKeys.SelectedIndex;
            if (selectedIndex != -1 && listBoxKeys.SelectedItem != null && e.KeyCode == Keys.Delete)
            {
                LogKey logKey = (LogKey)listBoxKeys.SelectedItem;
                DialogResult result = MessageBox.Show($"Are you SURE that you want to delete this entry?\n\nEvery code from {logKey.Date} will be permanently deleted.\nThis operation is not reversible!", "Deleting Entry: " + logKey.ToString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    // Try to warn and prevent the user deleting the most recent code history
                    if (selectedIndex == 0 && MessageBox.Show("This is the most recent log history, it contains the most recent save codes! Are you really sure you want to delete this entry?\n\nYou can't undo this action.", "WARNING", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                    listBoxKeys.Items.RemoveAt(selectedIndex);
                    SaveData.Remove(logKey.Value);
                    Dirty = true; // Save to .json next tick
                }
            }
        }
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
        private void PushKey(LogKey logKey)
        {
            int index = listBoxKeys.Items.Count;
            for (int i = 0; i < index; i++)
            {
                object item = listBoxKeys.Items[i];
                if (item == null) continue;

                LogKey key = (LogKey)item;
                if (key.Equals(logKey)) return; // Key already exists

                if (key.Date < logKey.Date)
                {
                    index = i;
                    break;
                }
            }

            listBoxKeys.Items.Insert(index, logKey);
        }
        private void UpdateEntries()
        {
            listBoxEntries.Items.Clear();
            if (listBoxKeys.SelectedItem == null) return;

            LogKey key = (LogKey)listBoxKeys.SelectedItem;
            if (!SaveData.ContainsKey(key.Value)) return;

            History history = SaveData[key.Value];

            foreach (Entry entry in history.Entries)
                listBoxEntries.Items.Add(entry);
        }
        #endregion

        #region Log Handling
        const string WorldNameKeyword = "Terrors of Nowhere";
        const string SaveStartKeyword = "  [START]";
        const string SaveEndKeyword = "[END]";

        private bool Dirty = false;
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

            AddEntry(logName, save, timestamp);
        }

        private void LogWatcher_OnTick(object? sender, EventArgs e)
        {
            CopyRecent();
            Export();
        }
        #endregion

        #region Data
        private Entry? RecentData;
        private Dictionary<string, History> SaveData;
        private void Import()
        {
            if (!File.Exists(Destination))
            {
                SaveData = new Dictionary<string, History>();
                return;
            }

            Dictionary<string, History>? data = null;

            try
            {
                string content = File.ReadAllText(Destination);
                data = JsonConvert.DeserializeObject<Dictionary<string, History>>(content);
            }
            catch { }

            if (data == null) data = new Dictionary<string, History>();
            else
            {
                foreach (var item in data)
                {
                    item.Value.Sort(); // Sort by dates just in case
                    PushKey(new LogKey(item.Key));

                    Entry? first = item.Value.Entries.FirstOrDefault(); // First should always be the most recent
                    if (first != null) SetRecent(first);
                }
            }

            SaveData = data;
            CopyRecent();
        }

        private void Export()
        {
            if (!Dirty) return;

            try
            {
                // Removed indentation to save space and make Serializing faster.
                string json = JsonConvert.SerializeObject(SaveData);
                File.WriteAllText(Destination, json);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error ocurred while trying to write your settings to a file.\n\nMake sure that the program contains permissions to write files in the current folder it's located at.\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateEntries();
            Dirty = false;
        }

        private void AddEntry(string dateKey, string content, DateTime timestamp)
        {
            if (!SaveData.ContainsKey(dateKey))
            {
                SaveData.Add(dateKey, new History());
                PushKey(new LogKey(dateKey));
            }

            History history = SaveData[dateKey];
            if (!history.Add(content, timestamp, out Entry? entry)) return;

#pragma warning disable CS8604 // Nullability is handled along with the return value of <History>.Add
            SetRecent(entry);
#pragma warning restore CS8604
            Dirty = true;
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