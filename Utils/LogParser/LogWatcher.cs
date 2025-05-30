﻿namespace ToNSaveManager.Utils.LogParser
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

        private Timer m_Timer = new Timer();
        public int Interval {
            get => m_Timer.Interval;
            set => m_Timer.Interval = Math.Max(value, 10);
        }

        public void Start()
        {
            m_Timer.Enabled = true;

            LogTick(null, null);
            m_Timer.Tick += LogTick;
            m_Timer.Start();
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
                        logContext = LogContext.CreateContext<T>(fileInfo.Name, fileInfo.FullName, !isOlder);
                        m_LogContextMap.Add(fileInfo.Name, logContext);

                        if (firstRun)
                        {
                            logContext.Position = GetParsedPos(logContext.DateKey);
                            logContext.RoomReadPos = logContext.Position;

                            if (!logContext.Authenticated) logContext.Position = 0;
                        }
                    }

                    // authentication bottleneck :(
                    if (logContext.Length == fileInfo.Length)
                    {
                        continue;
                    }

                    if (firstRun && logContext.Authenticated && !logContext.Validate(logContext.Position, fileInfo)) {
                        logContext.RoomReadPos = logContext.Position = fileInfo.Length;
                    } else {
                        ParseLog(fileInfo, logContext, sender == null);
                    }

                    if (!logContext.Initialized)
                    {
                        logContext.OnInit();

                        if (logContext.Authenticated) {
                            logContext.Position = logContext.RoomReadPos;
                            i = i - 1;
                            continue;
                        }
                    }

                    if (SkipParsedLogs && logContext.Authenticated)
                        SetParsedPos(logContext.DateKey, isOlder ? logContext.Position : logContext.RoomReadPos, firstRun);

                    logContext.Length = fileInfo.Length;
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

                                    if (logContext.Authenticated && !logContext.Initialized) break;
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
                ParsePickupGrab(line, lineDate, logContext)) { }

            if (OnLine != null)
                OnLine.Invoke(this, new OnLineArgs(line, lineDate, logContext));
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

                index = line.IndexOf("(");
                length = line.LastIndexOf(')') - index;
                string userID = line.Substring(index, length).Trim('(', ')', ' ', '\n', '\r');
                logContext.UserID = userID;
            }
            else
            {
                logContext.DisplayName = "Unknown";
            }

            logContext.Authenticated = true;
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
                logContext.Enter(instanceId, instanceId.Contains(HomeWorldID));

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
            if (line.Contains(UserJoinKeyword))
            {
                logContext.Join(ParsePlayerData(line, UserJoinKeyword));
                return true;
            }

            if (line.Contains(UserLeaveKeyword))
            {
                logContext.Leave(ParsePlayerData(line, UserLeaveKeyword));
                return true;
            }

            return false;
        }
        private static LogPlayer ParsePlayerData(string line, string keyword) {
            int start = line.IndexOf(keyword, StringComparison.InvariantCulture) + keyword.Length + 1;

            string guid = string.Empty;

            int id_start = line.LastIndexOf('(');
            int id_end = line.LastIndexOf(')');
            int length = line.Length - start;
            if (id_start > 0 && id_end > 0 && id_end > id_start) {
                guid = line.Substring(id_start + 1, (id_end - id_start) - 1).Trim();
                length = id_start - start;
            }

            string name = line.Substring(start, length).Trim();
            return new LogPlayer() { GUID = guid, Name = name };
        }

        const string PickupGrabKeyword = "[Behaviour] Pickup object: '";
        const string PickupGrabKeywordEnd = "' equipped = ";

        const string PickupDropKeyword = "[Behaviour] Drop object: '";
        const string PickupDropKeywordEnd = ", was equipped = ";
        private bool ParsePickupGrab(string line, DateTime lineDate, T logContext) {
            int index, length;
            string objectName;

            index = line.IndexOf(PickupGrabKeyword, StringComparison.InvariantCulture);
            if (index > 0) {
                index = index + PickupGrabKeyword.Length;
                length = line.IndexOf(PickupGrabKeywordEnd, index) - index;
                if (length < 0) return false;

                objectName = line.Substring(index, length);
                logContext.Pickup(objectName);
                return true;
            }

            index = line.IndexOf(PickupDropKeyword, StringComparison.InvariantCulture);
            if (index > 0) {
                index = index + PickupDropKeyword.Length;
                length = line.IndexOf(PickupDropKeywordEnd, index) - index;
                if (length < 0) return false;

                objectName = line.Substring(index, length);
                logContext.Drop(objectName);
                return true;
            }

            return false;
        }
    }
}
