namespace ToNSaveManager
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
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

        public LogWatcher()
        {
            var logPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat\VRChat";
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
            var deletedNameSet = new HashSet<string>(m_LogContextMap.Keys);
            m_LogDirectoryInfo.Refresh();

            if (m_LogDirectoryInfo.Exists)
            {
                var fileInfos = m_LogDirectoryInfo.GetFiles("output_log_*.txt", SearchOption.TopDirectoryOnly);
                Array.Sort(fileInfos, (a, b) => a.CreationTimeUtc.CompareTo(b.CreationTimeUtc));

                foreach (var fileInfo in fileInfos)
                {
                    fileInfo.Refresh();
                    if (!fileInfo.Exists)
                    {
                        continue;
                    }

                    LogContext? logContext;
                    if (m_LogContextMap.TryGetValue(fileInfo.Name, out logContext))
                    {
                        deletedNameSet.Remove(fileInfo.Name);
                    }
                    else
                    {
                        logContext = new LogContext(fileInfo.Name);
                        m_LogContextMap.Add(fileInfo.Name, logContext);
                    }

                    if (logContext.Length == fileInfo.Length)
                    {
                        continue;
                    }

                    logContext.Length = fileInfo.Length;
                    ParseLog(fileInfo, logContext);
                }
            }

            foreach (var name in deletedNameSet)
            {
                m_LogContextMap.Remove(name);
            }

            if (OnTick != null)
                OnTick.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Parses the log file starting from the current position and updates the log context.
        /// </summary>
        /// <param name="fileInfo">The file information of the log file to parse.</param>
        /// <param name="logContext">The log context to update.</param>
        private void ParseLog(FileInfo fileInfo, LogContext logContext)
        {
            try
            {
                using (var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 65536, false))
                {
                    stream.Position = logContext.Position;
                    // stream.Seek(logContext.Position, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                    {
                        while (true)
                        {
                            var line = streamReader.ReadLine();
                            if (line == null)
                            {
                                logContext.Position = stream.Position;
                                break;
                            }

                            if (line.Length == 0 || line.Length <= 36 || line[31] != '-')
                            {
                                continue;
                            }

                            DateTime lineDate;
                            if (!DateTime.TryParseExact(line.Substring(0, 19), "yyyy.MM.dd HH:mm:ss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out lineDate))
                            {
                                lineDate = DateTime.Now;   
                            }

                            if (ParseLocation(line, lineDate, logContext) ||
                                ParseDisplayName(line, lineDate, logContext) ||
                                ParsePlayerJoin(line, lineDate, logContext)) { }

                            if (OnLine != null)
                                OnLine.Invoke(this, new OnLineArgs(line, lineDate, logContext));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        const string UserAuthKeyword = "User Authenticated: ";
        private bool ParseDisplayName(string line, DateTime lineDate, LogContext logContext)
        {
            if (!line.Contains(UserAuthKeyword)) return false;

            int index = line.IndexOf(UserAuthKeyword) + UserAuthKeyword.Length;
            int length = line.IndexOf(" (usr_", index) - index;

            string displayName = line.Substring(index, length);
            logContext.DisplayName = displayName;

            return true;
        }

        const string LocationKeyword = "[Behaviour] Entering Room: ";
        private bool ParseLocation(string line, DateTime lineDate, LogContext logContext)
        {
            if (!line.Contains(LocationKeyword)) return false;

            var index = line.IndexOf(LocationKeyword) + LocationKeyword.Length;
            if (index >= line.Length) return false;

            var worldName = line.Substring(index);
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
                displayName = line.Substring(index + 1);

                logContext.Join(displayName);
                return true;
            }

            if (line.Contains(UserLeaveKeyword))
            {
                index = line.IndexOf(UserLeaveKeyword) + UserLeaveKeyword.Length;
                displayName = line.Substring(index);

                logContext.Leave(displayName);
                return true;
            }

            return false;
        }

        public class LogContext
        {
            public long Length;
            public long Position;

            public readonly string FileName;
            public string? DisplayName;

            // Recent Instance info
            public string? RoomName { get; private set; }
            public DateTime RoomDate { get; private set; }
            public readonly HashSet<string> Players;

            public LogContext(string fileName)
            {
                FileName = fileName;
                Players = new HashSet<string>();
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
        }
    }
}
