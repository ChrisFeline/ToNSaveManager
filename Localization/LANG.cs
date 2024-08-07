﻿using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using ToNSaveManager.Windows;

namespace ToNSaveManager.Localization {
    internal static class LANG {
        public struct LangKey {
            public string Key;
            public string Name;
            public string Chars;

            public override string ToString() {
                return Name;
            }
        }

        const string PREF_DEFAULT_KEY = "en-US";

        static Dictionary<string, Dictionary<string, string>> LanguageData = new Dictionary<string, Dictionary<string, string>>();

        static Dictionary<string, string> SelectedLang = new Dictionary<string, string>();
        internal static string SelectedKey { get; private set; } = PREF_DEFAULT_KEY;

        static Dictionary<string, string> SelectedDefault = new Dictionary<string, string>();
        static string SelectedDefaultKey = string.Empty;

        internal static List<LangKey> AvailableLang { get; private set; } = new List<LangKey>();
        static readonly Regex ReplacePattern = new Regex(@"\$\$(?<key>.*?)\$\$", RegexOptions.Compiled);
        const string PatternSearch = "$$";

        private static string? D(string key, params string[] args) {
#if DEBUG
            // if (!key.EndsWith(".TT")) Debug.WriteLine($"Missing key '{key}' in language pack '{SelectedKey}'");
#endif

            if (SelectedDefault.ContainsKey(key)) {
                return args.Length > 0 ? string.Format(SelectedDefault[key], args) : SelectedDefault[key];
            }

#if DEBUG
            if (!key.EndsWith(".TT")) Debug.WriteLine($"Invalid language key '{key}'");
#endif
            return null;
        }

        public static string? S(string key, params string[] args) {
            string? result;
            if (SelectedLang.ContainsKey(key)) {
                result = args.Length > 0 ? string.Format(SelectedLang[key], args) : SelectedLang[key];
            } else {
                result = D(key, args);
            }

            if (!string.IsNullOrEmpty(result) && result.Contains(PatternSearch)) {
                result = ReplacePattern.Replace(result, (v) => {
                    string k = v.Groups["key"].Value;
                    return S(k) ?? v.Value;
                });
            }

            return result;
        }

        public static (string?, string?) T(string key, params string[] args) {
            return (S(key, args), S(key + ".TT", args));
        }

        public static void C(Control control, string key, ToolTip? toolTip = null) {
            (string? tx, string? tt) = T(key);
            if (!string.IsNullOrEmpty(tx)) control.Text = tx;
            if (!string.IsNullOrEmpty(tt)) {
                if (toolTip == null) TooltipUtil.Set(control, tt);
                else toolTip.SetToolTip(control, tt);
            }
        }
        public static void C(ToolStripItem item, string key) {
            (string? text, string? tooltip) = T(key);
            if (!string.IsNullOrEmpty(text)) item.Text = text;
            if (!string.IsNullOrEmpty(tooltip)) item.ToolTipText = tooltip;
        }

        internal static void Select(string key) {
            Debug.WriteLine("Selecting language key: " + key);
            SelectedLang = LanguageData.ContainsKey(key) ? LanguageData[key] : LanguageData[key = SelectedDefaultKey];
            SelectedKey = key;
        }

        internal static string FindLanguageKey() {
            var currentCulture = System.Globalization.CultureInfo.CurrentUICulture;
            string langName = currentCulture.TwoLetterISOLanguageName;
            string fullLangName = currentCulture.Name;

            string[] languageKeys = LanguageData.Keys.ToArray();

            string foundKey = SelectedDefaultKey;
            for (int i = 0; i < 2; i++) {
                foreach (string key in languageKeys) {
                    bool check = i == 0 ?
                        key.Equals(fullLangName, StringComparison.OrdinalIgnoreCase) :
                        key.StartsWith(langName);

                    if (check) {
                        foundKey = key;
                        i = 3;
                        break;
                    }
                }
            }

            return foundKey;
        }

        internal static void ReloadAll() {
            MainWindow.Instance?.LocalizeContent();
            EditWindow.Instance?.LocalizeContent();
            ObjectivesWindow.Instance?.LocalizeContent();
            SettingsWindow.Instance?.LocalizeContent();
        }

        internal static void AddFromFile(string filePath) {
            string json = File.ReadAllText(filePath);
            var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (obj != null) {
                string key = Path.GetFileNameWithoutExtension(filePath);
                LanguageData.Add(key, obj);

                Debug.WriteLine("Added custom language with key: " + key);
                AvailableLang.Add(new LangKey() { Key = key, Chars = obj["DISPLAY_INIT"], Name = obj["DISPLAY_NAME"] });
                Select(key);

                SettingsWindow.Instance?.FillLanguageBox();
                ReloadAll();
            }
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
                                    AvailableLang.Add(new LangKey() { Key = key, Chars = obj["DISPLAY_INIT"], Name = obj["DISPLAY_NAME"] });
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
