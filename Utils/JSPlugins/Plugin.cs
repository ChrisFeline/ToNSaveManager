using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;
using ToNSaveManager.Utils.JSPlugins.API;

namespace ToNSaveManager.Utils.JSPlugins {
    internal static partial class JSEngine {
        internal class Plugin {
            internal struct Source {
                public string FilePath;
                public string FileID;
                public string FileName;

                internal Source(string filePath, string fileId) {
                    FilePath = filePath;
                    FileID = fileId;
                    FileName = Path.GetFileName(filePath);
                }
            }

            public ObjectInstance? ModuleInstance;
            public Source FileSource;
            private string FileID => FileSource.FileID;
            private string FilePath => FileSource.FilePath;

            internal Function? OnEventFunction;
            internal Function? OnTickFunction;
            internal Function? OnReadyFunction;
            internal Function? OnLineFunction;

            internal bool HasOnEvent => OnEventFunction != null;
            internal bool HasOnTick => OnTickFunction != null;
            internal bool HasOnReady => OnReadyFunction != null;
            internal bool HasOnLine => OnLineFunction != null;

            internal Plugin(Source fileSource) {
                FileSource = fileSource;
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
                    if (e is TimeoutException) {
                        Logger.Error($"Importing '{Path.GetFileName(FilePath)}' is taking too long. {e.Message}\n" + EngineInstance.Advanced.StackTrace);
                        return;    
                    }

                    Logger.Error($"Error trying to import '{Path.GetFileName(FilePath)}'\n" + GetStackTrace(e));
                }
            }

            public static Plugin? LoadFrom(Source source) {
                try {
                    string filePath = source.FilePath;
                    string fileId = source.FileID;

                    Logger.Log("Loading: " + fileId);

                    EngineInstance.Modules.Add(fileId, builder => {
                        builder.ExportObject("storage", new Storage(filePath + "on")); // .js+on
                        builder.AddSource(File.ReadAllText(filePath));
                    });

                    Plugin plugin = new Plugin(source);

                    Logger.Log("Loaded: " + fileId);
                    return plugin;
                } catch (Exception e) {
                    Logger.Error($"Error while loading module '{source.FileID}' : {e.Message}");
                    return null;
                }
            }
        }
    }
}
