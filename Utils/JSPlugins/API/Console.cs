using Jint;

namespace ToNSaveManager.Utils.JSPlugins.API {
    [JSEngineAPI("console")]
    internal class Console {
        static LoggerSource Logger => JSEngine.Logger;

        private static string Prefix => $"[{JSEngine.GetLastSyntaxSource()}] ";

        internal static void Register(Engine engine) {
            engine.SetValue("print", Print);
        }

        internal static void Print(params object[] message) {
            Logger.Log(Prefix + string.Join(' ', message));
        }
        public static void Log(params object[] message) {
            Logger.Log(Prefix + string.Join(' ', message));
        }
        public static void Error(params object[] message) {
            Logger.Error(Prefix + string.Join(' ', message));
        }
        public static void Warn(params object[] message) {
            Logger.Warning(Prefix + string.Join(' ', message));
        }
    }
}
