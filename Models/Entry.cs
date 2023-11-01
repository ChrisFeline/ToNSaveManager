using Newtonsoft.Json;
using System.Text;

namespace ToNSaveManager.Models
{
    internal class Entry
    {
        internal const string DateFormat = "MM/dd/yyyy | HH:mm:ss";

        public string Note = string.Empty;

        public DateTime Timestamp;
        public string Content;
        public string? Players;
        [JsonIgnore] public bool Fresh;
        [JsonIgnore] public int Length => Content.Length;

        public Entry(string content, DateTime timestamp)
        {
            Fresh = true;
            Content = content;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Timestamp.ToString(DateFormat));
            if (!string.IsNullOrEmpty(Note))
            {
                sb.Append(" | ");
                sb.Append(Note);
            }
            return sb.ToString();
        }

        public string GetTooltip(bool full)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Timestamp.ToString("F"));
            if (!string.IsNullOrEmpty(Note))
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append("Note: \n- ");
                sb.Append(Note);
            }
            if (full && !string.IsNullOrEmpty(Players))
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Players in room:");
                sb.Append(Players);
            }
            return sb.ToString();
        }

        public void CopyToClipboard()
        {
            Clipboard.SetText(Content);
        }
    }
}
