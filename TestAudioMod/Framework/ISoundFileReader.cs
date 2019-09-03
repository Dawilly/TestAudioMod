using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework {
    /// <summary>An interface describing an audio file reader.</summary>
    public interface ISoundFileReader {
        /// <summary>The number of channels the audio file supports. (1 - Mono, 2 - Stereo)</summary>
        int Channels { get; }

        /// <summary>The amount of time that has been decoded and played.</summary>
        TimeSpan DecodedTime { get; set; }

        /// <summary>The Sample Rate (in Hz) of the audio files. (How many samples per second).</summary>
        int SampleRate { get; }

        /// <summary>The total time it takes to play the audio file.</summary>
        TimeSpan TotalTime { get; }

        /// <summary>Free, release, and reset unmanaged resources.</summary>
        void Dispose();

        /// <summary>Reads a given number of samples and stores them into the provided buffer./></summary>
        /// <param name="buffer">The array where the samples will be stored.</param>
        /// <param name="offset">The number of samples to offset (skip).</param>
        /// <param name="count">The number of samples to read.</param>
        /// <returns>The number of samples that were read. If count is larger than what remains, the last remaining samples are provided.</returns>
        int ReadSamples(float[] buffer, int offset, int count);

        /// <summary>Resets the audio file back to the beginning.</summary>
        void Reset();
    }
}
