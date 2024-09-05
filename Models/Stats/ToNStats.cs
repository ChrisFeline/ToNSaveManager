using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Models.Index;

namespace ToNSaveManager.Models.Stats {
    internal class StatPropertyContainer {
        public string Key;
        public PropertyInfo Source;
        public PropertyInfoContainer Property;
        public string Name => Property.Name;
        public string KeyUpper;

        private StatsBase? m_StatsBase;
        public StatsBase? Instance => m_StatsBase ?? (m_StatsBase = (StatsBase?)Source.GetValue(null));

        public StatPropertyContainer(string key, PropertyInfo source, PropertyInfoContainer property) {
            Key = key;
            KeyUpper = key.ToUpperInvariant();
            Source = source;
            Property = property;
        }

        public bool CanWrite => Property.CanWrite;

        public object? GetValue() => Property.GetValue(Instance);
        public T? GetValue<T>() => (T?)Property.GetValue(Instance);
        public void SetValue(object? value) {
            Property.SetValue(Instance, value);
            Instance?.SetDirty();
        }

        public object? Value => GetValue();

        public int IntValue {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string? StringValue {
            get => GetValue<string?>();
            set => SetValue(value);
        }
    }

    internal static class ToNStats {
        static readonly LoggerSource Log = new LoggerSource(nameof(ToNStats));

        #region Settings
        private static string m_Destination = "Stats.json";
        internal static string Destination {
            get => m_Destination;
            set {
                Log.Print("Setting destination: " + value);
                m_Destination = Path.Combine(value, "Stats.json");
            }
        }
        #endregion

        #region Storage
        private static StatsData? m_Stats { get; set; }

        public static StatsData Local => m_Stats ?? (m_Stats = StatsData.Import(Destination));
        public static StatsLobby Lobby { get; private set; } = new StatsLobby();
        public static StatsRound Round { get; private set; } = new StatsRound();

        public static void Export(bool force) {
            Local.Export(force);
            TableModified.Clear();
        }
        #endregion

        #region ToN Methods
        const string KEY_LOBBY = nameof(Lobby);
        const string KEY_ROUND = nameof(Round);
        const string KEY_SURVIVALS = nameof(StatsData.Survivals);
        const string KEY_DEATHS = nameof(StatsData.Deaths);
        const string KEY_ALL_ROUNDS = nameof(StatsData.Rounds);
        const string KEY_LOBBY_ALL_ROUNDS = KEY_LOBBY + nameof(StatsData.Rounds);
        const string KEY_LOBBY_SURVIVALS = KEY_LOBBY + nameof(StatsData.Survivals);
        const string KEY_LOBBY_DEATHS = KEY_LOBBY + nameof(StatsData.Deaths);
        public static void AddRound(bool survived, bool ready) {
            Add(survived ? KEY_LOBBY_SURVIVALS : KEY_LOBBY_DEATHS);
            MarkModified(KEY_LOBBY_ALL_ROUNDS);

            if (ready) {
                Add(survived ? KEY_SURVIVALS : KEY_DEATHS);
                MarkModified(KEY_ALL_ROUNDS);
            }
        }

        const string KEY_TOP_STUNS = nameof(StatsData.TopStuns);
        const string KEY_TOP_STUNS_ALL = nameof(StatsData.TopStunsAll);
        const string KEY_LOBBY_TOP_STUNS = KEY_LOBBY + nameof(StatsData.TopStuns);
        const string KEY_LOBBY_TOP_STUNS_ALL = KEY_LOBBY + nameof(StatsData.TopStunsAll);

        const string KEY_STUNS = nameof(StatsData.Stuns);
        const string KEY_STUNS_ALL = nameof(StatsData.StunsAll);
        const string KEY_LOBBY_STUNS = KEY_LOBBY + nameof(StatsData.Stuns);
        const string KEY_LOBBY_STUNS_ALL = KEY_LOBBY + nameof(StatsData.StunsAll);
        const string KEY_ROUND_STUNS = KEY_ROUND + nameof(StatsRound.Stuns);
        const string KEY_ROUND_STUNS_ALL = KEY_ROUND + nameof(StatsRound.StunsAll);
        public static void AddStun(bool isLocal, bool ready) {
            Add(isLocal ? KEY_LOBBY_STUNS : KEY_LOBBY_STUNS_ALL);

            string roundKey = isLocal ? KEY_ROUND_STUNS : KEY_ROUND_STUNS_ALL;
            int roundStuns = Get<int>(roundKey) + 1;
            Set(roundKey, roundStuns);

            SetMax(isLocal ? KEY_LOBBY_TOP_STUNS : KEY_LOBBY_TOP_STUNS_ALL, roundStuns);

            if (ready) {
                Add(isLocal ? KEY_STUNS : KEY_STUNS_ALL);
                SetMax(isLocal ? KEY_TOP_STUNS : KEY_TOP_STUNS_ALL, roundStuns);
            } else {
                int topLobby = Get<int>(isLocal ? KEY_LOBBY_TOP_STUNS : KEY_LOBBY_TOP_STUNS_ALL);
                SetMax(isLocal ? KEY_TOP_STUNS : KEY_TOP_STUNS_ALL, topLobby);
            }
        }

