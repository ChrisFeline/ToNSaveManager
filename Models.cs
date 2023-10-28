using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace ToNSaveManager
{
    internal class History : IComparable<History>, INotifyPropertyChanged
    {
        // Implementation for 'INotifyPropertyChanged'
        public event PropertyChangedEventHandler? PropertyChanged;
        [JsonIgnore] private string m_Name = string.Empty;
        public string Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public string Guid = string.Empty;
        public DateTime Timestamp;
        public bool IsCustom;

        [JsonConstructor]
        private History()
        {
            Guid = string.Empty;
            Timestamp = DateTime.MinValue;
            IsCustom = false;
        }

        public void SetLogKey(string logKey)
        {
            if (IsCustom) return;
            Guid = logKey;

            // "2023-10-22_09-51-29"
            if (DateTime.TryParseExact(logKey, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                Timestamp = date;
            }

            Name = Timestamp.ToString(Entry.DateFormat);
        }

        public History(string logKey)
        {
            IsCustom = false;
            SetLogKey(logKey);
        }

        public History(string name, DateTime timestamp) {
            Guid = System.Guid.NewGuid().ToString();
            Name = name;
            Timestamp = timestamp;
            IsCustom = true;
        }

        public BindingList<Entry> Entries = new BindingList<Entry>();
        [JsonIgnore] public int Count => Entries.Count;

        public Entry this[int i]
        {
            get
            {
                if (i < 0 || i >= Count) throw new IndexOutOfRangeException();
                return Entries[i];
            }
            set
            {
                if (i < 0 || i >= Count) throw new IndexOutOfRangeException();
                Entries[i] = value;
            }
        }

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

        public bool Add(Entry entry)
        {
            int index = FindIndex(entry.Content, entry.Timestamp);
            if (index < 0) return false;
            Entries.Insert(index, entry);
            return true;
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

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(History? other)
        {
            if (other == null) return 0;

            if (!IsCustom && other.IsCustom) return -1;
            if (IsCustom && !other.IsCustom) return 1;
            return Timestamp.CompareTo(other.Timestamp);
        }
    }

    internal class Entry : INotifyPropertyChanged
    {
        internal const string DateFormat = "MM/dd/yyyy | HH:mm:ss";

        public event PropertyChangedEventHandler? PropertyChanged;
        [JsonIgnore] private string m_Note = string.Empty;
        public string Note
        {
            get { return m_Note; }
            set
            {
                m_Note = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Note)));
            }
        }

        public DateTime Timestamp;
        public string Content;
        public string? Players;
        [JsonIgnore] public bool Fresh;
        [JsonIgnore] public int Length => Content.Length;

        public Entry(string content, DateTime timestamp)
        {
            Fresh = true;
            Content = content;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Timestamp.ToString(DateFormat));
            if (!string.IsNullOrEmpty(Note))
            {
                sb.Append(" | ");
                sb.Append(Note);
            }
            return sb.ToString();
        }

        public string GetTooltip()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Timestamp.ToString("F"));
            if (!string.IsNullOrEmpty(Note))
            {
                sb.AppendLine();
                sb.Append("Note: ");
                sb.Append(Note);
            }
            return sb.ToString();
        }

        public void CopyToClipboard()
        {
            Clipboard.SetText(Content);
        }
    }

    internal class SaveData
    {
        const string LegacyDestination = "data.json";
        const string Destination = "SaveData.json";

        public BindingList<History> Collection { get; private set; } = new BindingList<History>();
        [JsonIgnore] public int Count => Collection.Count;
        [JsonIgnore] public bool IsDirty { get; private set; }

        public History this[int i]
        {
            get
            {
                if (i < 0 || i >= Count) throw new IndexOutOfRangeException();
                return Collection[i];
            }
            set
            {
                if (i < 0 || i >= Count) throw new IndexOutOfRangeException();
                Collection[i] = value;
            }
        }
        public History? this[string id]
        {
            get
            {
                return Collection.FirstOrDefault(v => v.Guid == id);
            }
        }

        public void SetDirty() => IsDirty = true;

        public void Add(History h)
        {
            int index = FindIndex(h);
            Collection.Insert(index, h);
        }
        public void Remove(string id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Collection[i].Guid == id)
                    Collection.RemoveAt(i);
            }
        }
        public void Remove(History h)
        {
            Collection.Remove(h);
        }
        public bool ContainsID(string id) => Collection.Any(v => v.Guid == id);

        private int FindIndex(History a)
        {
            for (int i = 0; i < Count; i++)
            {
                History b = Collection[i];
                if (a.IsCustom && !b.IsCustom) return i;
                if (b.IsCustom && !a.IsCustom) continue;

                if (b.Timestamp < a.Timestamp)
                    return i;
            }

            return Count;
        }

        public static SaveData Import()
        {
            SaveData? data = null;

            if (File.Exists(Destination))
            {
                try
                {
                    string content = File.ReadAllText(Destination);
                    data = JsonConvert.DeserializeObject<SaveData>(content);
                }
                catch { }
            }

            if (data == null)
                data = new SaveData();

            // Handle old save files
            if (File.Exists(LegacyDestination))
            {
                try
                {
                    string content = File.ReadAllText(LegacyDestination);
                    Dictionary<string, History>? legacy = JsonConvert.DeserializeObject<Dictionary<string, History>>(content);

                    if (legacy != null)
                    {
                        foreach (var item in legacy)
                        {
                            item.Value.SetLogKey(item.Key);
                            data.Add(item.Value);
                        }
                    }

                    // Force exporting to new file format
                    data.Export(true);

                    // Rename the old file
                    File.Move(LegacyDestination, LegacyDestination + ".old", true);
                } catch (Exception e)
                {
                    Debug.WriteLine("Could not import old file.\n\n" + e);
                }
            }

            // Sort them by dates, and keep collections on top
            Program.QuickSort(data.Collection, (a, b) => b.CompareTo(a));

            // Check items that might be the same between collections
            List<Entry> uniqueEntries = new List<Entry>();
            int i, j;
            Entry entry;
            for (i = 0; i < data.Count; i++)
            {
                History item = data[i];
                
                for (j = 0; j < item.Count; j++)
                {
                    entry = item[j];

                    int index = uniqueEntries.FindIndex(v => v.Timestamp == entry.Timestamp);
                    if (index != -1) {
                        entry = uniqueEntries[index];
                        item[j] = entry;
                    } else {
                        uniqueEntries.Add(entry);
                    }
                }
            }
            uniqueEntries.Clear();

            return data;
        }

        public void Export(bool force = false)
        {
            if (!IsDirty && !force) return;

            try
            {
                // Removed indentation to save space and make Serializing faster.
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(Destination, json);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error ocurred while trying to write your saves to a file.\n\nMake sure that the program contains permissions to write files in the current folder it's located at.\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IsDirty = false;
        }
    }
}
