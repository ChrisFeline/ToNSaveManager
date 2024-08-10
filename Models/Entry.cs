using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using ToNSaveManager.Localization;
using ToNSaveManager.Models.Index;

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

        internal static string GetDateFormat(bool isEntry = false)
        {
            bool use24Hour = Settings.Get.Use24Hour;
            bool invertMD = Settings.Get.InvertMD;
            bool showSeconds = Settings.Get.ShowSeconds;
            // Sometimes I get nightmares too
            return (!isEntry || Settings.Get.ShowDate ? (invertMD ? DateFormat_DD_MM : Date_MM_DD) + " | " : string.Empty) +
                // Append Time
                (use24Hour ?
                (showSeconds ? Time_24H_S : Time_24H) :
                (showSeconds ? Time_12H_S : Time_12H));
        }
    }

    internal class Entry {
        static string TextNote = "Note:";
        static string TextRound = "Round Type:";
        static string TextTerrors = "Terrors in round:";
        static string TextPlayers = "Players in room:";
        static string TextMap = "Map:";

        internal static void LocalizeContent() {
            TextNote = LANG.S("MAIN.ENTRY_NOTE") ?? "Note:";
            TextRound = LANG.S("MAIN.ENTRY_ROUND") ?? "Round Type:";
            TextTerrors = LANG.S("MAIN.ENTRY_TERRORS") ?? "Terrors in round:";
            TextPlayers = LANG.S("MAIN.ENTRY_PLAYERS") ?? "Players in room:";
            TextMap = LANG.S("MAIN.ENTRY_MAP") ?? "Map:";
        }

        public string Note = string.Empty;

        public DateTime Timestamp;
        public string Content;

        [JsonProperty("pc")]
        public int PlayerCount;
        public string? Players;

        // Obsolete
        [Obsolete] public string[]? RTerrors { get; set; }
        [Obsolete] public string? RType { get; set; }
        // Obsolete

        public ToNRoundType RT;
        public ToNIndex.TerrorInfo[]? TD;
        public int MapID = -1;

        public ToNRoundResult RResult;

        [JsonIgnore] public History? Parent;
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
            const string separator = " | ";
            StringBuilder sb = new StringBuilder();

            if (Settings.Get.SaveRoundInfo && Settings.Get.ShowWinLose)
            {
                sb.Append('[');
                sb.Append(RResult);
                sb.Append("] ");
            }

            sb.Append(Timestamp.ToString(EntryDate.GetDateFormat(true)));

            if (!string.IsNullOrEmpty(Note))
            {
                sb.Append(separator);
                sb.Append(Note);
            }

            return sb.ToString();
        }

        public string GetTooltip(bool showPlayers, bool showTerrors, bool showNote = true, bool showMap = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Timestamp.ToString("F"));
            if (!string.IsNullOrEmpty(Note) && showNote)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(TextNote + " \n- ");
                sb.Append(Note);
            }
            if (showTerrors && RResult != ToNRoundResult.R)
            {
                sb.AppendLine();
                sb.AppendLine();

                // sb.AppendLine("Round info: " + (RResult == ToNRoundResult.W ? "Survived" : "Died"));

#pragma warning disable CS0612 // Type or member is obsolete
                if (!string.IsNullOrEmpty(RType)) sb.AppendLine(TextRound + " " + RType);
                else sb.AppendLine(TextRound + " " + RT.ToString());
                
                if (TD != null && TD.Length > 0) {
                    sb.Append(TextTerrors);
                    for (int i = 0; i < TD.Length; i++) {
                        var t = TD[i];
                        sb.AppendLine();
                        sb.Append("- " + ToNIndex.Instance.GetTerror(t));
                    }
                } else if (RTerrors != null && RTerrors.Length > 0) {
                    sb.AppendLine(TextTerrors);
                    sb.AppendJoin("- ", RTerrors);
                }
#pragma warning restore CS0612 // Type or member is obsolete
            }
            if (showMap && MapID > -1) {
                sb.AppendLine();
                sb.AppendLine();

                var map = ToNIndex.Instance.GetMap(MapID);
                if (!map.IsEmpty) sb.AppendLine(TextMap + " " + map);
            }
            if (showPlayers && !string.IsNullOrEmpty(Players))
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(TextPlayers);
                sb.Append(Players);
            }
            return sb.ToString();
        }

        public void CopyToClipboard()
        {
#if DEBUG
            Debug.WriteLine("COPYING TO CLIPBOARD");
#endif
            // Windows 11 please... :[
            // Clipboard.Clear();
            Clipboard.SetDataObject(Content, false, 4, 200);
            // Clipboard.SetText(Content);
        }
    }
}
