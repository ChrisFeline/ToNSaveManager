using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace ToNSaveManager.Utils.JSPlugins.API.Audio {
    // https://github.com/naudio/NAudio/blob/5bd2860c20f8134a8021fa8e15f74b5209963e7d/NAudio/AudioFileReader.cs#L16
    internal class AudioReader : WaveStream, ISampleProvider {
        private WaveStream readerStream; // the waveStream which we will use for all positioning
        private readonly SampleChannel sampleChannel; // sample provider that gives us most stuff we need
        private readonly int destBytesPerSample;
        private readonly int sourceBytesPerSample;
        private readonly long length;
        private readonly object lockObject;

        public AudioReader(string filePath, bool fromUrl) {
            lockObject = new object();
            CreateReaderStream(filePath, fromUrl);
            sourceBytesPerSample = (readerStream.WaveFormat.BitsPerSample / 8) * readerStream.WaveFormat.Channels;
            sampleChannel = new SampleChannel(readerStream, false);
            destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
            length = SourceToDest(readerStream.Length);
        }

        /// <summary>
        /// Creates the reader stream, supporting all filetypes in the core NAudio library,
        /// and ensuring we are in PCM format
        /// </summary>
        /// <param name="fileName">File Name</param>
        private void CreateReaderStream(string fileName, bool fromUrl) {
            if (fromUrl) {
                readerStream = new MediaFoundationReader(fileName);
            } else if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)) {
                readerStream = new WaveFileReader(fileName);
                if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat) {
                    readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                    readerStream = new BlockAlignReductionStream(readerStream);
                }
            } else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)) {
                if (Environment.OSVersion.Version.Major < 6)
                    readerStream = new Mp3FileReader(fileName);
                else // make MediaFoundationReader the default for MP3 going forwards
                    readerStream = new MediaFoundationReader(fileName);
            } else if (fileName.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)) {
                readerStream = new AiffFileReader(fileName);
            } else {
                // fall back to media foundation reader, see if that can play it
                readerStream = new MediaFoundationReader(fileName);
            }
        }

        public override WaveFormat WaveFormat => sampleChannel.WaveFormat;

        public override long Length => length;

        public override long Position {
            get { return SourceToDest(readerStream.Position); }
            set { lock (lockObject) { readerStream.Position = DestToSource(value); } }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            var waveBuffer = new WaveBuffer(buffer);
            int samplesRequired = count / 4;
            int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
            return samplesRead * 4;
        }

        public int Read(float[] buffer, int offset, int count) {
            lock (lockObject) {
                return sampleChannel.Read(buffer, offset, count);
            }
        }

        public float Volume {
            get { return sampleChannel.Volume; }
            set { sampleChannel.Volume = value; }
        }

        private long SourceToDest(long sourceBytes) {
            return destBytesPerSample * (sourceBytes / sourceBytesPerSample);
        }

        private long DestToSource(long destBytes) {
            return sourceBytesPerSample * (destBytes / destBytesPerSample);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (readerStream != null) {
                    readerStream.Dispose();
                    readerStream = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
