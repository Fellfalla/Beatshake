using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public static class DataAnalyzer
    {
        /// <summary>
        /// The Tuple contains the three coefficients for a quadratic polynome f(x) = ax^2 + bx + c
        /// </summary>
        public static Tuple<double, double, double> CalculateCoefficients(IList<double> X, IList<double> Y)
        {
            if (X.Count != Y.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(Y), "Both arrays have to be of same size");
            }

            int measurePoints = X.Count;
            double[] XY = new double[measurePoints];
            double[] XX = new double[measurePoints];
            double[] XXX = new double[measurePoints];
            double[] XXXX = new double[measurePoints];
            double[] YXX = new double[measurePoints];

            // initialize Arrays
            for (int i = 0; i < measurePoints; i++)
            {
                XY[i] = X[i] * Y[i];
                XX[i] = X[i] * X[i];
                XXX[i] = X[i] * X[i] * X[i];
                XXXX[i] = X[i] * X[i] * X[i] * X[i];
                YXX[i] = X[i] * X[i] * Y[i];
            }

            double AvX = X.Average();
            double AvY = Y.Average();
            double AvXY = XY.Average();
            double AvXX = XX.Average();
            double AvXXX = XXX.Average();
            double AvXXXX = XXXX.Average();
            double AvYXX = YXX.Average();

            double aNum = (AvYXX - AvY * AvXX) * (AvXX - AvX * AvX) - ( AvXY - AvY * AvX ) * (AvXXX - AvX * AvXX) ;
            double aDom = (AvXXXX - AvXX * AvXX) * (AvXX - AvX * AvX) - Math.Pow(AvXXX - AvX*AvXX, 2);
            double a =  aNum / aDom;

            double bNum = AvXY - AvY*AvX - a*(AvXXX - AvX*AvXX);
            double bDom = AvXX - AvX*AvX;
            double b = bNum / bDom;

            double c = AvY - a*AvXX - b * AvX;

            return Tuple.Create(a,b,c);
        }

        /// <summary>
        /// Returns the x-Value where the peak will be expected.
        /// Min values and Max values are both handled as potential peaks.
        /// </summary>
        /// <param name="coefficients"></param>
        /// <returns></returns>
        public static double GetPeak(Tuple<double, double, double> coefficients)
        {
            // f(x)     =   ax^2 + bx + c
            // f'(x)    =   2ax + b
            // f''(x)   =   2a

            // Peak is at f'(x) = 0
            // x = -b / 2a

            return (-coefficients.Item2)/(2*coefficients.Item1);

        }

        /// <summary>
        /// Looks for the highest value of the given data.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="values"></param>
        /// <param name="index">Index of the peak value. Returns -1 if theres no peak.</param>
        /// <returns></returns>
        public static double GetPeak(IList<double> x, IList<IList<double>> values, out int index)
        {
            double max = double.NegativeInfinity;
            double maxX = double.NaN;
            double dataSets = values.Count;
            index = -1;

            // Get max Y-value and return that x value
            for (int i = 0; i < x.Count; i++)
            {
                double abs = 0;
                for (int j = 0; j < dataSets; j++)
                {
                    abs += Math.Abs(values[j][i]);
                }

                if (abs > max)
                {
                    max = abs;
                    maxX = x[i];
                    index = i;
                }
            }
            return maxX;
        }

        public static IEnumerable<double> GetFunctionIntersections(Tuple<double, double, double> func1,
            Tuple<double, double, double> func2)
        {
            // (a1 - a2)x^2 + (b1 - b2)x + (c1 - c2)
            var a = func1.Item1 - func2.Item1;
            var b = func1.Item2 - func2.Item2;
            var c = func1.Item3 - func2.Item3;
            var radicand = b*b - 4*a*c; //b^2 -4ac
            var dominator = 2*a;
            if (radicand < 0)
            {
                yield return double.NaN;
            }
            else if (Math.Abs(radicand) <= double.MinValue)
            {
                yield return -b/dominator;
            }
            else
            {
                var sqrtResult = Math.Sqrt(radicand);
                yield return (-b + sqrtResult)/dominator;
                yield return (-b - sqrtResult)/dominator;
            }
            yield break;
        }

        public static Tuple<double, double, double> NormalizeQuadraticFunction(Tuple<double, double, double> coefficients)
        {
            double maxValue = GetPeak(coefficients);
            double scale = 1/maxValue;
            return Tuple.Create(coefficients.Item1*scale, coefficients.Item2 * scale, coefficients.Item3 * scale);

        }

        public static bool IsMax(Tuple<double, double, double> coefficients)
        {
            return coefficients.Item1 <= 0;
        }

        public static bool IsMin(Tuple<double, double, double> coefficients)
        {
            return coefficients.Item1 >= 0;
        }
    }
}
