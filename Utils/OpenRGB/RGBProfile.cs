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
                Comment = $"This section defines a device, set the 'DeviceID' property to your device id number found on the generated '{GeneratedFileName}' file",
                DeviceID = 1,
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
                    new RGBDevice.Area {
                        Comment = "Set the 'UseRange' property to 'true' if you want to choose a range of led IDs, for example if you want to select from id '0' to id '7', set the Leds property to [0,7]",
                        UseRange = true,
                        Group = RGBGroupType.Terror,
                        //Leds = [ 0, 1, 2, 3, 4, 5, 6, 7 ]
                        Leds = [ 0, 7 ],
                        DarkOnStart = true
                    },
                ]
            },
            new RGBDevice() {
                Comment = "You can define multiple devices",
                DeviceID = 2,
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
