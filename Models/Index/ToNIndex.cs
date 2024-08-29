using Newtonsoft.Json;
using System.Numerics;

namespace ToNSaveManager.Models.Index {
    internal class ToNIndex {
#if !UNITY_EDITOR
        #region Import From Memory
        public static readonly ToNIndex Instance = Import();

        public static ToNIndex Import()
        {
            using (Stream? stream = Program.GetEmbededResource("index.json"))
            {
                if (stream == null) return new ToNIndex();

                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<ToNIndex>(json) ?? new ToNIndex();
                }
            }
        }
        #endregion
#endif

        #region Properties & Fields
        [JsonProperty("m")] public Dictionary<int, Map> Maps { get; private set; } = new();
        [JsonProperty("t")] public Dictionary<int, Terror> Terrors { get; private set; } = new();
        [JsonProperty("a")] public Dictionary<int, Terror> Alternates { get; private set; } = new();

        [JsonProperty("n")] public Dictionary<int, Terror> Moons { get; private set; } = new();
        [JsonProperty("s")] public Dictionary<int, Terror> Specials { get; private set; } = new();
        [JsonProperty("e")] public Dictionary<int, Terror> Events { get; private set; } = new();

        [JsonProperty("r")] public Dictionary<int, int[]> EightPRedirect { get; private set; } = new();
        [JsonProperty("p")] public Dictionary<int, Terror> EightPages { get; private set; } = new();

        [JsonProperty("u")] public Dictionary<int, Terror> Unbound { get; private set; } = new();

        [JsonProperty("c")] public Dictionary<int, Terror> Encounters { get; private set; } = new();

        [JsonProperty("i")] public Dictionary<int, Item> Items { get; private set; } = new();
        #endregion

        #region Index Access Methods
        /// <summary>
        /// Get Terror information based on it's index and group ID.
        /// </summary>
        /// <param name="terrorIndex">The Terror's index ID.</param>
        /// <param name="terrorGroup">The Terror's group flag.</param>
        public Terror GetTerror(int terrorIndex, TerrorGroup terrorGroup = TerrorGroup.Terrors)
        {
            Dictionary<int, Terror> table;
            switch (terrorGroup)
            {
                default:
                case TerrorGroup.Terrors: table = Terrors; break;
                case TerrorGroup.Alternates: table = Alternates; break;

                case TerrorGroup.EightPages:
                    // Handle 8 Pages Redirect
                    if (EightPages.ContainsKey(terrorIndex)) return EightPages[terrorIndex];
                    else if (EightPRedirect.ContainsKey(terrorIndex))
                    {
                        int[] redirect = EightPRedirect[terrorIndex];
                        return GetTerror(redirect[1], (TerrorGroup)redirect[0]);
                    }
                    else return Terror.Empty;

                case TerrorGroup.Unbound: table = Unbound; break;
                case TerrorGroup.Moons: table = Moons; break;
                case TerrorGroup.Specials: table = Specials; break;
                case TerrorGroup.Events: table = Events; break;
                case TerrorGroup.Encounter: table = Encounters; break;
            }
            return table.ContainsKey(terrorIndex) ? table[terrorIndex] : Terror.Empty;
        }

        /// <summary>
        /// Get Terror information based on it's index and group ID.
        /// </summary>
        /// <param name="terrorIndex">The Terror's index ID.</param>
        /// <param name="terrorGroup">The Terror's group ID.</param>
        public Terror GetTerror(int terrorIndex, int terrorGroupId) => GetTerror(terrorIndex, (TerrorGroup)terrorGroupId);

        /// <summary>
        /// Gets Terror information using a TerrorInfo container.
        /// </summary>
        /// <param name="terrorInfo"></param>
        /// <returns></returns>
        public Terror GetTerror(TerrorInfo terrorInfo) => GetTerror(terrorInfo.Index, terrorInfo.Group);

        /// <summary>
        /// Get Map information based on it's index value.
        /// </summary>
        /// <param name="mapIndex">The map index ID.</param>
        public Map GetMap(int mapIndex)
        {
            return Maps.ContainsKey(mapIndex) ? Maps[mapIndex] : Map.Empty;
        }
        /// <summary>
        /// Get Map information based on it's display name.
        /// </summary>
        /// <param name="mapName"></param>
        public Map GetMap(string mapName)
        {
            foreach (var pair in Maps)
            {
                if (pair.Value.Name == mapName) return pair.Value;
            }

            return Map.Empty;
        }
        #endregion

