﻿using System.Text;
using ToNSaveManager.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ToNSaveManager.Utils.Discord
{
    using Models;

    internal class Payload
    {
        [JsonProperty("embeds")]
        public Embed[] Embeds = new Embed[1] { new Embed() };

        [JsonIgnore]
        public Embed Embed => Embeds[0];
    }

    internal static class DSWebHook
    {
        static Entry? LastEntry;

        static readonly Embed[] Embeds = new Embed[1];

        static readonly Payload PayloadData = new Payload();
        static Embed EmbedData => PayloadData.Embed;

        static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        static readonly Queue<Entry> EntryQueue = new Queue<Entry>();
        static bool IsSending = false;
        
        internal static void Send(Entry entry, bool ignoreDuplicate = false)
        {
            if (!Settings.Get.DiscordWebhookEnabled) return;

            string? webhookUrl = Settings.Get.DiscordWebhookURL;
            if (string.IsNullOrEmpty(webhookUrl)) return;

            if (!ignoreDuplicate && LastEntry != null && LastEntry.Content == entry.Content) return;
            LastEntry = entry;

            EntryQueue.Enqueue(entry);
            if (IsSending) return;

            _ = Send(webhookUrl);
        }

        private static async Task Send(string webhookUrl)
        {
            IsSending = true;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    while (EntryQueue.Count > 0)
                    {
                        Entry entry = EntryQueue.Dequeue();
                        DateTime now = DateTime.Now;

                        if (EmbedData.Footer == null)
                        {
                            EmbedData.Footer = new EmbedFooter()
                            {
                                IconUrl = "https://github.com/ChrisFeline/ToNSaveManager/blob/main/Resources/xs_icon.png?raw=true",
                                ProxyIconUrl = "https://github.com/ChrisFeline/ToNSaveManager/blob/main/Resources/xs_icon.png?raw=true",
                                Text = "Terrors of Nowhere: Save Manager"
                            };
                        }

                        EmbedData.Description = string.Empty;
                        EmbedData.Timestamp = now;

                        if (!string.IsNullOrEmpty(entry.RType))
                            EmbedData.Description += "**Round Type**: `" + entry.RType + "`";

                        if (entry.RTerrors != null && entry.RTerrors.Length > 0)
                        {
                            if (EmbedData.Description.Length > 0) EmbedData.Description += "\n";
                            EmbedData.Description += "**Terrors in Round**: `" + string.Join("` `", entry.RTerrors) + "`";
                        }

                        if (entry.PlayerCount > 0)
                        {
                            if (EmbedData.Description.Length > 0) EmbedData.Description += "\n";
                            EmbedData.Description += $"**Player Count**: `{entry.PlayerCount}`";
                        }

                        string payloadData = JsonConvert.SerializeObject(PayloadData, JsonSettings);

                        MultipartFormDataContent form = new MultipartFormDataContent();
                        // Append file data
                        byte[] data = Encoding.Default.GetBytes(entry.Content);
                        form.Add(new ByteArrayContent(data, 0, data.Length), "Document", $"TON_BACKUP_{now.Year}_{now.Month}_{now.Day}.txt");
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
