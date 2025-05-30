﻿using Newtonsoft.Json;
using ToNSaveManager.Localization;
using ToNSaveManager.Models.Stats;

namespace ToNSaveManager.Models
{
    internal class Settings {
        internal static readonly Settings Default = new();
        internal static readonly Settings Get;
        internal static void Export() => Get.TryExport();

        const string LegacyDestination = "settings.json";
        public static string Destination { get; private set; }

        static Settings() {
            Destination = "Settings.json";
            Get = Import();
        }


        /// <summary>
        /// Custom location where save data will be stored.
        /// </summary>
        public string? DataLocation { get; set; }

        /// <summary>
        /// Discord WebHook URL for backups.
        /// </summary>
        public string? DiscordWebhookURL { get; set; }

        /// <summary>
        /// Discord WebHook backups toggle state.
        /// </summary>
        public bool DiscordWebhookEnabled { get; set; }

        /// <summary>
        /// Enables Discord rich presence for Terrors of Nowhere.
        /// </summary>
        public bool DiscordRichPresence { get; set; }

        public bool DiscordCustomState { get; set; }
        public RoundInfoTemplate GetDiscordTemplateState => DiscordCustomState ? DiscordTemplateState : Default.DiscordTemplateState;
        public RoundInfoTemplate DiscordTemplateState   = new RoundInfoTemplate("{MapName}");

        public bool DiscordCustomDetails { get; set; }
        public RoundInfoTemplate GetDiscordTemplateDetails => DiscordCustomDetails ? DiscordTemplateDetails : Default.DiscordTemplateDetails;
        public RoundInfoTemplate DiscordTemplateDetails = new RoundInfoTemplate("<js>RoundInt === 105 ? RoundType + ' (' + (PageCount + '/8') + ')' : RoundType</js>");

        public bool DiscordCustomImageText { get; set; }
        public RoundInfoTemplate GetDiscordTemplateImage => DiscordCustomImageText ? DiscordTemplateImage : Default.DiscordTemplateImage;
        public RoundInfoTemplate DiscordTemplateImage = new RoundInfoTemplate("{TerrorName}");

        public bool DiscordCustomIconText { get; set; }
        public RoundInfoTemplate GetDiscordTemplateIcon => DiscordCustomIconText ? DiscordTemplateIcon : Default.DiscordTemplateIcon;
        public RoundInfoTemplate DiscordTemplateIcon = new RoundInfoTemplate("<js>IsOptedIn ? (IsAlive ? (IsKiller ? 'Killer' : 'Alive') : 'Dead') : 'Opted Out'</js>");

        public bool DiscordCustomStartText { get; set; }
        public RoundInfoTemplate GetDiscordTemplateStart => DiscordCustomStartText ? DiscordTemplateStart : Default.DiscordTemplateStart;
        public RoundInfoTemplate DiscordTemplateStart = new RoundInfoTemplate("Starting a {RoundType} round...");

        /// <summary>
        /// Enables OpenRGB support.
        /// </summary>
        public bool OpenRGBEnabled { get; set; }

        /// <summary>
        /// Automatically copy newly detected save codes as you play.
        /// </summary>
        public bool AutoCopy { get; set; }
        public bool CopyOnOpen { get; set; } = true;
        public bool CopyOnJoin { get; set; } = true;
        public bool CopyOnSave { get; set; } = false;

        /// <summary>
        /// Play notification audio when a new save is detected.
        /// </summary>
        public bool PlayAudioSave { get; set; } = true;
        public bool PlayAudioCopy { get; set; } = true;

        /// <summary>
        /// Custom audio location, must be .wav
        /// </summary>
        public string? AudioLocation { get; set; }
        public string? AudioCopyLocation { get; set; }

        /// <summary>
        /// Saves a list of players that were in the same room as you at the time of the save.
        /// </summary>
        public bool SaveNames { get; set; }

        /// <summary>
        /// Save a list of terrors that you survived when saving.
        /// </summary>
        public bool SaveRoundInfo { get; set; } = true;
        public bool ShowWinLose { get; set; } = true;

        /// <summary>
        /// Automatically set a note to the save with the survived terrors.
        /// </summary>
        public bool SaveRoundNote { get; set; } = true;
        public RoundInfoTemplate RoundNoteTemplate { get; set; } = new RoundInfoTemplate("{TerrorName}");

        /// <summary>
        /// Skips already parsed logs to save startup performance.
        /// </summary>
        public bool SkipParsedLogs { get; set; } = true;

        /// <summary>
        /// The update rate for log parsing.
        /// </summary>
        public int LogUpdateRate { get; set; } = 1000;

        /// <summary>
        /// Send popup notifications to XSOverlay.
        /// </summary>
        public bool XSOverlay { get; set; }
        public int XSOverlayPort = Utils.XSOverlay.DefaultPort;

        /// Time format settings.
        public bool Use24Hour { get; set; } = true;
        public bool ShowSeconds { get; set; } = true;
        public bool InvertMD { get; set; }
        public bool ShowDate { get; set; } // Show full date in entries
        public bool ShowTime { get; set; } = true; // Show time in entries.

        /// <summary>
        /// If true, objectives items will be colored like the items in game.
        /// </summary>
        public bool ColorfulObjectives { get; set; } = true;

        /// <summary>
        /// Enables OSC and sends avatar parameters based on in-game events.
        /// </summary>
        public bool OSCEnabled { get; set; }

        /// <summary>
        /// Sends Terror color via OSC with HSV values.
        /// </summary>
        public bool OSCSendColor { get; set; }

