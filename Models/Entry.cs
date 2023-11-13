using Newtonsoft.Json;
using System.Text;

namespace ToNSaveManager.Models
{
    internal struct EntryDate
    {
        const string Date_MM_DD = "MM/dd/yyyy";
        const string DateFormat_DD_MM = "dd/MM/yyyy";

        const string Time_24H_S = "HH:mm:ss";
        const string Time_12H_S = "hh:mm:ss tt";
        const string Time_24H = "HH:mm";
        const string Time_12H = "hh:mm tt";

        internal static string GetDateFormat()
        {
            bool use24Hour = Settings.Get.Use24Hour;
            bool invertMD = Settings.Get.InvertMD;
            bool showSeconds = Settings.Get.ShowSeconds;
            return (invertMD ? DateFormat_DD_MM : Date_MM_DD) + " | " +
                // Append Time
                (use24Hour ?
                (showSeconds ? Time_24H_S : Time_24H) :
                (showSeconds ? Time_12H_S : Time_12H));
        }
    }

    internal class Entry
    {
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
            sb.Append(Timestamp.ToString(EntryDate.GetDateFormat()));
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
