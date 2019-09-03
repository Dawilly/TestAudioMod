using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework.WaveFile {
    public class WaveFileDecoder {
        public byte[] ChunkId { get; set; }
        public int FileSize { get; set; }
        public byte[] TypeId { get; set; }
        public byte[] FormatId { get; set; }
        public int FormatSize { get; set; }
        public int AudioFormat { get; set; }
        public int Channels { get; set; }
        public int SampleRate { get; set; }
        public int ByteRate { get; set; }
        public int BlockAlign { get; set; }
        public int BitDepth { get; set; }
        public byte[] DataId { get; set; }
        public int DataChunkSize { get; set; }

        public bool Initialize(BinaryReader reader) {
            if (reader == null) return false;
            try {
                this.ChunkId = reader.ReadBytes(4);
                this.FileSize = reader.ReadInt32();
                this.TypeId = reader.ReadBytes(4);
                this.FormatId = reader.ReadBytes(4);
                this.FormatSize = reader.ReadInt32();
                this.AudioFormat = reader.ReadInt16();
                this.Channels = reader.ReadInt16();
                this.SampleRate = reader.ReadInt32();
                this.ByteRate = reader.ReadInt32();
                this.BlockAlign = reader.ReadInt16();
                this.BitDepth = reader.ReadInt16();
                this.DataId = reader.ReadBytes(4);
                this.DataChunkSize = reader.ReadInt32();
            } catch (Exception e) {
                Console.WriteLine(e);
                return false;
            }

            if (Encoding.ASCII.GetString(this.ChunkId) != "RIFF") return false;
            if (Encoding.ASCII.GetString(this.TypeId) != "WAVE") return false;
            if (Encoding.ASCII.GetString(this.FormatId) != "fmt ") return false;
            if (Encoding.ASCII.GetString(this.DataId) != "data") return false;

            return true;
        }

        public void ReadRawSamples(BinaryReader reader, short[] raw, int count) {
            for (int i = 0; i < count; i++) {
                raw[i] = reader.ReadInt16();
            }
        }
    }
}