        #region Enums
        public enum TerrorGroup {
            Terrors,    // 0
            Alternates, // 1
            EightPages, // 2
            Unbound,    // 3
            Moons,      // 4
            Specials,   // 5
            Events,     // 6
            Encounter   // 7
        }
        #endregion

        #region Base Item Classes
        public interface IEntry
        {
            bool IsEmpty { get; set; }

            int Id { get; set; }
            Color Color { get; set; }
            string Name { get; set; }
        }

        public class EntryBase : IEntry
        {
            [JsonIgnore] public bool IsEmpty { get; set; }
            [JsonProperty("i")] public int Id { get; set; }
            [JsonProperty("n", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Name { get; set; } = string.Empty;
            [JsonProperty("c", DefaultValueHandling = DefaultValueHandling.Ignore)] public Color Color { get; set; } = Color.White;

            [JsonIgnore] public Vector3 HSV {
                get {
                    int max = Math.Max(Color.R, Math.Max(Color.G, Color.B));
                    int min = Math.Min(Color.R, Math.Min(Color.G, Color.B));

                    float hue = Color.GetHue() / 360f;
                    float sat = (max == 0) ? 0 : 1f - (1f * min / max);
                    float val = max / 255f;

                    return new Vector3(hue, sat, val);
                }
            }
        }
        #endregion

        #region Model Classes
        public class Terror : EntryBase {
            public static readonly Terror Empty = new Terror() { IsEmpty = true, Id = byte.MaxValue };
            public static readonly Terror Zero = new Terror() { IsEmpty = true, Id = 0 };

            [JsonProperty("b", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool CantBB { get; set; } // Can't participate in bb
            [JsonProperty("g", DefaultValueHandling = DefaultValueHandling.Ignore)] public TerrorGroup Group { get; set; }
            [JsonProperty("k", DefaultValueHandling = DefaultValueHandling.Ignore)] public string? Keyword { get; set; }
            [JsonProperty("p", DefaultValueHandling = DefaultValueHandling.Ignore)] public PhaseIndex[]? Phases { get; set; }

            public override string ToString() => Name;

            public struct PhaseIndex {
                [JsonProperty("n")] public string Name { get; set; }
                [JsonProperty("k")] public string Keyword { get; set; }
            }
        }

        public class Map : EntryBase
        {
            public static readonly Map Empty = new Map() { IsEmpty = true, Id = byte.MaxValue };

            [JsonProperty("t", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Creator = string.Empty;
            [JsonProperty("o", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Origin = string.Empty;
            [JsonProperty("e", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool EightP = false;

            public override string ToString() => Id + ". " + Name;
        }

        public class Item : EntryBase
        {
            public static readonly Item Empty = new Item() { IsEmpty = true, Id = byte.MaxValue };

            [JsonProperty("d", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Description { get; set; } = string.Empty;
            [JsonProperty("u", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Usage { get; set; } = string.Empty;
            [JsonProperty("s", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Store { get; set; } // Point Shop | Survival Shop | Secret Shop
        }

        public struct TerrorInfo
        {
            public static readonly TerrorInfo Empty = new TerrorInfo() { Group = TerrorGroup.Terrors, Index = byte.MaxValue };

            [JsonProperty("i")] public int Index;
            [JsonProperty("g", DefaultValueHandling = DefaultValueHandling.Ignore)] public TerrorGroup Group;
            [JsonProperty("p", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Phase;

            [JsonIgnore] private Terror? m_Value { get; set; }
            [JsonIgnore] public Terror Value { get => m_Value ?? (m_Value = Instance.GetTerror(this)); }
            [JsonIgnore] public string Name => !Value.IsEmpty && Phase > 0 && Value.Phases != null && Value.Phases.Length > 0 && Value.Phases.Length <= Phase ? Value.Phases[Phase - 1].Name : Value.Name;

            public TerrorInfo(int index, TerrorGroup group, int phase = 0)
            {
                Index = index;
                Group = group;
                Phase = phase;
            }

            public override string ToString() {
                return Index.ToString();
            }
        }
        #endregion
    }
}
