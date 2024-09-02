using Newtonsoft.Json;

namespace ToNSaveManager.Models.Stats {
    public class RoundInfoTemplate {
        public string FileName { get; set; }

        [JsonIgnore] private string m_FilePath { get; set; }
        public string FilePath {
            get => m_FilePath;
            set {
                m_FilePath = value;
                FileName = Path.GetFileName(m_FilePath);
            }
        }

        [JsonIgnore] private string m_Template { get; set; }
        [JsonIgnore] private string[] m_TemplateKeys { get; set; }
        public string Template {
            get => m_Template;
            set {
                m_Template = value;

                m_TemplateKeys = StatsWindow.MessageTemplatePattern.Matches(value).Select(m => {
                    string key = m.Value.Substring(1, m.Length - 2).ToUpperInvariant();
                    bool isLobby = key.StartsWith(StatsWindow.LOBBY_PREFIX, StringComparison.OrdinalIgnoreCase);
                    if (isLobby) key = key.Substring(StatsWindow.LOBBY_PREFIX.Length);
                    return key;
                }).Where(k => !string.IsNullOrEmpty(k) && StatsData.TableDictionary.ContainsKey(k)).ToArray();

                Logger.Debug("Template Keys: " + string.Join(", ", m_TemplateKeys));
            }
        }

        public RoundInfoTemplate(string filePath, string template) {
            m_TemplateKeys = Array.Empty<string>();
            m_FilePath = string.Empty; // Prevent nullability warning CS8618 ... :(
            m_Template = string.Empty;
            FileName = string.Empty;
            FilePath = filePath;
            Template = template;
        }

        public override string ToString() {
            return FileName;
        }

        [JsonIgnore] public bool IsModified => m_TemplateKeys.Any(StatsData.IsModified);

        public void WriteToFile() {
            if (!IsModified) return;

            string content = StatsWindow.ReplaceTemplate(Template);
            File.WriteAllText(FilePath, content);

            Logger.Debug($"Write File ({FileName}): {content}");
        }
    }
}
