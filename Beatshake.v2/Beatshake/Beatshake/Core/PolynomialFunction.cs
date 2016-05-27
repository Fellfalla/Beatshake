using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beatshake.ExtensionMethods;

namespace Beatshake.Core
{
    public enum DataFittingStrategy
    {
        LeastSquares,
        LagrangePolynom,
    }

    public class PolynomialFunction : ICloneable
    {
        public PolynomialFunction()
        {
            
        }

        public PolynomialFunction(double[] samplePoints, double[] sampleValues, int n = 2, DataFittingStrategy method = DataFittingStrategy.LeastSquares)
        {
            if (n != 2 || method != DataFittingStrategy.LeastSquares)
            {
                throw new NotImplementedException();
            }

            var tuple = DataAnalyzer.CalculateCoefficients(samplePoints, sampleValues);

            Coefficients = new double[] {tuple.Item3, tuple.Item2, tuple.Item1};

            Start = samplePoints.First();
            End = samplePoints.Last();
        }

        public PolynomialFunction(IEnumerable<double> samplePoints, IEnumerable<double> sampleValues) : this(samplePoints.ToArray(), sampleValues.ToArray())
        {
        }

        private double[] _coefficients;

        /// <summary>
        /// -1 If there are no Coefficients set.
        /// When new degree is set to fewer element count than old one is -> highest coefficients will be ignored and get lost
        /// </summary>
        public int Degree // todo: Write tests
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
        /// Start of the definition region of this function
        /// </summary>
        public double Start { get; set; }

        /// <summary>
        /// End of the definiction region of this function
        /// </summary>
        public double End { get; set; }

        /// <summary>
        /// Repressenting the coefficients of a polynomial function.
        /// The lower the index, the lower the exponent of the correlating variable.
        /// This means that the Variable with the highest exponent is multiplicated with the very last <see cref="Coefficients"/> entry.
        /// !!! Changes in this array wont be detected !!!
        /// </summary>
        public double[] Coefficients
        {
            get { return _coefficients; }
            set
            {
                _coefficients = value;
                _normalizedFunction = null; // Reset normalized version
            }
        }

        /// <summary>
        /// Source: https://de.wikipedia.org/wiki/Polynom -> Eigenschafte
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetGradient(double x)
        {
            // this commented code works and
            // is not deleted for eventually better perfomance in future
            //double grad = 0;
            //// f'(x) = 2ax + b
            //// the gradient is at interval beginning or ending the greatest
            //for (int i = 0; i < Coefficients.Length - 1 ; i++)
            //{
            //    var degree = i + 1;
            //    grad += (degree) * Coefficients[degree] * x.FastPower((uint) i);

            //}
            var deriv = GetDerivation();

            return deriv.ValueAt(x);
        }

        public PolynomialFunction GetDerivation()
        {
            var derivation = new PolynomialFunction();
            double[] derivedCoefficients = new double[Coefficients.Length-1];

            // multiply all coefficients with the value of the correlating x-exponent
            for (int i = 1; i < Coefficients.Length; i++)
            {
                derivedCoefficients[i - 1] = Coefficients[i]*i;
            }

            derivation.Coefficients = derivedCoefficients;
            return derivation;
        }

