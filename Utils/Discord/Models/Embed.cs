using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Utils.Discord.Models
{
    internal class Embed
    {
        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("color")]
        public uint Color { get; set; } = 16711680;

        [JsonProperty("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonProperty("footer")]
        public EmbedFooter? Footer { get; set; }

        /*
        [JsonProperty("author")]
        public EmbedAuthor Author { get; set; }

        [JsonProperty("video")]
        public EmbedVideo Video { get; set; }

        [JsonProperty("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }

        [JsonProperty("image")]
        public EmbedImage Image { get; set; }

        [JsonProperty("provider")]
        public EmbedProvider Provider { get; set; }

        [JsonProperty("fields")]
        public EmbedField[] Fields { get; set; }
        */
    }
}
