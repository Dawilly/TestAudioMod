using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.OpenAL {
    internal class OpenALSoundBuffer : IDisposable {
        private int dataBuffer;
        private int dataSize;
        private bool isDisposed;

        public OpenALSoundBuffer(AL Oal) {
            Oal.GenerateBuffers(1, out this.dataBuffer);
        }

        ~OpenALSoundBuffer() {
            this.Dispose(false);
        }

        public void BindDataBuffer() {

        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        private void Dispose(bool disposing) {
            if (!this.isDisposed) {
                // Clean up Managed Objects.
                if (disposing) {
                    
                }

                // Clean up Unmanaged Objects.
                //if (
            }
        }
    }
}
