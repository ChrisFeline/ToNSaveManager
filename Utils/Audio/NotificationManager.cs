using System.Media;
using ToNSaveManager.Models;

namespace ToNSaveManager.Utils {
    internal static class NotificationManager {
        // Get default notification in the embeded resources
        static readonly Stream? SecretSaveStream = Program.GetEmbededResource("notification_secret.wav");
        static readonly Stream? DefaultSaveStream = Program.GetEmbededResource("notification.wav");
        static readonly Stream? DefaultCopyStream = Program.GetEmbededResource("notification_copy.wav");

        static readonly SoundPlayer StreamPlayer = new SoundPlayer();
        static readonly SoundPlayer AudioPlayer = new SoundPlayer();

        internal static void Reset() {
            StreamPlayer.Stop();
            AudioPlayer.Stop();
        }

        internal static void PlaySave() {
            if (StartPlaying(Settings.Get.AudioLocation, Random.Shared.Next(0,127) == 87 ? SecretSaveStream : DefaultSaveStream)) {
                Settings.Get.AudioLocation = null;
                Settings.Export();
            }
        }

        internal static void PlayCopy() {
            if (StartPlaying(Settings.Get.AudioCopyLocation, DefaultCopyStream)) {
                Settings.Get.AudioCopyLocation = null;
                Settings.Export();
            }
        }

        static bool StartPlaying(string? fileName, Stream? fallback) {
            if (!string.IsNullOrEmpty(fileName)) {
                try {
                    if (File.Exists(fileName)) {
                        AudioPlayer.Stop();
                        AudioPlayer.SoundLocation = fileName;
                        AudioPlayer.Play();
                        return false;
                    }

                    Logger.Warning("Custom audio location is missing: " + fileName);
                } catch (Exception ex) {
                    Logger.Error("Error trying to play custom audio.");
                    Logger.Error(ex);
                }

                return true;
            }

            if (fallback != null) {
                StreamPlayer.Stop();
                StreamPlayer.Stream = null;

                fallback.Position = 0;
                StreamPlayer.Stream = fallback;

                StreamPlayer.Play();
            }

            return false;
        }
    }
}
