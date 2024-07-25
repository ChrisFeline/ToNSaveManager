using Newtonsoft.Json;

namespace ToNSaveManager.Models
{
    internal struct LegacyObjective
    {
        public bool IsCompleted;
        public string Name;

        public LegacyObjective()
        {
            IsCompleted = false;
            Name = string.Empty;
        }
    }

    internal class Objective
    {
        [JsonIgnore] public string? DisplayName { get; set; }
        [JsonIgnore] public string? Tooltip { get; set; }

        [JsonIgnore] public bool IsCompleted {
            get => !IsSeparator && MainWindow.SaveData.GetCompleted(Name);
            set
            {
                if (!IsSeparator) MainWindow.SaveData.SetCompleted(Name, value);
            }
        }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public bool IsSeparator { get; set; } = false;
        public Color DisplayColor { get; set; }

        [JsonConstructor]
        public Objective() {}

        /// <param name="name">Objective name</param>
        /// <param name="reference">Objective link to wiki</param>
        public Objective(string name, string description, string reference, Color color)
        {
            Name = name;
            Description = description;
            Reference = reference;
            DisplayColor = color.A > 0 ? color : Color.White;
        }

        public void Toggle()
        {
            IsCompleted = !IsCompleted;
        }

        public static Objective Separator(string name)
        {
            return new Objective()
            {
                Name = name,
                IsSeparator = true
            };
        }


        public override string ToString()
        {
            return string.IsNullOrEmpty(DisplayName) ? Name : DisplayName;
        }

        public static List<Objective> ImportFromMemory()
        {
            using (Stream? stream = Program.GetEmbededResource("objectives.json"))
            {
                if (stream == null) return new List<Objective>();

                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<Objective>>(json) ?? new List<Objective>();
                }
            }
        }
    }
}
