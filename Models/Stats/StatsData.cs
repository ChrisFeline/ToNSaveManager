using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToNSaveManager.Localization;
using ToNSaveManager.Models.Index;

namespace ToNSaveManager.Models.Stats
{
    public class PropertyInfoContainer
    {
        public PropertyInfo Property;
        public string Name => Property.Name;
        public string Key => Name.ToUpperInvariant();

        private MethodInfo? GetMethod { get; set; }
        private MethodInfo? SetMethod { get; set; }

        public bool CanWrite => Property.CanWrite && SetMethod != null && !SetMethod.IsPrivate;
        public bool IsStatic => GetMethod != null && GetMethod.IsStatic;

        public object? GetValue(object? instance) => Property.GetValue(instance);
        public void SetValue(object? instance, object? value) => Property.SetValue(instance, value);

        public PropertyInfoContainer(PropertyInfo property)
        {
            Property = property;
            GetMethod = property.GetGetMethod(false);
            SetMethod = property.GetSetMethod(false);
        }

        public override string ToString() {
            return Name;
        }
    }

    internal class StatsData
    {
        private static string m_Destination = "Stats.json";
        internal static string Destination
        {
            get => m_Destination;
            set
            {
                Logger.Log("Setting destination: " + value);
                m_Destination = Path.Combine(value, "Stats.json");
            }
        }

        [JsonIgnore] internal bool IsDirty = false;
        internal void SetDirty()
        {
            IsDirty = true;
        }

        #region Reflection
        internal static readonly PropertyInfoContainer[] TableProperties;
        internal static readonly Dictionary<string, PropertyInfoContainer> TableDictionary;
        internal static readonly HashSet<string> TableModified; // IDK

        static StatsData()
        {
            TableProperties = typeof(StatsData).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                .Where(p => !p.Name.StartsWith("_")).Select(p => new PropertyInfoContainer(p)).ToArray();

            TableModified = new(StringComparer.InvariantCultureIgnoreCase);
            TableDictionary = new(StringComparer.InvariantCultureIgnoreCase);
            foreach (var info in TableProperties)
            {
                TableDictionary.Add(info.Name, info);
                TableModified.Add(info.Name); // Start modified to apply changes on startup
            }
        }

        public static bool IsModified(string key) => TableModified.Contains(key);
        public static void MarkModified(string key)
        {
            if (!IsModified(key))
                TableModified.Add(key);
        }

        static readonly StringBuilder StrBuild = new StringBuilder();
        internal static void SetTerrorMatrix(TerrorMatrix terrorMatrix) {
            StrBuild.Clear();

            for (int i = 0; i < terrorMatrix.Length; i++) {
                if (StrBuild.Length > 0)
                    StrBuild.Append(" & ");

                StrBuild.Append(terrorMatrix[i].Name);
            }

            if (StrBuild.Length == 0) StrBuild.Append("???");

            RoundType = terrorMatrix.RoundType.ToString();
            MarkModified(nameof(RoundType));

            TerrorName = StrBuild.ToString();
            MarkModified(nameof(TerrorName));
        }
        internal static void SetLocation(ToNIndex.Map map) {
            MapName = map.IsEmpty ? "???" : map.Name;
            MarkModified(nameof(MapName));

            MapCreator = map.IsEmpty ? "???" : map.Creator;
            MarkModified(nameof(MapCreator));

            MapOrigin = map.IsEmpty ? "???" : map.Origin;
            MarkModified(nameof(MapName));
        }
        #endregion

        #region Properties
        public int Rounds => Deaths + Survivals;
        public int Deaths { get; set; } = 0;
        public int Survivals { get; set; } = 0;

        public void AddRound(bool survived)
        {
            if (survived) {
                Survivals++;
                MarkModified(nameof(Survivals));
            } else {
                Deaths++;
                MarkModified(nameof(Deaths));
            }

            MarkModified(nameof(Rounds));
            SetDirty();
        }

        public int Stuns { get; set; } = 0;
        public int GlobalStuns { get; set; } = 0;
        public void AddStun(bool isLocal)
        {
            if (isLocal) {
                Stuns++;
                MarkModified(nameof(Stuns));
            } else {
                GlobalStuns++;
                MarkModified(nameof(GlobalStuns));
            }

            SetDirty();
        }

        public int DamageTaken { get; set; } = 0;
        public void AddDamage(int damage)
        {
            DamageTaken += damage;
            MarkModified(nameof(DamageTaken));

            SetDirty();
        }

        public static string TerrorName { get; internal set; } = "???";
        public static string RoundType { get; internal set; } = "???";
        public static string MapName { get; internal set; } = "???";
        public static string MapCreator { get; internal set; } = "???";
        public static string MapOrigin { get; internal set; } = "???";

        public void Clear()
        {
            Logger.Debug("Clearing lobby stats");

            Deaths = 0;
            Survivals = 0;

            Stuns = 0;
            GlobalStuns = 0;

            DamageTaken = 0;
        }
        #endregion

        public static StatsData Import()
        {
            StatsData? statsData = null;

            try
            {
                if (File.Exists(Destination))
                {
                    string content = File.ReadAllText(Destination);
                    statsData = JsonConvert.DeserializeObject<StatsData>(content);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                statsData = null;
            }

            if (statsData == null)
            {
                statsData = new StatsData();
            }

            return statsData;
        }

        public void Export()
        {
            try
            {
                if (TableModified.Count > 0) TableModified.Clear();

                if (IsDirty)
                {
                    IsDirty = false;

                    string json = JsonConvert.SerializeObject(this);
                    File.WriteAllText(Destination, json);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show((LANG.S("MESSAGE.WRITE_SETTINGS_ERROR") ?? "An error occurred while trying to write your stats to a file.\n\nMake sure that the program contains permissions to write files in the current folder it's located at.") + "\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
