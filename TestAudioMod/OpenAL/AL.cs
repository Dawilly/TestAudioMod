using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.OpenAL {
    internal class AL {
        private readonly ALCalls nativeCalls;
        private readonly Loader loader;

        public IntPtr NativeLibrary => this.loader.NativeLibrary;

        public AL() {
            this.loader = new Loader();
            this.nativeCalls = new ALCalls(this.loader);
        }

        public void GenerateBuffers(int size, out int buffer) {
            this.GenerateBuffers(size, out int[] results);
            buffer = results[0];
        }

        private unsafe void GenerateBuffers(int size, out int[] buffers) {
            buffers = new int[size];

            //This would be disasterous if this was moved. Lock it in place.
            fixed (int* ptr = &buffers[0]) {
                this.nativeCalls.alGenBuffers(size, ptr);
            }
        }
    }
}
