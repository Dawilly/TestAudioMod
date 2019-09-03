using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NVorbis;

namespace Pathoschild.Stardew.TestAudioMod.Framework.OggFile {
    /// <summary>An implemention of <see cref="ISoundFileReader"/> that wraps an <see cref="VorbisReader"/>. Reads Ogg files.</summary>
    public class OggReader : ISoundFileReader, IDisposable {
        /*********
        ** Fields
        *********/
        /// <summary>The underlying ogg file reader.</summary>
        private readonly VorbisReader Reader;

        /*********
        ** Accessors
        *********/
        /// <summary>The number of channels the audio file supports. (1 - Mono, 2 - Stereo)</summary>
        public int Channels => this.Reader.Channels;

        /// <summary>The amount of time that has been decoded and played.</summary>
        public TimeSpan DecodedTime {
            get { return this.Reader.DecodedTime; }
            set { this.Reader.DecodedTime = value; }
        }

        /// <summary>The Sample Rate (in Hz) of the audio files. (How many samples per second).</summary>
        public int SampleRate => this.Reader.SampleRate;

        /// <summary>The total time it takes to play the audio file.</summary>
        public TimeSpan TotalTime => this.Reader.TotalTime;

        /*********
        ** Constructors
        *********/
        public OggReader(string path) {
            this.Reader = new VorbisReader(path);
        }

        /*********
        ** Public Methods
        *********/
        /// <summary>Free, release, and reset unmanaged resources.</summary>
        public void Dispose() {
            this.Reader.Dispose();
        }

        /// <summary>Reads a given number of samples and stores them into the provided buffer./></summary>
        /// <param name="buffer">The array where the samples will be stored.</param>
        /// <param name="offset">The number of samples to offset (skip).</param>
        /// <param name="count">The number of samples to read.</param>
        /// <returns>The number of samples that were read. If count is larger than what remains, the last remaining samples are provided.</returns>
        public int ReadSamples(float[] buffer, int offset, int count) {
            return this.Reader.ReadSamples(buffer, offset, count);
        }

        /// <summary>Resets the audio file back to the beginning.</summary>
        public void Reset() {
            this.Reader.DecodedTime = TimeSpan.Zero;
        }
    }
}
