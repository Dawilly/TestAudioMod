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
    }
}
