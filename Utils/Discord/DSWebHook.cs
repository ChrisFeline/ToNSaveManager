using System.Text;
using ToNSaveManager.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ToNSaveManager.Utils.Discord
{
    using Models;
    using Localization;

    internal class Payload
    {
        [JsonProperty("embeds")]
        public Embed[] Embeds = new Embed[1] { new Embed() };

        [JsonIgnore]
        public Embed Embed => Embeds[0];
    }

    internal static class DSWebHook {
        static string LABEL_PLAYER = "**Player**: `{0}`";
	    static string LABEL_ROUND = "**Round Type**: `{0}`";
        static string LABEL_MAP = "**Map**: `{0}`";
	    static string LABEL_TERRORS = "**Terrors in Round**: `{0}`";
        static string LABEL_TERRORS_SPLIT = "`, `";
        static string LABEL_COUNT = "**Player Count**: `{0}`";
        static string LABEL_NOTE = "**Note**: `{0}`";
        internal static void LocalizeContent() {
            LABEL_PLAYER = LANG.S("SETTINGS.DISCORDWEBHOOK.LABEL_PLAYER") ?? "**Player**: `{0}`";
            LABEL_ROUND = LANG.S("SETTINGS.DISCORDWEBHOOK.LABEL_ROUND") ?? "**Round Type**: `{0}`";
            LABEL_MAP = LANG.S("SETTINGS.DISCORDWEBHOOK.LABEL_MAP") ?? "**Map**: `{0}`";
            LABEL_TERRORS = LANG.S("SETTINGS.DISCORDWEBHOOK.LABEL_TERRORS") ?? "**Terrors in Round**: `{0}`";
            LABEL_TERRORS_SPLIT = LANG.S("SETTINGS.DISCORDWEBHOOK.LABEL_TERRORS_SPLIT") ?? "**Terrors in Round**: `{0}`";
            LABEL_COUNT = LANG.S("SETTINGS.DISCORDWEBHOOK.LABEL_COUNT") ?? "**Player Count**: `{0}`";
            LABEL_NOTE = LANG.S("SETTINGS.DISCORDWEBHOOK.LABEL_NOTE") ?? "**Note**: `{0}`";
        }

        static Entry? LastEntry;

        static readonly Embed[] Embeds = new Embed[1];

        static readonly Payload PayloadData = new Payload();
        static Embed EmbedData => PayloadData.Embed;

        static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        static readonly Queue<KeyValuePair<bool, Entry>> EntryQueue = new Queue<KeyValuePair<bool, Entry>>();
        static bool IsSending = false;
        
        internal static void Send(Entry entry, bool ignoreDuplicate = false)
        {
            if (!Settings.Get.DiscordWebhookEnabled) return;

            string? webhookUrl = Settings.Get.DiscordWebhookURL;
            if (string.IsNullOrEmpty(webhookUrl)) return;

            if (!ignoreDuplicate && LastEntry != null && LastEntry.Content == entry.Content) return;
            LastEntry = entry;

            EntryQueue.Enqueue(new KeyValuePair<bool, Entry>(ignoreDuplicate, entry));
            if (IsSending) return;

            _ = Send(webhookUrl);
        }

        private static async Task Send(string webhookUrl)
        {
            if (EmbedData.Footer == null) {
                EmbedData.Footer = new EmbedFooter() {
                    IconUrl = "https://github.com/ChrisFeline/ToNSaveManager/blob/main/Resources/xs_icon.png?raw=true",
                    ProxyIconUrl = "https://github.com/ChrisFeline/ToNSaveManager/blob/main/Resources/xs_icon.png?raw=true",
                    Text = "Terrors of Nowhere: Save Manager"
                };
            }

            IsSending = true;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    while (EntryQueue.Count > 0)
                    {
                        KeyValuePair<bool, Entry> entryData = EntryQueue.Dequeue();

                        Entry entry = entryData.Value;
                        bool ignoreDuplicate = entryData.Key;

                        DateTime time = entry.Timestamp;

                        EmbedData.Description = string.Empty;
                        EmbedData.Timestamp = time;
                        EmbedData.Color = TerrorMatrix.GetRoundColorFromType(entry.RT);

                        if (entry.Parent != null && !string.IsNullOrEmpty(entry.Parent.DisplayName))
                        {
                            // if (EmbedData.Description.Length > 0) EmbedData.Description += "\n";
                            EmbedData.Description += string.Format(LABEL_PLAYER, entry.Parent.DisplayName);
                        }

                        if (entry.RT != ToNRoundType.Unknown)
                        {
                            if (EmbedData.Description.Length > 0) EmbedData.Description += "\n";
                            EmbedData.Description += string.Format(LABEL_ROUND, entry.RT);
                        }

                        if (entry.MapID > -1) {
                            if (EmbedData.Description.Length > 0) EmbedData.Description += "\n";
                            EmbedData.Description += string.Format(LABEL_MAP, ToNIndex.Instance.GetMap(entry.MapID));
                        }

                        if (entry.TD != null && entry.TD.Length > 0)
                        {
                            if (EmbedData.Description.Length > 0) EmbedData.Description += "\n";
                            EmbedData.Description += string.Format(LABEL_TERRORS, string.Join(LABEL_TERRORS_SPLIT, entry.TD.Select(ToNIndex.Instance.GetTerror)));
                        }

                        if (entry.PlayerCount > 0)
                        {
                            if (EmbedData.Description.Length > 0) EmbedData.Description += "\n";
                            EmbedData.Description += string.Format(LABEL_COUNT, entry.PlayerCount);
                        }

                        if (ignoreDuplicate && !string.IsNullOrEmpty(entry.Note)) {
                            if (EmbedData.Description.Length > 0) EmbedData.Description += "\n";
                            EmbedData.Description += string.Format(LABEL_NOTE, entry.Note.Replace('`', '\''));
                        }

                        string payloadData = JsonConvert.SerializeObject(PayloadData, JsonSettings);

                        MultipartFormDataContent form = new MultipartFormDataContent();
                        // Append file data
                        byte[] data = Encoding.Default.GetBytes(entry.Content);
                        form.Add(new ByteArrayContent(data, 0, data.Length), "Document", $"TON_BACKUP_{time.Year}_{time.Month}_{time.Day}_{time.Hour.ToString("00")}{time.Minute.ToString("00")}{time.Second.ToString("00")}.txt");
                        // Append json
                        form.Add(new StringContent(payloadData, Encoding.UTF8, "application/json"), "payload_json");
                        _ = await httpClient.PostAsync(webhookUrl, form);

                        // Wait a second before send the next message
                        if (EntryQueue.Count > 0) await Task.Delay(1000);
                    }
                }
            }
            catch (Exception) { }

            IsSending = false;
        }

        #region Validation
        private static Regex WebhookUrlRegex = new Regex(@"^.*(discord|discordapp)\.com\/api\/webhooks\/([\d]+)\/([a-z0-9_-]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        internal static bool ValidateURL(string webhookUrl)
        {
            if (string.IsNullOrWhiteSpace(webhookUrl))
                return false;

            Match match = WebhookUrlRegex.Match(webhookUrl);
            if (match != null)
            {
                // ensure that the first group is a ulong, set the _webhookId
                // 0th group is always the entire match, and 1 is the domain; so start at index 2
                if (!(match.Groups[2].Success && ulong.TryParse(match.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out ulong webhookId)) || webhookId == 0)
                    return false;

                if (!match.Groups[3].Success)
                    return false;

                return true;
            }

            return false;
        }
        #endregion
    }
}
