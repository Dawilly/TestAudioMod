using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.TestAudioMod.Framework.Filters {
    internal class LowpassFilter : BiquadraticFilter {
        /// <summary>
        /// Constructor for a Low Pass Filter
        ///
        /// The filter's transfer function is:
        /// H(s) = (wm/Q) / (s^2 + s + 1)
        /// </summary>
        /// <param name="SampleRate">The Sampling Rate to be processed through the filter.</param>
        /// <param name="Frequency">The Frequency the filter will operate.</param>
        /// <param name="QFactor">The QFactor of the filter.</param>
        public LowpassFilter(int SampleRate, double Frequency, double QFactor) : base(SampleRate, Frequency, QFactor) {

        }

        /// <summary>Calculates the coefficients to produce the needed values for a Lowpass Filter.</summary>
        protected override void CalculateCoefficients() {
            double K = Math.Tan(MathHelper.Pi * this.Fc / this.SampleRate);
            double normal = 1 / (1 + K / this.QFactor + K * K);
            this.a0 = K * K * normal;
            this.a1 = 2 * this.a0;
            this.a2 = this.a0;
            this.b1 = 2 * (K * K - 1) * normal;
            this.b2 = (1 - K / this.QFactor + K * K) * normal;
        }
    }
}
