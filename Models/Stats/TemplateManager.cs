using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToNSaveManager.Models.Stats {
    internal class TemplateManager {
        internal static readonly Regex MessageTemplatePattern = new Regex(@"{\w+}", RegexOptions.Compiled);
        internal static readonly Regex MessageConditionalPattern = new Regex(@"\({(?<k>\w+)\}(?<c>(?:==|!=|>=|<=|>|<).+)?\?(?<a>.*):(?<b>.*)\)", RegexOptions.Compiled);
        internal static readonly Regex MessageConditionPatter = new Regex(@"{(?<k>\w+)\}(?<c>(?:==|!=|>=|<=|>|<).+)?", RegexOptions.Compiled);

        static readonly Dictionary<int, string> SubGroups = new Dictionary<int, string>();

        internal static string ReplaceTemplate(string template) {
            return MessageTemplatePattern.Replace(MessageConditionalPattern.Replace(template, UpdateConditionEvaluator), UpdateChatboxEvaluator);
        }

        static readonly string[] Operators = [
            "==", // 0
            ">=", // 1
            "<=", // 2
            "!=", // 3
            ">",  // 4
            "<",  // 5
        ];

        const char SYMBOL_KEY_O = '{';
        const char SYMBOL_KEY_C = '}';
        const char SYMBOL_OPEN  = '(';
        const char SYMBOL_CLOSE = ')';
        const char SYMBOL_PRE   = '?';
        const char SYMBOL_SPLIT = ':';

        static bool GetConditionValue(string key, string val_a, string val_b, out string output) {
            Match m = MessageConditionPatter.Match(key);
            key = m.Groups["k"].Value;

            if (!ToNStats.HasKey(key)) {
                output = string.Empty;
                return false;
            }

            object? value = ToNStats.Get(key);
            bool isNumber = ToNStats.GetType(key) == typeof(int);

            int operatorId = 0; // equals by default
            object? other = "True";

            string customOp = m.Groups["c"].Value;
            if (!string.IsNullOrEmpty(customOp)) {
                string op = new string(customOp.TakeWhile(c => !Char.IsDigit(c)).ToArray());
                operatorId = Array.IndexOf(Operators, op);
                if (operatorId < 0) operatorId = 0;

                other = customOp.Substring(Operators[operatorId].Length);
                if (isNumber && int.TryParse(other.ToString(), out int v)) {
                    other = v;
                } else if (isNumber) other = 0;
            } else if (isNumber) other = 0;

            bool result = false;
            if (isNumber) {
                int c = ((int)(value ?? 0)).CompareTo(other);

                switch (operatorId) {
                    default:
                    case 0: result = c == 0; break;
                    case 1: result = c >= 0; break;
                    case 2: result = c <= 0; break;
                    case 3: result = c != 0; break;
                    case 4: result = c > 0; break;
                    case 5: result = c < 0; break;
                }
            } else {
                result = string.Compare(value?.ToString(), other?.ToString()) == 0;
                if (operatorId > 0) result = !result;
            }

            output = ReplaceTemplate(result ? val_a : val_b);
            return true;
        }

        static string UpdateConditionEvaluator(Match m) {
            string groupString = m.Value;


            StringBuilder sb = new StringBuilder();

            int pre = -1;
            int spl = -1;

            int depth = 0;
            int i_key = -1;
            int opn = 0;

            StringBuilder sb_key = new StringBuilder();
            StringBuilder sb_opt = new StringBuilder();
            StringBuilder sb_val_a = new StringBuilder();
            StringBuilder sb_val_b = new StringBuilder();
            string opt = string.Empty; // Operator

            for (int i = 0; i < groupString.Length; i++) {
                char c = groupString[i];

                switch (c) {
                    case SYMBOL_KEY_C:
                    case SYMBOL_KEY_O:
                        if (depth == 1 && pre < 0) {
                            i_key = c == SYMBOL_KEY_O ? i : -1;
                        }
                        break;
                    case SYMBOL_OPEN:
                        depth++;
                        if (depth == 1) opn = i;
                        break;
                    case SYMBOL_CLOSE:
                        depth--;
                        if (depth == 0) {
                            if (GetConditionValue(sb_key.ToString(), sb_val_a.ToString(), sb_val_b.ToString(), out string output))
                                sb.Append(output);

                            pre = -1;
                            spl = -1;
                            i_key = -1;

                            sb_key.Clear();
                            sb_opt.Clear();
                            sb_val_a.Clear();
                            sb_val_b.Clear();
                            Logger.Debug("APPENDING: " + output);
                            continue;
                        }
                        break;
                    case SYMBOL_PRE:
                        if (depth == 1 && pre < 0) pre = i;
                        break;
                    case SYMBOL_SPLIT:
                        if (depth == 1 && pre > 0 && spl < 0) spl = i;
                        break;

                    default:
                        break;
                }

                if (depth == 0) {
                    sb.Append(c);
                    continue;
                }

                if (depth == 1 && pre < 0 && i > opn) {
                    sb_key.Append(c);
                } else if (pre > 0 && spl < 0 && i > pre) {
                    sb_val_a.Append(c);
                } else if (spl > 0 && i > spl) {
                    sb_val_b.Append(c);
                }
            }

            return sb.ToString();
        }

        static string UpdateChatboxEvaluator(Match m) {
            string key = m.Value.Substring(1, m.Length - 2).ToUpperInvariant();

            if (!string.IsNullOrEmpty(key)) return ToNStats.Get(key)?.ToString() ?? m.Value;

            return m.Value;
        }
    }
}
