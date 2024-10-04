using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Utils.OpenRGB {
    internal struct RGBDevice {
        [JsonProperty("//", DefaultValueHandling = DefaultValueHandling.Ignore)] public string? Comment { get; set; }

        public enum AreaType {
            Index,
            Range
        }

        public struct Area {
            [JsonProperty("//", DefaultValueHandling = DefaultValueHandling.Ignore)] public string? Comment { get; set; }
            public RGBGroupType Group { get; set; }
            public int[] Leds { get; set; } // Empty means all of them leds :3
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool UseRange { get; set; }
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool FlashOnMidnight;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public bool DarkOnStart;
        }

        public int DeviceID;
        public Area[] Areas;
    }
}
