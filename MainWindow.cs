using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;

namespace ToNSaveManager
{
    public partial class MainWindow : Form
    {
        const string Destination = "data.json";
        static LogWatcher LogWatcher = new LogWatcher();
        private bool Started;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // I know that SaveData won't be accessed before is not null
        public MainWindow() => InitializeComponent();
#pragma warning restore CS8618

        private void mainWindow_Loaded(object sender, EventArgs e)
        {
            Debug.WriteLine("Loaded Window");
        }

        private void mainWindow_Shown(object sender, EventArgs e)
        {
            if (Started) return;

            Import();
            LogWatcher.Start();
            LogWatcher.OnLine += LogWatcher_OnLine;
            Started = true;
        }

        private void listBoxEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEntries.SelectedIndex < 0 || listBoxEntries.SelectedItem == null) return;

            Entry entry = (Entry)listBoxEntries.SelectedItem;
            Clipboard.SetText(entry.Content);

            MessageBox.Show("Save string have been copied to Clipboard.\n" + entry.ToString(), "Copied");
            listBoxEntries.ClearSelected();
        }

        private void listBoxKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEntries();
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

            listBoxKeys.Invoke((MethodInvoker)delegate
            {
                listBoxKeys.Items.Insert(index, logKey);
            });
        }
        private void UpdateEntries()
        {
            listBoxKeys.Invoke((MethodInvoker)delegate
            {
                listBoxEntries.Items.Clear();
                if (listBoxKeys.SelectedItem == null) return;

                LogKey key = (LogKey)listBoxKeys.SelectedItem;
                if (!SaveData.ContainsKey(key.Value)) return;

                History history = SaveData[key.Value];

                foreach (Entry entry in history.Entries)
                    listBoxEntries.Items.Add(entry);
            });
        }

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
            end = end - index;

            string save = line.Substring(index, end);
            string logName = context.FileName.Substring(11, 19);

            AddEntry(logName, save, timestamp);
        }

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
            else foreach (var item in data) PushKey(new LogKey(item.Key));

            SaveData = data;
        }

        private void Export()
        {
            string json = JsonConvert.SerializeObject(SaveData, Formatting.Indented);
            File.WriteAllText(Destination, json);

            UpdateEntries();
        }

        public void AddEntry(string dateKey, string content, DateTime timestamp)
        {
            if (!SaveData.ContainsKey(dateKey))
            {
                SaveData.Add(dateKey, new History());
                PushKey(new LogKey(dateKey));
            }

            History history = SaveData[dateKey];
            if (!history.Add(content, timestamp)) return;

            Export();
            Debug.WriteLine($"{dateKey}: {timestamp}, Saved {content.Length} Bytes");
        }
    }

    internal class History
    {
        public List<Entry> Entries = new List<Entry>();
        [JsonIgnore] public int Count => Entries.Count;

        public int FindIndex(Entry entry)
        {
            for (int i = 0; i < Count; i++)
            {
                Entry e = Entries[i]; // Prevent doubles
                if (e.Content.Equals(entry.Content, StringComparison.OrdinalIgnoreCase))
                    return -1;

                if (e.Timestamp < entry.Timestamp)
                    return i;
            }

            return Count;
        }

        public bool Add(Entry entry)
        {
            int index = FindIndex(entry);
            if (index < 0) return false;

            Entries.Insert(index, entry);
            return true;
        }
        public bool Add(string content, DateTime timestamp)
        {
            return Add(new Entry(content, timestamp));
        }

        public void Sort()
        {
            Entries.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));
        }
    }

    internal class Entry
    {
        public DateTime Timestamp;
        public string Content;

        public Entry(string content, DateTime timestamp)
        {
            Content = content;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return Timestamp.ToString() + " | " + Content.Length + " Bytes";
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
            if (DateTime.TryParseExact(
                    value,
                    "yyyy-MM-dd_HH-mm-ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date
                ))
            {
                date = date.ToUniversalTime();
                Date = date;
            }
        }

        public bool Equals(LogKey other)
        {
            return Value.Equals(other);
        }

        public override string ToString()
        {
            return Date.ToString();
        }
    }
}