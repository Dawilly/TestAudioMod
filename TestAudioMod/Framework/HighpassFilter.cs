using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework {
    internal class HighpassFilter : BiquadraticFilter {
        public HighpassFilter(int SampleRate, float Frequency, float Q) : base(SampleRate, Frequency, Q) {

        }

        protected override void CalculateCoefficients() {
            throw new NotImplementedException();
        }
    }
}
