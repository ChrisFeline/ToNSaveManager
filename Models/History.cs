using Newtonsoft.Json;
using System.Globalization;

namespace ToNSaveManager.Models
{
    internal class History : IComparable<History>
    {
        public string Guid = string.Empty;
        public string Name = string.Empty;
        public DateTime Timestamp = DateTime.MinValue;
        public bool IsCustom = false;

        [JsonConstructor]
        private History() { }

        public History(string logKey)
        {
            IsCustom = false;
            SetLogKey(logKey);
        }

        public History(string name, DateTime timestamp)
        {
            Guid = System.Guid.NewGuid().ToString();
            Name = name;
            Timestamp = timestamp;
            IsCustom = true;
        }

        public void SetLogKey(string logKey)
        {
            if (IsCustom) return;
            Guid = logKey;

            // "2023-10-22_09-51-29"
            if (DateTime.TryParseExact(logKey, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                Timestamp = date;

            Name = Timestamp.ToString(Entry.DateFormat);
        }

        public List<Entry> Entries = new List<Entry>();
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

        public int Add(Entry entry)
        {
            int index = FindIndex(entry.Content, entry.Timestamp);
            if (index > -1) Entries.Insert(index, entry);
            return index;
        }

        public int Add(string content, DateTime timestamp, out Entry? entry)
        {
            int index = FindIndex(content, timestamp);
            if (index < 0)
            {
                entry = null;
                return index;
            }

            entry = new Entry(content, timestamp);
            Entries.Insert(index, entry);

            return index;
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
}
