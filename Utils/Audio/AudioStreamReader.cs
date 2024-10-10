using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ToNSaveManager.Utils.Audio {
    // https://github.com/naudio/NAudio/blob/f8568cd4ad20c0683389cc9bc878beb945528047/NAudio/AudioFileReader.cs
    public class AudioStreamReader : WaveStream, ISampleProvider {
        private WaveStream readerStream;
        private readonly SampleChannel sampleChannel;
        private readonly int destBytesPerSample;
        private readonly int sourceBytesPerSample;
        private readonly long length;
        private readonly object lockObject;

        //
        // Summary:
        //     File Name
        public string FileName { get; }

        //
        // Summary:
        //     WaveFormat of this stream
        public override WaveFormat WaveFormat => sampleChannel.WaveFormat;

        //
        // Summary:
        //     Length of this stream (in bytes)
        public override long Length => length;

        //
        // Summary:
        //     Position of this stream (in bytes)
        public override long Position {
            get {
                return SourceToDest(readerStream.Position);
            }
            set {
                lock (lockObject) {
                    readerStream.Position = DestToSource(value);
                }
            }
        }

        //
        // Summary:
        //     Gets or Sets the Volume of this AudioFileReader. 1.0f is full volume
        public float Volume {
            get {
                return sampleChannel.Volume;
            }
            set {
                sampleChannel.Volume = value;
            }
        }

        //
        // Summary:
        //     Initializes a new instance of AudioFileReader
        //
        // Parameters:
        //   fileName:
        //     The file to open
        public AudioStreamReader(string fileName) {
            lockObject = new object();
            FileName = fileName;
            CreateReaderStream(fileName);
            sourceBytesPerSample = readerStream.WaveFormat.BitsPerSample / 8 * readerStream.WaveFormat.Channels;
            sampleChannel = new SampleChannel(readerStream, forceStereo: false);
            destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
            length = SourceToDest(readerStream.Length);
        }

        /// <summary>
        /// Will always be wav because this is used for resources or whatever.
        /// </summary>
        /// <param name="stream"></param>
        public AudioStreamReader(Stream stream) {
            lockObject = new object();
            FileName = "stream";

            readerStream = new WaveFileReader(stream);
            if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat) {
                readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                readerStream = new BlockAlignReductionStream(readerStream);
            }

            sourceBytesPerSample = readerStream.WaveFormat.BitsPerSample / 8 * readerStream.WaveFormat.Channels;
            sampleChannel = new SampleChannel(readerStream, forceStereo: false);
            destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
            length = SourceToDest(readerStream.Length);
        }

        //
        // Summary:
        //     Creates the reader stream, supporting all filetypes in the core NAudio library,
        //     and ensuring we are in PCM format
        //
        // Parameters:
        //   fileName:
        //     File Name
        private void CreateReaderStream(string fileName) {
            if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)) {
                readerStream = new WaveFileReader(fileName);
                if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat) {
                    readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                    readerStream = new BlockAlignReductionStream(readerStream);
                }
            } else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)) {
                if (Environment.OSVersion.Version.Major < 6) {
                    readerStream = new Mp3FileReader(fileName);
                } else {
                    readerStream = new MediaFoundationReader(fileName);
                }
            } else if (fileName.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)) {
                readerStream = new AiffFileReader(fileName);
            } else {
                readerStream = new MediaFoundationReader(fileName);
            }
        }

        //
        // Summary:
        //     Reads from this wave stream
        //
        // Parameters:
        //   buffer:
        //     Audio buffer
        //
        //   offset:
        //     Offset into buffer
        //
        //   count:
        //     Number of bytes required
        //
        // Returns:
        //     Number of bytes read
        public override int Read(byte[] buffer, int offset, int count) {
            WaveBuffer waveBuffer = new WaveBuffer(buffer);
            int count2 = count / 4;
            return Read(waveBuffer.FloatBuffer, offset / 4, count2) * 4;
        }

        //
        // Summary:
        //     Reads audio from this sample provider
        //
        // Parameters:
        //   buffer:
        //     Sample buffer
        //
        //   offset:
        //     Offset into sample buffer
        //
        //   count:
        //     Number of samples required
        //
        // Returns:
        //     Number of samples read
        public int Read(float[] buffer, int offset, int count) {
            lock (lockObject) {
                return sampleChannel.Read(buffer, offset, count);
            }
        }

        //
        // Summary:
        //     Helper to convert source to dest bytes
        private long SourceToDest(long sourceBytes) {
            return destBytesPerSample * (sourceBytes / sourceBytesPerSample);
        }

        //
        // Summary:
        //     Helper to convert dest to source bytes
        private long DestToSource(long destBytes) {
            return sourceBytesPerSample * (destBytes / destBytesPerSample);
        }

        //
        // Summary:
        //     Disposes this AudioFileReader
        //
        // Parameters:
        //   disposing:
        //     True if called from Dispose
        protected override void Dispose(bool disposing) {
            if (disposing && readerStream != null) {
                readerStream.Dispose();
                readerStream = null;
            }

            base.Dispose(disposing);
        }
    }
}
