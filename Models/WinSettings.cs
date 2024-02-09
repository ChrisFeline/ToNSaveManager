using Newtonsoft.Json;

namespace ToNSaveManager.Models
{
    internal class WinSettings
    {
        internal static readonly WinSettings Get;
        internal static void Export() => Get.TryExport();

        public static string Destination = "window.json";

        static WinSettings()
        {
            Get = Import();
        }

        public int LastWindowWidth;
        public int LastWindowHeight;
        public int LastWindowSplit;

        public static WinSettings Import()
        {
            if (!string.IsNullOrEmpty(Program.ProgramDirectory))
                Destination = Path.Combine(Program.ProgramDirectory, Destination);

            WinSettings? settings;

            try {
                if (!File.Exists(Destination)) return new WinSettings();
                string content = File.ReadAllText(Destination);
                settings = JsonConvert.DeserializeObject<WinSettings>(content);
            }
            catch
            {
                settings = null;
            }

            return settings ?? new WinSettings();
        }

        private void TryExport()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(Destination, json);
            }
            catch { } // An error ocurred, but it wasn't important ¯\_(ツ)_/¯
        }
    }
}
