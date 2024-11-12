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
        internal static string? EvaluateTemplate(string template) {
            return JSEvaluator(MessageTemplatePattern.Replace(template, GetMatchKey));
        }

        static string GetMatchKey(Match m) {
            return m.Value.Substring(1, m.Length - 2);
        }
        static string UpdateChatboxEvaluator(Match m) {
            string key = GetMatchKey(m).ToUpperInvariant();

            if (!string.IsNullOrEmpty(key)) return ToNStats.Get(key)?.ToString() ?? m.Value;

            return m.Value;
        }

        static string JSEvaluator(Match m) {
            return JSEvaluator(m.Groups[1].Value) ?? "<js>ERROR</js>";
        }

        static string? JSEvaluator(string content) {
            try {
                return ToNStats.JSEngine.Evaluate(content).ToString();
            } catch (Exception e) {
                Logger.Error(e);
                return null;
            }
        }
    }
}
