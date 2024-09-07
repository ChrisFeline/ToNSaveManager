using Newtonsoft.Json;

namespace ToNSaveManager.Models.Stats {
    public class RoundInfoTemplate {
        public string FileName { get; set; }

        [JsonIgnore] private string m_FilePath { get; set; } = string.Empty;
        [JsonIgnore] private string m_FileDir { get; set; } = string.Empty;
        public string FilePath {
            get => m_FilePath;
            set {
                m_FilePath = string.IsNullOrEmpty(value) ? string.Empty : Path.GetFullPath(value);
                m_FileDir = string.IsNullOrEmpty(m_FilePath) ? string.Empty : Path.GetDirectoryName(m_FilePath) ?? string.Empty;
                FileName = Path.GetFileName(m_FilePath);
            }
        }

        [JsonIgnore] private string m_Template { get; set; }
        [JsonIgnore] private string[] m_TemplateKeys { get; set; }
        public string Template {
            get => m_Template;
            set {
                m_Template = value;

                m_TemplateKeys = TemplateManager.MessageTemplatePattern.Matches(value).Select(m => {
                    return m.Value.Substring(1, m.Length - 2).ToUpperInvariant();
                }).Where(k => !string.IsNullOrEmpty(k) && ToNStats.HasKey(k)).ToArray();

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

        [JsonIgnore] public bool IsModified => m_TemplateKeys.Any(ToNStats.IsModified);
        [JsonIgnore] public bool HasKeys => m_TemplateKeys.Length > 0;

        public void WriteToFile(bool force = false) {
            if (!IsModified && !force) return;

            if (!Directory.Exists(m_FileDir)) Directory.CreateDirectory(m_FileDir);

            string content = TemplateManager.ReplaceTemplate(Template);
            File.WriteAllText(FilePath, content);

            Logger.Debug($"Write File ({FileName}): {content}");
        }
    }
}
