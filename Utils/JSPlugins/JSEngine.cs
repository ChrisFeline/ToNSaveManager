using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Acornima.Ast;
using Jint;
using Jint.Native;
using Jint.Native.Function;
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

        internal static string GetName(this Function function) {
            const string @string = " { [native code] }";
            return function.ToString().Replace(@string, string.Empty);
        }
    }

    internal class JSEngineAPIAttribute : Attribute {
        public string? Name { get; set; }

        public JSEngineAPIAttribute(string name) {
            Name = name;
        }

        public JSEngineAPIAttribute() { }
    }

    internal static partial class JSEngine {
        internal static string scriptsPath = Path.Combine(Program.ProgramDirectory, "scripts");

        internal static readonly StringBuilder SharedSB = new StringBuilder();
        internal static readonly LoggerSource Logger = new LoggerSource("JavaScript");
        internal static bool Initialized { get; set; } = false;

#pragma warning disable CS8618
        internal static Engine EngineInstance;
#pragma warning restore CS8618
        static readonly HashSet<Plugin> Plugins = new ();

        static Function[] P_OnEvent_Fn = Array.Empty<Function>();
        static Function[] P_OnTick_Fn = Array.Empty<Function>();
        static Function[] P_OnReady_Fn = Array.Empty<Function>();
        static Function[] P_OnLine_Fn = Array.Empty<Function>();

        internal static string? GetStackTrace(Exception e) {
            if (e is JavaScriptException je) {
                return je.GetJavaScriptErrorString();
            }

            return e.Message;
        }

        internal static string GetLastSyntaxSource() {
            return EngineInstance.GetLastSyntaxElement()?.Location.SourceFile ?? "Unknown Source";
        }

        internal static void CreateEngine() {
            EngineInstance = new Jint.Engine(options => {
                options.EnableModules(scriptsPath);
                options.Interop.AllowWrite = false;
                options.TimeoutInterval(TimeSpan.FromSeconds(30)); // Timeout if it takes too long to import a script, sorry :(
            });

            // scan apis
            TypeReference? typeReference;
            bool hasApiAttr = false;

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
                typeReference = null;
                hasApiAttr = false;
                foreach (JSEngineAPIAttribute attr in type.GetCustomAttributes<JSEngineAPIAttribute>()) {
                    hasApiAttr = true;
                    if (string.IsNullOrEmpty(attr.Name)) continue;

                    if (typeReference == null)
                        typeReference = TypeReference.CreateTypeReference(EngineInstance, type);

                    EngineInstance.SetValue(attr.Name, typeReference);
                    Logger.Debug("Registered API: " + attr.Name);
                }

                if (hasApiAttr) {
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
            }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            P_OnEvent_Fn = Plugins.Where(p => p.HasOnEvent).Select(p => p.OnEventFunction).ToArray();
            P_OnTick_Fn = Plugins.Where(p => p.HasOnTick).Select(p => p.OnTickFunction).ToArray();
            P_OnLine_Fn = Plugins.Where(p => p.HasOnLine).Select(p => p.OnLineFunction).ToArray();
            P_OnReady_Fn = Plugins.Where(p => p.HasOnReady).Select(p => p.OnReadyFunction).ToArray();
#pragma warning restore CS8619

            API.OSC.Init();
            Initialized = true;
        }

        struct JSOperation {
            public Function[] Functions;
            public object?[]? Arguments;
            public Action<JsValue>? Callback { get; set; }
        }

        static readonly ConcurrentQueue<JSOperation> JSQueue = new ConcurrentQueue<JSOperation>();
        static readonly ManualResetEvent JSEvent = new ManualResetEvent(false);

        internal static void Enqueue(Function[] functions, params object?[] arguments) {
            JSOperation op = new JSOperation() {
                Functions = functions,
                Arguments = arguments.Length == 0 ? null : arguments
            };
            JSQueue.Enqueue(op);
            JSEvent.Set();
        }
        internal static void Enqueue(Function function, params object?[] arguments) {
            Enqueue([function], arguments);
        }

        internal static void Process() {
            while (true) {
                if (JSQueue.TryDequeue(out JSOperation operation)) {
                    Function[] funcs = operation.Functions;

                    JsValue[]? args = operation.Arguments?.Select(p => JsValue.FromObject(EngineInstance, p)).ToArray();
                    foreach (Function func in operation.Functions) {
                        try {
                            JsValue jsv = args != null ? func.Call(args) : func.Call();
                            operation.Callback?.Invoke(jsv);
                        } catch (Exception e) {
                            if (e is TimeoutException) {
                                API.Console.Error($"Calling '{func.GetName()}' is taking too long. {e.Message}\n" + EngineInstance.Advanced.StackTrace);
                                continue;
                            }

                            API.Console.Error($"An exception was thrown while calling {func.GetName()}.\n: {GetStackTrace(e)}");
                        }
                    }
                } else {
                    JSEvent.Reset();
                    JSEvent.WaitOne();
                }
            }
        }

        static Thread? JSThread = null;
        internal static void Initialize() {
            if (!Directory.Exists(scriptsPath) || JSThread != null) return;

            CreateEngine();

            JSThread = new Thread(new ThreadStart(Process)) { IsBackground = true };
            JSThread.Start();
        }

        internal static void InvokeOnEvent(WebSocketAPI.IEvent arg) {
            if (!Initialized || P_OnEvent_Fn.Length == 0) return;
            Enqueue(P_OnEvent_Fn, arg);
        }

        internal static void InvokeOnTick() {
            if (!Initialized || P_OnTick_Fn.Length == 0) return;
            Enqueue(P_OnTick_Fn);
        }

        internal static void InvokeOnReady() {
            if (!Initialized || P_OnReady_Fn.Length == 0) return;
            Enqueue(P_OnReady_Fn);
        }

        internal static void InvokeOnLine(string line) {
            if (!Initialized || P_OnLine_Fn.Length == 0) return;
            Enqueue(P_OnLine_Fn, line);
        }
    }
}
