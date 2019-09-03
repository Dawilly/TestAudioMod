using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NVorbis;

namespace Pathoschild.Stardew.TestAudioMod.Framework.OggFile {
    public class OggReader : ISoundFileReader, IDisposable {
        private readonly VorbisReader Reader;

        public int Channels => this.Reader.Channels;

        public TimeSpan TotalTime => this.Reader.TotalTime;

        public int SampleRate => this.Reader.SampleRate;

        public TimeSpan DecodedTime {
            get { return this.Reader.DecodedTime; }
            set { this.Reader.DecodedTime = value; }
        }

        public OggReader(string path) {
            this.Reader = new VorbisReader(path);
        }

        public void Dispose() {
            this.Reader.Dispose();
        }

        public int ReadSamples(float[] buffer, int offset, int count) {
            return this.Reader.ReadSamples(buffer, offset, count);
        }

        public void Reset() {
            this.Reader.DecodedTime = TimeSpan.Zero;
        }
    }
}
