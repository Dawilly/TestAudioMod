using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework.WaveFile {
    /// <summary>A simple decoder for .wav files.</summary>
    public class WaveFileDecoder {
        /*********
        ** Fields
        *********/
        /// <summary>The binary reader pointing to the wave file.</summary>
        private readonly BinaryReader reader;

        /*********
        ** Accessors
        *********/
        public bool IsValid { get; private set; }

        /*********
        ** Accessors: RIFF Header
        *********/
        /// <summary>The Id of the file container (Should contain "RIFF").</summary>
        public byte[] ChunkId { get; private set; }

        /// <summary>The total number of chunks following this integer value. (Labeled ChunkSize on official documents)</summary>
        public int FileSize { get; private set; }

        /// <summary>The type identifcation. Should contain "WAVE". (Labeled Format on official documents)</summary>
        public byte[] TypeId { get; private set; }

        /*********
        ** Accessors: Format Subchunk
        *********/
        /// <summary>The format identification. Should contain "fmt ". (Labeled Subchunk1ID on official documents)</summary>
        public byte[] FormatId { get; private set; }

        /// <summary>The size of the rest of the format subchunk (Labeled Subchunk1Size on official documents)</summary>
        public int FormatSize { get; private set; }

        /// <summary>Indicates form of compression if value is not 1. If value is 1 (PCM), it is linear quantization.</summary>
        public int AudioFormat { get; private set; }

        /// <summary>The number of channels the file supports (1 - Mono, 2 - Stereo). (Labeled NumChannels on official documents)</summary>
        public int Channels { get; private set; }

        /// <summary>The number of samples per second.</summary>
        public int SampleRate { get; private set; }

        /// <summary>SampleRate * Channels * BitDepth / 8</summary>
        public int ByteRate { get; private set; }

        /// <summary>Channels * BitDepth / 8</summary>
        public int BlockAlign { get; private set; }

        /// <summary>The number of bits per sample.</summary>
        public int BitDepth { get; private set; }

        /*********
        ** Accessors: Data Subchunk
        *********/
        /// <summary>The data identifications. Should contain "data". (Labeled Subchunk2ID on official documents)</summary>
        public byte[] DataId { get; private set; }

        /// <summary>The number of bytes in the data. (Labeled Subchunk2Size on official documents)</summary>
        public int DataChunkSize { get; private set; }

        /*********
        ** Constructor
        *********/
        public WaveFileDecoder(BinaryReader reader) {
            this.reader = reader;
            this.IsValid = this.Initialize();
        }

        /*********
        ** Public Methods
        *********/
        /// <summary>Reads a given number of raw samples into a provided buffer.</summary>
        /// <param name="raw">The buffer that will contain the read samples.</param>
        /// <param name="count">The number of samples to be read.</param>
        public void ReadRawSamples(short[] raw, int count) {
            for (int i = 0; i < count; i++) {
                raw[i] = this.reader.ReadInt16();
            }
        }

        /*********
        ** Private Methods
        *********/
        /// <summary>Decodes the header of a .wav file and sets the nessecary fields. Checks to see if the .wav is valid.</summary>
        /// <returns><c>True</c> if the file was valid, otherwise <c>false</c>.</returns>
        private bool Initialize() {
            if (this.reader == null) return false;
            try {
                this.ChunkId = this.reader.ReadBytes(4);
                this.FileSize = this.reader.ReadInt32();
                this.TypeId = this.reader.ReadBytes(4);
                this.FormatId = this.reader.ReadBytes(4);
                this.FormatSize = this.reader.ReadInt32();
                this.AudioFormat = this.reader.ReadInt16();
                this.Channels = this.reader.ReadInt16();
                this.SampleRate = this.reader.ReadInt32();
                this.ByteRate = this.reader.ReadInt32();
                this.BlockAlign = this.reader.ReadInt16();
                this.BitDepth = this.reader.ReadInt16();
                this.DataId = this.reader.ReadBytes(4);
                this.DataChunkSize = this.reader.ReadInt32();
            } catch (Exception e) {
                Console.WriteLine(e);
                return false;
            }

            if (Encoding.ASCII.GetString(this.ChunkId) != "RIFF") return false;
            if (Encoding.ASCII.GetString(this.TypeId) != "WAVE") return false;
            if (Encoding.ASCII.GetString(this.FormatId) != "fmt ") return false;
            if (Encoding.ASCII.GetString(this.DataId) != "data") return false;
            if (this.AudioFormat != 1) return false;

            return true;
        }
    }
}
