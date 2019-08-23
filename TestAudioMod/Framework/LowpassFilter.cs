using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathoschild.Stardew.TestAudioMod.Framework {
    internal class LowpassFilter : BiquadraticFilter {
        /// <summary>
        /// Constructor for a Low Pass Filter
        ///
        /// The filter's transfer function is:
        /// H(s) = (wm/Q) / (s^2 + s + 1)
        /// </summary>
        /// <param name="SampleRate">The Sampling Rate to be processed through the filter.</param>
        /// <param name="CutoffFrequency">The Cutoff Frequency the filter will operate on.</param>
        /// <param name="QFactor">The QFactor of the filter.</param>
        public LowpassFilter(int SampleRate, float CutoffFrequency, float QFactor) : base(SampleRate, CutoffFrequency, QFactor) {

        }

        /// <summary>
        /// Calculates the coefficients to produce the needed values for a Lowpass Filter.
        /// </summary>
        protected override void CalculateCoefficients() {
            double omegaNaught = 2 * Math.PI * this.Frequency / this.SampleRate;
            double cos_omegaNaught = Math.Cos(omegaNaught);
            double alpha = Math.Sin(omegaNaught) / (2 * this.QFactor);

            double b0 = (1 - cos_omegaNaught) / 2;
            double b1 = 1 - cos_omegaNaught;
            double b2 = (1 - cos_omegaNaught) / 2;
            double a0 = 1 + alpha;
            double a1 = -2 * cos_omegaNaught;
            double a2 = 1 - alpha;

            this.a0 = b0 / a0;
            this.a1 = b1 / a0;
            this.a2 = b2 / a0;
            this.a3 = a1 / a0;
            this.a4 = a2 / a0;
        }
    }
}
