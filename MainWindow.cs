using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

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

        private void mainWindow_Loaded(object sender, EventArgs e)
        {
            checkBoxAutoCopy.Checked = Settings.AutoCopy;
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

        private void listBoxEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEntries.SelectedIndex < 0 || listBoxEntries.SelectedItem == null) return;

            Entry entry = (Entry)listBoxEntries.SelectedItem;
            entry.CopyToClipboard();

            MessageBox.Show("Save string have been copied to Clipboard.\n" + entry.ToString(), "Copied");
            listBoxEntries.ClearSelected();
        }

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
    }

    internal class History
    {
        public List<Entry> Entries = new List<Entry>();
        [JsonIgnore] public int Count => Entries.Count;

        private int FindIndex(string content, DateTime timestamp)
        {
            for (int i = 0; i < Count; i++)
            {
                Entry e = Entries[i]; // Prevent doubles
                if (e.Content.Equals(content, StringComparison.OrdinalIgnoreCase))
                    return -1;

                if (e.Timestamp < timestamp)
                    return i;
            }

            return Count;
        }

        public bool Add(string content, DateTime timestamp, out Entry? entry)
        {
            int index = FindIndex(content, timestamp);
            if (index < 0)
            {
                entry = null;
                return false;
            }

            entry = new Entry(content, timestamp);
            Entries.Insert(index, entry);
            return true;
        }

        public void Sort()
        {
            Entries.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
        }
    }

    internal class Entry
    {
        internal const string DateFormat = "MM/dd/yyyy | HH:mm:ss";

        public DateTime Timestamp;
        public string Content;
        public bool Fresh;

        public Entry(string content, DateTime timestamp)
        {
            Fresh = true;
            Content = content;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return Timestamp.ToString(DateFormat) + " | " + Content.Length + " Bytes";
        }

        public void CopyToClipboard()
        {
            Debug.WriteLine("Copied to clipboard: " + ToString());
            Clipboard.SetText(Content);
        }
    }

    internal struct LogKey : IEquatable<LogKey>
    {
        public string Value;
        public DateTime Date;

        public LogKey(string value)
        {
            Value = value;
            // "2023-10-22_09-51-29"
            if (DateTime.TryParseExact(value, "yyyy-MM-dd_HH-mm-ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                Date = date;
            }
        }

        public bool Equals(LogKey other)
        {
            return Value.Equals(other);
        }

        public override string ToString()
        {
            return Date.ToString(Entry.DateFormat);
        }
    }
}