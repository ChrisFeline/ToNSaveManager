using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Utils.OpenRGB {
    internal class RGBProfile {
        internal const string FileName = "OpenRGB_Setup.json";
        internal const string GeneratedFileName = "openrgb_device_keys.json";

        public string IP = "127.0.0.1";
        public int Port = 6742;
        public int FPS = 60;

        public List<RGBDevice> Entries = new List<RGBDevice>() {
            new RGBDevice() {
                Comment = $"This section defines a device, set the name to your device name found on the generated '{GeneratedFileName}' file",
                Name = "SteelSeries Apex 3",
                Areas = [
                    new RGBDevice.Area {
                        Comment = "Leave 'Leds' empty to colorize all the Leds on the device. - 'Dark On Start' turns the LEDs off when the round is about to start.",
                        Group = RGBGroupType.Terror,
                        //Leds = [ 0, 1, 2, 3, 4, 5, 6, 7 ]
                        Leds = [  ], // Empty for full keyboard
                        DarkOnStart = true
                    },
                    new RGBDevice.Area {
                        Comment = $"Each led ID number can be found on the generated file '{GeneratedFileName}'",
                        Group = RGBGroupType.Round,
                        Leds = [ 8, 9 ],
                        FlashOnMidnight = true
                    },
                ]
            },
            new RGBDevice() {
                Comment = "You can define multiple devices",
                Name = "G502 HERO Gaming Mouse",
                Areas = [
                    new RGBDevice.Area {
                        Comment = "'Flash On Midnight' will make the LEDs blink when a midnight round is taking place.",
                        Group = RGBGroupType.Round,
                        Leds = [ 0, 1 ],
                        FlashOnMidnight = true
                    }
                ]
            }
        };

        internal void Export() {
            File.WriteAllText(FileName, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        internal static RGBProfile Import() {
            RGBProfile? profile = null;

            try {
                if (File.Exists(FileName)) {
                    string content = File.ReadAllText(FileName);
                    profile = JsonConvert.DeserializeObject<RGBProfile>(content);
                }

                if (profile == null) {
                    profile = new RGBProfile();
                    profile.Export();
                }
            } catch (Exception e) {
                Logger.Error(e);
                profile = new RGBProfile();
            }

            return profile;
        }
    }
}
