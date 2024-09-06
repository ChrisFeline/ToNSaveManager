using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using Microsoft.VisualBasic.Logging;
using ToNSaveManager.Models.Index;

namespace ToNSaveManager.Utils.Discord {
    internal class DSRichPresence {
        static LoggerSource Log = new LoggerSource("Discord");
        static DiscordRpcClient? Client;

        static RichPresence Presence = new RichPresence() {
            Details = "Details",
            State = "State",
            Timestamps = new Timestamps() {
                Start = DateTime.UtcNow
            },
            Assets = new Assets() {
                LargeImageKey = "icon_255_0",
                LargeImageText = "Intermission"
            }
        };
        static string DetailsText {
            get => Presence.Details;
            set => Presence.Details = value;
        }
        static string StateText {
            get => Presence.State;
            set => Presence.State = value;
        }
        static string ImageKey {
            get => Presence.Assets.LargeImageKey;
            set => Presence.Assets.LargeImageKey = value;
        }
        static string ImageText {
            get => Presence.Assets.LargeImageText;
            set => Presence.Assets.LargeImageText = value;
        }
        static string? IconKey {
            get => Presence.Assets.SmallImageKey;
            set => Presence.Assets.SmallImageKey = value;
        }
        static string? IconText {
            get => Presence.Assets.SmallImageText;
            set => Presence.Assets.SmallImageText = value;
        }
        static DateTime? Timestamp {
            get => Presence.Timestamps.Start;
            set => Presence.Timestamps.Start = value;
        }

        static bool IsDirty = true;
        static TerrorMatrix CurrentMatrix = TerrorMatrix.Empty;
        internal static void SetTerrorMatrix(TerrorMatrix terrorMatrix) {
            if (CurrentMatrix.Length == 0 && terrorMatrix.Length > 0) IsAlive = true;
            CurrentMatrix = terrorMatrix;
            SetRoundType(terrorMatrix.RoundType);
            SetDirty();
        }

        static ToNIndex.Map CurrentMap = ToNIndex.Map.Empty;
        internal static void SetLocation(ToNIndex.Map map) {
            if (CurrentMap.IsEmpty != map.IsEmpty) {
                Timestamp = DateTime.UtcNow;
            }
            CurrentMap = map;
            SetDirty();
        }

        static ToNRoundType CurrentRoundType = ToNRoundType.Unknown;
        static string CurrentRoundTypeAssetID = "icon_254_0";
        static void SetRoundType(ToNRoundType roundType) {
            if (CurrentRoundType != roundType) {
                CurrentRoundTypeAssetID = GetRoundAssetID(roundType);
                CurrentRoundType = roundType;
                SetDirty();
            }
        }

        static bool IsOptedIn = false;
        internal static void SetOptedIn(bool optedIn) {
            if (IsOptedIn != optedIn) {
                IsOptedIn = optedIn;
                SetDirty();
            }
        }

        static bool IsAlive = false;
        internal static void SetIsAlive(bool isAlive) {
            if (IsAlive != isAlive) {
                IsAlive = isAlive;
                SetDirty();
            }
        }

