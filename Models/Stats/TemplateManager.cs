using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToNSaveManager.Models.Stats {
    internal class TemplateManager {
        internal static readonly Regex MessageTemplatePattern = new Regex(@"{\w+}", RegexOptions.Compiled);
        internal static readonly Regex MessageGroupPattern = new Regex(@"<(\w+)>(.*)<\/\1>", RegexOptions.Compiled);

        internal static string ReplaceTemplate(string template) {
            return MessageTemplatePattern.Replace(template, UpdateChatboxEvaluator);
        }

        static string UpdateChatboxEvaluator(Match m) {
            string key = m.Value.Substring(1, m.Length - 2).ToUpperInvariant();

            if (!string.IsNullOrEmpty(key)) return ToNStats.Get(key)?.ToString() ?? m.Value;

            return m.Value;
        }
    }
}
