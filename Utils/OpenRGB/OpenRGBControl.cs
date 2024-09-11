using OpenRGB.NET;
using RGBColor = OpenRGB.NET.Color;
using Color = System.Drawing.Color;
using ToNSaveManager.Models.Index;
using Newtonsoft.Json;
using ToNSaveManager.Utils.Extensions;
using ToNSaveManager.Models;
using System.Diagnostics;

namespace ToNSaveManager.Utils.OpenRGB
{
    internal static class OpenRGBControl
    {
        static LoggerSource Log = new LoggerSource("OpenRGB");

        static OpenRgbClient? Client { get; set; }
        static Device[]? Devices { get; set; }

        static RGBProfile Profile => RGBProfile.Instance;

        static RGBAnimation AnimTerror = new RGBAnimation(RGBGroupType.Terror);
        static RGBAnimation AnimRound = new RGBAnimation(RGBGroupType.Round);
        static RGBAnimation AnimAlive = new RGBAnimation(RGBGroupType.Living);
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

                bool isMidnight = Terrors.TerrorCount > 0 && (Terrors.RoundType == ToNRoundType.Midnight || Terrors.IsSaboteur);
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
                        for (int j = 0; j < Profile.Entries.Length; j++)
                        {
                            RGBDevice entry = Profile.Entries[j];
                            if (!device.Index.Equals(entry.DeviceID)) continue;

                            for (int a = 0; a < entry.Areas.Length; a++)
                            {
                                RGBDevice.Area area = entry.Areas[a];
                                bool flashMidnight = (area.FlashOnMidnight && isMidnight) || (Terrors.IsSaboteur && Terrors.TerrorCount > 0);
                                var animGroup = AnimGroups[(int)area.Group];
                                if (animGroup.Refresh || flashMidnight) {
                                    RGBColor color = flashMidnight || (area.DarkOnStart && Terrors.TerrorCount == 0 && Terrors.RoundType != ToNRoundType.Intermission) ? animGroup.Flashy.ToRGBColor() : animGroup.Value.ToRGBColor();
                                    int len = area.Leds.Length;

                                    if (len == 0) {
                                        Array.Fill(colors, color);
                                    } else if (area.UseRange) {
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
                Dictionary<string, Dictionary<int, string>> rgbKeys = new ();

                Log.Debug("Found devices:");
                foreach (var device in Devices)
                {
                    string nameKey = $"ID: '{device.Index}'  |  NAME: '{device.Name}'";

                    Log.Debug(" - " + nameKey);

                    Dictionary<int, string> keys = new ();

                    var leds = device.Leds;
                    for (int i = 0; i < leds.Length; i++)
                    {
                        string name = leds[i].Name;
                        if (!string.IsNullOrEmpty(name)) keys[i] = name;
                    }

                    rgbKeys[nameKey] = keys;
                }

                File.WriteAllText(RGBProfile.GeneratedFileName, JsonConvert.SerializeObject(rgbKeys, Formatting.Indented));

                UpdateColorAnimation();
                SetIsAlive();
                // SetDamaged();
                if (UpdateTask == null) UpdateTask = Task.Run(StartUpdateLoop);
            }
        }

        internal static TerrorMatrix Terrors = TerrorMatrix.Empty;
        internal static void UpdateColorAnimation() {
            AnimTerror.SetTarget(Terrors.DisplayColor);
            AnimRound.SetTarget(Terrors.RoundColor);
            if (Terrors.IsSaboteur) {
                AnimTerror.SetTarget(Color.Red);
                AnimRound.SetTarget(Color.Red);
            }
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

        internal static void SetIsAlive() {
            Log.Debug("Is Alive: " + ToNGameState.IsAlive);
            AnimAlive.SetTarget(ToNGameState.IsAlive ? Color.Green : Color.Red);
        }
        internal static void SetDamaged() {
            AnimAlive.Value = new RGBTemp( Color.Red );
            AnimAlive.SetTarget(Color.Green);
        }
    }
}
