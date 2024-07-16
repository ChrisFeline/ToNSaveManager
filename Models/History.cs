using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;

using LogContext = ToNSaveManager.Utils.LogWatcher.LogContext;

namespace ToNSaveManager.Models
{
    internal class History : IComparable<History>
    {
        internal static string DestinationCustom = "Database/Custom";

        static string m_Destination = "Database";
        internal static string Destination
        {
            get => m_Destination;
            set
            {
                m_Destination = value;
                DestinationCustom = Path.Combine(m_Destination, "Custom");
            }
        }

        internal static void EnsureDestination()
        {
            if (!Directory.Exists(DestinationCustom))
                Directory.CreateDirectory(DestinationCustom);
        }

        #region Persistence
        private string JsonKey => Guid + ".json";
        private string JsonPath => Path.Combine(IsCustom ? DestinationCustom : Destination, JsonKey);

        internal bool IsDirty { get; private set; }
        internal void SetDirty() => IsDirty = true;

        [JsonIgnore] public List<Entry>? m_Database;
        [JsonIgnore] public List<Entry> Database
        {
            get
            {
                if (m_Database == null)
                {
                    string path = JsonPath;

                    if (File.Exists(path))
                    {
                        string content = File.ReadAllText(path);
                        m_Database = JsonConvert.DeserializeObject<List<Entry>>(content) ?? new List<Entry>();
                    }
                    else
                    {
                        m_Database = new List<Entry>();
                    }
                }

                return m_Database;
            }
        }

        internal void Export()
        {
            if (IsDirty && m_Database != null)
            {
                IsDirty = false;
                EnsureDestination();
                string content = JsonConvert.SerializeObject(m_Database);
                File.WriteAllText(JsonPath, content);
            }
        }
        #endregion

        public string Guid = string.Empty;
        public string Name = string.Empty;
        public DateTime Timestamp = DateTime.MinValue;
        public bool IsCustom = false;

        public string? DisplayName = string.Empty;

        [JsonConstructor]
        private History() { }

        public History(string logKey)
        {
            IsCustom = false;
            SetLogKey(logKey);
            SetDirty();
        }

        public History(string name, DateTime timestamp)
        {
            Guid = System.Guid.NewGuid().ToString();
            Name = name;
            Timestamp = timestamp;
            IsCustom = true;
            SetDirty();
        }

        public void SetLogContext(LogContext context)
        {
            DisplayName = context.DisplayName;
            SetDirty();
        }

        public void SetLogKey(string logKey)
        {
            if (IsCustom) return;
            Guid = logKey;

            // "2023-10-22_09-51-29"
            if (DateTime.TryParseExact(logKey, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                Timestamp = date;

            SetDirty();
        }


        [Obsolete] public List<Entry> Entries = new List<Entry>();

        public Entry this[int i]
        {
            get
            {
                if (i < 0 || i >= Database.Count) throw new IndexOutOfRangeException();
                return Database[i];
            }
            set
            {
                if (i < 0 || i >= Database.Count) throw new IndexOutOfRangeException();
                Database[i] = value;
            }
        }

        private int FindIndex(string content, DateTime timestamp)
        {
            timestamp = timestamp.ToUniversalTime();
            for (int i = 0; i < Database.Count; i++)
            {
                Entry e = Database[i]; // Prevent doubles
                if (e.Content.Equals(content, StringComparison.OrdinalIgnoreCase))
                    return -1;

                if (e.Timestamp.ToUniversalTime() < timestamp)
                    return i;
            }

            return Database.Count;
        }

        public int Add(Entry entry)
        {
            int index = FindIndex(entry.Content, entry.Timestamp);
            if (index > -1)
            {
                Database.Insert(index, entry);
                SetDirty();
            }
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
            Debug.WriteLine("Inserting Entry: " + entry);

            Database.Insert(index, entry);
            SetDirty();

            return index;
        }

        public override string ToString()
        {
            return IsCustom ? Name : Timestamp.ToString(EntryDate.GetDateFormat());
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
