using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NVorbis;

namespace Pathoschild.Stardew.TestAudioMod.Framework {
    /// <summary>An audio cue which wraps an Ogg Vorbis file.</summary>
    /// <remarks>Originally derived from <a href="https://gist.github.com/nickgravelyn/5580531"/>, with various improvements and fixes for SMAPI.</remarks>
    internal class SoundEffectCue : IModCue {
        /*********
        ** Fields
        *********/
        /// <summary>The underlying Ogg Vorbis audio reader.</summary>
        private readonly VorbisReader Reader;

        /// <summary>The underlying sound effect.</summary>
        private readonly DynamicSoundEffectInstance Effect;

        /// <summary>A background thread which manages the playback state, or <c>null</c> if not currently playing.</summary>
        private Thread PlaybackThread;

        /// <summary>A wait handle which is set when the playback thread ends.</summary>
        private readonly EventWaitHandle PlaybackWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        /// <summary>A wait handle which is set when the underlying effect is waiting data.</summary>
        private readonly EventWaitHandle NeedBufferHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        /// <summary>A buffer which contains the audio sample currently being played from the full clip.</summary>
        private readonly float[] VorbisBuffer;

        /// <summary>A buffer equivalent to <see cref="VorbisBuffer"/> converted to bytes for .</summary>
        private readonly byte[] SampleBuffer;

        /// <summary>The middle and default pitch value in the game code.</summary>
        /// <remarks>The game uses pitch values between 0 and 2400 with 1200 as the default, contrasted to <see cref="DynamicSoundEffectInstance"/> which uses -1 to 1 with 0 as the default.</remarks>
        private const int GameMiddlePitchValue = 1200;

        /// <summary>The default bit depth for audio used by XNA and XACT.</summary>
        private const int BitDepth = 16;

        /// <summary>The number of bytes XNA expects for any given data point pertaining to the audio data.</summary>
        private readonly int ValidBlockAlign;

        private BiquadraticFilter Filter;

        private double filterFrequency;
        private double qFactor;

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
        public bool IsLooped { get; set; }

        /// <summary>The audio playback state.</summary>
        public SoundState State => this.Effect.State;

        public double FilterFrequency {
            get {
                return this.filterFrequency * this.FilterPercentage;
            }
            set {
                if (!this.FilterEnabled) return;
                this.filterFrequency = value;
                this.Filter.Frequency = this.FilterFrequency;
            }
        }
        public double FilterQFactor {
            get {
                if (this.StaticQFactor) return this.qFactor;
                return this.qFactor * this.FilterPercentage;
            }
            set {
                if (!this.FilterEnabled) return;
                this.qFactor = value;
                this.Filter.QFactor = this.FilterQFactor;
            }
        }

        public double FilterPercentage { get; private set; }

        public bool FilterEnabled => this.Filter != null;
        public bool StaticQFactor { get; set; }

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The global name for the audio clip.</param>
        /// <param name="path">The absolute path to the <c>.ogg</c> file to load.</param>
        public SoundEffectCue(string name, string path) {
            this.Name = name;

            this.Reader = new VorbisReader(path);
            this.Effect = new DynamicSoundEffectInstance(this.Reader.SampleRate, (AudioChannels)this.Reader.Channels);

            this.SampleBuffer = new byte[this.Effect.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(500))];
            this.VorbisBuffer = new float[this.SampleBuffer.Length / 2];

            this.Effect.BufferNeeded += (s, e) => this.NeedBufferHandle.Set(); // when a buffer is needed, set our handle so the helper thread will read in more data
            this.ValidBlockAlign = (this.Reader.Channels * SoundEffectCue.BitDepth) / 8;
        }

        /// <summary>Free, release, and reset unmanaged resources.</summary>
        public void Dispose() {
            this.Dispose(true);
        }

        /// <summary>Begin playing the audio.</summary>
        public void Play() {
            this.Stop(AudioStopOptions.Immediate);

            lock (this.Effect)
                this.Effect.Play();

            this.StartThread();
        }

        /// <summary>Pause the audio playback.</summary>
        public void Pause() {
            lock (this.Effect)
                this.Effect.Pause();
        }

        /// <summary>Resume the audio playback.</summary>
        public void Resume() {
            lock (this.Effect)
                this.Effect.Resume();
        }

        /// <summary>Stop the audio playback, if applicable.</summary>
        /// <param name="options">When to stop the audio.</param>
        public void Stop(AudioStopOptions options) {
            lock (this.Effect) {
                switch (options) {
                    case AudioStopOptions.AsAuthored:
                        if (!this.Effect.IsDisposed)
                            this.IsLooped = false;
                        break;

                    case AudioStopOptions.Immediate:
                        if (!this.Effect.IsDisposed)
                            this.Effect.Stop();

                        this.Reader.DecodedTime = TimeSpan.Zero;

                        if (this.PlaybackThread != null) {
                            // set the handle to stop our thread
                            this.PlaybackWaitHandle.Set();
                            this.PlaybackThread = null;
                        }
                        break;
                }
            }
        }

        /// <summary>Set a predefined audio variable.</summary>
        /// <param name="key">The variable key.</param>
        /// <param name="value">The value to set.</param>
        public void SetVariable(string key, int value) {
            this.SetVariable(key, (float)value);
        }

