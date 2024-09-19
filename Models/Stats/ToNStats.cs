using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Utils.API;
using Jint;
using System.Windows.Forms.VisualStyles;

namespace ToNSaveManager.Models.Stats {
    internal class StatPropertyContainer {
        public string Key { get; private set; }
        public PropertyInfo Source { get; private set; }
        public PropertyInfoContainer Property;
        public string Name => Property.Name;
        public Type PropertyType => Property.PropertyType;
        public string KeyUpper { get; private set; }
        public string KeyTemplate { get; private set; }
        public string KeyLang { get; private set; }

        private StatsBase? m_StatsBase;
        public StatsBase? Instance => m_StatsBase ?? (m_StatsBase = (StatsBase?)Source.GetValue(null));

        public StatPropertyContainer(string key, PropertyInfo source, PropertyInfoContainer property) {
            Key = key;
            KeyUpper = key.ToUpperInvariant();
            KeyTemplate = '{' + Key + '}';
            KeyLang = "STATS.LABEL_" + KeyUpper;
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

        const string KEY_ONLINE_PLAYERS = nameof(StatsLobby.PlayersOnline);
        public static void AddPlayerCount(int players) {
            Set(KEY_ONLINE_PLAYERS, players);
        }

        const string KEY_DISPLAY_NAME = nameof(StatsLobby.DisplayName);
        const string KEY_DISCORD_NAME = nameof(StatsLobby.DiscordName);
        public static void SetDisplayName(string displayName, bool isDiscord) {
            if (isDiscord) {
                Set(KEY_DISCORD_NAME, displayName);
            } else Set(KEY_DISPLAY_NAME, displayName);
        }
        const string KEY_INSTANCE_ID = nameof(StatsLobby.InstanceURL);
        public static void SetInstanceURL(string instanceURL) {
            Set(KEY_INSTANCE_ID, instanceURL);
        }

        // Round only stuff
        const string KEY_ROUND_TYPE = nameof(StatsRound.RoundType);
        const string KEY_ROUND_INT = nameof(StatsRound.RoundInt);
        const string KEY_TERROR_NAME = nameof(StatsRound.TerrorName);
        public static void AddTerrors(TerrorMatrix terrorMatrix) {
            Set(KEY_ROUND_TYPE, MainWindow.GetRoundTypeName(terrorMatrix.RoundType));
            Set(KEY_ROUND_INT, (int)terrorMatrix.RoundType);
            Set(KEY_TERROR_NAME, terrorMatrix.Length > 0 ? terrorMatrix.GetTerrorNames() : (terrorMatrix.MapID < 0 ? "Overseer" : "???"));
        }

        const string KEY_IS_ALIVE = nameof(StatsRound.IsAlive);
        const string KEY_IS_KILLER = nameof(StatsRound.IsKiller);
        public static void AddIsAlive(bool value) => Set(KEY_IS_ALIVE, value);
        public static void AddIsKiller(bool value) => Set(KEY_IS_KILLER, value);

        const string KEY_MAP_NAME = nameof(StatsRound.MapName);
        const string KEY_MAP_INT = nameof(StatsRound.MapInt);
        const string KEY_MAP_CREATOR = nameof(StatsRound.MapCreator);
        const string KEY_MAP_ORIGIN = nameof(StatsRound.MapOrigin);
        public static void AddLocation(ToNIndex.Map map, bool killersSet) {
            Set(KEY_MAP_NAME, map.IsEmpty && killersSet ? "Somewhere" : map.Name);
            Set(KEY_MAP_CREATOR, map.Creator);
            Set(KEY_MAP_ORIGIN, map.Origin);
            Set(KEY_MAP_INT, map.Id);
        }

        const string KEY_PAGE_COUNT = nameof(StatsRound.PageCount);
        public static void AddPageCount(int pages) {
            Set(KEY_PAGE_COUNT, pages);
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
        public static void MarkModified(string key, StatPropertyContainer? property = null) {
            if (!IsModified(key)) {
                TableModified.Add(key);
            }

            JSEngine.SetValue(key, property == null ? Get(key) : property.GetValue());
        }

        public static T? Get<T>(string key) => PropertyDictionary.TryGetValue(key, out StatPropertyContainer? value) ? value.GetValue<T>() : default;
        public static object? Get(string key) => PropertyDictionary.TryGetValue(key, out StatPropertyContainer? value) ? value.GetValue() : default;
        public static Type? GetType(string key) => PropertyDictionary.TryGetValue(key, out StatPropertyContainer? value) ? value.PropertyType : null;

        public static void Set<T>(string key, T? val) {
            if (PropertyDictionary.TryGetValue(key, out StatPropertyContainer? value)) {
                T? original = Get<T>(key);

                if (original == null || !original.Equals(val)) {
                    value.SetValue(val);
                    MarkModified(key, value);

                    value.Instance?.SetDirty();

                    WebSocketAPI.EventStats.Send(key, val);
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
        internal static readonly string[] PropertyKeys;
        internal static readonly StatPropertyContainer[] PropertyValues;

        internal static readonly Dictionary<string, StatPropertyContainer[]> PropertyGroups = new Dictionary<string, StatPropertyContainer[]>();

        internal static readonly Jint.Engine JSEngine = new Jint.Engine(options => {
            options.LimitMemory(10_000_000);
        });

        static ToNStats() {
            Type baseType = typeof(StatsBase);
            List<StatPropertyContainer> values = new List<StatPropertyContainer>();
            foreach (PropertyInfo propertyInfo in typeof(ToNStats).GetProperties(BindingFlags.Public | BindingFlags.Static).Where(t => baseType.IsAssignableFrom(t.PropertyType))) {
                string prefix = propertyInfo.Name == "Local" ? string.Empty : propertyInfo.Name;

                foreach (PropertyInfo prop in propertyInfo.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                    string key = prop.DeclaringType != propertyInfo.PropertyType ? prefix + prop.Name : prop.Name;
                    var statProp = new StatPropertyContainer(key, propertyInfo, new PropertyInfoContainer(prop));
                    PropertyDictionary.Add(key, statProp);
                    MarkModified(key, statProp);
                    values.Add(statProp);
                    // Register this property on the interpreter
                }

                PropertyGroups[propertyInfo.Name] = values.ToArray();
                values.Clear();
            }

            PropertyKeys = TableModified.ToArray();
            PropertyValues = PropertyDictionary.Values.ToArray();
        }
        #endregion
    }
}