        static readonly Dictionary<ToNRoundType, int> RoundTypeToAssetID = new Dictionary<ToNRoundType, int>() {
            { ToNRoundType.Unknown, 0 },
            { ToNRoundType.Classic, 0 },
            { ToNRoundType.Fog, 4 },
            { ToNRoundType.Punished, 5 },        //  3
            { ToNRoundType.Sabotage, 6 },        //  4
            { ToNRoundType.Cracked, 1 },         //  5
            { ToNRoundType.Bloodbath, 2 },       //  6
            { ToNRoundType.Double_Trouble, 2 },  //  7 // Bloodbath - Two killers have same id
            { ToNRoundType.EX, 2 },              //  8 // Bloodbath - All killers have same id
            { ToNRoundType.Ghost, 7 },           //  9
            { ToNRoundType.Unbound, 9 },         // 10

            // Contains alternates
            { ToNRoundType.Midnight, 10 },
            { ToNRoundType.Alternate, 3 },         // 51
            { ToNRoundType.Fog_Alternate, 4 },     // 52
            { ToNRoundType.Ghost_Alternate, 7 },   // 53

            // Moons // Replace spaces with underscore
            { ToNRoundType.Mystic_Moon, -1}, // Start with 4
            { ToNRoundType.Blood_Moon, -2},  // 101
            { ToNRoundType.Twilight, -3},    // 102
            { ToNRoundType.Solstice, -4},    // 103

            // Specials
            { ToNRoundType.Eight_Pages, 8},

            { ToNRoundType.RUN, 104},          // 104
            { ToNRoundType.Cold_Night, 107},   // 107 // Winterfest

            // Beyond doing things or something
            { ToNRoundType.Custom, 11 }
        };
        internal static string GetRoundAssetID(ToNRoundType roundType) {
            if (RoundTypeToAssetID.TryGetValue(roundType, out int value)) {
                string assetId;

                // Moons
                if (value < 0) assetId = "icon_4_" + (Math.Abs(value) - 1);
                else if (value > 100) {
                    switch (value) {
                        case 104: return "icon_5_0";
                        case 107: return "icon_6_0"; // cold night
                        default: return "icon_254_0";
                    }
                }

                return "icon_r_" + value;
            }

            return "icon_254_0";
        }


        internal static void SetDirty() {
            IsDirty = true;
        }
        internal static void Send() {
            if (IsDirty) {
                IsDirty = false;

                string details = CurrentMatrix.Length > 0 ? CurrentMatrix.RoundType.ToString() + " on" : (CurrentMap.IsEmpty ? "Intermission" : "Traveling to");
                string state = CurrentMatrix.Length > 0 && CurrentMap.IsEmpty ? "Somewhere" : (CurrentMap.IsEmpty ? "Overseer's Court" : CurrentMap.Name);

                bool isHidden = CurrentMatrix.RoundType == ToNRoundType.Fog || CurrentMatrix.RoundType == ToNRoundType.Fog_Alternate || CurrentMatrix.RoundType == ToNRoundType.Eight_Pages;
                ImageKey = CurrentMatrix.Length > 0 ? ((CurrentMatrix.Length > 1 ? CurrentMatrix.Terror3 : CurrentMatrix.Terror1).AssetID ?? (isHidden ? "icon_254_1" : "icon_254_0")) : (CurrentMap.IsEmpty ? "icon_255_0" : "icon_254_0");
                ImageText = CurrentMatrix.Length > 0 ? CurrentMatrix.GetTerrorNames() : (CurrentMap.IsEmpty ? "Overseer" : "???");

                if ((CurrentMatrix.Length == 0 && !CurrentMap.IsEmpty)
                    || CurrentRoundType == ToNRoundType.Unbound
                    || CurrentRoundType == ToNRoundType.Bloodbath
                    || CurrentRoundType == ToNRoundType.Double_Trouble
                    || CurrentRoundType == ToNRoundType.Custom) {
                    ImageKey = CurrentRoundTypeAssetID; // placeholder

                    if (CurrentRoundType == ToNRoundType.Custom) {
                        ImageText = "What is Beyond up to now?";
                        details = "Custom";
                    } else if (CurrentMatrix.Length == 0) ImageText = $"Starting a {CurrentRoundType.ToString()} round...";
                }

                DetailsText = details;
                StateText = state;

                IconKey = CurrentMatrix.Length > 0 ? (IsAlive ? "status_alive" : "status_dead") : null;
                IconText = CurrentMatrix.Length > 0 ? (IsAlive ? "Alive" : "Died") : null;

                if (CurrentMatrix.IsSaboteur) {
                    IconKey = "status_killer";
                    IconText = "Killer";
                }

                Client?.SetPresence(Presence);
            }
        }

        internal static void Initialize() {
            Client = new DiscordRpcClient("1281246143224746035");

            Client.OnReady += Client_OnReady;
            Client.OnPresenceUpdate += Client_OnPresenceUpdate;

            Client.Initialize();

            SetDirty();
        }

        private static void Client_OnPresenceUpdate(object sender, PresenceMessage e) {
            Log.Debug("Received Update: " + e.Presence);
        }

        private static void Client_OnReady(object sender, ReadyMessage e) {
            Log.Debug("Received Ready from user: " + e.User.Username);
            SetDirty();
        }
    }
}
