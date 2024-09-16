using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Index;
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
            foreach (IEvent ev in EventBuffer.Values) {
                Send(JsonConvert.SerializeObject(ev));
            }
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

        internal static void Broadcast(string data) => Server?.WebSocketServices?.Broadcast(data);
        internal static void SendObject(object data) => Broadcast(JsonConvert.SerializeObject(data));
        internal static void SendEvent<T>(T value) where T : IEvent
        {
            if (Settings.Get.WebSocketEnabled) SendObject(value);
        }

        public interface IEvent {
            string Type { get; }
            int Id { get; }
            byte Command { get; set; }
        }

        #region Event Structs
        public struct EventValue<T> : IEvent {
            public string Type { get; private set; }
            public int Id { get; private set; }
            public byte Command { get; set; }
            public T Value { get; private set; }

            public EventValue(string type, int id, T value) {
                Type = type;
                Id = id;
                Value = value;
                Command = 0;
            }
        }

        public struct EventTerror : IEvent {
            public string Type => "TERRORS";
            public int Id => 0;

            /// <summary>
            /// 0 Set -
            /// 1 Reveal -
            /// 2 Hidden
            /// </summary>
            public byte Command { get; set; }

            public string[] Names { get; set; }
            public string DisplayName { get; set; }
            public uint DisplayColor { get; set; }
        }

        public struct EventRoundType : IEvent {
            public string Type => "ROUND_TYPE";
            public int Id => 1;

            /// <summary>
            /// 0 - Round Over
            /// 1 - Round Start
            /// </summary>
            public byte Command { get; set; }

            public ToNRoundType Value { get; set; }
            public string Name => Value.ToString();
            public string DisplayName => MainWindow.GetRoundTypeName(Value);
            public uint DisplayColor { get; set; } // Maybe a bit innacurate
        }

        public struct EventLocation : IEvent {
            public string Type => "LOCATION";
            public int Id => 2;
            public byte Command { get; set; }

            public string Name { get; set; }
            public string Creator { get; set; }
            public string Origin { get; set; }
        }
        #endregion

        #region Save Manager Events
        private static readonly Dictionary<int, IEvent> EventBuffer = new ();

        internal static Queue<IEvent> EventQueue = new Queue<IEvent>();
        private static void QueueEvent(IEvent ev) {
            EventQueue.Enqueue(ev);
            EventBuffer[ev.Id] = ev;
        }

        internal static void SendEventUpdate() {
            while (EventQueue.Count > 0) {
                SendEvent(EventQueue.Dequeue());
            }
        }

        internal static void SendTerrorMatrix(TerrorMatrix matrix) {
            EventTerror eventTerror = new EventTerror();
            eventTerror.DisplayName = matrix.GetTerrorNames();
            eventTerror.DisplayColor = ColorToUInt(matrix.DisplayColor);
            if (matrix.Terrors.Length > 0) eventTerror.Names = matrix.Terrors.Select(t => t.Name).ToArray();

            byte command;
            if (matrix.IsUnknown) command = 2;
            if (matrix.IsRevealed) command = 1;
            else command = 0;

            eventTerror.Command = command;
            QueueEvent(eventTerror);
        }

        internal static void SendRoundType(ToNRoundType roundType) {
            EventRoundType eventRoundType = new EventRoundType();
            eventRoundType.Value = roundType;
            eventRoundType.DisplayColor = TerrorMatrix.GetRoundColorFromType(roundType);
            eventRoundType.Command = (byte)(roundType == ToNRoundType.Intermission ? 0 : 1);
            QueueEvent(eventRoundType);
        }

        internal static void SendLocation(ToNIndex.Map map) {
            EventLocation eventLocation = new EventLocation();
            eventLocation.Name = map.Name;
            eventLocation.Creator = map.Creator;
            eventLocation.Origin = map.Origin;
            eventLocation.Command = (byte)(map.IsEmpty ? 0 : 1);
            QueueEvent(eventLocation);
        }

        internal static void SendValue<T>(string type, int id, T value) {
            QueueEvent(new EventValue<T>(type, id, value));
        }
        #endregion

        #region Helper Functions
        private static uint ColorToUInt(Color color) {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }
        #endregion
    }
}