        /// <summary>
        /// Sends other formats instead of HSV color values if OSCSendColor is enabled.
        /// 0 - HSV
        /// 1 - HSL
        /// 2 - RGB
        /// 3 - RGB32
        /// </summary>
        public byte OSCSendColorFormat { get; set; }

        /// <summary>
        /// Sends damage as an INT via OSC
        /// </summary>
        public bool OSCDamagedEvent { get; set; }
        public RoundInfoTemplate OSCDamageTemplate { get; set; } = new RoundInfoTemplate("Damage");
        public RoundInfoTemplate OSCDamageIntervalTemplate { get; set; } = new RoundInfoTemplate("1000");


        /// <summary>
        /// Sends a death event for known users specified on a list.
        /// </summary>
        public bool OSCDeathEvent { get; set; }
        public int OSCDeathDecay { get; set; } = 200; // Time to wait before resetting params
        public int OSCDeathCooldown { get; set; } = 500; // Time to wait before next death
        public string[] OSCDeathIDs { get; set; } = Array.Empty<string>();

        public int GetDeathID (string id) {
            if (OSCDeathIDs == null || OSCDeathIDs.Length == 0) return 0;
            return Array.IndexOf(OSCDeathIDs, id) + 1;
        }


        /// <summary>
        /// OSC Master Changed event for OSC.
        /// </summary>
        public bool OSCMasterChange { get; set; } = false;
        /// <summary>
        /// OSC Master Change event interval.
        /// </summary>
        public int OSCMasterChangeInterval { get; set; } = 500;

        /// <summary>
        /// Enables OSC chatbox messages.
        /// </summary>
        public bool OSCSendChatbox { get; set; } = false;

        /// <summary>
        /// The template used for the chatbox message. Some strings will be replaced.
        /// </summary>
        public RoundInfoTemplate OSCMessageInfoTemplate { get; set; } =
            new RoundInfoTemplate("- Lobby Stats -\nLobby Stuns : {LobbyStunsAll}\nLobby Stun Record : {LobbyTopStunsAll}\n<js>RoundStunsAll < 1 ? '' : 'Current Round Stuns : ' + {RoundStunsAll}</js>");

        /// <summary>
        /// How often the message will be repeated to VRC for a consistent chatbox.
        /// </summary>
        public int OSCMessageInterval { get; set; } = 5;

        /// <summary>
        /// Changes avatar when you become the saboteur on a Sabotage round.
        /// </summary>
        public bool OSCSabotageAvatarChange { get; set; } = false;

        /// <summary>
        /// Avatar ID to set on a Sabotage round.
        /// </summary>
        public string OSCSabotageAvatarID { get; set; } = string.Empty;

        #region OBS Features
        /// <summary>
        /// Enables writing round info to files using custom templates.
        /// </summary>
        public bool RoundInfoToFile { get; set; }
        public RoundInfoTemplate[] RoundInfoTemplates { get; set; } = [
            new ("ton_terror_name.txt", "{TerrorName}"),
            new ("ton_round_type.txt", "{RoundType}"),
            new ("ton_map_name.txt", "{MapName}<js>(MapOrigin ? 'from ' + MapOrigin : '') + (MapCreator ? 'by ' + MapCreator : '')</js>")
        ];
        #endregion

        /// <summary>
        /// Stores a github release tag if the player clicks no when asking for update.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public string? IgnoreRelease { get; set; }

        /// <summary>
        /// Used internally for language selection.
        /// </summary>
        public string SelectedLanguage { get; set; } = string.Empty;

        /// <summary>
        /// Enable the save manager WebSocket API to receive events externally.
        /// </summary>
        public bool WebSocketEnabled { get; set; } = false;
        public bool WebTrackerCompatibility { get; set; } = true;
        public int WebSocketPort { get; set; } = 11398;

        /// <summary>
        /// Import a settings instance from the local json file
        /// </summary>
        /// <returns>Deserialized Settings object, or else Default Settings object.</returns>
        public static Settings Import()
        {
            string destination = Path.Combine(Program.DataLocation, Destination);
            Settings? settings;

            try
            {
                if (File.Exists(LegacyDestination))
                    File.Move(LegacyDestination, Destination);

                if (File.Exists(Destination) && !File.Exists(destination))
                {
                    File.Copy(Destination, destination, true);
                    File.Move(Destination, Destination + ".old");
                }

                Destination = destination;

                if (File.Exists(Destination)) {
                    string content = File.ReadAllText(Destination);
                    settings = JsonConvert.DeserializeObject<Settings>(content);
                } else settings = null;
            }
            catch (Exception ex)
            {
                Logger.Error("An error ocurred while trying to import your settings.\n" + ex);
                settings = null;
            }

            if (settings == null)
                settings = new Settings();

            string selectedLanguage = settings.SelectedLanguage;
            Logger.Debug("Selected language in settings is: " + selectedLanguage);

            if (string.IsNullOrEmpty(selectedLanguage)) {
                Logger.Info("Searching language key...");
                selectedLanguage = LANG.FindLanguageKey();
            }

            LANG.Select(selectedLanguage);

            return settings;
        }

        private void TryExport()
        {
            try {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(Destination, json);
            } catch (Exception e) {
                MessageBox.Show((LANG.S("MESSAGE.WRITE_SETTINGS_ERROR") ?? "An error ocurred while trying to write your settings to a file.\n\nMake sure that the program contains permissions to write files in the current folder it's located at.") + "\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