        const string KEY_DAMAGE = nameof(StatsData.DamageTaken);
        const string KEY_LOBBY_DAMAGE = KEY_LOBBY + nameof(StatsData.DamageTaken);
        public static void AddDamage(int damage, bool ready) {
            Add(KEY_LOBBY_DAMAGE, damage);
            if (ready) Add(KEY_DAMAGE, damage);
        }

        // Round only stuff
        const string KEY_ROUND_TYPE = nameof(StatsRound.RoundType);
        const string KEY_TERROR_NAME = nameof(StatsRound.TerrorName);
        public static void AddTerrors(TerrorMatrix terrorMatrix) {
            Set(KEY_ROUND_TYPE, terrorMatrix.RoundType.ToString());
            Set(KEY_TERROR_NAME, terrorMatrix.GetTerrorNames());
        }

        const string KEY_MAP_NAME = nameof(StatsRound.MapName);
        const string KEY_MAP_CREATOR = nameof(StatsRound.MapCreator);
        const string KEY_MAP_ORIGIN = nameof(StatsRound.MapOrigin);
        public static void AddLocation(ToNIndex.Map map) {
            Set(KEY_MAP_NAME, map.IsEmpty ? "???" : map.Name);
            Set(KEY_MAP_CREATOR, map.IsEmpty ? "???" : map.Creator);
            Set(KEY_MAP_ORIGIN, map.IsEmpty ? "???" : map.Origin);
        }

        static readonly string[] LobbyClearKeys = [
            KEY_LOBBY_SURVIVALS, KEY_LOBBY_DEATHS, KEY_LOBBY_STUNS, KEY_LOBBY_STUNS_ALL, KEY_LOBBY_DAMAGE,
            KEY_LOBBY_TOP_STUNS, KEY_LOBBY_TOP_STUNS_ALL
        ];
        public static void ClearLobby() => Reset(LobbyClearKeys);

        static readonly string[] ClearRoundKeys = [ KEY_ROUND_STUNS, KEY_ROUND_STUNS_ALL ];
        public static void ClearRound() => Reset(ClearRoundKeys);
        #endregion

        #region Accessors
        public static bool HasKey(string key) => PropertyDictionary.ContainsKey(key);
        public static bool IsModified(string key) => TableModified.Contains(key);
        public static void MarkModified(string key) {
            if (!IsModified(key)) {
                TableModified.Add(key);
                Log.Debug("Marked Modified: " + key);
            }
        }

        public static T? Get<T>(string key) => PropertyDictionary.TryGetValue(key, out StatPropertyContainer? value) ? value.GetValue<T>() : default;
        public static object? Get(string key) => PropertyDictionary.TryGetValue(key, out StatPropertyContainer? value) ? value.GetValue() : default;

        public static void Set<T>(string key, T? val) {
            if (PropertyDictionary.TryGetValue(key, out StatPropertyContainer? value)) {
                T? original = Get<T>(key);

                if (original == null || !original.Equals(val)) {
                    value.SetValue(val);
                    MarkModified(key);

                    value.Instance?.SetDirty();
                }
            }
        }
        public static void Reset(string key) => Set<object>(key, null);
        public static void Reset(string[] keys) {
            foreach (string k in keys) Reset(k);
        }

        public static void Add(string key, int val = 1) {
            int value = Get<int>(key);
            Set(key, value + val);
        }
        public static void SetMax(string key, int val) {
            int value = Get<int>(key);
            if (val > value) Set(key, val);
        }
        #endregion

        #region Reflection
        internal static readonly HashSet<string> TableModified = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        internal static readonly Dictionary<string, StatPropertyContainer> PropertyDictionary = new Dictionary<string, StatPropertyContainer>(StringComparer.InvariantCultureIgnoreCase);

        internal static readonly StatPropertyContainer[] PropertyValues;
        internal static readonly string[] PropertyKeys;

        static ToNStats() {
            Type baseType = typeof(StatsBase);
            foreach (PropertyInfo propertyInfo in typeof(ToNStats).GetProperties(BindingFlags.Public | BindingFlags.Static).Where(t => baseType.IsAssignableFrom(t.PropertyType))) {
                string prefix = propertyInfo.Name == "Local" ? string.Empty : propertyInfo.Name;

                foreach (PropertyInfo prop in propertyInfo.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                    string key = prop.DeclaringType != propertyInfo.PropertyType ? prefix + prop.Name : prop.Name;
                    var statProp = new StatPropertyContainer(key, propertyInfo, new PropertyInfoContainer(prop));
                    Log.Debug("Full Name: " + propertyInfo.PropertyType.FullName + " | " + prop.Name);
                    PropertyDictionary.Add(key, statProp);
                    MarkModified(key);
                }
            }

            PropertyKeys = PropertyDictionary.Keys.Select(k => '{' + k + '}').ToArray();
            PropertyValues = PropertyDictionary.Values.ToArray();
        }
        #endregion
    }
}
