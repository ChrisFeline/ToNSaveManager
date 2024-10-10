using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Utils.Audio;
using ToNSaveManager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ToNSaveManager.Utils {
    internal static class NotificationManager {
        // Get default notification in the embeded resources
        static readonly Stream? SecretSaveStream = Program.GetEmbededResource("notification_secret.wav");
        static readonly Stream? DefaultSaveStream = Program.GetEmbededResource("notification.wav");
        static readonly Stream? DefaultCopyStream = Program.GetEmbededResource("notification_copy.wav");

        static CachedSound? SaveDefault { get; set; }
        static CachedSound? CopyDefault { get; set; }

        static NotificationManager() {
            if (DefaultSaveStream != null)
                SaveDefault = new CachedSound(DefaultSaveStream);

            if (DefaultCopyStream != null)
                CopyDefault = new CachedSound(DefaultCopyStream);
        }

        internal static void PlaySave() {
            if (StartPlaying(Settings.Get.AudioLocation, SaveDefault)) {
                Settings.Get.AudioLocation = null;
                Settings.Export();
            }
        }

        internal static void PlayCopy() {
            if (StartPlaying(Settings.Get.AudioCopyLocation, CopyDefault)) {
                Settings.Get.AudioCopyLocation = null;
                Settings.Export();
            }
        }

        static bool StartPlaying(string? fileName, CachedSound? fallback) {
            if (!string.IsNullOrEmpty(fileName)) {
                try {
                    if (File.Exists(fileName)) {
                        AudioPlaybackEngine.Instance.PlaySound(fileName);
                        return false;
                    }

                    Logger.Warning("Custom audio location is missing: " + fileName);
                } catch (Exception ex) {
                    Logger.Error("Error trying to play custom audio.");
                    Logger.Error(ex);
                }

                return true;
            }

            if (fallback != null)
                AudioPlaybackEngine.Instance.PlaySound(fallback);

            return false;
        }
    }

    internal class AudioPlaybackEngine : IDisposable {
        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);

        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2) {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public void PlaySound(string fileName) {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input) {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels) {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2) {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void PlaySound(CachedSound sound) {
            AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        private void AddMixerInput(ISampleProvider input) {
            mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        public void Dispose() {
            outputDevice.Dispose();
        }
    }
}
