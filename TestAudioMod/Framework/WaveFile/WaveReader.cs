using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework.WaveFile {
    public class WaveReader : IDisposable {
        private readonly BinaryReader wavFile;
        private WaveFileDecoder decoder;

        public WaveReader(string fileName) {
            this.wavFile = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read));
            this.decoder = new WaveFileDecoder();
        }

        public bool Initialize() {
            
        }

        public int ReadSamples(float[] buffer, int offset, int count) {
            return 0;
        }

        public void Dispose() {
            this.wavFile.Dispose();
        }
    }
}