        /// <summary>Set a predefined audio variable.</summary>
        /// <param name="key">The variable key.</param>
        /// <param name="value">The value to set.</param>
        public void SetVariable(string key, float value) {
            switch (key) {
                case "Volume":
                    this.Effect.Volume = MathHelper.Clamp(value / 100, 0, 1);
                    break;

                case "Pitch":
                    this.Effect.Pitch = MathHelper.Clamp((value - SoundEffectCue.GameMiddlePitchValue) / SoundEffectCue.GameMiddlePitchValue, -1, 1); // see remarks on GameMiddlePitchValue
                    break;

                case "Frequency":
                    this.FilterPercentage = MathHelper.Clamp((value / 100), 0, 1);
                    if (this.Filter == null) return;
                    this.Filter.Adjust(this.FilterFrequency, this.FilterQFactor);
                    return;

                default:
                    throw new NotSupportedException($"Unknown audio variable '{key}'.");
            }
        }

        /// <summary>Get the value of a predefined audio variable.</summary>
        /// <param name="key">The variable key.</param>
        public float GetVariable(string key) {
            switch (key) {
                case "Volume":
                    return this.Effect.Volume * 100;

                case "Pitch":
                    return (this.Effect.Pitch * SoundEffectCue.GameMiddlePitchValue) + SoundEffectCue.GameMiddlePitchValue; // see remarks on GameMiddlePitchValue

                case "Frequency":
                    return (float) this.FilterPercentage;

                default:
                    throw new NotSupportedException($"Unknown audio variable '{key}'.");
            }
        }

        public void EnableFilter(FilterType type, double Fc, double Q) {
            this.FilterPercentage = 1.0;
            this.filterFrequency = Fc;
            this.qFactor = Q;

            switch (type) {
                case FilterType.LowPass:
                    this.Filter = new LowpassFilter(this.Reader.SampleRate, Fc, Q);
                    break;
                case FilterType.HighPass:
                    this.Filter = new HighpassFilter(this.Reader.SampleRate, Fc, Q);
                    break;
                case FilterType.Bandpass:
                    this.Filter = new BandpassFilter(this.Reader.SampleRate, Fc, Q);
                    break;
            }

            return;
        }

        public void DisableFilter() {
            this.Filter = null;
            return;
        }

        /*********
        ** Protected methods
        *********/
        /// <summary>Perform cleanup when the instance is being destroyed by the garbage collector.</summary>
        ~SoundEffectCue() {
            this.Dispose(false);
        }

        /// <summary>Free, release, and reset unmanaged resources.</summary>
        /// <param name="isDisposing">Whether the instance is being disposed (rather than finalized by the garbage collector).</param>
        protected void Dispose(bool isDisposing) {
            this.PlaybackWaitHandle.Set();
            this.Effect.Dispose();
            this.Reader.Dispose();

            if (isDisposing)
                GC.SuppressFinalize(this);
        }

        private void StartThread() {
            if (this.PlaybackThread == null) {
                this.PlaybackWaitHandle.Reset();
                this.NeedBufferHandle.Reset();
                this.PlaybackThread = new Thread(this.StreamThread);
                this.PlaybackThread.Start();
            }
        }

        /// <summary>Manage the audio stream while it's playing.</summary>
        // This is where the magic happens.
        private void StreamThread() {
            while (!this.Effect.IsDisposed) {
                // sleep until we need a buffer
                while (!this.Effect.IsDisposed && !this.PlaybackWaitHandle.WaitOne(0) && !this.NeedBufferHandle.WaitOne(0))
                    Thread.Sleep(50);

                // if the thread is waiting to exit, leave
                if (this.PlaybackWaitHandle.WaitOne(0))
                    break;

                // ensure the effect isn't disposed
                lock (this.Effect) {
                    if (this.Effect.IsDisposed)
                        break;
                }

                // read the next chunk of data
                int samplesRead = this.Reader.ReadSamples(this.VorbisBuffer, 0, this.VorbisBuffer.Length);
                
                // out of data and looping? reset the reader and read again

                // It seems this.Effects.IsLooped is not supported (or implemented?). Thus we'll rely
                // on SoundEffectCue's own IsLooped.
                if (samplesRead == 0 && this.IsLooped) {
                    this.Reader.DecodedTime = TimeSpan.Zero;
                    samplesRead = this.Reader.ReadSamples(this.VorbisBuffer, 0, this.VorbisBuffer.Length);
                }

                // Check to see if we're consuming the correct size of data chunks.
                int blockCheck = samplesRead % this.ValidBlockAlign;

                if (samplesRead > 0) {
                    // Process through filter first
                    if (this.FilterEnabled) {
                        this.Filter.Process(this.VorbisBuffer);
                    }

                    //Convert to bytes
                    for (int i = 0; i < samplesRead; i++) {
                        short sValue = (short)Math.Max(Math.Min(short.MaxValue * this.VorbisBuffer[i], short.MaxValue), short.MinValue);
                        this.SampleBuffer[i * 2] = (byte)(sValue & 0xff);
                        this.SampleBuffer[i * 2 + 1] = (byte)((sValue >> 8) & 0xff);
                    }

                    // If we are not consuming the correct size of data chunks, add on empty blocks
                    // that will not emit sound. This should be no more than 3 addition bytes at most
                    // and may not matter, nor be heard, by the user.
                    if (blockCheck != 0) {
                        for (int i = 0; i < blockCheck; i++) {
                            this.SampleBuffer[samplesRead + i] = 0;
                        }

                        samplesRead += blockCheck;
                    }

                    // submit our buffers
                    lock (this.Effect) {
                        // ensure the effect isn't disposed
                        if (this.Effect.IsDisposed)
                            break;

                        // Submit Channel 1
                        this.Effect.SubmitBuffer(this.SampleBuffer, 0, samplesRead);
                        // Submit Channel 2
                        this.Effect.SubmitBuffer(this.SampleBuffer, samplesRead, samplesRead);
                    }
                }

                // reset our handle
                this.NeedBufferHandle.Reset();
            }
        }
    }
}
