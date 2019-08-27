using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public BandpassFilter(int SampleRate, float Frequency, float QFactor) : base(SampleRate, Frequency, QFactor) {

        }

        /// <summary>
        /// Calculates the coefficients to produce the needed values for a Bandpass Filter.
        /// </summary>
        protected override void CalculateCoefficients() {
            double omegaNaught = 2 * Math.PI * this.Frequency / (float)this.SampleRate;
            double cos_omegaNaught = Math.Cos(omegaNaught);
            double sin_omegaNaught = Math.Sin(omegaNaught);
            double alpha = sin_omegaNaught / (2 * this.QFactor);

            double b0 = alpha;
            double b1 = 0;
            double b2 = -alpha;
            double a0 = 1 + alpha;
            double a1 = -2 * cos_omegaNaught;
            double a2 = 1 - alpha;

            this.a0 = b0 / a0;
            this.a1 = b1 / a0;
            this.a2 = b2 / a0;

        }
    }
}
