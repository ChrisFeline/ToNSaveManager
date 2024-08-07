using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToNSaveManager.Localization;

namespace ToNSaveManager.Models {
    internal class StatsData {
        private static string m_Destination = "Stats.json";
        public static string Destination {
            get => m_Destination;
            set {
                Debug.WriteLine("Setting destination: " + value);
                m_Destination = Path.Combine(value, "Stats.json");
            }
        }

        [JsonIgnore] internal bool IsDirty = false;
        internal void SetDirty() {
            IsDirty = true;
        }

        #region Properties
        public int Rounds => Deaths + Survivals;
        public int Deaths { get; set; } = 0;
        public int Survivals { get; set; } = 0;

        public void AddRound(bool survived) {
            Debug.WriteLine("Adding +1 round stat, survived: " + survived);

            if (survived) Survivals++;
            else Deaths++;

            SetDirty();
        }

        public int Stuns { get; set; } = 0;
        public int GlobalStuns { get; set; } = 0;
        public void AddStun(bool isLocal) {
            Debug.WriteLine("Adding +1 stun, isLocal: " + isLocal);

            if (isLocal) Stuns++;
            else GlobalStuns++;

            SetDirty();
        }

        public int DamageTaken { get; set; } = 0;
        public void AddDamage(int damage) {
            Debug.WriteLine($"Adding +{damage} damage.");

            DamageTaken += damage;

            SetDirty();
        }

        public void Clear() {
            Debug.WriteLine("Clearing lobby stats");

            Deaths = 0;
            Survivals = 0;

            Stuns = 0;
            GlobalStuns = 0;

            DamageTaken = 0;
        }
        #endregion

        public static StatsData Import() {
            StatsData? statsData = null;

            try {
                if (File.Exists(Destination)) {
                    string content = File.ReadAllText(Destination);
                    statsData = JsonConvert.DeserializeObject<StatsData>(content);
                }
            } catch {
                statsData = null;
            }

            if (statsData == null) {
                statsData = new StatsData();
            }

            return statsData;
        }

        public void Export() {
            try {
                if (IsDirty) {
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
