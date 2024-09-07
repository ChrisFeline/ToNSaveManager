namespace ToNSaveManager.Utils.LogParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using ToNSaveManager.Models;
    using Timer = System.Windows.Forms.Timer;

    // Based on: https://github.com/vrcx-team/VRCX/blob/634f465927bfaef51bc04e67cf1659170953fac9/LogWatcher.cs
    public class LogWatcher<T> where T : LogContext
    {
        public class OnLineArgs : EventArgs
        {
            public DateTime Timestamp { get; private set; }
            public string Content { get; private set; }
            public T Context { get; private set; }

            public OnLineArgs(string content, DateTime timestamp, T context)
            {
                Content = content;
                Timestamp = timestamp;
                Context = context;
            }
        }

        public event EventHandler<OnLineArgs>? OnLine;
        public event EventHandler<EventArgs>? OnTick;

        private DirectoryInfo m_LogDirectoryInfo;
        private readonly Dictionary<string, T> m_LogContextMap = new Dictionary<string, T>();

        internal static string GetVRChatDataLocation() =>
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat";

        #region From Settings
        bool RecordInstanceLogs => Settings.Get.RecordInstanceLogs;
        bool SkipParsedLogs => Settings.Get.SkipParsedLogs;

        long GetParsedPos(string name) => MainWindow.SaveData.GetParsedPos(name);
        void SetParsedPos(string name, long pos, bool save) => MainWindow.SaveData.SetParsedPos(name, pos, save);

        string HomeWorldID = string.Empty;
        #endregion

        public LogWatcher(string homeWorldID)
        {
            var logPath = GetVRChatDataLocation() + @"\VRChat";
            m_LogDirectoryInfo = new DirectoryInfo(logPath);
            HomeWorldID = homeWorldID;
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

                    T? logContext;
                    if (!m_LogContextMap.TryGetValue(fileInfo.Name, out logContext))
                    {
                        logContext = LogContext.CreateContext<T>(fileInfo.Name);
                        m_LogContextMap.Add(fileInfo.Name, logContext);

                        if (firstRun)
                        {
                            logContext.Position = GetParsedPos(logContext.DateKey);
                            logContext.RoomReadPos = logContext.Position;
                        }
                    }

                    if (logContext.Length == fileInfo.Length)
                    {
                        continue;
                    }

                    logContext.IsRecent = !isOlder;
                    logContext.Length = fileInfo.Length;
                    ParseLog(fileInfo, logContext, sender == null);

                    if (!logContext.Initialized)
                    {
                        logContext.SetInit();
                    }

                    if (SkipParsedLogs)
                        SetParsedPos(logContext.DateKey, isOlder ? logContext.Position : logContext.RoomReadPos, firstRun);
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
                        // Logger.Debug("Removing unnecessary key: " + key);
                        SetParsedPos(key, -1, true);
                    }
                }
            }

            if (OnTick != null)
                OnTick.Invoke(this, EventArgs.Empty);
        }

        public T? GetEarliestContext()
        {
            T? context = null;
            foreach (var pair in m_LogContextMap)
            {
                if (context == null || pair.Value.RoomDate > context.RoomDate)
                    context = pair.Value;
            }

            return context;
        }

        static readonly Regex LogPattern = new Regex(@"^\d{4}.\d{2}.\d{2} \d{2}:\d{2}:\d{2}\s", RegexOptions.Compiled);
        static readonly StringBuilder LogBuilder = new StringBuilder();

