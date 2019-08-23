using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// By David Weil (SgtPickles)
namespace Pathoschild.Stardew.TestAudioMod.Framework {
    /// <summary>
    /// This class represents a second order (different equation) recurisve linear filter.
    /// It contains two poles (two zeroes in the denominator) and two zeroes (two zeroes in the numerator)
    /// of a transfer function. 
    ///
    /// In short, this is used to filter certain frequencies within a given audio signal.
    /// </summary>
    internal abstract class BiquadraticFilter {
        /// <summary>Unwrapped Frequency</summary>
        private float hertz;
        /// <summary>Unwrapped QFactor</summary>
        private float qFactor;

        /// <summary>Coefficient Variable. Represents a coefficient within a transfer function H(z).</summary>
        protected double a0;

        /// <summary>Coefficient Variable. Represents a coefficient within a transfer function H(z).</summary>
        protected double a1;

        /// <summary>Coefficient Variable. Represents a coefficient within a transfer function H(z).</summary>
        protected double a2;

        /// <summary>Coefficient Variable. Represents a coefficient within a transfer function H(z).</summary>
        protected double a3;

        /// <summary>Coefficient Variable. Represents a coefficient within a transfer function H(z).</summary>
        protected double a4;

        /// <summary>State Variable. Represents the previous signal computation.</summary>
        protected double ZA_1;

        /// <summary>State Variable. Represents the previous of the previous signal computation.</summary>
        protected double ZA_2;

        /// <summary>State Variable. Represents the previous signal computation.</summary>
        protected double ZB_1;

        /// <summary>State Variable. Represents the previous of the previous signal computation.</summary>
        protected double ZB_2;

        /// <summary>The Quality Factor of the filter.</summary>
        public float QFactor {
            get { return this.qFactor; }
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException("Q-Factor of Filter cannot be less than or equal to 0.");
                }
                this.qFactor = value;
                this.CalculateCoefficients();
            }
        }

        /// <summary>A provided frequency for the filter. Used for centre, cutoff, etc.</summary>
        public float Frequency {
            get { return this.hertz; }
            set {
                if (this.SampleRate < value * 2) {
                    throw new ArgumentOutOfRangeException("The SampleRate must be at least twice the size of Frequency.");
                }
                this.hertz = value;
                this.CalculateCoefficients();
            }
        }

        /// <summary>Sampling Rate to be processed through the filter. Must be greater than twice the frequency.</summary>
        public int SampleRate { get; private set; }

        /// <summary>
        /// Constructor for a Biquadractic Filter Class.
        /// </summary>
        /// <param name="SampleRate">The Sampling Rate to be processed through the filter.</param>
        /// <param name="Frequency">The Frequency the filter will operate on (centre, cutoff, etc).</param>
        /// <param name="QFactor">THe Quality Factor of the filter.</param>
        public BiquadraticFilter(int SampleRate, float Frequency, float QFactor) {
            // Validate the values.
            if (SampleRate <= 0) {
                throw new ArgumentOutOfRangeException("SampleRate must be greater than 0");
            } else if (Frequency <= 0) {
                throw new ArgumentOutOfRangeException("CuttOff Frequency must be greater than 0");
            } else if (QFactor <= 0) {
                throw new ArgumentOutOfRangeException("Q-Factor must be greater than 0");
            }

            this.SampleRate = SampleRate;
            this.Frequency = Frequency;
            this.QFactor = QFactor;
        }

        /// <summary>Process (Pass) a single point of a signal.</summary>
        /// <param name="input">A signal data point.</param>
        /// <returns>The processed data point.</returns>
        public float Process(float input) {
            double results = this.a0 * input + this.a1 * this.ZA_1 + this.a2 * this.ZA_2 - this.a3 * this.ZB_1 - this.a4 * this.ZB_2;
            this.UpdatePreviousSignals(input, results);
            return (float)results;
        }

        /// <summary>Process (Pass) an array of signal data points.</summary>
        /// <param name="input">An array of data points pertaining to a signal.</param>
        public void Process(float[] input) {
            for (int i = 0; i < input.Length; i++) input[i] = this.Process(input[i]);
        }

        /// <summary>
        /// Updates the previous signals (Memory), Z1 and Z2. Should be executed for each call of Process.
        /// </summary>
        /// <param name="input">The signal data point being processed.</param>
        /// <param name="output">The processed data point.</param>
        protected void UpdatePreviousSignals(float input, double output) {
            this.ZA_2 = this.ZA_1;
            this.ZA_1 = input;
            //--
            this.ZB_2 = this.ZB_1;
            this.ZB_1 = output;
        }

        /// <summary>
        /// Calculates the Coefficients of the transfer function. Changes based on the
        /// filter configuration, Frequency and SampleRate.
        /// </summary>
        protected abstract void CalculateCoefficients();
    }
}
