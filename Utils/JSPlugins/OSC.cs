using Jint;
using Jint.Native;
using Jint.Native.Function;

namespace ToNSaveManager.Utils.JSPlugins {
    // Re-Wrapping anything here so I can handle type errors and such...
    internal class OSC {
        internal static readonly OSC Instance = new();

        public void Send(string path, object value) => OSCLib.Send(path, value);
        public void SendFloat(string path, float value) => OSCLib.Send(path, value);
        public void SendInt(string path, int value) => OSCLib.Send(path, value);
        public void SendBool(string path, bool value) => OSCLib.Send(path, value);
        public void SendParameter(string name, float value) => OSCLib.SendParameter(name, value);
        public void SendParameter(string name, bool value) => OSCLib.SendParameter(name, value);
        public void SendChatbox(string message, bool direct = true, bool complete = false) => OSCLib.SendChatbox(message, direct, complete);
        public void SetChatboxTyping(bool value) => OSCLib.SetChatboxTyping(value);
        public void MoveVertical(float value) => OSCLib.MoveVertical(value);
        public void MoveHorizontal(float value) => OSCLib.MoveHorizontal(value);
        public void LookHorizontal(float value) => OSCLib.LookHorizontal(value);
        public void UseAxisRight(float value) => OSCLib.UseAxisRight(value);
        public void GrabAxisRight(float value) => OSCLib.GrabAxisRight(value);
        public void MoveHoldFB(float value) => OSCLib.MoveHoldFB(value);
        public void SpinHoldCW(float value) => OSCLib.SpinHoldCW(value);
        public void SpinHoldUD(float value) => OSCLib.SpinHoldUD(value);
        public void SpinHoldLR(float value) => OSCLib.SpinHoldLR(value);
        public void MoveForward(bool value) => OSCLib.MoveForward(value);
        public void MoveBackward(bool value) => OSCLib.MoveBackward(value);
        public void MoveLeft(bool value) => OSCLib.MoveLeft(value);
        public void MoveRight(bool value) => OSCLib.MoveRight(value);
        public void LookLeft(bool value) => OSCLib.LookLeft(value);
        public void LookRight(bool value) => OSCLib.LookRight(value);
        public void Jump(bool value) => OSCLib.Jump(value);
        public void Run(bool value) => OSCLib.Run(value);
        public void ComfortLeft(bool value) => OSCLib.ComfortLeft(value);
        public void ComfortRight(bool value) => OSCLib.ComfortRight(value);
        public void GrabRight(bool value) => OSCLib.GrabRight(value);
        public void DropRight(bool value) => OSCLib.DropRight(value);
        public void UseRight(bool value) => OSCLib.UseRight(value);
        public void GrabLeft(bool value) => OSCLib.GrabLeft(value);
        public void DropLeft(bool value) => OSCLib.DropLeft(value);
        public void UseLeft(bool value) => OSCLib.UseLeft(value);
        public void PanicButton(bool value) => OSCLib.PanicButton(value);
        public void QuickMenuToggleLeft(bool value) => OSCLib.QuickMenuToggleLeft(value);
        public void QuickMenuToggleRight(bool value) => OSCLib.QuickMenuToggleRight(value);
        public void Voice(bool value) => OSCLib.Voice(value);
        public void SetAvatar(string id) => OSCLib.SetAvatar(id);

        public void Register(string path, Function? function) {
            if (string.IsNullOrEmpty(path) || function == null) return;
            StartListening();

            AddAt(path, function);
            Console.Instance.Log($"Listening to OSC Path: {path}");
        }

        public void RegisterParam(string name, Function? function) {
            if (string.IsNullOrEmpty(name) || function == null) return;
            StartListening();

            AddAt("/avatar/parameters/" + name, function);
        }

        private static Dictionary<string, List<Function>> RegistredPaths = new Dictionary<string, List<Function>>();
        private static List<Function>? GetAt(string path) => RegistredPaths.TryGetValue(path, out List<Function>? list) ? list : null;
        private static void AddAt(string path, Function function) {
            if (!RegistredPaths.ContainsKey(path))
                RegistredPaths.Add(path, new List<Function>());

            List<Function> list = RegistredPaths[path];
            if (!list.Contains(function)) list.Add(function);
        }

        private static void ReceiveMessage(string path, object?[] values) {
            List<Function>? list = GetAt(path);
            if (list == null || list.Count == 0) return;

            JsValue val = JsValue.FromObject(JSEngine.EngineInstance, values);
            foreach (Function function in list) {
                try {
                    function.Call(path, val);
                } catch (Exception e) {
                    Console.Instance.Error("An exception was thrown while calling a function.\n" + JSEngine.GetStackTrace(e));
                }
            }
        }

        private static bool IsListening { get; set; } = false;
        private static void StartListening() {
            if (IsListening) return;

            IsListening = true;
            OSCLib.StartOSCMonitor(ReceiveMessage);
        }
    }
}
