using Jint;
using System.Text;
using System.Text.RegularExpressions;

namespace ToNSaveManager.Models.Stats {
    internal class TemplateManager {
        internal static readonly Regex MessageEvalPattern = new Regex(@"<js>(.+?)<\/js>", RegexOptions.Compiled);
        internal static readonly Regex MessageTemplatePattern = new Regex(@"{\w+}", RegexOptions.Compiled);

        internal static string ReplaceTemplate(string template) {
            return MessageEvalPattern.Replace(MessageTemplatePattern.Replace(template, UpdateChatboxEvaluator), JSEvaluator);
        }

        static string UpdateChatboxEvaluator(Match m) {
            string key = m.Value.Substring(1, m.Length - 2).ToUpperInvariant();

            if (!string.IsNullOrEmpty(key)) return ToNStats.Get(key)?.ToString() ?? m.Value;

            return m.Value;
        }

        static string JSEvaluator(Match m) {
            string content = m.Groups[1].Value;

            try {
                return ToNStats.JSEngine.Evaluate(content).ToString();
            } catch (Exception) {
                return "<js>ERROR</js>";
            }
        }
    }
}
