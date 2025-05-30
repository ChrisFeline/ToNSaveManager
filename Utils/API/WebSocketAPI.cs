using Jint.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Models.Stats;
using ToNSaveManager.Utils.JSPlugins;
using ToNSaveManager.Utils.LogParser;
// using WebSocketSharp;
// using WebSocketSharp.Server;
using WebSocketServer = WatsonWebsocket.WatsonWsServer;

namespace ToNSaveManager.Utils.API {
    internal class WebSocketAPI {
        static readonly LoggerSource Logger = new LoggerSource(nameof(WebSocketAPI));
        internal static WebSocketServer? Server;

        static void OnOpen(WatsonWebsocket.ClientMetadata metadata) {
            Logger.Log("WebSocket client connected: " + metadata.Guid + " | " + metadata.Ip);

            SendEvent(new EventConnected() {
                Args = EventBuffer.ToArray(),
                DisplayName = ToNLogContext.Instance?.DisplayName ?? string.Empty,
                UserID = ToNLogContext.Instance?.UserID ?? string.Empty
            }, metadata);

            SendEvent(new EventValue<string?>("INSTANCE", ToNLogContext.Instance?.InstanceID), metadata);

            // Send All Stats
            foreach (string key in ToNStats.PropertyKeys) {
                SendEvent(new EventStats() { Name = key, Value = ToNStats.Get(key) }, metadata);
            }
        }

        const int DEFAULT_PORT = 11398;
        internal static void Initialize() {
            if (Settings.Get.WebSocketEnabled && Server == null) {
                int port = Settings.Get.WebSocketPort > 0 ? Settings.Get.WebSocketPort : DEFAULT_PORT;
                Server = new WebSocketServer(new List<string>() { IPAddress.Loopback.ToString(), "localhost" }, port);
                //Server.AddWebSocketService<WebSocketAPI>("/");

                Server.ClientConnected += Server_ClientConnected;
                Server.ClientDisconnected += Server_ClientDisconnected;
            }

            if (Settings.Get.WebSocketEnabled && Server != null && !Server.IsListening) {
                Logger.Log("Starting Server...");
                Server.Start();
            } else if (!Settings.Get.WebSocketEnabled && Server != null && Server.IsListening) {
                Logger.Log("Stopping Server...");
                Server.Stop();
                Server.Dispose();
                Server = null;
            }
        }

        private static void Server_ClientDisconnected(object? sender, WatsonWebsocket.DisconnectionEventArgs e) {
            Logger.Log("WebSocket client disconnected" + e.Client.Guid);
        }

        private static void Server_ClientConnected(object? sender, WatsonWebsocket.ConnectionEventArgs e) {
            OnOpen(e.Client);
        }

        internal static void Broadcast(string data) {
            if (Server == null) return;
            Logger.Debug("Broadcasting: " + data);

            foreach (var metadata in Server.ListClients()) {
                _ = Server.SendAsync(metadata.Guid, data).Result;
            }
        }
        internal static void SendEvent<T>(T value, WatsonWebsocket.ClientMetadata? metadata) where T : IEvent {
            if (!Settings.Get.WebSocketEnabled) return;
            string jsonData = JsonConvert.SerializeObject(value);

            try {
                if (metadata != null) {
                    Logger.Debug("Sending: " + jsonData);
                    _ = Server?.SendAsync(metadata.Guid, jsonData).Result;
                    return;
                }

                Broadcast(jsonData);
            } catch (Exception e) {
                Logger.Error(e);
#if DEBUG
                throw;
#endif
            }
        }

        public interface IEvent {
            string Type { get; }
            byte Command { get; set; }
        }

        #region Event Structs
        public struct EventConnected : IEvent {
            public string Type => "CONNECTED";
            [JsonIgnore] public byte Command { get; set; }

            public string DisplayName { get; set; }
            public string UserID { get; set; }
            public IEvent[] Args { get; set; }
        }

        public struct EventValue<T> : IEvent {
            public string Type { get; private set; }
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public byte Command { get; set; }
            public T Value { get; private set; }

            public EventValue(string type, T value) {
                Type = type;
                Value = value;
                Command = 0;
            }
        }

