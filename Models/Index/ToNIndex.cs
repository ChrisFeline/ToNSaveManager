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
        [JsonProperty("x")] public HashSet<ulong> SpecialSnowflakes = new();

        [JsonProperty("m")] public Dictionary<int, Map> Maps { get; private set; } = new();
        [JsonProperty("t")] public Dictionary<int, Terror> Terrors { get; private set; } = new();
        [JsonProperty("a")] public Dictionary<int, Terror> Alternates { get; private set; } = new();

        [JsonProperty("n")] public Dictionary<int, Terror> Moons { get; private set; } = new();
        [JsonProperty("s")] public Dictionary<int, Terror> Specials { get; private set; } = new();
        [JsonProperty("e")] public Dictionary<int, Terror> Events { get; private set; } = new();

        [JsonProperty("r")] public Dictionary<int, int[]> EightPRedirect { get; private set; } = new();
        [JsonProperty("p")] public Dictionary<int, Terror> EightPages { get; private set; } = new();

        [JsonProperty("u")] public Dictionary<int, Terror> Unbound { get; private set; } = new();

        [JsonProperty("i")] public Dictionary<string, Item> Items { get; private set; } = new();

        [JsonIgnore] private List<Terror>? m_AllTerrors { get; set; }
        [JsonIgnore]
        public List<Terror> AllTerrors {
            get {
                if (m_AllTerrors == null) {
                    // Javascript.NET ??? tf?
                    m_AllTerrors = [
                        .. Terrors.Values,
                        .. Alternates.Values,
                        .. Moons.Values,
                        .. Specials.Values,
                        .. Events.Values,
                        .. EightPages.Values,
                        .. Unbound.Values,
                    ];
                }

                return m_AllTerrors;
            }
        }
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

        /// <summary>
        /// Get Item information based on it's object name.
        /// </summary>
        public Item? GetItem(string? key) {
            if (string.IsNullOrEmpty(key)) return Item.Empty;
            return Items.TryGetValue(key, out Item? item) ? item : null;
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
            public int ID => Id; // oops, facilitate JS access to this value

#if NO_SPOILERS
            [JsonIgnore] private string m_Name { get; set; } = string.Empty;
            [JsonProperty("n", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Name { get => "SPOILERS"; set => m_Name = value; }
#else
            [JsonProperty("n", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Name { get; set; } = string.Empty;
#endif

            [JsonProperty("c", DefaultValueHandling = DefaultValueHandling.Ignore)] public Color Color { get; set; } = Color.White;
        }
#endregion

        #region Model Classes
        public class Terror : EntryBase {
            public static readonly Terror Empty = new Terror() { IsEmpty = true, Id = byte.MaxValue };
            public static readonly Terror Zero = new Terror() { IsEmpty = true, Id = 0 };

            [JsonProperty("t", DefaultValueHandling = DefaultValueHandling.Ignore)] public string? InternalName { get; set; } // Possible name attached to Enrage states.
            [JsonProperty("b", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool CantBB { get; set; } // Can't participate in bb
            [JsonProperty("s", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool Stunnable { get; set; }
            [JsonProperty("g", DefaultValueHandling = DefaultValueHandling.Ignore)] public TerrorGroup Group { get; set; }
            [JsonProperty("p", DefaultValueHandling = DefaultValueHandling.Ignore)] public PhaseIndex[]? Phases { get; set; }
            [JsonProperty("e", DefaultValueHandling = DefaultValueHandling.Ignore)] public Encounter[]? Encounters { get; set; }
            [JsonProperty("v", DefaultValueHandling = DefaultValueHandling.Ignore)] public Dictionary<ToNRoundType, string>? Variants { get; set; }

            [JsonIgnore] public string? AssetID => $"icon_{(int)Group}_{Id}";

            public override string ToString() => Name;

            public struct PhaseIndex {
                [JsonProperty("n")] public string Name { get; set; }
                [JsonProperty("k")] public string Keyword { get; set; }
                [JsonProperty("b")] public bool Increment { get; set; }
            }

            public struct Encounter {
                [JsonProperty("n")] public string Name { get; set; }
                [JsonProperty("s")] public string Suffix { get; set; } // for osc, for example: HHI
                [JsonProperty("k")] public string Keyword { get; set; }
                [JsonProperty("r")] public ToNRoundType RoundType { get; set; }
            }
        }

        public class Map : EntryBase
        {
            public static readonly Map Empty = new Map() { IsEmpty = true, Id = byte.MaxValue, Name = "Overseer's Court" };

            [JsonProperty("t", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Creator = string.Empty;
            [JsonProperty("o", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Origin = string.Empty;
            [JsonProperty("e", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool EightP = false;

            public override string ToString() => Id + ". " + Name;
        }

        public class Item : EntryBase
        {
            public enum StoreType {
                EnkephalinShop, // 0
                SurvivalShop,   // 1
                EventShop,      // 2
                SpecialEvents,  // 3 - Like winterfest and such
                RoleItems,      // 4 - Contributors and such
            }

            public static readonly Item Empty = new Item() { IsEmpty = true, Id = byte.MinValue };

            [JsonProperty("s", DefaultValueHandling = DefaultValueHandling.Ignore)] public StoreType Store { get; set; } = StoreType.EnkephalinShop;
            [JsonProperty("p", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Points { get; set; } = 0;
            [JsonProperty("u", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Unlock { get; set; } = 0;
            [JsonProperty("v", DefaultValueHandling = DefaultValueHandling.Ignore)] public int[] Variants { get; set; } = Array.Empty<int>();
        }

        public struct TerrorInfo
        {
            public static readonly TerrorInfo Empty = new TerrorInfo() { Group = TerrorGroup.Terrors, Index = byte.MaxValue, IsEmpty = true };

            [JsonProperty("i")] public int Index { get; set; }
            [JsonProperty("r", DefaultValueHandling = DefaultValueHandling.Ignore)] public ToNRoundType RoundType { get; set; }
            [JsonProperty("g", DefaultValueHandling = DefaultValueHandling.Ignore)] public TerrorGroup Group { get; set; }
            [JsonProperty("p", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Phase { get; set; }
            [JsonProperty("e", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Encounter { get; set; } = -1;
            [JsonProperty("l", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Level { get; set; } = 1;

            [JsonIgnore] private Terror? m_Value { get; set; }
            [JsonIgnore] public Terror Value { get => m_Value ?? (m_Value = Instance.GetTerror(this)); }
            [JsonIgnore] public string Name {
                get {
                    string name = "???";

                    if (Value.IsEmpty)
                        return name;
                    else if (Value.Variants != null && Value.Variants.ContainsKey(RoundType))
                        name = Value.Variants[RoundType];
                    else if (Encounter > -1 && Value.Encounters != null && Value.Encounters.Length > 0 && Encounter < Value.Encounters.Length)
                        name = Value.Encounters[Encounter].Name;
                    else if (Phase > 0 && Value.Phases != null && Value.Phases.Length > 0 && Phase <= Value.Phases.Length)
                        name = Value.Phases[Phase - 1].Name;
                    else
                        name = Value.Name;

                    if (Level > 1) name += $" (LVL {Level})";

#if NO_SPOILERS
                    name = "SPOILERS";
#endif

                    return name;
                }
            }

            [JsonIgnore] public string? AssetID {
                get {
                    if (Value.IsEmpty) return null; // replace for something
                    else if (Value.Variants != null && Value.Variants.ContainsKey(RoundType)) return Value.AssetID + "_v" + (int)RoundType;
                    else if (Encounter > -1 && Value.Encounters != null && Value.Encounters.Length > 0 && Encounter < Value.Encounters.Length) return Value.AssetID + "_e" + Encounter;
                    else if (RoundType == ToNRoundType.Midnight && Value.Group == TerrorGroup.Alternates) return "icon_m_" + Value.Id;

                    return Value.AssetID;
                }
            }

            [JsonIgnore] public bool IsEmpty { get; private set; }

            public TerrorInfo(int index, TerrorGroup group, int phase = 0)
            {
                Index = index;
                Group = group;
                Phase = phase;
                IsEmpty = index == byte.MaxValue;
            }

            public override string ToString() {
                return Index.ToString();
            }
        }
#endregion
    }
}
