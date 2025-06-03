using System.Diagnostics;

namespace ToNSaveManager.Utils.JSPlugins.API.Audio {
    [JSEngineAPI("AudioPlayer")]
    internal static class AudioPlayer {
        static AudioPlaybackEngine Playback => AudioPlaybackEngine.Instance;

        static string YTDLPath = Path.Combine(Program.ProgramDirectory, "yt-dlp.exe");
        static ProcessStartInfo? YTDLInfo;
        static string? GetUrl(string url) {
            if (!File.Exists(YTDLPath)) {
                Logger.Error($"Error trying to resolve '{url}'. The binary 'yt-dlp.exe' was not found.");
                return null;
            }

            if (YTDLInfo == null) {
                YTDLInfo = new ProcessStartInfo() {
                    FileName = YTDLPath,
                    Arguments = "-q -f bestaudio --get-url " + url,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }

            using (Process process = new Process()) {
                process.StartInfo = YTDLInfo;
                process.Start();

                process.WaitForExit();

                string stdout = process.StandardOutput.ReadToEnd().Trim();

                return stdout;
            }
        }

        static Dictionary<string, AudioSource> AudioSourceList = new ();
        static AudioSource GetAudioSource() {
            string source = JSEngine.GetLastSyntaxSource();
            if (AudioSourceList.ContainsKey(source)) {
                return AudioSourceList[source];
            } else {
                return AudioSourceList[source] = new ();
            }
        }
        static AudioSource CurrentSource => GetAudioSource();

        internal class AudioSource {
            private float m_Volume = 1.0f;
            internal float Volume {
                get => m_Volume;
                set {
                    m_Volume = value;
                    if (Reader != null) Reader.volume = value;
                }
            }

            internal bool IsPlaying => Reader != null && !Reader.isDisposed;

            internal AutoDisposeFileReader? Reader;

            internal void Play(string filePath, bool fromUrl = false) {
                try {
                    Reader?.Stop();
                    Reader = Playback.PlaySound(filePath, fromUrl);
                    Reader.volume = Volume;
                } catch (Exception e) {
                    Logger.Error($"An error ocurred trying to play audio from: {filePath}\n{e}");
                }
            }
            internal void PlayYT(string url) {
                string? _url = GetUrl(url);
                if (string.IsNullOrEmpty(_url)) {
                    Reader?.Stop();
                    Logger.Error("Couldn't play url: " + url);
                    return;
                }

                Play(_url, true);
            }

            internal void Stop() {
                Reader?.Stop();
                Reader = null;
            }
        }

        public static bool IsPlaying => CurrentSource.IsPlaying;

        public static void SetVolume(float value) => CurrentSource.Volume = Math.Max(0f, Math.Min(1f, value));
        public static float GetVolume() => CurrentSource.Volume;

        public static void Play(string filePath) {
            string? fullPath = Files.ResolvePath(filePath, true);
            
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath)) {
                Console.Error($"Could not find file: {fullPath}");
                return;
            }

            Console.Log($"Playing File: {filePath} ({fullPath})");

            CurrentSource.Play(fullPath);
        }
        public static void PlayURL(string fileUrl) {
            Console.Log($"Playing URL: {fileUrl}");
            CurrentSource.Play(fileUrl, true);
        }
        public static void PlayYTDL(string url) {
            Console.Log($"Playing YTDL: {url}");
            CurrentSource.PlayYT(url);
        }

        public static void Stop() {
            string source = JSEngine.GetLastSyntaxSource();

            if (AudioSourceList.ContainsKey(source)) {
                Console.Log("Stopping audio.");
                AudioSourceList[source].Stop();
            } else {
                Console.Log("No audio currently playing.");
            }
        }
    }
}
