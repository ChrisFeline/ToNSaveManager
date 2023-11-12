using Newtonsoft.Json;

namespace ToNSaveManager.Models
{
    internal class Objective
    {
        public bool IsCompleted { get; set; } = false;
        [JsonIgnore] public string Name { get; set; } = string.Empty;
        [JsonIgnore] public string Description { get; set; } = string.Empty;
        [JsonIgnore] public string Reference { get; set; } = string.Empty;
        [JsonIgnore] public bool IsSeparator { get; set; } = false;
        [JsonIgnore] public Color DisplayColor { get; set; }

        [JsonConstructor]
        public Objective() { }

        /// <param name="name">Objective name</param>
        /// <param name="reference">Objective link to wiki</param>
        public Objective(string name, string description, string reference, Color color)
        {
            Name = name;
            Description = description;
            Reference = reference;
            DisplayColor = color.A > 0 ? color : Color.White;
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
            return Name;
        }

        public bool ShouldSerializeIsCompleted() => !IsSeparator;
    }
}
