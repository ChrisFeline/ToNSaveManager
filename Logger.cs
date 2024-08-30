using System;
using System.Diagnostics;
using System.Text;

namespace ToNSaveManager {
    internal static class Logger {
        private static readonly StringBuilder SharedStringBuilder = new StringBuilder();

        internal enum LogType {
            Log,
            Debug,
            Warning,
            Info,
            Error
        }

        private static readonly StreamWriter LogFileWriter;
        static Logger () {
            LogFileWriter = new StreamWriter("output.log", append: false);
        }

        internal static void Log(string message, LogType logType = LogType.Log) {
            lock (SharedStringBuilder) {
                SharedStringBuilder.Clear();
                var now = DateTime.Now;
                SharedStringBuilder.Append(now.ToString("yyyy.MM.dd HH:mm:ss "));
                SharedStringBuilder.Append(logType.ToString().ToUpperInvariant().PadRight(10));
                SharedStringBuilder.Append(" -  ");
                SharedStringBuilder.Append(message);

                // Console.WriteLine(SharedStringBuilder.ToString());
                System.Diagnostics.Debug.WriteLine(SharedStringBuilder.ToString());
                LogFileWriter.WriteLine(SharedStringBuilder.ToString());
                LogFileWriter.Flush();
            }
        }

        [Conditional("DEBUG")] // Debug builds only
        internal static void Debug(string message) => Log(message, LogType.Debug);

        internal static void Warning(string message) => Log(message, LogType.Warning);
        internal static void Info(string message) => Log(message, LogType.Info);
        internal static void Error(string message) => Log(message, LogType.Error);
    }
}
