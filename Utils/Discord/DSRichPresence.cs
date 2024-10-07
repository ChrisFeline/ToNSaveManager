using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Index;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

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
            },
            Party = new Party() {
                Max = 20,
                Size = 0
            }
        };
        static string DetailsText {
            get => Presence.Details;
            set => Presence.Details = LimitByteLength(value);
        }
        static string StateText {
            get => Presence.State;
            set => Presence.State = LimitByteLength(value);
        }
        static string ImageKey {
            get => Presence.Assets.LargeImageKey;
            set => Presence.Assets.LargeImageKey = AssetRedirects.ContainsKey(value) ? AssetRedirects[value] : value;
        }
        static string? ImageText {
            get => Presence.Assets.LargeImageText;
            set => Presence.Assets.LargeImageText = LimitByteLength(value);
        }
        static string? IconKey {
            get => Presence.Assets.SmallImageKey;
            set => Presence.Assets.SmallImageKey = value;
        }
        static string? IconText {
            get => Presence.Assets.SmallImageText;
            set => Presence.Assets.SmallImageText = LimitByteLength(value);
        }
        static DateTime? Timestamp {
            get => Presence.Timestamps.Start;
            set => Presence.Timestamps.Start = value;
        }
        static string? PartyID {
            get => Presence.Party.ID;
            set => Presence.Party.ID = value;
        }
        static Party.PrivacySetting PartyPrivacy {
            get => Presence.Party.Privacy;
            set => Presence.Party.Privacy = value;
        }

        static bool IsDirty = true;

        static TerrorMatrix CurrentMatrix => ToNGameState.Terrors;
        static ToNIndex.Map CurrentMap => ToNGameState.Location;
        static int PageCount => ToNGameState.PageCount;
        static bool IsAlive => ToNGameState.IsAlive;
        static bool IsOptedIn => ToNGameState.IsOptedIn;

        internal static void UpdateTimestamp() {
            Timestamp = DateTime.UtcNow;
            SetDirty();
        }

        static ToNRoundType CurrentRoundType = ToNRoundType.Intermission;
        static string CurrentRoundTypeAssetID = "icon_254_0";
        internal static void SetRoundType(ToNRoundType roundType) {
            if (CurrentRoundType != roundType) {
                CurrentRoundTypeAssetID = GetRoundAssetID(roundType);
                CurrentRoundType = roundType;
                SetDirty();
            }
        }

        static Regex InstanceTagsPattern = new Regex(@"~(hidden|friends|private|group)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        static Regex InstanceRegionPattern = new Regex(@"~region\((\w+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        static Regex InstanceGroupPattern = new Regex(@"~groupAccessType\((\w+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static bool IsPlayingTerrors;
        private static string TerrorsInstanceID = string.Empty;

        internal static void SetInstanceID(string instanceId, bool isHome) {
            IsPlayingTerrors = isHome;
            TerrorsInstanceID = instanceId;
            if (!MainWindow.Started) return;

            UpdateTimestamp();
            Log.Debug("Setting instance home: " + isHome);

            string partyId = string.Empty;
            if (isHome) {
                int index1 = instanceId.IndexOf(':') + 1;
                if (index1 > 0) {
                    int index2 = instanceId.IndexOf('~', index1);

                    string lobbyId = index2 < 0 ? instanceId.Substring(index1) : instanceId.Substring(index1, index2 - index1);

                    if (!string.IsNullOrEmpty(lobbyId)) {
                        Match match = InstanceRegionPattern.Match(instanceId);
                        string region = match == null ? string.Empty : match.Groups[1].Value;

                        match = InstanceTagsPattern.Match(instanceId);
                        string tag = match == null ? "public" : match.Groups[1].Value;

                        PartyPrivacy = tag != "public" ? Party.PrivacySetting.Private : Party.PrivacySetting.Public;
                        partyId = lobbyId + "-" + region;

                        Log.Debug("Setting party id: " + instanceId);
                    }
                }
            }

            PartyID = partyId;

            Initialize();
            SetDirty();
        }

        const int INSTANCE_MAX_PLAYERS = 16;
        internal static void SetPlayerCount(int playerCount) {
            Presence.Party.Max = Math.Max(INSTANCE_MAX_PLAYERS, playerCount);
            Presence.Party.Size = playerCount;
            SetDirty();
        }

        static readonly Dictionary<ToNRoundType, int> RoundTypeToAssetID = new Dictionary<ToNRoundType, int>() {
            { ToNRoundType.Intermission, 0 },
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
                // Moons
                if (value < 0) return "icon_4_" + (Math.Abs(value) - 1);
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
            if (IsDirty && Client != null && Client.IsInitialized && Client.CurrentUser != null) {
                IsDirty = false;

                if (ToNGameState.IsEmulated) return;

                //string details = CurrentMatrix.Length > 0 ? CurrentMatrix.RoundType.ToString() + (CurrentMatrix.RoundType == ToNRoundType.Eight_Pages ? $" ({PageCount}/8)" : " on") : (CurrentMap.IsEmpty ? "Intermission" : "Traveling to");
                string details = Settings.Get.GetDiscordTemplateDetails.GetString();
                string state = Settings.Get.GetDiscordTemplateState.GetString();
                ImageText = Settings.Get.GetDiscordTemplateImage.GetString();
                // ImageText = CurrentMatrix.Length > 0 ? CurrentMatrix.GetTerrorNames() : (CurrentMap.IsEmpty ? "Overseer" : "???");

                bool isHidden = CurrentMatrix.RoundType == ToNRoundType.Fog || CurrentMatrix.RoundType == ToNRoundType.Fog_Alternate || CurrentMatrix.RoundType == ToNRoundType.Eight_Pages;
                ImageKey = CurrentMatrix.Length > 0 ? ((CurrentMatrix.Length > 1 ? CurrentMatrix.Terror3 : CurrentMatrix.Terror1).AssetID ?? (isHidden ? "icon_254_1" : "icon_254_0")) : (CurrentMap.IsEmpty ? "icon_255_0" : "icon_254_0");

                if (!string.IsNullOrEmpty(CustomAssetID_0) && ImageKey == "icon_255_0")
                    ImageKey = CustomAssetID_0;

                if ((CurrentMatrix.Length == 0 && !CurrentMap.IsEmpty)
                    || CurrentRoundType == ToNRoundType.Unbound
                    || CurrentRoundType == ToNRoundType.Bloodbath
                    || CurrentRoundType == ToNRoundType.Double_Trouble
                    || CurrentRoundType == ToNRoundType.Custom) {
                    ImageKey = CurrentRoundTypeAssetID; // placeholder

                    if (CurrentRoundType == ToNRoundType.Custom) {
                        ImageText = "What is Beyond up to now?";
                        details = "Custom";
                    } else if (CurrentMatrix.Length == 0) ImageText = Settings.Get.GetDiscordTemplateStart.GetString();
                }

                DetailsText = details;
                StateText = state;

                IconText = CurrentMatrix.Length > 0 ? Settings.Get.GetDiscordTemplateIcon.GetString() : null;

                IconKey = CurrentMatrix.Length > 0 ? (IsOptedIn ? (IsAlive ? "status_alive" : "status_dead") : "status_boring") : null;
                if (IsOptedIn && CurrentMatrix.IsSaboteur) {
                    IconKey = "status_killer";

                    if (!string.IsNullOrEmpty(CustomAssetID_1))
                        ImageKey = CurrentMatrix.Length > 0 ? CustomAssetID_1 : (CustomAssetID_0 ?? CustomAssetID_1);
                }

                Client.SetPresence(Presence);
            }
        }

        // Check game running
        internal static bool VRCIsOpen;

        static async Task WaitForVRChat_Interval() {
            while (true) {
                Log.Debug("Attempting to find game process...");

                try {
                    var vrc = Process.GetProcessesByName("VRChat");
                    if (vrc.Length > 0) {
                        Log.Debug("Game process found, initializing.");

                        VRCIsOpen = true;
                        Initialize();

                        Log.Debug("Waiting for game exit.");
                        while (vrc.Length > 0) {
                            await Task.Delay(60000);
                            vrc = Process.GetProcessesByName("VRChat");
                        }

                        Log.Warning("VRChat game has closed.");
                    }

                    VRCIsOpen = false;
                } catch (Exception e) {
                    Log.Error("Error trying to find game process.");
                    Log.Error(e);
                    VRCIsOpen = false;
                }

                Initialize();
                // Retry to find vrc process in 5 seconds.
                Log.Debug("Could not find VRChat process, retrying in 30 seconds.");
                await Task.Delay(60000);
            }
        }

        internal static void VRCProcessTest() {
            _ = WaitForVRChat_Interval();
        }

        static bool CurrentInitState = false;
        internal static void Initialize(bool onStart = false) {
            if (onStart) {
                SetInstanceID(TerrorsInstanceID, IsPlayingTerrors);
                VRCProcessTest();
                return;
            }

            bool currentState = Settings.Get.DiscordRichPresence && IsPlayingTerrors && VRCIsOpen;
            if (CurrentInitState != currentState) {
                CurrentInitState = currentState;

                if (CurrentInitState) {
                    Initialize_Internal();
                } else {
                    DeInitialize_Internal();
                }
            }
        }

        static void Initialize_Internal() {
            Log.Debug("Init Internal");

            if (Client == null || Client.IsDisposed) {
                Client = new DiscordRpcClient("1281246143224746035");
                // Client.RegisterUriScheme("438100");
                Client.OnReady += Client_OnReady;
                Client.OnPresenceUpdate += Client_OnPresenceUpdate;
            }

            if (!Client.IsInitialized) {
                Log.Debug("Initializing");

                Client.Initialize();
                SetDirty();
            }
        }
        static void DeInitialize_Internal() {
            if (Client != null) {
                Log.Debug("Stopping");

                Client.ClearPresence();
                Client.OnReady -= Client_OnReady;
                Client.OnPresenceUpdate -= Client_OnPresenceUpdate;
                Client.Dispose();
                Client = null;
            }
        }

        private static void Client_OnPresenceUpdate(object sender, PresenceMessage e) {
            Log.Debug("Received Update: " + e.Presence);
        }

        static string? CustomAssetID_0 = null;
        static string? CustomAssetID_1 = null;

        private static void Client_OnReady(object sender, ReadyMessage e) {
            ulong userId = e.User.ID;
            Log.Debug("Received Ready from user: " + userId);

            ToNGameState.SetDisplayName(e.User.DisplayName, true);

            if (ToNIndex.Instance.SpecialSnowflakes.Contains(userId)) {
                Log.Info("Hello my special snowflake: " + userId);
                CustomAssetID_0 = $"icon_p_{userId}_0";
                CustomAssetID_1 = $"icon_p_{userId}_1";
            }

            SetDirty();
        }

        static Dictionary<string, string> AssetRedirects => new Dictionary<string, string>() {
            { "icon_0_47_e0",  "https://i.imgur.com/iBXJYMa.gif" }, // HHI
            { "icon_0_83",     "https://i.imgur.com/MErskLH.gif" }, // Lain
            { "icon_1_28",     "https://i.imgur.com/dkib59R.gif" }, // Roblander
            { "icon_1_28_v50", "https://i.imgur.com/R0DwhQ1.gif" }, // Roblander Midnight
            { "icon_1_10", "https://i.imgur.com/y6mPG7p.gif" }, // zm64
            { "icon_m_10", "https://i.imgur.com/p0HG7Xc.gif" }, // zm64 midnight
            { "icon_1_3", "https://i.imgur.com/NwEWIyn.gif" }, // parhelion
            { "icon_m_3", "https://i.imgur.com/U6wGpqV.gif" }, // Parhelion Midnight
        };

        // https://github.com/vrcx-team/VRCX/blob/6825616d5fbf7fa68121ba80858f11626ea5eeb8/Dotnet/Discord.cs#L135
        // https://stackoverflow.com/questions/1225052/best-way-to-shorten-utf8-string-based-on-byte-length
        private static string? LimitByteLength(string? str, int maxBytesLength = 127) {
            if (str == null) return str;
            var bytesArr = Encoding.UTF8.GetBytes(str);
            var bytesToRemove = 0;
            var lastIndexInString = str.Length - 1;
            while (bytesArr.Length - bytesToRemove > maxBytesLength) {
                bytesToRemove += Encoding.UTF8.GetByteCount(new char[] { str[lastIndexInString] });
                --lastIndexInString;
            }
            return Encoding.UTF8.GetString(bytesArr, 0, bytesArr.Length - bytesToRemove);
        }
    }
}
