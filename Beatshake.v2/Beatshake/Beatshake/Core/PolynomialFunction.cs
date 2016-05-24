using System;
using Beatshake.ExtensionMethods;

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

        /// <summary>
        /// Repressenting the coefficients of a polynomial function.
        /// The lower the index, the lower the exponent of the correlating variable.
        /// This means that the Variable with the highest exponen is multiplicated with the very last <see cref="Coefficients"/> entry.
        /// </summary>
        public double[] Coefficients
        {
            get { return _coefficients; }
            set { _coefficients = value; }
        }

        /// <summary>
        /// Source: https://de.wikipedia.org/wiki/Polynom -> Eigenschafte
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetGradient(double x)
        {
            double grad = 0;
            // f'(x) = 2ax + b
            // the gradient is at interval beginning or ending the greatest
            for (int i = 0; i < Coefficients.Length - 1 ; i++)
            {
                var degree = i + 1;
                grad += (degree) * Coefficients[degree] * x.FastPower((uint) i);

            }
            return grad;
        }


    }
}