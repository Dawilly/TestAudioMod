using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework.WaveFile {
    public class WaveReader : ISoundFileReader, IDisposable {
        private readonly BinaryReader wavFile;
        private WaveFileDecoder decoder;
        private long sampleStartPos;
        private int decodedSamples;
        private short[] wArray;

        public int TotalSampleCount;

        public int Channels => this.decoder.Channels;

        public TimeSpan TotalTime => TimeSpan.FromSeconds(this.TotalSampleCount / this.SampleRate);

        public int SampleRate => this.decoder.SampleRate;

        public TimeSpan DecodedTime {
            get { return TimeSpan.FromSeconds(this.decodedSamples / this.SampleRate); }
            set => throw new NotImplementedException();
        }

        public WaveReader(string fileName) {
            this.wavFile = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read));
            this.decoder = new WaveFileDecoder();
            
            this.Initialize();
        }

        public bool Initialize() {
            if (this.wavFile == null) return false;

            if (!this.decoder.Initialize(this.wavFile)) return false;

            this.TotalSampleCount = this.decoder.DataChunkSize / (this.decoder.BitDepth / 8) / (this.decoder.Channels);
            this.sampleStartPos = this.wavFile.BaseStream.Position;
            this.decodedSamples = 0;

            return true;
        }

        public int ReadSamples(float[] buffer, int count) {
            int checkedCount = Math.Min(this.TotalSampleCount - this.decodedSamples, count);

            if (this.wArray == null || this.wArray.Length != buffer.Length) {
                this.wArray = new short[buffer.Length];
            }

            this.decoder.ReadRawSamples(this.wavFile, this.wArray, checkedCount);
            this.decodedSamples += checkedCount;

            float[] _output = this.convertShortToFloat(this.wArray);

            Buffer.BlockCopy(_output, 0, buffer, 0, checkedCount * sizeof(float));

            return checkedCount;
        }

        public void Dispose() {
            this.wavFile.Dispose();
        }

        private float[] convertShortToFloat(short[] array) {
            return Array.ConvertAll(array, e => e / (float)short.MaxValue);
        }

        public int ReadSamples(float[] buffer, int offset, int count) {
            return this.ReadSamples(buffer, count);
        }

        public void Reset() {
            this.decodedSamples = 0;
            this.wavFile.BaseStream.Position = this.sampleStartPos;
        }
    }
}
