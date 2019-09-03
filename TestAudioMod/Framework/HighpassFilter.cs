using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.TestAudioMod.Framework {
    internal class HighpassFilter : BiquadraticFilter {
        /// <summary>
        /// Constructor for a High Pass Filter
        /// </summary>
        /// <param name="SampleRate">The Sampling Rate to be processed through the filter.</param>
        /// <param name="Frequency">The Frequency the filter will operate.</param>
        /// <param name="QFactor">The QFactor of the filter.</param>
        public HighpassFilter(int SampleRate, double Frequency, double Q) : base(SampleRate, Frequency, Q) {

        }

        /// <summary>Calculates the coefficients to produce the needed values for a Highpass Filter.</summary>
        protected override void CalculateCoefficients() {
            double K = Math.Tan(MathHelper.Pi * this.Fc / this.SampleRate);
            double normal = 1 / (1 + K / this.QFactor + K * K);
            this.a0 = 1 * normal;
            this.a1 = -2 * this.a0;
            this.a2 = this.a0;
            this.b1 = 2 * (K * K - 1) * normal;
            this.b2 = (1 - K / this.QFactor + K * K) * normal;
        }
    }
}
