namespace ToNSaveManager.Utils.JSPlugins {
    internal class Storage {
        static LoggerSource Logger => JSEngine.Logger;

        private readonly Dictionary<string, object?> Data = new Dictionary<string, object?>();
        private string FilePath;
        private string FileName;

        public Storage(string filePath) {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            string content = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;
            Data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object?>>(content) ?? new();
        }

        public void Set(string id, object value) {
            bool contains = Data.ContainsKey(id);

            object? old = contains ? Data[id] : null;
            Data[id] = value; // New value

            try {
                Export();
            } catch (Exception e) {
                Logger.Error($"Error trying to store value to '{id}' as {value} in {FileName}; {e.Message}");
                if (contains) {
                    Data[id] = old; // Revert to old value
                } else {
                    Data.Remove(id);
                }
                // Try again
                Export();
            }
        }

        public void Del(string id) {
            if (Data.ContainsKey(id)) {
                Data.Remove(id);
                Export();
            }
        }

        public object? Get(string id) {
            object? value = null;
            _ = Data.TryGetValue(id, out value);
            return value;
        }

        private void Export() {
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
            File.WriteAllText(FilePath, data);
        }
    }
}
