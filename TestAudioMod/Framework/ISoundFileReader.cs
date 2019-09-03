using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework {
    public interface ISoundFileReader {
        int Channels { get; }
        TimeSpan TotalTime { get; }
        int SampleRate { get; }
        TimeSpan DecodedTime { get; set; }


        void Dispose();
        int ReadSamples(float[] buffer, int offset, int count);
        void Reset();
    }
}
