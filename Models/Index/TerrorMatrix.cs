﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Models.Index {
    internal struct TerrorMatrix {
        const string ROUND_TYPE_ALTERNATE = " (Alternate)";
        internal static readonly TerrorMatrix Empty = new TerrorMatrix() { IsEmpty = true };

        public bool IsEmpty { get; private set; }

        public ToNIndex.TerrorInfo[] Terrors;
        public int TerrorCount;
        public int StartIndex;
        public int ActualCount => TerrorCount - StartIndex;

        static readonly StringBuilder StrBuild = new StringBuilder();
        internal string GetTerrorAssets(string splitter = " & ") {
            StrBuild.Clear();

            for (int i = StartIndex; i < Length; i++) {
                if (StrBuild.Length > 0)
                    StrBuild.Append(splitter);

                StrBuild.Append(Terrors[i].AssetID);
            }

            return StrBuild.ToString();
        }
        public string GetTerrorNames(string splitter = " & ") {
            StrBuild.Clear();

            for (int i = StartIndex; i < Length; i++) {
                if (StrBuild.Length > 0)
                    StrBuild.Append(splitter);

                StrBuild.Append(Terrors[i].Name);
            }

            if (StrBuild.Length == 0) StrBuild.Append("???");
            return StrBuild.ToString();
        }

        public string RoundTypeRaw;
        public ToNRoundType RoundType;
        public bool IsSaboteur;
        public int MapID = -1;

        public bool IsUnknown;
        public bool IsRevealed;

        public int Length => Math.Min(TerrorCount, Terrors.Length);
        public ToNIndex.TerrorInfo this[int i] {
            get => Terrors[i];
            set => Terrors[i] = value;
        }

        public ToNIndex.TerrorInfo Terror1 => Terrors != null && Terrors.Length > 0 ? Terrors[0] : ToNIndex.TerrorInfo.Empty;
        public ToNIndex.TerrorInfo Terror2 => Terrors != null && Terrors.Length > 1 ? Terrors[1] : ToNIndex.TerrorInfo.Empty;
        public ToNIndex.TerrorInfo Terror3 => Terrors != null && Terrors.Length > 2 ? Terrors[2] : ToNIndex.TerrorInfo.Empty;

        // For emulator only
        internal void MarkEncounter() {
            for (int i = StartIndex; i < Length; i++) {
                ToNIndex.Terror terr = Terrors[i].Value;
                if (terr.Encounters != null && terr.Encounters.Length > 0)
                    Terrors[i].Encounter = 0;
            }
        }

        public override string ToString() {
            return string.Join(", ", Terrors);
        }

        public TerrorMatrix(ToNRoundType roundType) : this() {
            RoundType = roundType;
        }

        public TerrorMatrix() {
            RoundTypeRaw = string.Empty;
            RoundType = ToNRoundType.Intermission;
            Terrors = []; // what is this? JavaScript?
        }

        public TerrorMatrix(string roundType, params int[] indexes) {
            int index = roundType.IndexOf(ROUND_TYPE_ALTERNATE, StringComparison.InvariantCulture);
            bool isAlt = index > 0;
            if (isAlt) roundType = roundType.Substring(0, index);

            RoundTypeRaw = GetEngRoundType(roundType, isAlt);
            RoundType = GetRoundType(RoundTypeRaw);

            if (indexes.Length == 0) {
                Terrors = [];
                return;
            }

            Terrors = new ToNIndex.TerrorInfo[indexes.Length];

            switch (RoundType) {
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
                    Terrors = [new(indexes[0], RoundType == ToNRoundType.Alternate || isAlt ? ToNIndex.TerrorGroup.Alternates : ToNIndex.TerrorGroup.Terrors)];
                    TerrorCount = 1;
                    break;

                case ToNRoundType.Unbound:
                    Terrors = [new(indexes[0], ToNIndex.TerrorGroup.Unbound)];
                    TerrorCount = 1;
                    break;

                case ToNRoundType.Bloodbath:
                case ToNRoundType.Double_Trouble:
                case ToNRoundType.EX:
                case ToNRoundType.Midnight:
                    int lvl = 1;

                    if (RoundType == ToNRoundType.Midnight) { // Handle Monarch
                        TerrorCount = 3;
                        if (indexes[2] == 19) {
                            StartIndex = 2;
                            indexes[0] = indexes[1] = 255;
                        } else if (indexes[0] == indexes[1]) {
                            lvl = 2;
                            StartIndex = 1;
                        }
                    } else {
                        if (RoundType == ToNRoundType.Double_Trouble)
                            FirstEntry = new ToNIndex.TerrorInfo(indexes[0], ToNIndex.TerrorGroup.Terrors);

                        Dictionary<int, int> dupes = new Dictionary<int, int>();

                        for (int i = 0; i < indexes.Length; i++) {
                            int val = indexes[i];
                            if (dupes.ContainsKey(val)) {
                                lvl++;
                                dupes[val]++;
                            } else {
                                dupes[val] = 0;
                            }
                        }

                        if (lvl > 1) indexes = dupes.OrderByDescending(v => v.Value).Select(k => k.Key).ToArray();
                        TerrorCount = indexes.Length;

                        dupes.Clear();
                    }

                    Terrors = new ToNIndex.TerrorInfo[indexes.Length];
                    for (int i = 0; i < Terrors.Length; i++) {
                        ToNIndex.TerrorInfo info = new(indexes[i], i > 1 && RoundType == ToNRoundType.Midnight ? ToNIndex.TerrorGroup.Alternates : ToNIndex.TerrorGroup.Terrors);
                        if ((i == StartIndex || RoundType == ToNRoundType.Double_Trouble) && lvl > 1) info.Level = lvl;
                        Terrors[i] = info;
                    }
                    break;

                case ToNRoundType.Mystic_Moon:
                case ToNRoundType.Blood_Moon:
                case ToNRoundType.Twilight:
                case ToNRoundType.Solstice:
                    index = (int)RoundType - (int)ToNRoundType.Mystic_Moon;
                    Terrors = [new(index, ToNIndex.TerrorGroup.Moons)];
                    TerrorCount = 1;
                    break;

                case ToNRoundType.RUN:
                    Terrors = [new(0, ToNIndex.TerrorGroup.Specials)];
                    TerrorCount = 1;
                    break;

                case ToNRoundType.Eight_Pages: // ???
                    Terrors = [new(indexes[0], ToNIndex.TerrorGroup.EightPages)];
                    TerrorCount = 1;
                    break;

                case ToNRoundType.Cold_Night:
                    Terrors = [new(0, ToNIndex.TerrorGroup.Events)];
                    TerrorCount = 1;
                    break;

                case ToNRoundType.Custom:
                default:
                    Terrors = [];
                    TerrorCount = 0;
                    break;
            }

            // Append round type to terror entries, this makes variants work
            for (int i = 0; i < Terrors.Length; i++) {
                Terrors[i].RoundType = RoundType;
            }
        }

        private ToNIndex.TerrorInfo FirstEntry = ToNIndex.TerrorInfo.Empty;
        public Color DisplayColor => GetDisplayColor();
        public Color RoundColor => (RoundType == ToNRoundType.Intermission ? Color.White : RoundTypeColors.ContainsKey(RoundType) ? Color.FromArgb((int)RoundTypeColors[RoundType]) : Color.White);

        private Color GetDisplayColor() {
            if (TerrorCount > 0 && Terrors.Length > 0 &&
                        RoundType != ToNRoundType.Eight_Pages &&
                        RoundType != ToNRoundType.Fog &&
                        RoundType != ToNRoundType.Fog_Alternate) {
                if (!FirstEntry.IsEmpty) return FirstEntry.Value.Color;

                Color c = Color.White;
                int R = 0, G = 0, B = 0, L = 0;
                for (int i = StartIndex; i < TerrorCount; i++) {
                    if (i > 2) break;

                    if (Terrors[i].IsEmpty) continue;
                    c = Terrors[i].Value.Color;

                    R += c.R;
                    G += c.G;
                    B += c.B;
                    L++;
                }

                // prevent division by zero
                if (L == 0) return Color.White;

                return Color.FromArgb(R / L, G / L, B / L);
            } else return Color.White;
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
            "Cold Night"  , "コールドナイト", // Winterfest

            // New
            "Unbound"     , "アンバウンド",
            "Ghost"       , "ゴースト",

            "Double Trouble", "ダブル・トラブル",

            // Beyond's favorite
            "Custom"      , "カスタム",
        };

        static string GetEngRoundType(string roundType, bool alternate) {
            int index = Array.IndexOf(RoundTypeNames, roundType);
            string name = index < 0 ? roundType : RoundTypeNames[index - index % 2];
            if (alternate) name += " Alternate";
            return name;
        }

        static ToNRoundType GetRoundType(string raw) {
            raw = raw.Replace(' ', '_').Replace("8", "Eight");
            return Enum.TryParse(typeof(ToNRoundType), raw, out object? result) && result != null ? (ToNRoundType)result : ToNRoundType.Intermission;
        }

        internal static uint GetRoundColorFromType(ToNRoundType RoundType) => RoundTypeColors.ContainsKey(RoundType) ? RoundTypeColors[RoundType] : 16721714;

        internal static readonly Dictionary<ToNRoundType, uint> RoundTypeColors = new Dictionary<ToNRoundType, uint>()
        {
            { ToNRoundType.Intermission,        16721714 },
            { ToNRoundType.Classic,             0xFFFFFF },
            { ToNRoundType.Fog,                 0x808486 },
            { ToNRoundType.Fog_Alternate,       0x808486 },
            { ToNRoundType.Punished,            0xFFF800 },
            { ToNRoundType.Sabotage,            0x3BF37D },
            { ToNRoundType.Cracked,             0xFF00D3 },

            { ToNRoundType.Bloodbath,           0xF51313 },
            { ToNRoundType.Double_Trouble,      0xF51313 },
            { ToNRoundType.EX,                  0xF51313 },

            { ToNRoundType.Midnight,            0xFF0000 },
            { ToNRoundType.Alternate,           0xF1F1F1 },

            { ToNRoundType.Mystic_Moon,         0xB0DEF9 },
            { ToNRoundType.Twilight,            0xF8A900 },
            { ToNRoundType.Blood_Moon,          0xF51313 },
            { ToNRoundType.Solstice,            0x3BF3B3 },

            { ToNRoundType.RUN,                 0xC15E3D },
            { ToNRoundType.Eight_Pages,         0xFFFFFF },

            { ToNRoundType.Cold_Night,          0xA37BE4 },
            { ToNRoundType.GIGABYTE,            0xA37BE4 },

            { ToNRoundType.Unbound,             0xF17944 },
            { ToNRoundType.Ghost,               0xC3F7FF },
            { ToNRoundType.Ghost_Alternate,     0xC3F7FF },

            { ToNRoundType.Custom,              0x2672FF },
        };
    }
}
