using Newtonsoft.Json;

namespace ToNSaveManager
{
    internal class AppSettings
    {
        const string Destination = "settings.json";

        private bool IsDirty;

        [JsonIgnore] private bool m_AutoCopy = true;
        /// <summary>
        /// Automatically copy newly detected save codes as you play.
        /// </summary>
        public bool AutoCopy {
            get { return m_AutoCopy; }
            set {
                if (value == m_AutoCopy) return;

                m_AutoCopy = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Import a settings instance from the local json file
        /// </summary>
        /// <returns>Deserialized Settings object, or else Default Settings object.</returns>
        public static AppSettings Import()
        {
            AppSettings? settings;

            if (!File.Exists(Destination)) return new AppSettings();

            try {
                string content = File.ReadAllText(Destination);
                settings = JsonConvert.DeserializeObject<AppSettings>(content);
            } catch {
                settings = null;
            }

            return settings ?? new AppSettings();
        }

        public void Export()
        {
            if (!IsDirty) return;

            try {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(Destination, json);
            } catch (Exception e) {
                MessageBox.Show("An error ocurred while trying to write your settings to a file.\n\nMake sure that the program contains permissions to write files in the current folder it's located at.\n\n" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IsDirty = false;
        }
    }
}