        public IEnumerable<double> GetPeaks()
        {
            var derivation = GetDerivation();
            if (derivation.Degree <= 0)
            {
                yield break;
            }
            else if (derivation.Degree == 1)
            {
                // f(x) = ax + b =!= 0 falls ax = -b -> x = -b/a
                yield return -derivation.Coefficients[0]/ derivation.Coefficients[1];
            }
            else if (derivation.Degree == 2)
            {
                var a = derivation.Coefficients[2];
                var b = derivation.Coefficients[1];
                var c = derivation.Coefficients[0];
                // Midnignt formular

                foreach (var nst in Utility.MidnightFormula(a, b, c))
                {
                    yield return nst;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public int CoefficientCount
        {
            get
            {
                return Coefficients?.Length ?? 0;
            }
        }

        public void Scale(double scale)
        {
            for (int i = 0; i < CoefficientCount; i++) // this could be parallelized
            {
                Coefficients[i] *= scale;
            }
        }

        /// <summary>
        /// Normalizes the function in a way, that the highest peak has a value of 1
        /// </summary>
        /// <returns></returns>
        public PolynomialFunction Normalize()
        {
            if (_normalizedFunction == null)
            {
                double maxValue = GetPeaks().Max();
                double scale = 1 / maxValue;
                _normalizedFunction = (PolynomialFunction)Clone();
                _normalizedFunction.Scale(scale);
            }

            return (PolynomialFunction) _normalizedFunction.Clone();

        }

        private PolynomialFunction _normalizedFunction;

        /// <summary>
        /// Clones the executing class
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var clone = new PolynomialFunction();

            clone.Coefficients = Coefficients;

            return clone;
        }

        /// <summary>
        /// Returns the functional-Value f(x) at a specific point x
        /// </summary>
        /// <param name="x">The point where this function should be evaluated</param>
        /// <returns></returns>
        public double ValueAt(double x)
        {
            double result = 0;
            for (uint i = 0; i < CoefficientCount; i++)
            {
                result += Coefficients[i]*x.FastPower(i);
            }

            return result;
        }

        public double GetCoefficient(int degree)
        {
            if (Coefficients == null)
            {
                return 0;
            }
            if (degree < Coefficients.Length)
            {
                return Coefficients[degree];
            }
            else // the x^degree is eliminated by a coefficient with value 0
            {
                return 0;
            }
        }

        public double GetIntegralDifference(PolynomialFunction other, double start, double end)
        {
            double integral = 0; // this integral means the speed difference

            // sort for func with highest degree
            PolynomialFunction func1;
            PolynomialFunction func2;
            if (this.Degree >= other.Degree)
            {
                func1 = this;
                func2 = other;
            }
            else
            {
                func1 = other;
                func2 = this;
            }

            if (func1.Degree == -1)
            {
                return 0;
            }
            if (func1.Degree == 0) // const
            {
                integral = end - start*Math.Abs(func1.GetCoefficient(0) - func2.GetCoefficient(0));
            }
            else if (func1.Degree == 1) // lin
            {
                // Substract functions from each other
                var a = func1.GetCoefficient(1) - func2.GetCoefficient(1);
                var b = func1.GetCoefficient(0) - func2.GetCoefficient(0);

                // https://www.wolframalpha.com/input/?i=integral+abs%28ax%2Bb%29
                var upper = 0.5*(end*(a*end + 2*b)*Math.Sign(a*end + b));
                var lower = 0.5*(start*(a* start + 2*b)*Math.Sign(a* start + b));
                integral = upper - lower;
            }
            else if (func1.Degree == 2)
            {
                // https://www.wolframalpha.com/input/?i=integral+abs%28ax^2%2Bbx%2Bc%29
                var a = func1.GetCoefficient(2) - func2.GetCoefficient(2);
                var b = func1.GetCoefficient(1) - func2.GetCoefficient(1);
                var c = func1.GetCoefficient(0) - func2.GetCoefficient(0);

                var upper = (end*(6*c + end*(3*b + 2*a*end))*Math.Sign(c + end*(b + a*end))) / 6;
                var lower = (start*(6*c + start * (3*b + 2*a* start))*Math.Sign(c + start * (b + a* start))) / 6;
                return upper - lower;
            }
            else // solve numerical
            {
                double delta = (end - start) / BeatshakeSettings.IntegralPrecision;
                for (double x = start; x < end; x += delta)
                {
                    var diff = other.ValueAt(x) - ValueAt(x);
                    integral += Math.Abs(diff) * delta;
                }
            }

            return integral;
        }

        public IEnumerable<double> GetIntersectionsWith(PolynomialFunction other)
        {
            // todo: test
            if (this.Degree != 2 ||other.Degree != 2)
            {
                throw new NotImplementedException();
            }
            else
            {
                // (a1 - a2)x^2 + (b1 - b2)t + (c1 - c2)
                var a = this.GetCoefficient(2) - other.GetCoefficient(2);
                var b = this.GetCoefficient(1) - other.GetCoefficient(1);
                var c = this.GetCoefficient(0) - other.GetCoefficient(0);
                return Utility.MidnightFormula(a, b, c);
            }

        }


        /// <summary>Gibt eine Zeichenfolge zurück, die das aktuelle Objekt darstellt.</summary>
        /// <returns>Eine Zeichenfolge, die das aktuelle Objekt darstellt.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < CoefficientCount; i++)
            {
                builder.Append(string.Format(" {0:+#;-#}x^{1}", GetCoefficient(i), i));
            }
            
            return builder.ToString().Trim();
        }
    }

    public interface ICloneable
    {
        /// <summary>
        /// Clones the executing class
        /// </summary>
        /// <returns></returns>
        object Clone();
    }
}