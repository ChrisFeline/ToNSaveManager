using Jint.Native;

namespace ToNSaveManager.Utils.JSPlugins {
    internal class Console {
        internal static readonly Console Instance = new ();

        static LoggerSource Logger => JSEngine.Logger;

        private static string Prefix => $"[{JSEngine.GetLastSyntaxSource()}] ";

        internal void Print(params object[] message) {
            Logger.Log(Prefix + string.Join(' ', message));
        }
        public void Log(params object[] message) {
            Logger.Log(Prefix + string.Join(' ', message));
        }
        public void Error(params object[] message) {
            Logger.Error(Prefix + string.Join(' ', message));
        }
        public void Warn(params object[] message) {
            Logger.Warning(Prefix + string.Join(' ', message));
        }
    }
}
