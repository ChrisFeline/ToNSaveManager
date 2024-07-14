using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Models;
using Newtonsoft.Json;

namespace ToNSaveManager.Utils.Discord
{
    using Models;
    using System.Diagnostics;

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
    }
}
