using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Pathoschild.Stardew.TestAudioMod.Framework
{
    /// <summary>An audio cue which wraps a <see cref="SoundEffect"/>.</summary>
    internal class SoundEffectCue : IModCue
    {
        /*********
        ** Fields
        *********/
        /// <summary>The underlying raw sound effect.</summary>
        private readonly SoundEffect RawEffect;

        /// <summary>The underlying sound effect instance.</summary>
        private readonly SoundEffectInstance Effect;

        /// <summary>The middle and default pitch value in the game code.</summary>
        /// <remarks>The game uses pitch values between 0 and 2400 with 1200 as the default, contrasted to <see cref="SoundEffectInstance"/> which uses -1 to 1 with 0 as the default.</remarks>
        private const int GameMiddlePitchValue = 1200;


        /*********
        ** Accessors
        *********/
        /// <summary>The global name for the audio clip.</summary>
        public string Name { get; set; }

        /// <summary>Whether the audio is currently stopped.</summary>
        public bool IsStopped => this.Effect.State == SoundState.Stopped;

        /// <summary>Whether the audio is currently being stopped.</summary>
        public bool IsStopping => false;

        /// <summary>Whether the audio is currently playing.</summary>
        public bool IsPlaying => this.Effect.State == SoundState.Playing;

        /// <summary>Whether the audio is currently paused.</summary>
        public bool IsPaused => this.Effect.State == SoundState.Paused;

        /// <summary>Whether to loop the audio when it reaches the end.</summary>
        public bool IsLooped
        {
            get => this.Effect.IsLooped;
            set => this.Effect.IsLooped = value;
        }

        /// <summary>The audio playback state.</summary>
        public SoundState State => this.Effect.State;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The global name for the audio clip.</param>
        /// <param name="effect">The underlying sound effect.</param>
        public SoundEffectCue(string name, SoundEffect effect)
        {
            this.Name = name;
            this.RawEffect = effect;
            this.Effect = this.RawEffect.CreateInstance();
        }

        /// <summary>Free, release, and reset unmanaged resources.</summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>Begin playing the audio.</summary>
        public void Play()
        {
            this.Stop(AudioStopOptions.Immediate);

            lock (this.Effect)
                this.Effect.Play();
        }

        /// <summary>Pause the audio playback.</summary>
        public void Pause()
        {
            lock (this.Effect)
                this.Effect.Pause();
        }

        /// <summary>Resume the audio playback.</summary>
        public void Resume()
        {
            lock (this.Effect)
                this.Effect.Resume();
        }

        /// <summary>Stop the audio playback, if applicable.</summary>
        /// <param name="options">When to stop the audio.</param>
        public void Stop(AudioStopOptions options)
        {
            lock (this.Effect)
                this.Effect.Stop(immediate: options == AudioStopOptions.Immediate);
        }

        /// <summary>Set a predefined audio variable.</summary>
        /// <param name="key">The variable key.</param>
        /// <param name="value">The value to set.</param>
        public void SetVariable(string key, int value)
        {
            this.SetVariable(key, (float)value);
        }

        /// <summary>Set a predefined audio variable.</summary>
        /// <param name="key">The variable key.</param>
        /// <param name="value">The value to set.</param>
        public void SetVariable(string key, float value)
        {
            // Conversion notes:
            // The game's custom variables are a bit different than the values recognized by SoundEffectInstance:
            //
            //    variable  | in Stardew Valley                            | in SoundEffectInstance
            //    --------- | -------------------------------------------- | ----------------------
            //    Volume    | % value, int 0 to 100, default 0.            | % value, float 0 to 1.
            //    Pitch     | relative value, int 0 to 2400, default 1200. | relative value, float -1 to 1, default 0.
            //    Frequency | % value?, int 0 to 100, default 0.           | ?
            switch (key)
            {
                case "Volume":
                    this.Effect.Volume = MathHelper.Clamp(value / 100, 0, 1);
                    break;

                case "Pitch":
                    this.Effect.Pitch = MathHelper.Clamp((value - SoundEffectCue.GameMiddlePitchValue) / SoundEffectCue.GameMiddlePitchValue, -1, 1); // see remarks on GameMiddlePitchValue
                    break;

                case "Frequency":
                    //throw new NotImplementedException();
                    break;

                default:
                    throw new NotSupportedException($"Unknown audio variable '{key}'.");
            }
        }

        /// <summary>Get the value of a predefined audio variable.</summary>
        /// <param name="key">The variable key.</param>
        public float GetVariable(string key)
        {
            // See conversion notes in SetVariable.
            switch (key)
            {
                case "Volume":
                    return this.Effect.Volume * 100;

                case "Pitch":
                    return (this.Effect.Pitch * SoundEffectCue.GameMiddlePitchValue) + SoundEffectCue.GameMiddlePitchValue; // see remarks on GameMiddlePitchValue

                case "Frequency":
                    return -1;//throw new NotImplementedException();

                default:
                    throw new NotSupportedException($"Unknown audio variable '{key}'.");
            }
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Perform cleanup when the instance is being destroyed by the garbage collector.</summary>
        ~SoundEffectCue()
        {
            this.Dispose(false);
        }

        /// <summary>Free, release, and reset unmanaged resources.</summary>
        /// <param name="isDisposing">Whether the instance is being disposed (rather than finalized by the garbage collector).</param>
        protected void Dispose(bool isDisposing)
        {
            this.Effect.Dispose();
            this.RawEffect.Dispose();
            if (isDisposing)
                GC.SuppressFinalize(this);
        }
    }
}
