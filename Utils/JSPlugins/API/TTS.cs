using System.Speech.Synthesis;

namespace ToNSaveManager.Utils.JSPlugins.API {
    [JSEngineAPI("TTS")]
    internal static class TTS {
        static Dictionary<string, SpeechSource> AudioSourceList = new();
        static SpeechSource GetSpeechSource() {
            string source = JSEngine.GetLastSyntaxSource();
            if (AudioSourceList.ContainsKey(source)) {
                return AudioSourceList[source];
            } else {
                return AudioSourceList[source] = new();
            }
        }
        static SpeechSource CurrentSource => GetSpeechSource();

        internal class SpeechSource {
            private int m_Volume = 100;
            internal int Volume {
                get => m_Volume;
                set {
                    m_Volume = value;
                    if (Synth != null) Synth.Volume = value;
                }
            }

            internal bool IsSpeaking => Synth != null && LastPrompt != null && !LastPrompt.IsCompleted;

            internal SpeechSynthesizer? Synth;
            internal Prompt? LastPrompt;

            internal void Speak(string prompt) {
                if (Synth == null) {
                    Synth = new SpeechSynthesizer();
                    Synth.SetOutputToDefaultAudioDevice();
                    Synth.Volume = m_Volume;
                }

                Stop();
                LastPrompt = Synth.SpeakAsync(prompt);
            }

            internal void Stop() {
                LastPrompt = null;
                Synth?.SpeakAsyncCancelAll();
            }
        }

        public static bool IsSpeaking => CurrentSource.IsSpeaking;

        public static void SetVolume(int value) => CurrentSource.Volume = Math.Max(0, Math.Min(100, value));
        public static float GetVolume() => CurrentSource.Volume;

        public static void Speak(string content) {
            if (string.IsNullOrEmpty(content)) return;
            Console.Log("TTS: " + content);
            CurrentSource.Speak(content);
        }

        public static void Stop() {
            CurrentSource.Stop();
        }
    }
}