        public struct EventStats : IEvent {
            public string Type => "STATS";
            [JsonIgnore] public byte Command { get; set; }

            public string Name { get; set; }
            public object? Value { get; set; }

            internal static void Send(string name, object? value) {
                QueueEvent(new EventStats() { Name = name, Value = value });
            }
        }

        public struct EventDeath : IEvent {
            public string Type => "DEATH";
            [JsonIgnore] public byte Command { get; set; }

            public string Name { get; set; }
            public string Message { get; set; }
            public bool IsLocal { get; set; }

            internal static void Send(string name, string message, bool isLocal) {
                QueueEvent(new EventDeath() { Name = name, Message = message, IsLocal = isLocal }, false);
            }
        }

        public struct EventTerror : IEvent {
            public string Type => "TERRORS";

            /// <summary>
            /// 0 Set -
            /// 1 Reveal -
            /// 2 Hidden
            /// </summary>
            public byte Command { get; set; }

            public string[] Names { get; set; }
            public string?[] Assets { get; set; }
            public string DisplayName { get; set; }
            public uint DisplayColor { get; set; }

            internal static void Send(TerrorMatrix matrix) {
                EventTerror eventTerror = new EventTerror();
                eventTerror.DisplayName = matrix.GetTerrorNames();
                eventTerror.DisplayColor = ColorToUInt(matrix.DisplayColor);
                if (matrix.Terrors.Length > 0) {
                    eventTerror.Names = new string[matrix.ActualCount];
                    eventTerror.Assets = new string[matrix.ActualCount];

                    for (int i = 0; i < eventTerror.Names.Length; i++) {
                        eventTerror.Names[i] = matrix[i + matrix.StartIndex].Name;
                        eventTerror.Assets[i] = matrix[i + matrix.StartIndex].AssetID;
                    }
                }

                byte command;
                if (matrix.IsUnknown) command = 2;
                else if (matrix.IsRevealed) command = 1;
                else command = (byte)(matrix.IsEmpty ? 255 : (matrix.Length > 0 ? 0 : 4));

                eventTerror.Command = command;
                QueueEvent(eventTerror);
            }
        }

        public struct EventRoundType : IEvent {
            public string Type => "ROUND_TYPE";

            /// <summary>
            /// 0 - Round Over
            /// 1 - Round Start
            /// </summary>
            public byte Command { get; set; }

            public ToNRoundType Value { get; set; }
            public string Name => Value.ToString();
            public string DisplayName => MainWindow.GetRoundTypeName(Value);
            public uint DisplayColor { get; set; } // Maybe a bit innacurate

            internal static void Send(ToNRoundType roundType) {
                EventRoundType eventRoundType = new EventRoundType();
                eventRoundType.Value = roundType;
                eventRoundType.DisplayColor = TerrorMatrix.GetRoundColorFromType(roundType);
                eventRoundType.Command = (byte)(roundType == ToNRoundType.Intermission ? 0 : 1);
                QueueEvent(eventRoundType);
            }
        }

        public struct EventLocation : IEvent {
            public string Type => "LOCATION";
            public byte Command { get; set; }

            public string Name { get; set; }
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Creator { get; set; }
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string Origin { get; set; }

            internal static void Send(ToNIndex.Map map) {
                EventLocation eventLocation = new() {
                    Name = map.Name,
                    Creator = map.Creator,
                    Origin = map.Origin,
                    Command = (byte)(map.IsEmpty ? 0 : 1)
                };
                QueueEvent(eventLocation);
            }
        }
        
        public struct EventItem : IEvent {
            public string Type => "ITEM";
            public byte Command { get; set; }

            public string Name { get; set; }
            public int ID { get; set; }

            internal static void Send(ToNIndex.Item item) {
                EventItem eventItem = new() {
                    Name = item.Name,
                    ID = item.Id,
                    Command = (byte)(item.IsEmpty ? 0 : 1)
                };
                QueueEvent(eventItem);
            }
        }

        public struct EventPlayerJoin : IEvent {
            const string EVENT_PLAYER_JOIN = "PLAYER_JOIN";
            const string EVENT_PLAYER_LEAVE = "PLAYER_LEAVE";

