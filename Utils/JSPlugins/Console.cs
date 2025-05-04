namespace ToNSaveManager.Utils.JSPlugins {
    internal class Console {
        static LoggerSource Logger => JSEngine.Logger;

        private string Prefix;

        internal Console(string prefix) {
            Prefix = $"[{prefix}] ";
        }

        public void Log(object message) {
            Logger.Log(Prefix + message);
        }
        public void Error(object message) {
            Logger.Error(Prefix + message);
        }
        public void Warn(object message) {
            Logger.Warning(Prefix + message);
        }
    }
}
