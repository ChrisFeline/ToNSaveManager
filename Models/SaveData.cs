using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using ToNSaveManager.Localization;

namespace ToNSaveManager.Models
{
    internal class SaveData
    {
        const int CURRENT_VERSION = 2;

        const string LegacyDestination = "data.json";
        const string LegacyFileName = "SaveData.json";
        const string FileName = "SaveIndex.json";
        static string DefaultLocation = Path.Combine(Program.DataLocation, FileName);
        static string LegacyLocation = Path.Combine(Program.LegacyDataLocation, FileName);
        static string LegacyLocationOld = Path.Combine(Program.LegacyDataLocation + "_Old", FileName);

        static string m_Destination { get; set; } = FileName;
        internal static string Destination
        {
            get => m_Destination;
            set
            {
                m_Destination = value;
                string? dirName = Path.GetDirectoryName(m_Destination);
                History.Destination = Path.Combine(dirName ?? "./", "Database");
                StatsData.Destination = dirName ?? "./";
            }
        }
        
        public Dictionary<string, long> ParsedLog { get; private set; } = new Dictionary<string, long>();
        public List<History> Collection { get; private set; } = new List<History>();

        #region Objectives
        public HashSet<string> Completed = new HashSet<string>();

        public bool GetCompleted(string name) => Completed.Contains(name);
        public void SetCompleted(string name, bool value)
        {
            if (!string.IsNullOrEmpty(name) && value == !Completed.Contains(name))
            {
                if (value) Completed.Add(name);
                else Completed.Remove(name);

                SetDirty();
            }
        }

        [Obsolete] // Legacy Objectives
        public List<LegacyObjective> Objectives { get; private set; } = new ();
        public bool ShouldSerializeObjectives() => false;
        #endregion

        public int Version = 0;
        public static SaveData Empty => new SaveData() { Version = CURRENT_VERSION };

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

        public int Add(History h)
        {
            int index = FindIndex(h);
            Collection.Insert(index, h);
            return index;
        }
        public void Remove(string id)
        {
            for (int i = 0; i < Count; i++)
            {
                History c = Collection[i];
                if (c.Guid == id)
                {
                    Collection.RemoveAt(i);
                    if (!c.IsCustom && ParsedLog.ContainsKey(id)) ParsedLog.Remove(id);
                }
            }
        }
        public void Remove(History h)
        {
            Collection.Remove(h);
            if (!h.IsCustom && ParsedLog.ContainsKey(h.Guid)) ParsedLog.Remove(h.Guid);
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

        public long GetParsedPos(string name) => ParsedLog.ContainsKey(name) ? Math.Max(0, ParsedLog[name]) : 0;
        public void SetParsedPos(string name, long pos, bool save)
        {
            if (pos < 0)
            {
                ParsedLog.Remove(name);
            } else
            {
                ParsedLog[name] = pos;
            }

            if (save) SetDirty();
        }

        public static void OpenDataLocation()
        {
            MainWindow.OpenExternalLink(Path.GetDirectoryName(Destination) ?? string.Empty);
        }
        public static void SetDataLocation(bool reset)
        {
            string selectedFolder;
            string selDir;
            if (reset)
            {
                if (string.IsNullOrEmpty(Settings.Get.DataLocation)) return;
                selectedFolder = DefaultLocation;
                selDir = Program.DataLocation;
            }
            else
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.SelectedPath = Path.GetDirectoryName(Destination) ?? string.Empty;
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result != DialogResult.OK) return;

                selDir = selectedFolder = folderBrowserDialog.SelectedPath;
                selectedFolder = Path.Combine(selectedFolder, FileName);
            }

