using Jint;
using Jint.Native;
using Jint.Native.Function;

namespace ToNSaveManager.Utils.JSPlugins.API {
    // Re-Wrapping anything here so I can handle type errors and such...
    [JSEngineAPI("OSC")]
    internal static class OSC {
        static LoggerSource Logger => JSEngine.Logger;

        public static void Send(string path, object value) => OSCLib.Send(path, value);
        public static void SendFloat(string path, float value) => OSCLib.Send(path, value);
        public static void SendInt(string path, int value) => OSCLib.Send(path, value);
        public static void SendBool(string path, bool value) => OSCLib.Send(path, value);
        public static void SendParameter(string name, float value) => OSCLib.SendParameter(name, value);
        public static void SendParameter(string name, bool value) => OSCLib.SendParameter(name, value);
        public static void SendChatbox(string message, bool direct = true, bool complete = false) => OSCLib.SendChatbox(message, direct, complete);
        public static void SetChatboxTyping(bool value) => OSCLib.SetChatboxTyping(value);
        public static void MoveVertical(float value) => OSCLib.MoveVertical(value);
        public static void MoveHorizontal(float value) => OSCLib.MoveHorizontal(value);
        public static void LookHorizontal(float value) => OSCLib.LookHorizontal(value);
        public static void UseAxisRight(float value) => OSCLib.UseAxisRight(value);
        public static void GrabAxisRight(float value) => OSCLib.GrabAxisRight(value);
        public static void MoveHoldFB(float value) => OSCLib.MoveHoldFB(value);
        public static void SpinHoldCW(float value) => OSCLib.SpinHoldCW(value);
        public static void SpinHoldUD(float value) => OSCLib.SpinHoldUD(value);
        public static void SpinHoldLR(float value) => OSCLib.SpinHoldLR(value);
        public static void MoveForward(bool value) => OSCLib.MoveForward(value);
        public static void MoveBackward(bool value) => OSCLib.MoveBackward(value);
        public static void MoveLeft(bool value) => OSCLib.MoveLeft(value);
        public static void MoveRight(bool value) => OSCLib.MoveRight(value);
        public static void LookLeft(bool value) => OSCLib.LookLeft(value);
        public static void LookRight(bool value) => OSCLib.LookRight(value);
        public static void Jump(bool value) => OSCLib.Jump(value);
        public static void Run(bool value) => OSCLib.Run(value);
        public static void ComfortLeft(bool value) => OSCLib.ComfortLeft(value);
        public static void ComfortRight(bool value) => OSCLib.ComfortRight(value);
        public static void GrabRight(bool value) => OSCLib.GrabRight(value);
        public static void DropRight(bool value) => OSCLib.DropRight(value);
        public static void UseRight(bool value) => OSCLib.UseRight(value);
        public static void GrabLeft(bool value) => OSCLib.GrabLeft(value);
        public static void DropLeft(bool value) => OSCLib.DropLeft(value);
        public static void UseLeft(bool value) => OSCLib.UseLeft(value);
        public static void PanicButton(bool value) => OSCLib.PanicButton(value);
        public static void QuickMenuToggleLeft(bool value) => OSCLib.QuickMenuToggleLeft(value);
        public static void QuickMenuToggleRight(bool value) => OSCLib.QuickMenuToggleRight(value);
        public static void Voice(bool value) => OSCLib.Voice(value);
        public static void SetAvatar(string id) => OSCLib.SetAvatar(id);

        public static void Register(string path, Function? function) {
            if (string.IsNullOrEmpty(path) || function == null) return;

            if (JSEngine.Initialized) {
                Console.Error("Listening to OSC paths or parameters can only be registered at initialization.");
                return;
            }

            AddAt(path, function);
            Console.Log($"Listening to OSC Path: {path}");
        }

        public static void RegisterParam(string name, Function? function) {
            if (string.IsNullOrEmpty(name) || function == null) return;

            if (JSEngine.Initialized) {
                Console.Error("Listening to OSC paths or parameters can only be registered at initialization.");
                return;
            }

            AddAt("/avatar/parameters/" + name, function);
        }

        internal static void Init() {
            if (RegistredPaths.Count == 0) return;

            foreach (var kvp in RegistredPaths) {
                var path = kvp.Key;
                var value = kvp.Value;

                RegistredPathsFinal[path] = value.ToArray();
                value.Clear();
            }

            RegistredPaths.Clear();
            StartListening();
        }

        private static Dictionary<string, List<Function>> RegistredPaths = new Dictionary<string, List<Function>>();
        private static Dictionary<string, Function[]> RegistredPathsFinal = new Dictionary<string, Function[]>();

        private static Function[]? GetAt(string path) => RegistredPathsFinal.TryGetValue(path, out Function[]? list) ? list : null;

        private static void AddAt(string path, Function function) {
            if (!RegistredPaths.ContainsKey(path))
                RegistredPaths.Add(path, new List<Function>());

            List<Function> list = RegistredPaths[path];
            if (!list.Contains(function)) list.Add(function);
        }

        private static void ReceiveMessage(string path, object?[] values) {
            var list = GetAt(path);
            if (list == null || list.Length == 0 || values == null || values.Length == 0) return;

            object? value = values[0];
            if (value == null) return;

            JSEngine.Enqueue(list, path, values);
        }

        private static bool IsListening { get; set; } = false;
        private static void StartListening() {
            if (IsListening) return;

            Logger.Log("OSC Listening Requested!");
            IsListening = true;
            OSCLib.StartOSCMonitor(ReceiveMessage);
        }
    }
}
