using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace Pathoschild.Stardew.TestAudioMod.OpenAL {
    internal class OpenALSound : IDisposable {
        private readonly OpenALSoundBuffer soundBuffer;

        private bool isDisposed;

        public int Volume { get; set; }
        public int Pitch { get; set; }
        public SoundState State { get; private set; }

        public OpenALSound(AL Oal) {
            this.soundBuffer = new OpenALSoundBuffer(Oal);
            //Check Error
        }

        ~OpenALSound() {
            this.Dispose(false);
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (!this.isDisposed) {

            }
        }
    }
}
