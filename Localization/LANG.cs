using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using ToNSaveManager.Models;

namespace ToNSaveManager.Localization {
    internal static class LANG {
        const string PREF_DEFAULT_KEY = "en-US";

        static Dictionary<string, Dictionary<string, string>> LanguageData = new Dictionary<string, Dictionary<string, string>>();

        static Dictionary<string, string> SelectedLang = new Dictionary<string, string>();
        static string SelectedKey = PREF_DEFAULT_KEY;

        static string SelectedDefault = string.Empty;

        public static string? S(string key) {
            return SelectedLang.ContainsKey(key) ? SelectedLang[key] : null;
        }

        public static void Select(string key) {
            SelectedLang = LanguageData.ContainsKey(key) ? LanguageData[key] : LanguageData[key = SelectedDefault];
            SelectedKey = key;
        }

        internal static void Initialize() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] streamNames = assembly.GetManifestResourceNames();

            string? firstKey = null;
            foreach (string name in streamNames) {
                string[] split = name.Split('.');
                if (name.EndsWith(".json") && Array.IndexOf(split, "Localization") > 0 && Array.IndexOf(split, "Language") > 0) {
                    using (Stream? stream = assembly.GetManifestResourceStream(name)) {
                        if (stream != null) {
                            using (StreamReader reader = new StreamReader(stream)) {
                                string json = reader.ReadToEnd();
                                var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                                if (obj != null) {
                                    string key = split[split.Length - 2];
                                    LanguageData.Add(key, obj);

                                    if (key == PREF_DEFAULT_KEY) {
                                        SelectedDefault = key;
                                        Select(key);

                                        Debug.WriteLine("Found default language.");
                                    }

                                    if (string.IsNullOrEmpty(firstKey)) firstKey = key;

                                    Debug.WriteLine("Added language with key: " + key);
                                }
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(SelectedDefault) && !string.IsNullOrEmpty(firstKey)) {
                Debug.WriteLine("Default prefered language not found, using " + firstKey);

                SelectedDefault = firstKey;
                Select(SelectedDefault);
            } else if (string.IsNullOrEmpty(firstKey)) {
                throw new Exception("Could not load any language pack.");
            }
        }
    }
}
