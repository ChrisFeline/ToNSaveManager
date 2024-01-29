namespace ToNSaveManager.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using ToNSaveManager.Models;
    using Timer = System.Windows.Forms.Timer;

    // Based on: https://github.com/vrcx-team/VRCX/blob/634f465927bfaef51bc04e67cf1659170953fac9/LogWatcher.cs
    public class LogWatcher
    {
        public class OnLineArgs : EventArgs
        {
            public DateTime Timestamp { get; private set; }
            public string Content { get; private set; }
            public LogContext Context { get; private set; }

            public OnLineArgs(string content, DateTime timestamp, LogContext context)
            {
                Content = content;
                Timestamp = timestamp;
                Context = context;
            }
        }

        public event EventHandler<OnLineArgs>? OnLine;
        public event EventHandler<EventArgs>? OnTick;

        private DirectoryInfo m_LogDirectoryInfo;
        private readonly Dictionary<string, LogContext> m_LogContextMap = new Dictionary<string, LogContext>();

        internal static string GetVRChatDataLocation() =>
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat";

        #region From Settings
        bool RecordInstanceLogs => Settings.Get.RecordInstanceLogs;
        bool SkipParsedLogs => Settings.Get.SkipParsedLogs;

        long GetParsedPos(string name) => MainWindow.SaveData.GetParsedPos(name);
        void SetParsedPos(string name, long pos, bool save) => MainWindow.SaveData.SetParsedPos(name, pos, save);
        #endregion

        public LogWatcher()
        {
            var logPath = GetVRChatDataLocation() + @"\VRChat";
            m_LogDirectoryInfo = new DirectoryInfo(logPath);
        }

        public void Start()
        {
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Enabled = true;

            LogTick(null, null);
            timer.Tick += LogTick;
            timer.Start();
        }

        private void LogTick(object? sender, EventArgs? e)
        {
            bool firstRun = sender == null && SkipParsedLogs;
            m_LogDirectoryInfo.Refresh();

            if (m_LogDirectoryInfo.Exists)
            {
                var fileInfos = m_LogDirectoryInfo.GetFiles("output_log_*.txt", SearchOption.TopDirectoryOnly);
                Array.Sort(fileInfos, (a, b) => a.CreationTimeUtc.CompareTo(b.CreationTimeUtc));

                int length = fileInfos.Length;
                int lastIndex = length - 1;
                for (int i = 0; i < length; i++)
                {
                    bool isOlder = i < lastIndex;
                    FileInfo fileInfo = fileInfos[i];

                    fileInfo.Refresh();
                    if (!fileInfo.Exists)
                    {
                        continue;
                    }

                    LogContext? logContext;
                    if (!m_LogContextMap.TryGetValue(fileInfo.Name, out logContext))
                    {
                        logContext = new LogContext(fileInfo.Name);
                        m_LogContextMap.Add(fileInfo.Name, logContext);

                        if (firstRun) logContext.Position = GetParsedPos(logContext.DateKey);
                    }

                    if (logContext.Length == fileInfo.Length)
                    {
                        continue;
                    }

                    logContext.Length = fileInfo.Length;
                    ParseLog(fileInfo, logContext, sender == null);

                    if (!logContext.Initialized)
                    {
                        logContext.SetInit();
                    }

                    if (SkipParsedLogs)
                        SetParsedPos(logContext.DateKey, logContext.Position, firstRun);
                }
            }

            // Remove logs keys that doesn't exist anymore
            if (firstRun)
            {
                string[] parsedLogKeyArray = MainWindow.SaveData.ParsedLog.Keys.ToArray();
                foreach (string key in parsedLogKeyArray)
                {
                    if (!m_LogContextMap.Values.Any(v => v.DateKey == key))
                    {
                        // Debug.WriteLine("Removing unnecessary key: " + key);
                        SetParsedPos(key, -1, true);
                    }
                }
            }

            if (OnTick != null)
                OnTick.Invoke(this, EventArgs.Empty);
        }

        public LogContext? GetEarliestContext()
        {
            LogContext? context = null;
            foreach (var pair in m_LogContextMap)
            {
                if (context == null || pair.Value.RoomDate > context.RoomDate)
                    context = pair.Value;
            }

            return context;
        }

        static readonly Regex LogPattern = new Regex(@"^\d{4}.\d{2}.\d{2} \d{2}:\d{2}:\d{2}\s", RegexOptions.Compiled);
        static readonly StringBuilder LogBuilder = new StringBuilder();

        /// <summary>
        /// Parses the log file starting from the current position and updates the log context.
        /// </summary>
        /// <param name="fileInfo">The file information of the log file to parse.</param>
        /// <param name="logContext">The log context to update.</param>
        private void ParseLog(FileInfo fileInfo, LogContext logContext, bool skip = false)
        {
            try
            {
                using (var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 65536, false))
                {
                    stream.Position = logContext.Position;

                    using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        bool isNull;
                        while (true)
                        {
                            var line = streamReader.ReadLine();
                            isNull = line == null;

#pragma warning disable CS8604
                            if (isNull || LogPattern.IsMatch(line))
                            {
                                if (LogBuilder.Length > 0)
                                {
                                    HandleLine(LogBuilder.ToString(), logContext);
                                    LogBuilder.Clear();
                                }

                                if (isNull)
                                {
                                    logContext.Position = stream.Position;
                                    break;
                                }
                            }
#pragma warning restore CS8604

                            if (LogBuilder.Length > 0) LogBuilder.AppendLine();
                            LogBuilder.Append(line);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void HandleLine(string line, LogContext logContext)
        {
            if (line.Length == 0 || line.Length <= 36 || line[31] != '-') return;

            DateTime lineDate;
            if (!DateTime.TryParseExact(line.Substring(0, 19), "yyyy.MM.dd HH:mm:ss",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out lineDate))
            {
                lineDate = DateTime.Now;
            }
            if (ParseLocation(line, lineDate, logContext) ||
                ParseDisplayName(line, lineDate, logContext) ||
                ParsePlayerJoin(line, lineDate, logContext) ||
                (RecordInstanceLogs && ParseUdonException(line, lineDate, logContext))) { }

            if (OnLine != null)
                OnLine.Invoke(this, new OnLineArgs(line, lineDate, logContext));

            if (RecordInstanceLogs) logContext.AddLog(line);
        }

        const string UserAuthKeyword = "User Authenticated: ";
        private bool ParseDisplayName(string line, DateTime lineDate, LogContext logContext)
        {
            if (!line.Contains(UserAuthKeyword)) return false;

            int index = line.IndexOf(UserAuthKeyword) + UserAuthKeyword.Length;
            int length = line.IndexOf(" (", index) - index;

            if (index < line.Length && index + length < line.Length)
            {
                string displayName = line.Substring(index, length);
                logContext.DisplayName = displayName.Trim();
            } else
            {
                logContext.DisplayName = "Unknown";
            }

            return true;
        }

        const string LocationKeyword = "[Behaviour] Entering Room: ";
        private bool ParseLocation(string line, DateTime lineDate, LogContext logContext)
        {
            if (!line.Contains(LocationKeyword)) return false;

            var index = line.IndexOf(LocationKeyword) + LocationKeyword.Length;
            if (index >= line.Length) return false;

            var worldName = line.Substring(index).Trim('\n', '\r');
            logContext.Enter(worldName, lineDate);

            return true;
        }

        const string UserJoinKeyword = "[Behaviour] OnPlayerJoined";
        const string UserLeaveKeyword = "[Behaviour] OnPlayerLeft";
        private bool ParsePlayerJoin(string line, DateTime lineDate, LogContext logContext)
        {
            int index;
            string displayName;

            if (line.Contains(UserJoinKeyword))
            {
                index = line.IndexOf(UserJoinKeyword) + UserJoinKeyword.Length;
                displayName = line.Substring(index + 1).Trim();

                logContext.Join(displayName);
                return true;
            }

            if (line.Contains(UserLeaveKeyword))
            {
                index = line.IndexOf(UserLeaveKeyword) + UserLeaveKeyword.Length;
                displayName = line.Substring(index + 1).Trim();

                logContext.Leave(displayName);
                return true;
            }

            return false;
        }

        private bool ParseUdonException(string line, DateTime lineTime, LogContext logContext)
        {
            const string errorMatchStr = "[UdonBehaviour] An exception occurred during Udon execution";

            if (!line.Contains(errorMatchStr)) return false;

            logContext.AddException(line);
            return true;
        }

        public class LogContext
        {
            public long Length;
            public long Position;
            public bool Initialized { get; private set; }
            public void SetInit() => Initialized = true;

            public readonly string FileName;
            public readonly string DateKey;
            public string? DisplayName;

            // Recent Instance info
            public string? RoomName { get; private set; }
            public DateTime RoomDate { get; private set; }
            public readonly HashSet<string> Players;

            public readonly StringBuilder InstanceExceptions; // For debugging
            public readonly StringBuilder InstanceLogs;

            // Hold temporal data in this log instance
            private Dictionary<string, object> Data = new Dictionary<string, object>();
            public bool TryGet<T>(string key, out T? result)
            {
                if (HasKey(key))
                {
                    result = (T?)Data[key];
                    return true;
                }

                result = default(T);
                return false;
            }
            public bool HasKey(string key) => Data.ContainsKey(key);
            public T? Get<T>(string key)
            {
                return HasKey(key) ? (T)Data[key] : default(T);
            }
            public void Set<T>(string key, T? value)
            {
                if (value == null)
                {
                    Rem(key);
                    return;
                }

                Data[key] = value;
            }
            public void Rem(string key)
            {
                if (HasKey(key)) Data.Remove(key);
            }

            public LogContext(string fileName)
            {
                FileName = fileName;
                DateKey = FileName.Substring(11, 19);
                Players = new HashSet<string>();
                InstanceExceptions = new StringBuilder();
                InstanceLogs = new StringBuilder();
            }

            /// <summary>
            /// Called when a player leaves the room.
            /// </summary>
            public void Join(string displayName)
            {
                if (!Players.Contains(displayName))
                    Players.Add(displayName);
            }

            /// <summary>
            /// Called when a player joins the room.
            /// </summary>
            public void Leave(string displayName)
            {
                if (Players.Contains(displayName))
                    Players.Remove(displayName);
            }

            /// <summary>
            /// Called when user joins a new room.
            /// </summary>
            public void Enter(string name, DateTime date)
            {
                RoomName = name;
                RoomDate = date;
                Players.Clear();
                InstanceExceptions.Clear();
                InstanceLogs.Clear();
            }

            public void AddException(string exceptionLog)
            {
                InstanceExceptions.AppendLine(exceptionLog);
                InstanceExceptions.AppendLine();
            }
            public void AddLog(string line)
            {
                InstanceLogs.AppendLine(line);
            }

            /// <summary>
            /// Get's a list of players in this room as a string.
            /// </summary>
            public string GetRoomString(bool lineBreak = true)
            {
                StringBuilder sb = new StringBuilder();
                const string start = "- ";
                sb.Append(start);
                sb.AppendJoin(lineBreak ? Environment.NewLine + start : ", ", Players);
                return sb.ToString();
            }

            public string GetRoomLogs()
            {
                return InstanceLogs.ToString();
            }
            public string GetRoomExceptions()
            {
                return InstanceExceptions.ToString();
            }
        }
    }
}
