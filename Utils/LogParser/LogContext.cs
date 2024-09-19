using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Utils.LogParser {
    public class LogContext {
        public bool IsRecent;

        public long Length;
        public long Position;
        public long ReadPos;
        public bool Initialized { get; private set; }
        public virtual void OnInit() {
            Initialized = true;
        }

        public string FileName { get; private set; } = string.Empty;
        public string DateKey { get; private set;  } = string.Empty;
        public string? DisplayName;
        public string? UserID;
        public bool Authenticated;

        // Recent Instance info
        public bool IsLeavingRoom { get; private set; }
        public long RoomReadPos { get; set; }
        public string? RoomName { get; private set; }

        public string? InstanceID { get; private set; }
        public bool IsHomeWorld { get; private set; }

        public DateTime RoomDate { get; private set; }
        public readonly HashSet<string> Players = new HashSet<string>();

        public readonly StringBuilder InstanceExceptions = new StringBuilder(); // For debugging
        public readonly StringBuilder InstanceLogs = new StringBuilder();

        /*
        // Hold temporal data in this log instance
        private Dictionary<string, object> Data = new Dictionary<string, object>();
        public bool TryGet<T>(string key, out T? result) {
            if (HasKey(key)) {
                result = (T?)Data[key];
                return true;
            }

            result = default;
            return false;
        }
        public bool HasKey(string key) => Data.ContainsKey(key);
        public T? Get<T>(string key) {
            return HasKey(key) ? (T)Data[key] : default;
        }
        public void Set<T>(string key, T? value) {
            if (value == null) {
                Rem(key);
                return;
            }

            Data[key] = value;
        }
        public void Rem(string key) {
            if (HasKey(key)) Data.Remove(key);
        }
        */

        public virtual void OnAwake() { }

        public static T CreateContext<T>(string fileName, bool isRecent) where T : LogContext {
            T context = Activator.CreateInstance<T>();
            context.FileName = fileName;
            context.DateKey = fileName.Substring(11, 19);
            context.IsRecent = isRecent;
            context.OnAwake();
            return context;
        }

        /// <summary>
        /// Called when a player joins the room.
        /// </summary>
        public virtual void Join(string displayName) {
            if (!Players.Contains(displayName))
                Players.Add(displayName);
        }

        /// <summary>
        /// Called when a player leaves the room.
        /// </summary>
        public virtual void Leave(string displayName) {
            if (Players.Contains(displayName))
                Players.Remove(displayName);
        }

        /// <summary>
        /// Called when user joins a new room.
        /// </summary>
        public virtual void Enter(string name, DateTime date) {
            RoomName = name;
            RoomDate = date;
            Players.Clear();
            InstanceExceptions.Clear();
            InstanceLogs.Clear();

            Logger.Info("Entering Room Name: " + name);
            IsLeavingRoom = false;
        }
        /// <summary>
        /// Called when user is leaving room.
        /// </summary>
        public virtual void Exit() {
            Logger.Info("Leaving instance...");
            IsLeavingRoom = true;
        }
        public virtual void Enter(string instanceID, bool isHomeWorld) {
            InstanceID = instanceID;
            IsHomeWorld = isHomeWorld;

            Logger.Info($"Instace [{isHomeWorld}] : {instanceID}");
            IsLeavingRoom = false;
        }

        public void AddException(string exceptionLog) {
            InstanceExceptions.AppendLine(exceptionLog);
            InstanceExceptions.AppendLine();
        }
        public void AddLog(string line) {
            InstanceLogs.AppendLine(line);
        }

        /// <summary>
        /// Get's a list of players in this room as a string.
        /// </summary>
        public string GetRoomString(bool lineBreak = true) {
            StringBuilder sb = new StringBuilder();
            const string start = "- ";
            sb.Append(start);
            sb.AppendJoin(lineBreak ? Environment.NewLine + start : ", ", Players);
            return sb.ToString();
        }

        public string GetRoomLogs() {
            return InstanceLogs.ToString();
        }
        public string GetRoomExceptions() {
            return InstanceExceptions.ToString();
        }
    }
}
