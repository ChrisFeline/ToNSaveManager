using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToNSaveManager.Localization;

namespace ToNSaveManager.Models.Stats
{
    internal class StatsBase {
        [JsonIgnore] internal bool IsDirty = false;
        internal void SetDirty() => IsDirty = true;

        public int Stuns { get; set; } = 0;
        public int StunsAll { get; set; } = 0;
    }

    internal class StatsRound : StatsBase {
        public string TerrorName { get; internal set; } = "???";
        public string RoundType { get; internal set; } = "???";
        public string MapName { get; internal set; } = "???";
        public string MapCreator { get; internal set; } = "???";
        public string MapOrigin { get; internal set; } = "???";
        public string ItemName { get; internal set; } = string.Empty;

        public bool IsAlive { get; internal set; } = true;
        public bool IsKiller { get; internal set; } = false;
        public bool IsStarted { get; internal set; } = false;

        public int RoundInt { get; internal set; } = 0;
        public int MapInt { get; internal set; } = 255;

        public int PageCount { get; internal set; } = 0;
    }

    internal class StatsLobby : StatsData {
        public int PlayersOnline { get; internal set; } = 0;
        public bool IsOptedIn { get; internal set; } = false;

        // Player data
        public string DisplayName { get; internal set; } = "Unknown";
        public string DiscordName { get; internal set; } = "Unknown";
        public string InstanceURL { get; internal set; } = string.Empty;
    }

    internal class StatsData : StatsBase {
        public int Rounds => Deaths + Survivals;
        public int Deaths { get; set; } = 0;
        public int Survivals { get; set; } = 0;
        public int DamageTaken { get; set; } = 0;
        public int TopStuns { get; set; } = 0;
        public int TopStunsAll { get; set; } = 0;

        public static StatsData Import(string path)
        {
            StatsData? statsData;

            try {
                if (File.Exists(path)) {
                    string content = File.ReadAllText(path);
                    statsData = JsonConvert.DeserializeObject<StatsData>(content);
                } else statsData = null;
            } catch (Exception ex) {
                Logger.Error(ex);
                statsData = null;
            }

            if (statsData == null) statsData = new StatsData();
            statsData.Destination = path;

            return statsData;
        }

        [JsonIgnore] internal string? Destination { get; set; }
        public void Export(bool force)
        {
            if (string.IsNullOrEmpty(Destination)) return;

            try {
                if (IsDirty || force) {
                    Logger.Debug("Exporting stats data!");
                    IsDirty = false;

                    string json = JsonConvert.SerializeObject(this);
                    File.WriteAllText(Destination, json);
                }
            } catch (Exception e) {
                MessageBox.Show((LANG.S("MESSAGE.WRITE_SETTINGS_ERROR") ?? "An error occurred while trying to write your stats to a file.\n\nMake sure that the program contains permissions to write files in the current folder it's located at.") + "\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
