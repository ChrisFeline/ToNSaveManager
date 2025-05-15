namespace ToNSaveManager.Utils.JSPlugins.API.Audio {
    [JSEngineAPI("AudioPlayer")]
    internal static class AudioPlayer {
        static AudioPlaybackEngine Playback => AudioPlaybackEngine.Instance;

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

            internal void Play(string filePath) {
                Reader?.Stop();
                Reader = Playback.PlaySound(filePath);
                Reader.volume = Volume;
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

            Console.Log($"Playing: {filePath} ({fullPath})");

            CurrentSource.Play(fullPath);
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
