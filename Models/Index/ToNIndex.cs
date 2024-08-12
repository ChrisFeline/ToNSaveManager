using Newtonsoft.Json;
using System;

namespace ToNSaveManager.Models.Index {
    internal class ToNIndex {
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

        #region Properties & Fields
        [JsonProperty("m")] public Dictionary<int, Map> Maps { get; private set; } = new();
        [JsonProperty("t")] public Dictionary<int, Terror> Terrors { get; private set; } = new();
        [JsonProperty("a")] public Dictionary<int, Terror> Alternates { get; private set; } = new();

        [JsonProperty("n")] public Dictionary<int, Terror> Moons { get; private set; } = new();
        [JsonProperty("s")] public Dictionary<int, Terror> Specials { get; private set; } = new();
        [JsonProperty("e")] public Dictionary<int, Terror> Events { get; private set; } = new();

        [JsonProperty("er")] public Dictionary<int, int[]> EightPRedirect { get; private set; } = new();
        [JsonProperty("ep")] public Dictionary<int, Terror> EightPages { get; private set; } = new();

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
            Terrors,
            Alternates,
            EightPages,
            Unbound,
            Moons,
            Specials,
            Events,
            Encounter
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
            [JsonProperty("c", DefaultValueHandling = DefaultValueHandling.Ignore)] public Color Color { get; set; }
            [JsonProperty("n", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Name { get; set; } = string.Empty;
        }
        #endregion

        #region Model Classes
        public class Terror : EntryBase
        {
            public static readonly Terror Empty = new Terror() { IsEmpty = true };

            [JsonProperty("b", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool CantBB { get; set; } // Can't participate in bb
            [JsonProperty("g", DefaultValueHandling = DefaultValueHandling.Ignore)] public TerrorGroup Group { get; set; }
            [JsonProperty("k", DefaultValueHandling = DefaultValueHandling.Ignore)] public string? Keyword { get; set; }

            public override string ToString() => Name;
        }

        public class Map : EntryBase
        {
            public static readonly Map Empty = new Map() { IsEmpty = true };

            [JsonProperty("t", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Creator = string.Empty;
            [JsonProperty("o", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Origin = string.Empty;
            [JsonProperty("e", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool EightP = false;

            public override string ToString() => Name;
        }

        public class Item : EntryBase
        {
            public static readonly Item Empty = new Item() { IsEmpty = true };

            [JsonProperty("d", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Description { get; set; } = string.Empty;
            [JsonProperty("u", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Usage { get; set; } = string.Empty;
            [JsonProperty("s", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Store { get; set; } // Point Shop | Survival Shop | Secret Shop
        }

        public struct TerrorInfo
        {
            public static readonly TerrorInfo Empty = new TerrorInfo() { Group = TerrorGroup.Terrors, Index = 0 };

            [JsonProperty("i")] public int Index;
            [JsonProperty("g")] public TerrorGroup Group;

            public TerrorInfo(int index, TerrorGroup group)
            {
                Index = index;
                Group = group;
            }
        }
        #endregion
    }
}
