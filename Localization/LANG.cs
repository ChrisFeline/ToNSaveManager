using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace ToNSaveManager.Localization {
    internal static class LANG {
        const string PREF_DEFAULT_KEY = "en-US";

        static Dictionary<string, Dictionary<string, string>> LanguageData = new Dictionary<string, Dictionary<string, string>>();

        static Dictionary<string, string> SelectedLang = new Dictionary<string, string>();
        static string SelectedKey = PREF_DEFAULT_KEY;

        static Dictionary<string, string> SelectedDefault = new Dictionary<string, string>();
        static string SelectedDefaultKey = string.Empty;

        private static string? D(string key, params string[] args) {
            Debug.WriteLine($"Missing key '{key}' in language pack '{SelectedKey}'");

            if (SelectedDefault.ContainsKey(key)) {
                return args.Length > 0 ? string.Format(SelectedDefault[key], args) : SelectedDefault[key];
            }

            Debug.WriteLine($"Invalid language key '{key}'");
            return null;
        }

        public static string? S(string key, params string[] args) {
            string? result;
            if (SelectedDefault.ContainsKey(key)) {
                result = args.Length > 0 ? string.Format(SelectedLang[key], args) : SelectedLang[key];
            } else {
                result = D(key, args);
            }

#if DEBUG
            // For debugging language strings, because I get lost easily
            if (!string.IsNullOrEmpty(result)) result = "!!" + result;
#endif
            return result;
        }

        public static (string?, string?) T(string key, params string[] args) {
            return (S(key, args), S(key + ".TT", args));
        }

        public static void C(Control control, string key) {
            (string? text, string? tooltip) = T(key);
            if (!string.IsNullOrEmpty(text)) control.Text = text;
            if (!string.IsNullOrEmpty(tooltip)) TooltipUtil.Set(control, tooltip);
        }
        public static void C(ToolStripItem item, string key) {
            (string? text, string? tooltip) = T(key);
            if (!string.IsNullOrEmpty(text)) item.Text = text;
            if (!string.IsNullOrEmpty(tooltip)) item.ToolTipText = tooltip;
        }

        internal static void Select(string key) {
            SelectedLang = LanguageData.ContainsKey(key) ? LanguageData[key] : LanguageData[key = SelectedDefaultKey];
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
                                        SelectedDefault = obj;
                                        SelectedDefaultKey = key;
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

            if (string.IsNullOrEmpty(SelectedDefaultKey) && !string.IsNullOrEmpty(firstKey)) {
                Debug.WriteLine("Default prefered language not found, using " + firstKey);

                SelectedDefault = LanguageData[firstKey];
                SelectedDefaultKey = firstKey;
                Select(SelectedDefaultKey);
            } else if (string.IsNullOrEmpty(firstKey)) {
                throw new Exception("Could not load any language pack.");
            }
        }
    }
}
