using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ToNSaveManager.Models.Stats {
    public class RoundInfoTemplate {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
        public string FileName { get; set; }

        [JsonIgnore] private string m_FilePath { get; set; } = string.Empty;
        [JsonIgnore] private string m_FileDir { get; set; } = string.Empty;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
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

                List<string> foundKeys =
                [
                    .. TemplateManager.MessageTemplatePattern.Matches(value).Select(m => {
                        return m.Value.Substring(1, m.Length - 2).ToUpperInvariant();
                    }).Where(k => !string.IsNullOrEmpty(k) && ToNStats.HasKey(k)),
                ];

                if (TemplateManager.MessageEvalPattern.IsMatch(value)) {
                    foreach (Match match in TemplateManager.MessageEvalPattern.Matches(value)) {
                        foundKeys.AddRange(ToNStats.PropertyKeys.Where(k => match.Value.Contains(k) && !foundKeys.Contains(k)));
                    }
                }

                m_TemplateKeys = foundKeys.ToArray();
                foundKeys.Clear();
            }
        }

        [JsonConstructor]
        public RoundInfoTemplate(string filePath, string template) {
            m_TemplateKeys = Array.Empty<string>();
            m_FilePath = string.Empty; // Prevent nullability warning CS8618 ... :(
            m_Template = string.Empty;
            FileName = string.Empty;
            FilePath = filePath;
            Template = template;
        }
        public RoundInfoTemplate(string template) : this(string.Empty, template) { }

        public override string ToString() {
            return FileName;
        }

        [JsonIgnore] public bool IsModified => m_TemplateKeys.Any(ToNStats.IsModified);
        [JsonIgnore] public bool HasKeys => m_TemplateKeys.Length > 0;

        internal void WriteToFile(bool force = false) {
            if (!IsModified && !force) return;

            if (!Directory.Exists(m_FileDir)) Directory.CreateDirectory(m_FileDir);

            string content = GetString();
            File.WriteAllText(FilePath, content);

            Logger.Debug($"Write File ({FileName}): {content}");
        }

        internal string GetString(bool jsOnly = false) {
            if (jsOnly) {
                return TemplateManager.EvaluateTemplate(Template) ?? string.Empty;
            }
            return TemplateManager.ReplaceTemplate(Template);
        }
    }
}
