using System;

namespace Beatshake.Core
{
    public class PolynomialFunction
    {
        private double[] _coefficients;

        /// <summary>
        /// -1 If there are no Coefficients set.
        /// When new degree is set to fewer element count than old one is -> highest coefficients will be ignored and get lost
        /// </summary>
        public int Degree
        {
            get
            {
                if (Coefficients != null) return Coefficients.Length - 1;
                else
                {
                    return -1;
                }
            }
            set
            {
                if (value == -1)
                {
                    // Reset the function
                    Reset();
                    return;
                }
                else if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("value", "Value >= -1 excpected");
                }

                var coefficientCount = value + 1;
                var newCoefficients = new double[coefficientCount];

                if (Coefficients == null)
                {
                    // skip all other Ifs
                }
                else if (coefficientCount > Coefficients.Length)
                {
                    Array.Copy(Coefficients, newCoefficients, Coefficients.Length);
                }
                else if (coefficientCount == Coefficients.Length)
                {
                    // no resize needed
                    return;
                }
                else // new Array is smaller
                {
                    // Delete highest old coefficients and copy remaining ones
                    Array.Copy(Coefficients, newCoefficients, newCoefficients.Length);
                }

                Coefficients = newCoefficients;
            }
        }

        /// <summary>
        /// Removes all the coefficients and sets the function to initial state
        /// </summary>
        public void Reset()
        {
            Coefficients = null;
        }

        public double[] Coefficients
        {
            get { return _coefficients; }
            set { _coefficients = value; }
        }
    }
}