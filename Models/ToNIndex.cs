using Newtonsoft.Json;
using System;

namespace ToNSaveManager.Models
{
    internal struct TerrorMatrix
    {
        const string ROUND_TYPE_ALTERNATE = " (Alternate)";
        internal static TerrorMatrix Empty = new TerrorMatrix();

        public ToNIndex.TerrorInfo[] Terrors;

        public string RoundTypeRaw;
        public ToNRoundType RoundType;
        public bool IsSaboteur;

        public ToNIndex.TerrorInfo Terror1 => Terrors != null && Terrors.Length > 0 ? Terrors[0] : ToNIndex.TerrorInfo.Empty;
        public ToNIndex.TerrorInfo Terror2 => Terrors != null && Terrors.Length > 1 ? Terrors[1] : ToNIndex.TerrorInfo.Empty;
        public ToNIndex.TerrorInfo Terror3 => Terrors != null && Terrors.Length > 2 ? Terrors[2] : ToNIndex.TerrorInfo.Empty;

        public TerrorMatrix()
        {
            RoundTypeRaw = string.Empty;
            RoundType = ToNRoundType.Unknown;
            Terrors = []; // what is this? JavaScript?
        }

        public TerrorMatrix(string roundType, params int[] indexes)
        {
            int index = roundType.IndexOf(ROUND_TYPE_ALTERNATE);
            bool isAlt = index > 0;
            if (isAlt) roundType = roundType.Substring(0, index);

            Terrors = new ToNIndex.TerrorInfo[indexes.Length];
            RoundTypeRaw = GetEngRoundType(roundType, isAlt);
            RoundType = GetRoundType(RoundTypeRaw);

            switch (RoundType)
            {
                // first index only
                case ToNRoundType.Classic:
                case ToNRoundType.Fog:
                case ToNRoundType.Ghost:
                case ToNRoundType.Punished:
                case ToNRoundType.Sabotage:
                case ToNRoundType.Cracked:
                // index is alt
                case ToNRoundType.Alternate:
                case ToNRoundType.Fog_Alternate:
                case ToNRoundType.Ghost_Alternate:
                    Terrors = [ new (indexes[0], RoundType == ToNRoundType.Alternate || isAlt ? ToNIndex.TerrorGroup.Alternates : ToNIndex.TerrorGroup.Terrors) ];
                    break;

                case ToNRoundType.Unbound:
                    Terrors = [ new(indexes[0], ToNIndex.TerrorGroup.Unbound) ];
                    break;

                case ToNRoundType.Bloodbath:
                case ToNRoundType.Double_Trouble:
                case ToNRoundType.EX:
                case ToNRoundType.Midnight:
                    if (RoundType == ToNRoundType.Double_Trouble) {
                        int ind = Array.IndexOf(indexes, indexes[0], 1);
                        if (ind < 0) (indexes[0], indexes[2]) = (indexes[2], indexes[0]);
                        else if (ind > 1) (indexes[2], indexes[1]) = (indexes[1], indexes[2]);
                    }

                    Terrors = new ToNIndex.TerrorInfo[indexes.Length];
                    for (int i = 0; i < Terrors.Length; i++) {
                        Terrors[i] = new(indexes[i], i > 1 && RoundType == ToNRoundType.Midnight ? ToNIndex.TerrorGroup.Alternates : ToNIndex.TerrorGroup.Terrors);
                    }

                    break;

                case ToNRoundType.Mystic_Moon:
                case ToNRoundType.Blood_Moon:
                case ToNRoundType.Twilight:
                case ToNRoundType.Solstice:
                    index = (int)RoundType - 9;
                    Terrors = [ new(index, ToNIndex.TerrorGroup.Moons) ];
                    break;

                case ToNRoundType.RUN:
                    Terrors = [new(0, ToNIndex.TerrorGroup.Specials)];
                    break;

                case ToNRoundType.Eight_Pages: // ???
                    Terrors = [new(indexes[0], ToNIndex.TerrorGroup.EightPages)];
                    break;

                case ToNRoundType.Cold_Night:
                    Terrors = [new(0, ToNIndex.TerrorGroup.Events)];
                    break;

                default:
                    Terrors = [];
                    break;
            }
        }

