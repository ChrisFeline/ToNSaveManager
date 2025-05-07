using System;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;

namespace ToNSaveManager.Utils.JSPlugins {
    internal static partial class JSEngine {
        internal class Plugin {
            public ObjectInstance? ModuleInstance;
            public string FileID;
            public string FilePath;

            private Function? OnEventFunction;
            private Function? OnTickFunction;
            private Function? OnReadyFunction;
            private Function? OnLineFunction;

            internal bool HasOnEvent => OnEventFunction != null;
            internal bool HasOnTick => OnTickFunction != null;
            internal bool HasOnReady => OnReadyFunction != null;
            internal bool HasOnLine => OnLineFunction != null;

            private Console ConsoleInstance;

            internal Plugin(string fileID, string filePath, Console console) {
                ConsoleInstance = console;
                FileID = fileID;
                FilePath = filePath;
            }

            public void SendEvent(JsValue @event) {
                try {
                    OnEventFunction?.Call(@event);
                } catch (Exception e) {
                    ConsoleInstance.Error("An exception was thrown while calling 'OnEvent()' function.\n" + GetStackTrace(e));
                }
            }
            public void SendTick() {
                try {
                    OnTickFunction?.Call();
                } catch (Exception e) {
                    ConsoleInstance.Error("An exception was thrown while calling 'OnTick()' function.\n" + GetStackTrace(e));
                }
            }
            public void SendReady() {
                try {
                    OnReadyFunction?.Call();
                } catch (Exception e) {
                    ConsoleInstance.Error("An exception was thrown while calling 'OnReady()' function.\n" + GetStackTrace(e));
                }
            }
            public void SendLine(string line) {
                try {
                    OnLineFunction?.Call(line);
                } catch (Exception e) {
                    ConsoleInstance.Error("An exception was thrown while calling 'OnLine()' function.\n': " + GetStackTrace(e));
                }
            }

            private string? GetStackTrace(Exception e) {
                if (e is JavaScriptException je) {
                    return je.GetJavaScriptErrorString();
                }

                return e.Message;
            }

            public void Import() {
                try {
                    Logger.Info("Importing: " + FileID);

                    ObjectInstance instance = EngineInstance.Modules.Import(FileID);

                    if (instance.Get("OnEvent") is Function f1 && f1 != null) {
                        OnEventFunction = f1;
                    }
                    if (instance.Get("OnReady") is Function f2 && f2 != null) {
                        OnReadyFunction = f2;
                    }
                    if (instance.Get("OnTick") is Function f3 && f3 != null) {
                        OnTickFunction = f3;
                    }
                    if (instance.Get("OnLine") is Function f4 && f4 != null) {
                        OnLineFunction = f4;
                    }

                    Logger.Info("Imported: " + FileID);
                    ModuleInstance = instance;
                } catch (Exception e) {
                    Logger.Error($"Error trying to import '{Path.GetFileName(FilePath)}'\n" + GetStackTrace(e));
                }
            }

            public static Plugin? LoadFrom(string filePath) {
                try {
                    string fileId = Path.GetFileName(filePath);
                    Logger.Log("Loading: " + fileId);

                    Console console = new Console(fileId);

                    EngineInstance.Modules.Add(fileId, builder => {
                        builder.ExportObject("console", console);
                        builder.ExportObject("storage", new Storage(filePath + "on")); // .js+on
                        builder.ExportObject("WS", new WS(fileId));
                        builder.ExportFunction("print", console.Print);
                        // Unsafe, but better for stacktrace, sorry
                        builder.AddSource(File.ReadAllText(filePath));
                    });

                    Plugin plugin = new Plugin(fileId, filePath, console);

                    Logger.Log("Loaded: " + fileId);
                    return plugin;
                } catch (Exception e) {
                    Logger.Error($"Error while loading module '{Path.GetFileName(filePath)}' : {e.Message}");
                    return null;
                }
            }
        }
    }
}
