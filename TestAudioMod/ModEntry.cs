using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Pathoschild.Stardew.TestAudioMod.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Pathoschild.Stardew.TestAudioMod
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Where to load the sound from.</summary>
        private ContentSource Source = ContentSource.ModFolder;

        /// <summary>The cue name to play.</summary>
        private string CueName = null;

        /// <summary>The current sound to play.</summary>
        private ICue Cue;

        /// <summary>The frequency to play.</summary>
        /// <remarks>This is a percentage value(?), 0 to 100, default 0.</remarks>
        private int Frequency = 100;

        /// <summary>The pitch to play.</summary>
        /// <remarks>This is a relative value(?), 0 to 2400, default 1200.</remarks>
        private int Pitch = 1200;

        private float Pan = 0f;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            switch (e.Button)
            {
                // toggle audio source
                case SButton.Tab:
                    this.SetAudio(this.Source == ContentSource.GameContent ? ContentSource.ModFolder : ContentSource.GameContent, this.CueName);
                    break;

                // set audio clip
                case SButton.D1:
                    this.SetAudio(this.Source, null);
                    break;

                case SButton.D2:
                    this.SetAudio(this.Source, "SinWave"); // supports Pitch
                    break;

                case SButton.D3:
                    this.SetAudio(this.Source, "wind"); // supports Frequency; GameContent cue only works after loading a save
                    if (this.Source == ContentSource.ModFolder) {
                        IModCue cue = (IModCue)this.Cue;
                        cue.EnableFilter(FilterType.LowPass, 3000, 0.707);
                    }
                    break;

                case SButton.D4:
                    this.SetAudio(this.Source, "stillnessInTheRain"); // mod sound only
                    break;

                case SButton.D5:
                    this.SetAudio(this.Source, "flute");
                    break;

                case SButton.D6:
                    this.SetAudio(this.Source, "rain");
                    if (this.Source == ContentSource.ModFolder) {
                        IModCue cue = (IModCue)this.Cue;
                        cue.EnableFilter(FilterType.LowPass, 20000, 2.90);
                    }
                    break;

                // set frequency
                case SButton.Up:
                case SButton.Down:
                    {
                        int change = e.Button == SButton.Up ? 10 : -10;
                        this.Frequency = (int)MathHelper.Clamp(this.Frequency + change, 0, 100);
                    }
                    break;

                // set pitch
                case SButton.Left:
                case SButton.Right:
                    {
                        int change = e.Button == SButton.Right ? 240 : -240;
                        this.Pitch = (int)MathHelper.Clamp(this.Pitch + change, 0, 2400);
                    }
                    break;
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (this.Cue != null)
            {
                ICue sound = this.Cue;
                sound.SetVariable("Volume", 100);
                sound.SetVariable("Frequency", this.Frequency);
                sound.SetVariable("Pitch", this.Pitch);
                if (sound is IModCue) sound.SetVariable("Pan", this.Pan);

                if (!sound.IsPlaying)
                    sound.Play();

                if (e.IsMultipleOf(30))
                    this.Monitor.Log($"Playing {sound.Name} from {this.Source} @ freq: {sound.GetVariable("Frequency")}, vol: {sound.GetVariable("Volume")}, pitch: {sound.GetVariable("Pitch")}, state: {this.GetPlaybackState(sound)}");
            }
        }

        /// <summary>Switch the audio source.</summary>
        /// <param name="source">Where to load the sound from.</param>
        /// <param name="cueName">The cue name to play.</param>
        private void SetAudio(ContentSource source, string cueName)
        {
            if (source == this.Source && cueName == this.CueName)
                return;

            // remove last sound
            this.Cue?.Stop(AudioStopOptions.Immediate);

            // set new sound
            this.Source = source;
            this.CueName = cueName;
            if (cueName != null)
            {
                switch (source)
                {
                    case ContentSource.GameContent:
                        this.Cue = Game1.soundBank.GetCue(cueName);
                        break;

                    case ContentSource.ModFolder:
                        {
                            IModCue sound = this.Helper.Content.ExtendedLoad<IModCue>($"assets/{cueName}.ogg");
                            sound.IsLooped = true;
                            this.Cue = sound;
                        }
                        break;
                }
            }
            else
                this.Cue = null;
        }

        /// <summary>Get a summary of the playback state for an audio cue.</summary>
        /// <param name="cue">The audio cue.</param>
        private string GetPlaybackState(ICue cue)
        {
            if (cue is IModCue modCue)
                return modCue.State.ToString();

            string state = "";
            if (cue.IsPlaying)
                state += $" {SoundState.Playing}";
            if (cue.IsPaused)
                state += $" {SoundState.Paused}";
            if (cue.IsStopped)
                state += $" {SoundState.Stopped}";
            if (cue.IsStopping)
                state += " Stopping";
            return state.Trim();
        }
    }
}
