using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Utils.OpenRGB {
    internal struct RGBDevice {
        public enum AreaType {
            Index,
            Range
        }

        public struct Area {
            [JsonProperty("//", DefaultValueHandling = DefaultValueHandling.Ignore)] public string? Comment { get; set; }
            public AreaType Type { get; set; }
            public RGBGroupType Group { get; set; }
            public int[] Leds { get; set; } // Empty means all of them leds :3
            public bool FlashOnMidnight;
            public bool DarkOnStart;
        }

        public string Name;
        public Area[] Areas;
    }
}
