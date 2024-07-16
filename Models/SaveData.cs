using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace ToNSaveManager.Models
{
    internal class SaveData
    {
        const string LegacyDestination = "data.json";
        const string FileName = "SaveData.json";
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
                History.Destination = Path.Combine(Path.GetDirectoryName(m_Destination) ?? "./", "Database");
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
            if (reset)
            {
                if (string.IsNullOrEmpty(Settings.Get.DataLocation)) return;
                selectedFolder = DefaultLocation;
            }
            else
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.SelectedPath = Path.GetDirectoryName(Destination) ?? string.Empty;
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result != DialogResult.OK) return;

                selectedFolder = folderBrowserDialog.SelectedPath;
                selectedFolder = Path.Combine(selectedFolder, FileName);
            }

            try
            {
                // Make a backup of this save file, just in case
                if (File.Exists(Destination))
                {
                    File.Copy(Destination, Path.Combine(Path.GetDirectoryName(Settings.Destination) ?? string.Empty, FileName + ".backup_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
                    File.Move(Destination, selectedFolder);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error ocurred while trying to copy your files to the selected location.\n\nMake sure that the program contains permissions to write files to the destination.\nPath: {selectedFolder}\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Destination = selectedFolder;
            Settings.Get.DataLocation = reset ? null : Destination;
            Settings.Export();

            if (reset) MessageBox.Show("Save data location has been reset to default.", "Reset Custom Data Location");
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
                    Debug.WriteLine("Read from legacy...");
                }

                destination = DefaultLocation;
            }


            SaveData? data = null;

            string filePath = string.Empty;
            try
            {
                if (File.Exists(Destination) && !File.Exists(destination))
                {
                    File.Copy(Destination, destination, true);
                    File.Move(Destination, Destination + ".old");
                }

                Destination = destination;
                filePath = readFromLegacy ? LegacyLocation : Destination;
                Debug.WriteLine("Reading from: " + filePath);

                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    data = JsonConvert.DeserializeObject<SaveData>(content);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to import your save:\n\n" + ex, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        File.Copy(filePath, filePath + $".backup-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}");
                } catch
                {
                    Application.Exit();
                    return new SaveData();
                }
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
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Could not import old file.\n\n" + e);
                }
            }

            // Sort them by dates, and keep collections on top
            data.Collection.Sort((a, b) => b.CompareTo(a));

#pragma warning disable CS0612
            // Check items that might be the same between collections
            List<Entry> uniqueEntries = new List<Entry>();
            int i, j;
            Entry entry;
            for (i = 0; i < data.Count; i++)
            {
                History item = data[i];

                for (j = 0; j < item.Entries.Count; j++)
                {
                    entry = item.Entries[j];

                    int index = uniqueEntries.FindIndex(v => v.Timestamp == entry.Timestamp);
                    if (index != -1)
                    {
                        entry = uniqueEntries[index];
                        item.Entries[j] = entry;
                    }
                    else
                    {
                        uniqueEntries.Add(entry);
                    }
                }
            }
            uniqueEntries.Clear();

            if (data.Objectives.Count > 0)
            {
                Debug.WriteLine("Importing old objectives...");

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
                MessageBox.Show($"An error ocurred while trying to write your saves to a file.\n\nMake sure that the program contains permissions to write files to the destination.\nPath: {Destination}\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /*
        private static void GetDefaultObjectives()
        {
            return new List<Objective>() // Initialize default objectives
            {
                // Event items
                Objective.Separator("Event Items Unlocks"),
                new Objective("Sealed Sword", "Found in Museum. Break case with a stun tool.", "https://terror.moe/items/sealed_sword", Color.FromArgb(224, 208, 183)),
                new Objective("Gran Faust", "Survive Arkus with the Sealed Sword.", "https://terror.moe/items/gran_faust", Color.FromArgb(255, 95, 198)),
                new Objective("Divine Avenger", "Survive Arkus with the Sealed Sword after hitting them at least two times.", "https://terror.moe/items/divine_avenger", Color.FromArgb(255, 213, 90)),
                new Objective("Maxwell", "Found in Its Maze. (spawns once per round)", "https://terror.moe/items/maxwell", Color.FromArgb(212, 212, 212)),
                new Objective("Rock", "Survive Fusion Pilot.", "https://terror.moe/items/rock", Color.FromArgb(180, 180, 180)),
                new Objective("Illumina", "Survive Bliss.", "https://terror.moe/items/illumina", Color.FromArgb(215, 134, 255)),
                new Objective("Redbull", "Survive Roblander.", "https://terror.moe/items/redbull", Color.FromArgb(255, 61, 61)),
                new Objective("Omori Plush", "Survive Something.", "https://terror.moe/items/omori_plush", Color.FromArgb(125, 133, 159)),
                new Objective("Paradise Lost", "Beat the shit out of Apostles.", "https://terror.moe/items/paradise_lost", Color.FromArgb(255, 94, 93)),
                // Skin Unlocks
                Objective.Separator("Item Skin Unlocks"),
                new Objective("Red Medkit", "Survive Virus with Medkit.", "https://terror.moe/items/medkit", Color.FromArgb(255, 48, 42)),
                new Objective("Psycho Coil", "Survive Psychosis with Glow Coil.", "https://terror.moe/items/glow_coil", Color.FromArgb(158, 184, 255)),
                new Objective("Bloody Teleporter", "Survive a Bloodbath round with Teleporter.", "https://terror.moe/items/teleporter", Color.FromArgb(255, 77, 71)),
                new Objective("Pale Suitcase", "Survive an Alternate round with Teleporter.", "https://terror.moe/items/teleporter", Color.FromArgb(158, 254, 255)),
                new Objective("Bloody Coil", "Survive a Bloodbath round with Speed Coil.", "https://terror.moe/items/speed_coil", Color.FromArgb(255, 77, 71)),
                new Objective("Bloody Bat", "Survive a Bloodbath round with Metal Bat.", "https://terror.moe/items/metal_bat", Color.FromArgb(255, 77, 71)),
                new Objective("Metal Pipe", "Survive an Alternate round with Metal Bat.", "https://terror.moe/items/metal_bat", Color.FromArgb(197, 197, 197)),
                new Objective("Colorable Bat", "Survive a Cracked round with Metal Bat.", "https://terror.moe/items/metal_bat", Color.FromArgb(187, 76, 255)),
                new Objective("Justitia", "Survive a Midnight round with Metal Bat.", "https://terror.moe/items/metal_bat", Color.FromArgb(127, 242, 243)),
                new Objective("Twilight Coil", "Survive Apocalypse Bird with Chaos Coil.", "https://terror.moe/items/chaos_coil", Color.FromArgb(255, 232, 158)),
                new Objective("Pale Pistol", "Survive an Alternate round with Antique Revolver.", "https://terror.moe/items/antique_revolver", Color.FromArgb(158, 254, 255))
            };
        }
        */
    }
}
