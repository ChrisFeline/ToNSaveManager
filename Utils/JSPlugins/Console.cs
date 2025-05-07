using Jint.Native;

namespace ToNSaveManager.Utils.JSPlugins {
    internal class Console {
        static LoggerSource Logger => JSEngine.Logger;

        private string Prefix;

        internal Console(string prefix) {
            Prefix = $"[{prefix}] ";
        }

        internal void Print(params JsValue[] message) {
            Logger.Log(Prefix + string.Join(' ', message.Select(a => a.ToString())));
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
