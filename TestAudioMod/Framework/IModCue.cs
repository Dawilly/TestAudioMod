using Microsoft.Xna.Framework.Audio;
using StardewValley;

namespace Pathoschild.Stardew.TestAudioMod.Framework
{
    /// <summary>An audio cue loaded through the SMAPI audio API.</summary>
    public interface IModCue : ICue
    {
        /// <summary>The global name for the audio clip.</summary>
        new string Name { get; set; }

        /// <summary>The audio playback state.</summary>
        SoundState State { get; }

        /// <summary>Whether to loop the audio when it reaches the end.</summary>
        bool IsLooped { get; set; }

        /// <summary>The frequency (in Hz) the filter will use as a cutoff / centre.</summary>
        double FilterFrequency { get; set; }

        /// <summary>The Quality Factor (unitless) the filter will use during filter operations.</summary>
        double FilterQFactor { get; set; }

        /// <summary>The percentage of the frequency to use as a cutoff / centre.</summary>
        double FrequencyPercentage { get; }

        /// <summary>The percentage of the quality factor to use during filter operations.</summary>
        double QPercentage { get; }

        /// <summary>Determines if a filter has been enabled to the sound effect.</summary>
        bool FilterEnabled { get; }

        /// <summary>TO-DO: Refactor to remove this.</summary>
        bool StaticQFactor { get; set; }

        /// <summary>Attachs a filter, altering how the instance sounds when played.</summary>
        /// <param name="type">The type of filter to be applied.</param>
        /// <param name="Fc">The cutoff/centre frequency to operate at.</param>
        /// <param name="FcPercent">The inital percentage of the frequency to apply.</param>
        /// <param name="Q">The quality factor to operate at.</param>
        /// <param name="QPercent">The initial percentage of the quality factor to apply.</param>
        void EnableFilter(FilterType type, double Fc, double FcPercent, double Q, double QPercent);

        /// <summary>Attachs a filter, altering how the instance sounds when played. Initializes Fc and Q to operate at 100%</summary>
        /// <param name="type">The type of filter to be applied.</param>
        /// <param name="Fc">The cutoff/centre frequency to operate at.</param>
        /// <param name="Q">The quality factor to operate at.</param>
        void EnableFilter(FilterType type, double fc, double q);

        /// <summary>Removes the filter, if applicable.</summary>
        void DisableFilter();
    }
}