            try {
                // Make a backup of this save file, just in case
                if (File.Exists(Destination)) {
                    File.Copy(Destination, Path.Combine(Path.GetDirectoryName(Settings.Destination) ?? string.Empty, FileName + ".backup_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
                    File.Move(Destination, selectedFolder, true);
                }

                string currentDatabase = History.Destination;
                string currentDatabaseCustom = History.DestinationCustom;
                string databaseFolder = Path.Combine(selDir, "Database");
                string databaseFolderCustom = Path.Combine(selDir, "Database", "Custom");

                if (!Directory.Exists(databaseFolderCustom))
                    Directory.CreateDirectory(databaseFolderCustom);

                string[] files = Directory.GetFiles(currentDatabase);
                foreach (string file in files) {
                    string filename = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(databaseFolder, filename), true);
                }
                files = Directory.GetFiles(currentDatabaseCustom);
                foreach (string file in files) {
                    string filename = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(databaseFolderCustom, filename), true);
                }

                Directory.Move(currentDatabase, currentDatabase + ".backup_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            }
            catch (Exception e) {
                MessageBox.Show(LANG.S("MESSAGE.COPY_FILES_ERROR", selectedFolder) ?? $"An error ocurred while trying to copy your files to the selected location.\n\nMake sure that the program contains permissions to write files to the destination.\nPath: {selectedFolder}" + "\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Destination = selectedFolder;
            Settings.Get.DataLocation = reset ? null : Destination;
            Settings.Export();

            if (reset) MessageBox.Show( LANG.S("MESSAGE.SAVE_LOCATION_RESET") ?? "Save data location has been reset to default.", LANG.S("MESSAGE.SAVE_LOCATION_RESET.TITLE") ?? "Reset Custom Data Location");
        }

        public static SaveData Import()
        {
            string? destination = Settings.Get.DataLocation;

            bool readFromLegacy = false;
            if (string.IsNullOrEmpty(destination))
            {
                if (!File.Exists(DefaultLocation) && (File.Exists(LegacyLocation) || File.Exists(LegacyLocation = LegacyLocationOld)))
                {
                    readFromLegacy = true;
                    Logger.Debug("Read from legacy...");
                }

                destination = DefaultLocation;
            }


            SaveData? data = null;

            string filePath = string.Empty;
            try
            {
                bool noDest = !File.Exists(destination);
                if (File.Exists(Destination) && noDest)
                {
                    File.Copy(Destination, destination, true);
                    File.Move(Destination, Destination + ".old");
                }

                Destination = destination;
                filePath = readFromLegacy ? LegacyLocation : Destination;
                Logger.Debug("Reading from: " + filePath);

                string legacyData = Path.Combine(Path.GetDirectoryName(Destination) ?? string.Empty, LegacyFileName);
                if (File.Exists(legacyData) && noDest)
                    File.Copy(legacyData, Destination, false);

                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    data = JsonConvert.DeserializeObject<SaveData>(content);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show((LANG.S("MESSAGE.IMPORT_SAVE_ERROR") ?? "Error trying to import your save.") + "\n\n" + ex, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (!Program.CreateFileBackup(filePath))
                {
                    Application.Exit();
                    return new SaveData();
                }
            }

            if (data == null)
                data = new SaveData();

            if (data.Version != CURRENT_VERSION)
            {
                Program.CreateFileBackup(filePath);
                data.Version = CURRENT_VERSION;
                data.SetDirty();
            }

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
                    data.SetDirty();

                    // Rename the old file
                    File.Move(LegacyDestination, LegacyDestination + ".old", true);
                }
                catch (Exception e)
                {
                    Logger.Debug("Could not import old file.\n\n" + e);
                }
            }

            // Sort them by dates, and keep collections on top
            data.Collection.Sort((a, b) => b.CompareTo(a));

#pragma warning disable CS0612
            // Check items that might be the same between collections
            int i, j;
            Entry entry;
            for (i = 0; i < data.Count; i++)
            {
                History item = data[i];
                if (item.Entries.Count == 0) continue;

                for (j = 0; j < item.Entries.Count; j++)
                {
                    entry = item.Entries[j];
                    item.Add(entry);

                    int index = History.UniqueEntries.FindIndex(v => v.Timestamp == entry.Timestamp);
                    if (index != -1)
                    {
                        entry = History.UniqueEntries[index];
                        item.Entries[j] = entry;
                    }
                    else
                    {
                        History.UniqueEntries.Add(entry);
                    }
                }

                item.Entries.Clear();
                data.SetDirty();
            }

            if (data.Objectives.Count > 0)
            {
                Logger.Debug("Importing old objectives...");

                LegacyObjective[] defaultObjectives = GetLegacyObjectives();
                for (i = 0; i < defaultObjectives.Length; i++)
                {
                    if (i < data.Objectives.Count)
                    {
                        LegacyObjective o = data.Objectives[i];
                        data.SetCompleted(defaultObjectives[i].Name, o.IsCompleted);
                    }
                }

                data.Objectives.Clear();
                data.SetDirty();
            }
#pragma warning restore CS0612

            data.Export();
            return data;
        }

        public void Export(bool force = false)
        {
            try
            {
                if (IsDirty || force)
                {
                    IsDirty = false;
                    // Removed indentation to save space and make Serializing faster.
                    string json = JsonConvert.SerializeObject(this);
                    File.WriteAllText(Destination, json);
                }

                foreach (History h in Collection)
                {
                    h.Export();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show((LANG.S("MESSAGE.WRITE_SAVE_ERROR", Destination) ?? $"An error ocurred while trying to write your saves to a file.\n\nMake sure that the program contains permissions to write files to the destination.\nPath: {Destination}") + "\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static LegacyObjective[] GetLegacyObjectives()
        {
            return new LegacyObjective[] // Initialize default objectives
            {
                // Event items
                new LegacyObjective() { Name = string.Empty }, // Separator
                new LegacyObjective() { Name = "Sealed Sword" },
                new LegacyObjective() { Name = "Gran Faust" },
                new LegacyObjective() { Name = "Divine Avenger" },
                new LegacyObjective() { Name = "Maxwell" },
                new LegacyObjective() { Name = "Rock" },
                new LegacyObjective() { Name = "Illumina" },
                new LegacyObjective() { Name = "Redbull" },
                new LegacyObjective() { Name = "Omori Plush" },
                new LegacyObjective() { Name = "Paradise Lost" },
                // Skin Unlocks
                new LegacyObjective() { Name = string.Empty }, // Separator
                new LegacyObjective() { Name = "Red Medkit" },
                new LegacyObjective() { Name = "Psycho Coil" },
                new LegacyObjective() { Name = "Bloody Teleporter" },
                new LegacyObjective() { Name = "Pale Suitcase" },
                new LegacyObjective() { Name = "Bloody Coil" },
                new LegacyObjective() { Name = "Bloody Bat" },
                new LegacyObjective() { Name = "Metal Pipe" },
                new LegacyObjective() { Name = "Colorable Bat" },
                new LegacyObjective() { Name = "Justitia" },
                new LegacyObjective() { Name = "Twilight Coil" },
                new LegacyObjective() { Name = "Pale Pistol" },
            };
        }
    }
}