#pragma warning disable CS8601, CS8605, CS8600, CS8604 // Possible null reference assignment.
        // https://stackoverflow.com/a/22975649
        readonly static FieldInfo charPosField = typeof(StreamReader).GetField("_charPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        readonly static FieldInfo byteLenField = typeof(StreamReader).GetField("_byteLen", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        readonly static FieldInfo charBufferField = typeof(StreamReader).GetField("_charBuffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        static long GetReaderPosition(StreamReader reader)
        {
            // shift position back from BaseStream.Position by the number of bytes read
            // into internal buffer.
            int byteLen = (int)byteLenField.GetValue(reader);
            var position = reader.BaseStream.Position - byteLen;

            // if we have consumed chars from the buffer we need to calculate how many
            // bytes they represent in the current encoding and add that to the position.
            int charPos = (int)charPosField.GetValue(reader);
            if (charPos > 0)
            {
                char[] charBuffer = (char[])charBufferField.GetValue(reader);
                var encoding = reader.CurrentEncoding;
                var bytesConsumed = encoding.GetBytes(charBuffer, 0, charPos).Length;
                position += bytesConsumed;
            }

            return position;
        }
#pragma warning restore CS8601, CS8605, CS8600, CS8604 // Possible null reference assignment.

        /// <summary>
        /// Parses the log file starting from the current position and updates the log context.
        /// </summary>
        /// <param name="fileInfo">The file information of the log file to parse.</param>
        /// <param name="logContext">The log context to update.</param>
        private void ParseLog(FileInfo fileInfo, T logContext, bool skip = false)
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

                                    logContext.ReadPos = GetReaderPosition(streamReader);
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

        private void HandleLine(string line, T logContext)
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
                RecordInstanceLogs && ParseUdonException(line, lineDate, logContext)) { }

            if (OnLine != null)
                OnLine.Invoke(this, new OnLineArgs(line, lineDate, logContext));

            if (RecordInstanceLogs) logContext.AddLog(line);
        }

        const string UserAuthKeyword = "User Authenticated: ";
        private bool ParseDisplayName(string line, DateTime lineDate, T logContext)
        {
            if (!line.Contains(UserAuthKeyword)) return false;

            int index = line.IndexOf(UserAuthKeyword, StringComparison.InvariantCulture) + UserAuthKeyword.Length;
            int length = line.IndexOf(" (", index, StringComparison.InvariantCulture) - index;

            if (index > -1 && length > 0 && index < line.Length && index + length < line.Length)
            {
                string displayName = line.Substring(index, length);
                logContext.DisplayName = displayName.Trim();
            }
            else
            {
                logContext.DisplayName = "Unknown";
            }

            return true;
        }

        internal const string LocationKeyword = "[Behaviour] Entering Room: ";
        internal const string InstanceKeyword = "[Behaviour] Joining wrld_";
        internal const string LeavingKeyword = "[Behaviour] OnLeftRoom";
        internal const int InstanceKeywordLength = 20;
        private bool ParseLocation(string line, DateTime lineDate, T logContext)
        {
            if (line.Contains(InstanceKeyword))
            {
                var index = line.IndexOf(InstanceKeyword, StringComparison.InvariantCulture) + InstanceKeywordLength;
                if (index >= line.Length) return false;

                var instanceId = line.Substring(index).Trim('\n', '\r');
                logContext.Enter(instanceId, instanceId.StartsWith(HomeWorldID));

                return true;
            }

            if (line.Contains(LocationKeyword))
            {
                var index = line.IndexOf(LocationKeyword, StringComparison.InvariantCulture) + LocationKeyword.Length;
                if (index >= line.Length) return false;

                var worldName = line.Substring(index).Trim('\n', '\r');
                logContext.Enter(worldName, lineDate);

                logContext.RoomReadPos = logContext.ReadPos;

                return true;
            }

            if (line.Contains(LeavingKeyword)) {
                logContext.Exit();
                return true;
            }

            return false;
        }

        const string UserJoinKeyword = "[Behaviour] OnPlayerJoined";
        const string UserLeaveKeyword = "[Behaviour] OnPlayerLeft";
        private bool ParsePlayerJoin(string line, DateTime lineDate, T logContext)
        {
            int index;
            string displayName;

            if (line.Contains(UserJoinKeyword))
            {
                index = line.IndexOf(UserJoinKeyword, StringComparison.InvariantCulture) + UserJoinKeyword.Length;
                displayName = line.Substring(index + 1).Trim();

                logContext.Join(displayName);
                return true;
            }

            if (line.Contains(UserLeaveKeyword))
            {
                index = line.IndexOf(UserLeaveKeyword, StringComparison.InvariantCulture) + UserLeaveKeyword.Length;
                displayName = line.Substring(index + 1).Trim();

                logContext.Leave(displayName);
                return true;
            }

            return false;
        }

        private bool ParseUdonException(string line, DateTime lineTime, T logContext)
        {
            const string errorMatchStr = "[UdonBehaviour] An exception occurred during Udon execution";

            if (!line.Contains(errorMatchStr)) return false;

            logContext.AddException(line);
            return true;
        }
    }
}
