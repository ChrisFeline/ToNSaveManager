namespace ToNSaveManager
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
                        Debug.WriteLine("Added Context: " + fileInfo.Name);
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

                            if (DateTime.TryParseExact(
                                    line.Substring(0, 19),
                                    "yyyy.MM.dd HH:mm:ss",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out var lineDate
                                ))
                            {
                                lineDate = lineDate.ToUniversalTime();
                            }
                            else
                            {
                                lineDate = DateTime.Now.ToUniversalTime();
                            }

                            if (ParseLocation(line, logContext) ||
                                ParseDisplayName(line, logContext)) { }

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
        private bool ParseDisplayName(string line, LogContext logContext)
        {
            if (!line.Contains(UserAuthKeyword)) return false;

            int index = line.IndexOf(UserAuthKeyword) + UserAuthKeyword.Length;
            int length = line.IndexOf(" (usr_", index) - index;

            string displayName = line.Substring(index, length);
            logContext.DisplayName = displayName;

            return true;
        }

        const string LocationKeyword = "[Behaviour] Entering Room: ";
        private bool ParseLocation(string line, LogContext logContext)
        {
            if (!line.Contains(LocationKeyword)) return false;

            var index = line.IndexOf(LocationKeyword) + LocationKeyword.Length;
            if (index >= line.Length) return false;

            var worldName = line.Substring(index);
            logContext.RecentWorld = worldName;

            return true;
        }

        public class LogContext
        {
            public long Length;
            public long Position;

            public string FileName;
            public string? DisplayName;
            public string? RecentWorld;

            public LogContext(string fileName)
            {
                FileName = fileName;
            }
        }
    }
}
