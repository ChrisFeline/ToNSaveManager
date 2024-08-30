using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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
        internal static bool IsRightToLeft = false;

        static Dictionary<string, string> SelectedDefault = new Dictionary<string, string>();
        static string SelectedDefaultKey = string.Empty;

        internal static List<LangKey> AvailableLang { get; private set; } = new List<LangKey>();
        static readonly Regex ReplacePattern = new Regex(@"\$\$(?<key>.*?)\$\$", RegexOptions.Compiled);
        const string PatternSearch = "$$";

        private static string? D(string key, params string[] args) {
#if DEBUG
            // if (!key.EndsWith(".TT")) Logger.Debug($"Missing key '{key}' in language pack '{SelectedKey}'");
#endif

            if (SelectedDefault.ContainsKey(key)) {
                return args.Length > 0 ? string.Format(SelectedDefault[key], args) : SelectedDefault[key];
            }

#if DEBUG
            if (!key.EndsWith(".TT")) Logger.Debug($"Invalid language key '{key}'");
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

#if DEBUG
            // if (!string.IsNullOrEmpty(result)) result = '!' + result;
#endif

            return result;
        }

        public static (string?, string?) T(string key, params string[] args) {
            return (S(key, args), S(key + ".TT", args));
        }

        public static void C(Control control, string key, ToolTip? toolTip = null, string? value = null) {
            (string? tx, string? tt) = T(key);
            if (!string.IsNullOrEmpty(tx)) control.Text = tx;
            else if (!string.IsNullOrEmpty(value)) control.Text = value;
            if (!string.IsNullOrEmpty(tt)) {
                if (toolTip == null) TooltipUtil.Set(control, tt);
                else toolTip.SetToolTip(control, tt);
            }

            control.RightToLeft = IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }
        public static void C(ToolStripItem item, string key) {
            (string? text, string? tooltip) = T(key);
            if (!string.IsNullOrEmpty(text)) item.Text = text;
            if (!string.IsNullOrEmpty(tooltip)) item.ToolTipText = tooltip;

            item.RightToLeft = IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }

        internal static void Select(string key) {
            Logger.Debug("Selecting language key: " + key);
            SelectedLang = LanguageData.ContainsKey(key) ? LanguageData[key] : LanguageData[key = SelectedDefaultKey];
            SelectedKey = key;
            IsRightToLeft = SelectedLang.ContainsKey("RIGHT_TO_LEFT") && SelectedLang["RIGHT_TO_LEFT"] == "YES";
        }

        internal static string FindLanguageKey() {
#if DEBUG // Only print on Debug, not on release.
            Logger.Debug("Finding Language Key...");
            CultureInfo ci = CultureInfo.InstalledUICulture;

            Logger.Debug("Default Language Info:");
            Logger.Debug(string.Format("* Name: {0}", ci.Name));
            Logger.Debug(string.Format("* Display Name: {0}", ci.DisplayName));
            Logger.Debug(string.Format("* English Name: {0}", ci.EnglishName));
            Logger.Debug(string.Format("* 2-letter ISO Name: {0}", ci.TwoLetterISOLanguageName));
            Logger.Debug(string.Format("* 3-letter ISO Name: {0}", ci.ThreeLetterISOLanguageName));
            Logger.Debug(string.Format("* 3-letter Win32 API Name: {0}", ci.ThreeLetterWindowsLanguageName));
#endif

            var currentCulture = CultureInfo.InstalledUICulture;
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
            StatsWindow.Instance?.LocalizeContent();
        }

        internal static void AddFromFile(string filePath) {
            string json = File.ReadAllText(filePath);
            var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (obj != null) {
                string key = Path.GetFileNameWithoutExtension(filePath);
                if (!LanguageData.ContainsKey(key)) {
                    AvailableLang.Add(new LangKey() { Key = key, Chars = obj["DISPLAY_INIT"], Name = obj["DISPLAY_NAME"] });
                    LanguageData.Add(key, obj);
                } else {
                    int index = AvailableLang.FindIndex(v => v.Key == key);
                    if (index > -1)
                        AvailableLang[index] = new LangKey() { Key = key, Chars = obj["DISPLAY_INIT"], Name = obj["DISPLAY_NAME"] };

                    LanguageData[key] = obj;
                }

                Logger.Debug("Added custom language with key: " + key);
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

                                        Logger.Debug("Found default language.");
                                    }

                                    if (string.IsNullOrEmpty(firstKey)) firstKey = key;

                                    Logger.Debug("Added language with key: " + key);
                                    AvailableLang.Add(new LangKey() { Key = key, Chars = obj["DISPLAY_INIT"], Name = obj["DISPLAY_NAME"] });
                                }
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(SelectedDefaultKey) && !string.IsNullOrEmpty(firstKey)) {
                Logger.Debug("Default prefered language not found, using " + firstKey);

                SelectedDefault = LanguageData[firstKey];
                SelectedDefaultKey = firstKey;
                Select(SelectedDefaultKey);
            } else if (string.IsNullOrEmpty(firstKey)) {
                throw new Exception("Could not load any language pack.");
            }

#if DEBUG
            Dictionary<string, Dictionary<string, string>> missingKeys = new Dictionary<string, Dictionary<string, string>>();
            bool hasMissingKeys = false;
            foreach (var lang in LanguageData) {
                string langKey = lang.Key;
                if (langKey == "ts-TS") continue;

                foreach (var pair in SelectedDefault) {
                    if (lang.Value.ContainsKey(pair.Key)) continue;

                    Logger.Debug($"'{langKey}' Missing Key: '{pair.Key}'");
                    if (!missingKeys.ContainsKey(langKey)) missingKeys[langKey] = new Dictionary<string, string>();
                    missingKeys[langKey][pair.Key] = pair.Value;
                    hasMissingKeys = true;
                }
            }

            if (hasMissingKeys) {
                string _json = JsonConvert.SerializeObject(missingKeys, Formatting.Indented);
                File.WriteAllText("LANG_KEYS_PENDING.json", _json);
            }
#endif
        }
    }
}