            public string Type { get; private set; }
            [JsonIgnore] public byte Command { get; set; }

            public string Value { get; set; }
            public string ID { get; set; }

            internal static void Send(LogPlayer player, bool joined) {
                EventPlayerJoin eventPlayer = new EventPlayerJoin() {
                    Type = joined ? EVENT_PLAYER_JOIN : EVENT_PLAYER_LEAVE,
                    Value = player.Name,
                    ID = player.GUID
                };
                QueueEvent(eventPlayer);
            }
        }
        
        public struct EventCustom : IEvent {
            public string Type => "CUSTOM";
            [JsonIgnore] public byte Command { get; set; }

            public string Source { get; set; }
            public string Name { get; set; }
            public object? Value { get; set; }

            internal static void Send(string source, string name, object? value = null) {
                EventCustom eventCustom = new EventCustom() {
                    Source = source,
                    Name = name,
                    Value = value
                };

                QueueEvent(eventCustom, false);
            }
        }
        #endregion

        #region Save Manager Events
        private static List<IEvent> EventBuffer = new();
        internal static void ClearBuffer() {
            Logger.Debug("CLEARING BUFFER!!!");
            EventBuffer.Clear();
        }

        internal static Queue<IEvent> EventQueue = new Queue<IEvent>();
        private static void QueueEvent(IEvent ev, bool buffer = true) {
            if (MainWindow.Started) EventQueue.Enqueue(ev);
            if (buffer) EventBuffer.Add(ev);

            JSEngine.InvokeOnEvent(ev);
        }

        internal static void SendEventUpdate() {
            while (EventQueue.Count > 0) {
                SendEvent(EventQueue.Dequeue(), null);
            }
        }

        internal static void SendValue<T>(string type, T value, bool buffer = true) {
            QueueEvent(new EventValue<T>(type, value), buffer);
        }
        #endregion

        #region Live Tracker Compatibility
        public struct EventTracker : IEvent {
            public string Type => "TRACKER";
            [JsonIgnore] public byte Command { get; set; }

            [JsonProperty("event")] public string Event { get; set; }
            [JsonProperty("args")] public string[] Args { get; private set; }

            public EventTracker(string eventName, string[] args) {
                Event = eventName;
                Args = args;
            }
        }

        static Dictionary<string, Regex>? RegularExpressions;

        public class RegexConverter : JsonConverter<Regex> {
            public override Regex ReadJson(JsonReader reader, Type objectType, Regex? existingValue, bool hasExistingValue, JsonSerializer serializer) {
                var regexPattern = JToken.Load(reader).ToString();
                return new Regex(regexPattern, RegexOptions.Compiled); // Create the Regex object
            }

            public override void WriteJson(JsonWriter writer, Regex? value, JsonSerializer serializer) {
                writer.WriteValue(value?.ToString());
            }
        }

        internal static void OnReadLine(string line) {
            line = line.TrimEnd();

            if (RegularExpressions == null) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        const string url = "https://app.tontrack.me/regex.json";
                        string result = client.GetStringAsync(url).Result;

                        RegularExpressions = JsonConvert.DeserializeObject<Dictionary<string, Regex>>(result, new RegexConverter());
                    }
                } catch (Exception ex) {
                    Logger.Error("An error ocurred while trying to Fetch data from: tontrack.me\n" + ex);
                    return;
                }
            }

            if (RegularExpressions != null) {
                foreach (KeyValuePair<string, Regex> pair in RegularExpressions) {
                    Regex regex = pair.Value;
                    if (regex.IsMatch(line)) {
                        Match match = regex.Match(line);

                        string[] array = new string[match.Groups.Count - 1];
                        for (int i = 1; i < match.Groups.Count; i++)
                            array[i - 1] = match.Groups[i].Value;

                        QueueEvent(new EventTracker(pair.Key, array));
                        break;
                    }
                }
            }
        }
        #endregion

        #region Helper Functions
        private static uint ColorToUInt(Color color) {
            return (uint)((color.R << 16) + (color.G << 8) + color.B);
        }
        #endregion
    }
}
