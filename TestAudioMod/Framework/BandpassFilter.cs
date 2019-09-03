using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.TestAudioMod.Framework {
    internal class BandpassFilter : BiquadraticFilter {
        /// <summary>
        /// Constructor for a Band Pass Filter.
        ///
        /// The filter's transfer function is:
        /// H(s) = (wm/Q * s) / (s^2 + (wm/Q * s) + wm^2)
        /// 
        /// </summary>
        /// <param name="SampleRate">The Sampling Rate to be processed through the filter.</param>
        /// <param name="Frequency">The Centre Frequency the filter will operate on.</param>
        /// <param name="QFactor">The Quality Factor of the filter.</param>
        public BandpassFilter(int SampleRate, double Frequency, double QFactor) : base(SampleRate, Frequency, QFactor) {

        }

        /// <summary>Calculates the coefficients to produce the needed values for a Bandpass Filter.</summary>
        protected override void CalculateCoefficients() {
            double K = Math.Tan(MathHelper.Pi * this.Fc / this.SampleRate);
            double normal = 1 / (1 + K / this.QFactor + K * K);
            this.a0 = K / this.QFactor * normal;
            this.a1 = 0;
            this.a2 = -this.a0;
            this.b1 = 2 * (K * K - 1) * normal;
            this.b2 = (1 - K / this.QFactor + K * K) * normal;
        }
    }
}
