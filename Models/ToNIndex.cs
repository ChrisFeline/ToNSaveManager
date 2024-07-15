using Newtonsoft.Json;

namespace ToNSaveManager.Models
{
    internal struct TerrorMatrix
    {
        internal static TerrorMatrix Empty => new TerrorMatrix();

        public string[] TerrorNames;
        public string RoundTypeRaw;
        public ToNRoundType RoundType;

        public TerrorMatrix()
        {
            TerrorNames = new string[0];
            RoundTypeRaw = string.Empty;
            RoundType = ToNRoundType.Unknown;
        }

        public TerrorMatrix(string roundType, params int[] indexes)
        {
            RoundTypeRaw = GetEngRoundType(roundType);
            RoundType = GetRoundType(RoundTypeRaw);

            switch (RoundType)
            {
                // first index only
                case ToNRoundType.Classic:
                case ToNRoundType.Fog:
                case ToNRoundType.Punished:
                case ToNRoundType.Sabotage:
                case ToNRoundType.Cracked:
                case ToNRoundType.Alternate: // index is alt
                    int index = indexes[0];
                    string name = ToNIndex.Instance[index, RoundType == ToNRoundType.Alternate];
                    TerrorNames = new string[] { name };
                    break;

                case ToNRoundType.Bloodbath:
                case ToNRoundType.Midnight:
                    TerrorNames = new string[indexes.Length];
                    for (int i = 0; i < TerrorNames.Length; i++)
                        TerrorNames[i] = ToNIndex.Instance[indexes[i], i > 1 && RoundType == ToNRoundType.Midnight];
                    break;

                case ToNRoundType.Mystic_Moon:
                case ToNRoundType.Twilight:
                case ToNRoundType.Blood_Moon:
                case ToNRoundType.Solstice:
                    TerrorNames = new string[1] { MoonNames[RoundType] };
                    break;

                case ToNRoundType.RUN:
                    TerrorNames = new string[] { "The Meatball Man" };
                    break;
                case ToNRoundType.Eight_Pages: // ???
                    TerrorNames = new string[] { "???" }; // ???
                    break;

                default:
                    TerrorNames = new string[0] { };
                    break;
            }
        }

        public string GetNames(string separator = ", ")
        {
            return string.Join(separator, TerrorNames);
        }

        // Horrible syntax, sorry
        static readonly string[] RoundTypeNames = new string[]
        {
            // Normal
            "Classic"     , "クラシック",
            "Fog"         , "霧", // Like classic
            "Punished"    , "パニッシュ", // Like classic
            "Sabotage"    , "サボタージュ", // Like classic
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

            // Beyond's favorite
            "Custom"      , "カスタム", // IGNORE SAVES FOR THIS ONE
        };

        static string GetEngRoundType(string roundType)
        {
            int index = Array.IndexOf(RoundTypeNames, roundType);
            return index < 0 ? roundType : RoundTypeNames[index - (index % 2)];
        }

        static readonly Dictionary<ToNRoundType, string> MoonNames = new () {
            { ToNRoundType.Mystic_Moon, "PSYCHOSIS" },
            { ToNRoundType.Blood_Moon, "VIRUS" },
            { ToNRoundType.Twilight, "APOCALYPSE BIRD" },
            { ToNRoundType.Solstice, "PANDORA" } // not the music streaming platform
        };

        static ToNRoundType GetRoundType(string raw)
        {
            raw = raw.Replace(' ', '_').Replace("8", "Eight");
            return Enum.TryParse(typeof(ToNRoundType), raw, out object? result) && result != null ? (ToNRoundType)result : ToNRoundType.Unknown;
        }

        internal static uint GetRoundColorFromType(ToNRoundType RoundType) => RoundTypeColors.ContainsKey(RoundType) ? RoundTypeColors[RoundType] : 16721714;

        internal static readonly Dictionary<ToNRoundType, uint> RoundTypeColors = new Dictionary<ToNRoundType, uint>()
        {
            { ToNRoundType.Unknown,     16721714 },
            { ToNRoundType.Classic,     0xFFFFFF },
            { ToNRoundType.Fog,         0x808486 },
            { ToNRoundType.Punished,    0xFFF800 },
            { ToNRoundType.Sabotage,    0x3BF37D },
            { ToNRoundType.Cracked,     0xFF00D3 },
            { ToNRoundType.Bloodbath,   0xF51313 },

            { ToNRoundType.Midnight,    0xE23232 },
            { ToNRoundType.Alternate,   0xF1F1F1 },

            { ToNRoundType.Mystic_Moon, 0xB0DEF9 },
            { ToNRoundType.Twilight,    0xF8A900 },
            { ToNRoundType.Blood_Moon,  0xF51313 },
            { ToNRoundType.Solstice,    0x3BF3B3 },

            { ToNRoundType.RUN,         0xC15E3D },
            { ToNRoundType.Eight_Pages, 0xFFFFFF },
            { ToNRoundType.Custom,      0x000000 }, // Ignored

            { ToNRoundType.Cold_Night,  0xA37BE4 },
        };
    }

    public enum ToNRoundResult
    {
        R, // Respawn
        W, // Win
        L, // Lose
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
        RUN, Eight_Pages, Custom,

        // Events
        Cold_Night
    }

    internal class ToNIndex
    {
        public static readonly ToNIndex Instance = Import();

        public Dictionary<int, string> Terrors = new Dictionary<int, string>();
        public Dictionary<int, string> Alternates = new Dictionary<int, string>();

        public static ToNIndex Import()
        {
            using (Stream? stream = Program.GetEmbededResource("index.json"))
            {
                if (stream == null) return new ToNIndex();

                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ToNIndex>(json) ?? new ToNIndex();
                }
            }
        }

        public string this[int index, bool alternate]
            => alternate ? GetAlternate(index) : GetTerror(index);

        public string GetTerror(int index)
        {
            return Terrors.ContainsKey(index) ? Terrors[index] : "???";
        }
        public string GetAlternate(int index)
        {
            return Alternates.ContainsKey(index) ? Alternates[index] : "???";
        }
    }
}
