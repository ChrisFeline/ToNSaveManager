﻿using OpenRGB.NET;
using RGBColor = OpenRGB.NET.Color;
using Color = System.Drawing.Color;
using ToNSaveManager.Models.Index;
using Newtonsoft.Json;
using ToNSaveManager.Utils.Extensions;
using ToNSaveManager.Models;
using System.Drawing.Drawing2D;

namespace ToNSaveManager.Utils.OpenRGB
{
    internal class RGBProfile {
        const string FileName = "OpenRGB_Setup.json";
        internal const string GeneratedFileName = "openrgb_device_keys.json";

        public string IP = "127.0.0.1";
        public int Port = 6742;
        public int FPS = 60;

        public List<RGBDevice> Entries = new List<RGBDevice>() {
            new RGBDevice() {
                Name = "SteelSeries Apex 3",
                Areas = [
                    new RGBDevice.Area {
                        Comment = "Leave 'Leds' empty to colorize all the Leds on the device. - 'Dark On Starts' turns the LEDs off when the round is about to start.",
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

    internal static class OpenRGBControl
    {
        static LoggerSource Log = new LoggerSource("OpenRGB");

        static OpenRgbClient? Client { get; set; }
        static Device[]? Devices { get; set; }

        static readonly RGBProfile Profile = RGBProfile.Import();

        static RGBAnimation AnimTerror = new RGBAnimation();
        static RGBAnimation AnimRound = new RGBAnimation();
        static RGBAnimation AnimAlive = new RGBAnimation();
        static readonly RGBAnimation[] AnimGroups = [AnimTerror, AnimRound, AnimAlive];

        static Task? UpdateTask;

        internal static void StartUpdateLoop()
        {
            DateTime currentFrame, previousFrame = DateTime.Now;
            int interval = 1000 / Profile.FPS;

            while (Client != null && Client.Connected && Devices != null)
            {
                // Game Loop Logic
                currentFrame = DateTime.Now;
                // Delta time
                float elapsed = (float)(currentFrame - previousFrame).TotalSeconds;

                bool isMidnight = Terrors.TerrorCount > 0 && Terrors.RoundType == ToNRoundType.Midnight;
                bool shouldUpdate = isMidnight;
                for (int i = 0; i < AnimGroups.Length; i++)
                {
                    if (AnimGroups[i].Update(3, elapsed)) shouldUpdate = true;
                }

                if (shouldUpdate)
                {
                    for (int i = 0; i < Devices.Length; i++)
                    {
                        Device device = Devices[i];
                        RGBColor[] colors = device.Colors;

                        bool dirty = false;
                        for (int j = 0; j < Profile.Entries.Count; j++)
                        {
                            RGBDevice entry = Profile.Entries[j];
                            if (!device.Name.Equals(entry.Name)) continue;

                            for (int a = 0; a < entry.Areas.Length; a++)
                            {
                                RGBDevice.Area area = entry.Areas[a];
                                bool flashMidnight = area.FlashOnMidnight && isMidnight;
                                var animGroup = AnimGroups[(int)area.Group];
                                if (animGroup.Refresh || flashMidnight) {
                                    RGBColor color = flashMidnight || (area.DarkOnStart && Terrors.TerrorCount == 0 && Terrors.RoundType != ToNRoundType.Intermission) ? animGroup.Flashy.ToRGBColor() : animGroup.Value.ToRGBColor();
                                    int len = area.Leds.Length;

                                    if (len == 0) {
                                        Array.Fill(colors, color);
                                    } else if (area.Type == RGBDevice.AreaType.Range) {
                                        len = len - 1;
                                        for (int l = 0; l < len; l += 2) {
                                            colors.FillRange(color, area.Leds[l], area.Leds[l + 1]);
                                        }
                                    } else {
                                        for (int l = 0; l < len; l++) {
                                            colors.SetSafe(color, area.Leds[l]);
                                        }
                                    }

                                    dirty = true;
                                }
                            }
                        }

                        if (dirty) Client.UpdateLeds(i, colors);
                    }
                }

                previousFrame = currentFrame;
                Thread.Sleep(interval);
            }

            UpdateTask = null;
        }

        internal static void DeInitialize() {
            if (Client == null) return;

            try {
                Client.Dispose();
                Client = null;
            } catch (Exception e) {
                Log.Error("Error trying to dispose OpenRGB Client: " + e);
            }
        }

        internal static void Initialize()
        {
            if (Client == null) Client = new OpenRgbClient(Profile.IP, Profile.Port, autoConnect: false);

            if (!Client.Connected && Client.TryConnect())
            {
                Devices = Client.GetAllControllerData();
                Dictionary<string, Dictionary<int, string>> rgbKeys = new Dictionary<string, Dictionary<int, string>>();

                Log.Debug("Found devices:");
                foreach (var device in Devices)
                {
                    Log.Debug(device.Name);

                    Dictionary<int, string> keys = new Dictionary<int, string>();
                    var leds = device.Leds;
                    for (int i = 0; i < leds.Length; i++)
                    {
                        string name = leds[i].Name;
                        if (!string.IsNullOrEmpty(name)) keys[i] = name;
                    }

                    rgbKeys[device.Name] = keys;
                }

                File.WriteAllText(RGBProfile.GeneratedFileName, JsonConvert.SerializeObject(rgbKeys, Formatting.Indented));

                UpdateColorAnimation();
                if (UpdateTask == null) UpdateTask = Task.Run(StartUpdateLoop);
            }
        }

        static TerrorMatrix Terrors = TerrorMatrix.Empty;
        internal static void UpdateColorAnimation() {
            AnimTerror.SetTarget(Terrors.DisplayColor);
            AnimRound.SetTarget(Terrors.RoundColor);
        }
        internal static void SetTerrorMatrix(TerrorMatrix matrix)
        {
            Terrors = matrix;
            if (!MainWindow.Started || !Settings.Get.OpenRGBEnabled) return;

            Initialize();

            if (Client != null && Client.Connected) {
                try {
                    Devices = Client.GetAllControllerData();
                } catch {
                    Log.Error("Could not get controller data.");
                    Devices = null;
                }
            }

            UpdateColorAnimation();
        }
    }
}