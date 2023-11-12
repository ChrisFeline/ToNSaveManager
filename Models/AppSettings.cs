using Newtonsoft.Json;
using System.Diagnostics;

namespace ToNSaveManager.Models
{
    internal class Settings
    {
        internal static readonly Settings Get;
        internal static void Export() => Get.TryExport();

        const string LegacyDestination = "settings.json";
        static string Destination;

        static Settings()
        {
            Destination = "Settings.json";
            Get = Import();
        }

        /// <summary>
        /// Automatically copy newly detected save codes as you play.
        /// </summary>
        public bool AutoCopy { get; set; }

        /// <summary>
        /// Play notification audio when a new save is detected.
        /// </summary>
        public bool PlayAudio { get; set; }

        /// <summary>
        /// Custom audio location, must be .wav
        /// </summary>
        public string? AudioLocation { get; set; }

        /// <summary>
        /// Saves a list of players that were in the same room as you at the time of the save.
        /// </summary>
        public bool SaveNames { get; set; }

        /// <summary>
        /// Send popup notifications to XSOverlay.
        /// </summary>
        public bool XSOverlay { get; set; }
        public int XSOverlayPort = Utils.XSOverlay.DefaultPort;

        /// Time format settings.
        public bool Use24Hour { get; set; } = true;
        public bool ShowSeconds { get; set; } = true;
        public bool InvertMD { get; set; }

        /// <summary>
        /// If true, objectives items will be colored like the items in game.
        /// </summary>
        public bool ColorfulObjectives { get; set; } = true;

        /// <summary>
        /// Stores a github release tag if the player clicks no when asking for update.
        /// </summary>
        public string? IgnoreRelease { get; set; }

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
                if (!File.Exists(Destination)) return new Settings();

                string content = File.ReadAllText(Destination);
                settings = JsonConvert.DeserializeObject<Settings>(content);
            }
            catch
            {
                settings = null;
            }

            return settings ?? new Settings();
        }

        private void TryExport()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(Destination, json);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error ocurred while trying to write your settings to a file.\n\nMake sure that the program contains permissions to write files in the current folder it's located at.\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
