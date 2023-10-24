using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace ToNSaveManager
{
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
        public string Note;
        public bool Fresh;

        public Entry(string content, DateTime timestamp)
        {
            Fresh = true;
            Content = content;
            Timestamp = timestamp;
            Note = string.Empty;
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
