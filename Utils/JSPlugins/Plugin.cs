using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using ToNSaveManager.Utils.API;

namespace ToNSaveManager.Utils.JSPlugins {
    internal static partial class JSEngine {
        internal class Plugin {
            public ObjectInstance ModuleInstance;

            private Function? OnEventFunction;
            private Function? OnTickFunction;
            private Function? OnReadyFunction;
            private Function? OnLineFunction;

            internal bool HasOnEvent => OnEventFunction != null;
            internal bool HasOnTick => OnTickFunction != null;
            internal bool HasOnReady => OnReadyFunction != null;
            internal bool HasOnLine => OnLineFunction != null;

            private Console ConsoleInstance;

            internal Plugin(ObjectInstance instance, Console console) {
                ModuleInstance = instance;
                ConsoleInstance = console;
            }

            public void SendEvent(JsValue @event) {
                try {
                    OnEventFunction?.Call(@event);
                } catch (Exception e) {
                    ConsoleInstance.Error("An exception was thrown while calling 'function OnEvent()' event: " + e.Message);
                }
            }
            public void SendTick() {
                try {
                    OnTickFunction?.Call();
                } catch (Exception e) {
                    ConsoleInstance.Error("An exception was thrown while calling 'function OnTick()' event: " + e.Message);
                }
            }
            public void SendReady() {
                try {
                    OnReadyFunction?.Call();
                } catch (Exception e) {
                    ConsoleInstance.Error("An exception was thrown while calling 'function OnReady()' event: " + e.Message);
                }
            }
            public void SendLine(string line) {
                try {
                    OnLineFunction?.Call(line);
                } catch (Exception e) {
                    ConsoleInstance.Error("An exception was thrown while calling 'function OnLine()' event: " + e.Message);
                }
            }

            public static Plugin? Import(string filePath) {
                try {
                    string fileId = Path.GetFileName(filePath);
                    Logger.Info("Importing: " + fileId);

                    string tempId = Guid.NewGuid().ToString();
                    Console console = new Console(fileId);

                    EngineInstance.Modules.Add(tempId, builder => {
                        builder.ExportObject("console", console);
                        builder.ExportObject("storage", new Storage(filePath + "on")); // .js+on
                        builder.ExportObject("WS", new WS(fileId));
                    });

                    string header = "import { console, storage, WS } from '" + tempId + "';\n";
                    header += File.ReadAllText(filePath);

                    EngineInstance.Modules.Add(fileId, header);

                    ObjectInstance instance = EngineInstance.Modules.Import(fileId);

                    Plugin plugin = new Plugin(instance, console);

                    if (instance.Get("OnEvent") is Function f1 && f1 != null) {
                        plugin.OnEventFunction = f1;
                    }
                    if (instance.Get("OnReady") is Function f2 && f2 != null) {
                        plugin.OnReadyFunction = f2;
                    }
                    if (instance.Get("OnTick") is Function f3 && f3 != null) {
                        plugin.OnTickFunction = f3;
                    }
                    if (instance.Get("OnLine") is Function f4 && f4 != null) {
                        plugin.OnLineFunction = f4;
                    }

                    Logger.Info("Imported: " + fileId);
                    return plugin;
                } catch (Exception e) {
                    Logger.Error($"Error trying to import '{Path.GetFileName(filePath)}' : {e.Message}");
                    return null;
                }
            }
        }
    }
}
