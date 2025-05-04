using Jint;
using Jint.Native;
using ToNSaveManager.Utils.API;

/* TODO:
 * Add more important or useful events to the websocket api, this will indirectly benefit this api.
 * Document all this crap
*/

namespace ToNSaveManager.Utils.JSPlugins {
    internal static partial class JSEngine {
        internal static readonly LoggerSource Logger = new LoggerSource("JavaScript");
        static bool Initialized { get; set; } = false;

#pragma warning disable CS8618
        internal static Engine EngineInstance;
#pragma warning restore CS8618
        static readonly HashSet<Plugin> Plugins = new ();

        static readonly List<Plugin> P_OnEvent = new();
        static readonly List<Plugin> P_OnTick = new();
        static readonly List<Plugin> P_OnReady = new();
        static readonly List<Plugin> P_OnLine = new();

        internal static void Initialize() {
            string scriptsPath = Path.Combine(Program.ProgramDirectory, "scripts");
            if (!Directory.Exists(scriptsPath)) return;

            EngineInstance = new Jint.Engine(options => {
                options.EnableModules(scriptsPath);
                options.Interop.AllowWrite = false;
                options.TimeoutInterval(TimeSpan.FromSeconds(60)); // Timeout if it takes too long to import a script, sorry :(
            });

            var storage = new Storage(scriptsPath);
            EngineInstance.SetValue("TON", API.Instance);
            EngineInstance.SetValue("OSC", OSC.Instance);

            foreach (string file in Directory.GetFiles(scriptsPath).Where(f => !f.StartsWith('.') && f.EndsWith(".js"))) {
                Plugin? plugin = Plugin.Import(file);
                if (plugin != null) {
                    Plugins.Add(plugin);
                    // Post process plugin
                    if (plugin.HasOnEvent) P_OnEvent.Add(plugin);
                    if (plugin.HasOnReady) P_OnReady.Add(plugin);
                    if (plugin.HasOnTick ) P_OnTick.Add(plugin);
                    if (plugin.HasOnLine ) P_OnLine.Add(plugin);
                }
            }

            Initialized = true;
        }

        internal static void InvokeOnEvent(WebSocketAPI.IEvent arg) {
            if (!Initialized || P_OnEvent.Count == 0) return;

            var eventValue = JsValue.FromObject(EngineInstance, arg);
            foreach (Plugin plugin in P_OnEvent) plugin.SendEvent(eventValue);
        }

        internal static void InvokeOnTick() {
            if (!Initialized) return;

            foreach (Plugin plugin in P_OnTick) plugin.SendTick();
        }

        internal static void InvokeOnReady() {
            if (!Initialized) return;

            foreach (Plugin plugin in P_OnReady) plugin.SendReady();
        }

        internal static void InvokeOnLine(string line) {
            if (!Initialized || P_OnLine.Count == 0) return;

            foreach (Plugin plugin in P_OnLine) plugin.SendLine(line);
        }
    }
}
