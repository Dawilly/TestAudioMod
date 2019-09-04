using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework.WaveFile {
    /// <summary>An implementation of <see cref="ISoundFileReader"/> that reads .wav files.</summary>
    public class WaveReader : ISoundFileReader, IDisposable {
        /*********
        ** Fields
        *********/
        /// <summary>The <see cref="BinaryReader"/> created to read the .wav file.</summary>
        private readonly BinaryReader wavFile;

        /// <summary>The decoder tasked to break down the file structure into readable chunks.</summary>
        private WaveFileDecoder decoder;

        /// <summary>The wave file's starting position (the beginning of the samples).</summary>
        private long sampleStartPos;

        /// <summary>The amount of samples that have been decoded.</summary>
        private int decodedSamples;

        /// <summary>The working array for decoding, reading and converting samples.</summary>
        private short[] wArray;


        /*********
        ** Accessors
        *********/
        /// <summary>The total amount of samples within the wave file (one sample has both left and right speaker data).</summary>
        public int TotalSampleCount { get; private set; }

        /// <summary>The number of channels the audio file supports. (1 - Mono, 2 - Stereo)</summary>
        public int Channels => this.decoder.Channels;

        /// <summary>The amount of time that has been decoded and played.</summary>
        public TimeSpan DecodedTime {
            get => TimeSpan.FromSeconds(this.decodedSamples / this.SampleRate);
            set => throw new NotImplementedException();
        }

        /// <summary>The Sample Rate (in Hz) of the audio files. (How many samples per second).</summary>
        public int SampleRate => this.decoder.SampleRate;

        /// <summary>The total time it takes to play the wave file.</summary>
        public TimeSpan TotalTime => TimeSpan.FromSeconds(this.TotalSampleCount / this.SampleRate);

        /*********
        ** Constructors
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="fileName">The absolute path to the wave file.</param>
        public WaveReader(string fileName) {
            this.wavFile = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read));
            this.decoder = new WaveFileDecoder(this.wavFile);
            
            this.Initialize();
        }

        /*********
        ** Public methods
        *********/
        /// <summary>Free, release, and reset unmanaged resources.</summary>
        public void Dispose() {
            this.wavFile.Dispose();
        }

        /// <summary>Reads a given number of samples and stores them into the provided buffer./></summary>
        /// <param name="buffer">The array where the samples will be stored.</param>
        /// <param name="count">The number of samples to read.</param>
        /// <returns>The number of samples that were read. If count is larger than what remains, the last remaining samples are provided.</returns>
        public int ReadSamples(float[] buffer, int count) {
            int checkedCount = Math.Min(this.TotalSampleCount - this.decodedSamples, count);

            if (this.wArray == null || this.wArray.Length != buffer.Length) {
                this.wArray = new short[buffer.Length];
            }

            this.decoder.ReadRawSamples(this.wArray, checkedCount);
            this.decodedSamples += checkedCount;

            float[] _output = this.ConvertShortToFloat(this.wArray);

            //This static function is a godsend.
            Buffer.BlockCopy(_output, 0, buffer, 0, checkedCount * sizeof(float));

            return checkedCount;
        }

        /// <summary>Reads a given number of samples and stores them into the provided buffer./></summary>
        /// <param name="buffer">The array where the samples will be stored.</param>
        /// <param name="offset">The number of samples to offset (skip).</param>
        /// <param name="count">The number of samples to read.</param>
        /// <returns>The number of samples that were read. If count is larger than what remains, the last remaining samples are provided.</returns>
        public int ReadSamples(float[] buffer, int offset, int count) {
            return this.ReadSamples(buffer, count);
        }

        /// <summary>Resets the audio file back to the beginning.</summary>
        public void Reset(bool forceClose) {
            this.decodedSamples = 0;
            this.wavFile.BaseStream.Position = this.sampleStartPos;
            if (forceClose)
                this.wavFile.Close();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Converts an array of shorts to floats.</summary>
        /// <param name="array">The array of shorts to be converted.</param>
        /// <returns>An array of float values.</returns>
        private float[] ConvertShortToFloat(short[] array) {
            return Array.ConvertAll(array, e => e / (float)short.MaxValue);
        }

        /// <summary>Initializes the instance, decoding the header of the wave file and setting values.</summary>
        /// <returns><c>True</c> if the decoding was successful, otherwise <c>false</c>.</returns>
        public bool Initialize() {
            if (this.wavFile == null) return false;

            if (!this.decoder.IsValid) return false;

            this.TotalSampleCount = this.decoder.DataChunkSize / (this.decoder.BitDepth / 8);
            this.sampleStartPos = this.wavFile.BaseStream.Position;
            this.decodedSamples = 0;

            return true;
        }
    }
}
