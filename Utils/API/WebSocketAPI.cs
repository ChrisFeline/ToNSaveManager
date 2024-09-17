﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Utils.LogParser;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ToNSaveManager.Utils.API {
    internal class WebSocketAPI : WebSocketBehavior {
        static readonly LoggerSource Logger = new LoggerSource("WS-API");
        internal static WebSocketServer? Server;

        protected override void OnMessage(MessageEventArgs e) {
            Logger.Log("Received: " + e.Data);
        }

        protected override void OnOpen() {
            Logger.Log("WebSocket client connected.");

            SendEvent(new EventConnected() {
                Args = EventBuffer.ToArray(),
                DisplayName = ToNLogContext.Instance?.DisplayName ?? string.Empty,
                UserID = ToNLogContext.Instance?.UserID ?? string.Empty
            });
            
            SendValue("INSTANCE", ToNLogContext.Instance?.InstanceID);
        }

        internal static void Initialize() {
            if (Settings.Get.WebSocketEnabled && Server == null) {
                const string url = "ws://localhost:11398";
                Server = new WebSocketServer(url);
                Server.AddWebSocketService<WebSocketAPI>("/");
            }

            if (Settings.Get.WebSocketEnabled && Server != null && !Server.IsListening) {
                Logger.Debug("Starting Server...");
                Server.Start();
            } else if (!Settings.Get.WebSocketEnabled && Server != null && Server.IsListening) {
                Logger.Debug("Stopping Server...");
                Server.Stop();
                Server.RemoveWebSocketService("/");
                Server = null;
            }
        }

        internal static void Broadcast(string data) {
            Logger.Debug("Broadcasting: " + data);
            Server?.WebSocketServices?.Broadcast(data);
        }
        internal static void SendObject(object data) => Broadcast(JsonConvert.SerializeObject(data));
        internal static void SendEvent<T>(T value) where T : IEvent
        {
            if (Settings.Get.WebSocketEnabled) SendObject(value);
        }

        public interface IEvent {
            string Type { get; }
            byte Command { get; set; }
        }

        #region Event Structs
        public struct EventConnected : IEvent {
            public string Type => "CONNECTED";
            public byte Command { get; set; }

            public IEvent[] Args { get; set; }

            public string DisplayName { get; set; }
            public string UserID { get; set; }
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
            public byte Command { get; set; }

            public string Name { get; set; }
            public object? Value { get; set; }

            internal static void Send(string name, object? value) {
                QueueEvent(new EventStats() { Name = name, Value = value });
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

                    for (int i = matrix.StartIndex; i < matrix.Length; i++) {
                        eventTerror.Names[i] = matrix[i].Name;
                        eventTerror.Assets[i] = matrix[i].AssetID;
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
            public string Creator { get; set; }
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
        #endregion

        #region Save Manager Events
        private static List<IEvent> EventBuffer = new();
        internal static void ClearBuffer() {
            Logger.Debug("CLEARING BUFFER!!!");
            EventBuffer.Clear();
        }

        internal static Queue<IEvent> EventQueue = new Queue<IEvent>();
        private static void QueueEvent(IEvent ev) {
            if (MainWindow.Started) EventQueue.Enqueue(ev);
            EventBuffer.Add(ev);
        }

        internal static void SendEventUpdate() {
            while (EventQueue.Count > 0) {
                SendEvent(EventQueue.Dequeue());
            }
        }

        internal static void SendValue<T>(string type, T value) {
            QueueEvent(new EventValue<T>(type, value));
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
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }
        #endregion
    }
}