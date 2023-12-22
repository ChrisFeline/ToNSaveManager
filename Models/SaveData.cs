using Newtonsoft.Json;
using System.Diagnostics;

namespace ToNSaveManager.Models
{
    internal class SaveData
    {
        const string LegacyDestination = "data.json";
        const string FileName = "SaveData.json";
        static string DefaultLocation = Path.Combine(Program.DataLocation, FileName);
        static string Destination = FileName;
        
        public HashSet<string> ParsedLogs { get; private set; } = new HashSet<string>();
        public List<Objective> Objectives { get; private set; } = new List<Objective>();
        public List<History> Collection { get; private set; } = new List<History>();

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
                    if (!c.IsCustom && ParsedLogs.Contains(id)) ParsedLogs.Remove(id);
                }
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

        public bool WasParsed(string name) => ParsedLogs.Contains(name);
        public void SetParsed(string name)
        {
            if (!WasParsed(name))
            {
                ParsedLogs.Add(name);
                SetDirty();
            }
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

            if (string.IsNullOrEmpty(destination))
                destination = DefaultLocation;

            SaveData? data = null;

            try
            {
                if (File.Exists(Destination) && !File.Exists(destination))
                {
                    File.Copy(Destination, destination, true);
                    File.Move(Destination, Destination + ".old");
                }

                Destination = destination;

                if (File.Exists(Destination))
                {
                    string content = File.ReadAllText(Destination);
                    data = JsonConvert.DeserializeObject<SaveData>(content);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to import your save:\n\n" + ex, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (index != -1)
                    {
                        entry = uniqueEntries[index];
                        item[j] = entry;
                    }
                    else
                    {
                        uniqueEntries.Add(entry);
                    }
                }
            }
            uniqueEntries.Clear();

            // Validate Objectives
            List<Objective> defaultObjectives = GetDefaultObjectives();
            for (i = 0; i < defaultObjectives.Count; i++)
            {
                if (i < data.Objectives.Count)
                    defaultObjectives[i].IsCompleted = data.Objectives[i].IsCompleted;
            }
            data.Objectives = defaultObjectives;

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
                MessageBox.Show($"An error ocurred while trying to write your saves to a file.\n\nMake sure that the program contains permissions to write files to the destination.\nPath: {Destination}\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IsDirty = false;
        }

        internal static List<Objective> GetDefaultObjectives()
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
                new Objective("Pale Pistol", "Survive an Alternate round with Antique Revolver.", "https://terror.moe/items/antique_revolver", Color.FromArgb(158, 254, 255)),
            };
        }
    }
}