        // Horrible syntax, sorry
        static readonly string[] RoundTypeNames = new string[]
        {
            // Normal
            "Classic"     , "クラシック",
            "Fog"         , "霧", // Like classic
            "Punished"    , "パニッシュ", // Like classic
            "Sabotage"    , "サボタージュ", // Like classic

            "Sabotage"    , "Among Us",   // April fools
            "Sabotage"    , "アモングアス", // April fools

            "Cracked"     , "狂気", // Like classic
            "Bloodbath"   , "ブラッドバス", // (0, 1, 2)

            // Contains alternates
            "Midnight"    , "ミッドナイト", // (0, 1, 2) last index is alt
            "Alternate"   , "オルタネイト", // first index is alt

            // Moons
            "Mystic Moon" , "ミスティックムーン", // Psychosis
            "Blood Moon"  , "ブラッドムーン",    // Virus >:[
            "Twilight"    , "トワイライト",      // Some cursed big bird idk
            "Solstice"    , "ソルスティス",      // Pandora

            // Special
            "RUN"         , "走れ！", // The meatball man
            "8 Pages"     , "8ページ",

            // Events
            "Cold Night"  , "冷たい夜", // Winterfest

            // New
            "Unbound"     , "アンバウンド",
            "Ghost"       , "ゴースト",

            "Double Trouble", "ダブル・トラブル",
        };

        static string GetEngRoundType(string roundType, bool alternate)
        {
            int index = Array.IndexOf(RoundTypeNames, roundType);
            string name = index < 0 ? roundType : RoundTypeNames[index - (index % 2)];
            if (alternate) name += " Alternate";
            return name;
        }

        static ToNRoundType GetRoundType(string raw)
        {
            raw = raw.Replace(' ', '_').Replace("8", "Eight");
            return Enum.TryParse(typeof(ToNRoundType), raw, out object? result) && result != null ? (ToNRoundType)result : ToNRoundType.Unknown;
        }

        internal static uint GetRoundColorFromType(ToNRoundType RoundType) => RoundTypeColors.ContainsKey(RoundType) ? RoundTypeColors[RoundType] : 16721714;

        internal static readonly Dictionary<ToNRoundType, uint> RoundTypeColors = new Dictionary<ToNRoundType, uint>()
        {
            { ToNRoundType.Unknown,             16721714 },
            { ToNRoundType.Classic,             0xFFFFFF },
            { ToNRoundType.Fog,                 0x808486 },
            { ToNRoundType.Fog_Alternate,       0x808486 },
            { ToNRoundType.Punished,            0xFFF800 },
            { ToNRoundType.Sabotage,            0x3BF37D },
            { ToNRoundType.Cracked,             0xFF00D3 },

            { ToNRoundType.Bloodbath,           0xF51313 },
            { ToNRoundType.Double_Trouble,      0xF51313 },
            { ToNRoundType.EX,                  0xF51313 },

            { ToNRoundType.Midnight,            0xE23232 },
            { ToNRoundType.Alternate,           0xF1F1F1 },

            { ToNRoundType.Mystic_Moon,         0xB0DEF9 },
            { ToNRoundType.Twilight,            0xF8A900 },
            { ToNRoundType.Blood_Moon,          0xF51313 },
            { ToNRoundType.Solstice,            0x3BF3B3 },

            { ToNRoundType.RUN,                 0xC15E3D },
            { ToNRoundType.Eight_Pages,         0xFFFFFF },

            { ToNRoundType.Cold_Night,          0xA37BE4 },

            { ToNRoundType.Unbound,             0xF17944 },
            { ToNRoundType.Ghost,               0xC3F7FF },
            { ToNRoundType.Ghost_Alternate,     0xC3F7FF },
        };
    }
    
    public enum ToNRoundType
    {
        Unknown, // Default

        // Normal
        Classic, Fog,
        Punished, Sabotage,
        Cracked, Bloodbath,

        // Contains alternates
        Midnight, Alternate,

        // Moons // Replace spaces with underscore
        Mystic_Moon, Blood_Moon, Twilight, Solstice,

        // Special // Replace 8 with Eight
        RUN, Eight_Pages,

        // New
        Cold_Night,

        Unbound, // Don't know how it works yet
        Ghost,

        Fog_Alternate,
        Ghost_Alternate,

        GIGABYTE,

        Double_Trouble, // Bloodbath - Two killers have same id
        EX              // Bloodbath - All killers have same id
    }

    public enum ToNRoundResult {
        R, // Respawn
        W, // Win
        D, // Leaving
    }

    internal class ToNIndex
    {
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

        public Dictionary<int, Map> Maps { get; private set; } = new();

