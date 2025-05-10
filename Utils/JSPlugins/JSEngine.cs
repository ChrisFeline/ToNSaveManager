using System.Reflection;
using System.Text;
using Acornima.Ast;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Interop;
using ToNSaveManager.Models;
using ToNSaveManager.Utils.API;

/* TODO:
 * Add more important or useful events to the websocket api, this will indirectly benefit this api.
 * Document all this crap
*/

namespace ToNSaveManager.Utils.JSPlugins {
    internal static class JSEngineExtensions {
        static MethodInfo? Method_GetLastSyntaxElement;

        internal static Node? GetLastSyntaxElement(this Engine engine) {
            if (Method_GetLastSyntaxElement == null) {
                Method_GetLastSyntaxElement = typeof(Engine).GetMethod("GetLastSyntaxElement", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            return (Node?)Method_GetLastSyntaxElement?.Invoke(engine, null);
        }

        internal static Engine SetType<T>(this Engine engine, string name) {
            return engine.SetValue(name, Jint.Runtime.Interop.TypeReference.CreateTypeReference<T>(engine));
        }
    }

    internal class JSEngineAPIAttribute : Attribute {
        public string Name { get; set; }

        public JSEngineAPIAttribute(string name) {
            Name = name;
        }
    }

    internal static partial class JSEngine {
        internal static readonly StringBuilder SharedSB = new StringBuilder();
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

        internal static string? GetStackTrace(Exception e) {
            if (e is JavaScriptException je) {
                return je.GetJavaScriptErrorString();
            }

            return e.Message;
        }

        internal static string GetLastSyntaxSource() {
            return EngineInstance.GetLastSyntaxElement()?.Location.SourceFile ?? "Unknown Source";
        }

        internal static void Initialize() {
            string scriptsPath = Path.Combine(Program.ProgramDirectory, "scripts");
            if (!Directory.Exists(scriptsPath)) return;

            EngineInstance = new Jint.Engine(options => {
                options.EnableModules(scriptsPath);
                options.Interop.AllowWrite = false;
                options.TimeoutInterval(TimeSpan.FromSeconds(60)); // Timeout if it takes too long to import a script, sorry :(
            });

            var storage = new Storage(scriptsPath);
            // scan apis
            TypeReference? typeReference;

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                typeReference = null;
                foreach (JSEngineAPIAttribute attr in type.GetCustomAttributes<JSEngineAPIAttribute>()) {
                    if (typeReference == null)
                        typeReference = TypeReference.CreateTypeReference(EngineInstance, type);

                    EngineInstance.SetValue(attr.Name, typeReference);
                    Logger.Debug("Registered API: " + attr.Name);
                }

                if (typeReference != null) {
                    MethodInfo? registerMethod = type.GetMethod("Register", BindingFlags.NonPublic | BindingFlags.Static);
                    if (registerMethod != null) {
                        registerMethod.Invoke(null, [EngineInstance]);
                        Logger.Debug("Called Register()");
                    }
                }
            }
            // general
            EngineInstance.SetValue("Settings", Settings.Get);

            // pre-process
            foreach (string file in Directory.GetFiles(scriptsPath, "*.js", SearchOption.AllDirectories).Where(f => !f.StartsWith('.') && f.EndsWith(".js"))) {
                string fileId = Path.GetRelativePath(scriptsPath, file).Replace('\\', '/');
                Plugin.Source source = new Plugin.Source(file, fileId);

                Plugin? plugin = Plugin.LoadFrom(source);
                if (plugin != null) Plugins.Add(plugin);
            }
            // post-process
            foreach (Plugin plugin in Plugins) {
                plugin.Import();
                if (plugin.HasOnEvent) P_OnEvent.Add(plugin);
                if (plugin.HasOnReady) P_OnReady.Add(plugin);
                if (plugin.HasOnTick) P_OnTick.Add(plugin);
                if (plugin.HasOnLine) P_OnLine.Add(plugin);
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
