using System;
using System.Diagnostics;
using System.Text;

namespace ToNSaveManager {
    internal class LoggerSource {
        private string Prefix;

        internal LoggerSource(string prefix) {
            Prefix = $"[{prefix}] ";
        }

        internal void Log(object? message, LogType logType = LogType.Log) => Logger.Log(Prefix + message, logType);
        internal void Print(object? message) => Log(message, LogType.Log);
        [Conditional("DEBUG")] internal void Debug(string? message) => Logger.Debug(Prefix + message);
        [Conditional("DEBUG")] internal void Debug(object? message) => Logger.Debug(Prefix + message);
        internal void Warning(string? message) => Logger.Warning(Prefix + message);
        internal void Warning(object? message) => Logger.Warning(Prefix + message);
        internal void Info(string? message) => Logger.Info(Prefix + message);
        internal void Info(object? message) => Logger.Info(Prefix + message);
        internal void Error(string? message) => Logger.Error(Prefix + message);
        internal void Error(object? message) => Logger.Error(Prefix + message);
    }

    internal enum LogType {
        Log,
        Debug,
        Warning,
        Info,
        Error
    }

    internal static class Logger {
        private static readonly StringBuilder SharedStringBuilder = new StringBuilder();

        private static readonly StreamWriter LogFileWriter;
        static Logger () {
            LogFileWriter = new StreamWriter("output.log", append: false);
        }

        internal static void Log(string? message, LogType logType = LogType.Log) {
            lock (SharedStringBuilder) {
                SharedStringBuilder.Clear();
                var now = DateTime.Now;
                SharedStringBuilder.Append(now.ToString("yyyy.MM.dd HH:mm:ss "));
                SharedStringBuilder.Append(logType.ToString().ToUpperInvariant().PadRight(10));
                SharedStringBuilder.Append(" -  ");
                SharedStringBuilder.Append(message ?? string.Empty);

                // Console.WriteLine(SharedStringBuilder.ToString());
                System.Diagnostics.Debug.WriteLine(SharedStringBuilder.ToString());
                LogFileWriter.WriteLine(SharedStringBuilder.ToString());
                LogFileWriter.Flush();
            }
        }
        internal static void Log(object? message, LogType logType = LogType.Log) => Log(message + string.Empty, logType);
        internal static void LogFormat(string message, LogType logType = LogType.Log, params string[] args)
            => Log(string.Format(message, args), logType);

        [Conditional("DEBUG")] // Debug builds only
        internal static void Debug(string? message) => Log(message, LogType.Debug);
        [Conditional("DEBUG")] // Debug builds only
        internal static void Debug(object? message) => Log(message, LogType.Debug);

        internal static void Warning(string? message) => Log(message, LogType.Warning);
        internal static void Warning(object? message) => Log(message, LogType.Warning);
        internal static void WarningFormat(string message, params string[] args) => LogFormat(message, LogType.Warning, args);

        internal static void Info(string? message) => Log(message, LogType.Info);
        internal static void Info(object? message) => Log(message, LogType.Info);

        internal static void Error(string? message) => Log(message, LogType.Error);
        internal static void Error(object? message) => Log(message, LogType.Error);
        internal static void ErrorFormat(string message, params string[] args) => LogFormat(message, LogType.Error, args);
    }
}