        public Dictionary<int, Terror> Terrors { get; private set; } = new();
        public Dictionary<int, Terror> Alternates { get; private set; } = new();

        public Dictionary<int, Terror> Moons { get; private set; } = new();
        public Dictionary<int, Terror> Specials { get; private set; } = new();
        public Dictionary<int, Terror> Events { get; private set; } = new();

        public Dictionary<int, int[]> EightPRedirect { get; private set; } = new();
        public Dictionary<int, Terror> EightPages { get; private set; } = new();

        public Dictionary<int, Terror> Unbound { get; private set; } = new();

        public Dictionary<int, Item> Items { get; private set; } = new();

        /// <summary>
        /// Get Terror information based on it's index and group ID.
        /// </summary>
        /// <param name="terrorIndex">The Terror's index ID.</param>
        /// <param name="terrorGroup">The Terror's group flag.</param>
        public Terror GetTerror(int terrorIndex, TerrorGroup terrorGroup = TerrorGroup.Terrors) {
            Dictionary<int, Terror> table;
            switch (terrorGroup) {
                default:
                case TerrorGroup.Terrors:    table = Terrors;    break;
                case TerrorGroup.Alternates: table = Alternates; break;

                case TerrorGroup.EightPages:
                    // Handle 8 Pages Redirect
                    if (EightPages.ContainsKey(terrorIndex)) return EightPages[terrorIndex];
                    else if (EightPRedirect.ContainsKey(terrorIndex)) {
                        int[] redirect = EightPRedirect[terrorIndex];
                        return GetTerror(redirect[1], (TerrorGroup)redirect[0]);
                    } else return Terror.Empty;

                case TerrorGroup.Unbound:    table = Unbound;    break;
                case TerrorGroup.Moons:      table = Moons;      break;
                case TerrorGroup.Specials:   table = Specials;   break;
                case TerrorGroup.Events:     table = Events;     break;
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
        public Map GetMap(int mapIndex) {
            return Maps.ContainsKey(mapIndex) ? Maps[mapIndex] : Map.Empty;
        }
        public Map GetMap(string mapName) {
            foreach (var pair in Maps) {
                if (pair.Value.Name == mapName) return pair.Value;
            }

            return Map.Empty;
        }

        public enum TerrorGroup {
            Terrors,
            Alternates,
            EightPages,
            Unbound,
            Moons,
            Specials,
            Events
        }

        public interface IEntry {
            bool IsEmpty { get; set; }

            int Id { get; set; }
            Color Color { get; set; }
            string Name { get; set; }
        }

        public class EntryBase : IEntry {
            [JsonIgnore] public bool IsEmpty { get; set; }
            [JsonProperty("i")] public int Id { get; set; }
            [JsonProperty("c", DefaultValueHandling = DefaultValueHandling.Ignore)] public Color Color { get; set; }
            [JsonProperty("n", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Name { get; set; } = string.Empty;
        }

        public class Terror : EntryBase {
            public static readonly Terror Empty = new Terror() { IsEmpty = true };

            [JsonProperty("b", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool CantBB { get; set; } // Can't participate in bb
            [JsonProperty("g", DefaultValueHandling = DefaultValueHandling.Ignore)] public TerrorGroup Group { get; set; }

            public override string ToString() => Name;
        }

        public class Map : EntryBase {
            public static readonly Map Empty = new Map() { IsEmpty = true };

            [JsonProperty("t", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Creator = string.Empty;
            [JsonProperty("o", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Origin = string.Empty;
            [JsonProperty("e", DefaultValueHandling = DefaultValueHandling.Ignore)] public bool EightP = false;

            public override string ToString() => Name;
        }

        public class Item : EntryBase {
            public static readonly Item Empty = new Item() { IsEmpty = true };

            [JsonProperty("d", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Description { get; set; } = string.Empty;
            [JsonProperty("u", DefaultValueHandling = DefaultValueHandling.Ignore)] public string Usage { get; set; } = string.Empty;
            [JsonProperty("s", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Store { get; set; } // Point Shop | Survival Shop | Secret Shop
        }

        public struct TerrorInfo {
            public static readonly TerrorInfo Empty = new TerrorInfo() { Group = TerrorGroup.Terrors, Index = 0 };

            [JsonProperty("i")] public int Index;
            [JsonProperty("g")] public TerrorGroup Group;

            public TerrorInfo(int index, TerrorGroup group) {
                Index = index;
                Group = group;
            }
        }
    }
}
