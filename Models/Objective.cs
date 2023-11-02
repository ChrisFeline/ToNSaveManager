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

        [JsonConstructor]
        public Objective() { }

        /// <param name="name">Objective name</param>
        /// <param name="reference">Objective link to wiki</param>
        public Objective(string name, string description, string reference)
        {
            Name = name;
            Description = description;
            Reference = reference;
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
