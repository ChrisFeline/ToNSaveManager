﻿using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace ToNSaveManager.Utils.JSPlugins.API.Audio {
    class AudioPlaybackEngine : IDisposable {
        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mixer;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2) {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public AutoDisposeFileReader PlaySound(string fileName) {
            var input = new AudioFileReader(fileName);
            var reader = new AutoDisposeFileReader(input);
            AddMixerInput(reader);
            return reader;
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

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);
    }

    class CachedSound {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }
        public CachedSound(string audioFileName) {
            using (var audioFileReader = new AudioFileReader(audioFileName)) {
                // TODO: could add resampling in here if required
                WaveFormat = audioFileReader.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
            }
        }
    }

    class CachedSoundSampleProvider : ISampleProvider {
        private readonly CachedSound cachedSound;
        private long position;

        public CachedSoundSampleProvider(CachedSound cachedSound) {
            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count) {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
    }

    internal class AutoDisposeFileReader : ISampleProvider {
        private readonly AudioFileReader reader;
        internal bool isDisposed;
        internal float volume = 1f;
        public AutoDisposeFileReader(AudioFileReader reader) {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count) {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0) {
                reader.Dispose();
                isDisposed = true;
            } else if (volume < 1.0f) {
                for (int i = 0; i < buffer.Length; i++) {
                    buffer[i] *= volume;
                }
            }
            return read;
        }

        public void Stop() {
            if (isDisposed) return;
            reader.Dispose();
            isDisposed = true;
        }

        public WaveFormat WaveFormat { get; private set; }
    }
}
