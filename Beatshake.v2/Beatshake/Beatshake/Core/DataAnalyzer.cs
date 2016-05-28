using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.ExtensionMethods;

namespace Beatshake.Core
{
    /// <summary>
    /// There are existing packaged:
    /// maybe checkout following: http://www.alglib.net/
    /// </summary>
    public static class DataAnalyzer
    {
        public static double[] CalculateCoefficients(IEnumerable<double> samplePoints,
            IEnumerable<double> sampleValues)
        {
            return CalculateCoefficients(samplePoints.ToArray(), sampleValues.ToArray());
        }

        public static double[] LinearInterpolation(double x1, double x2, double y1, double y2)
        {
            var a = (y2 - y1)/(x2 - x1);
            var b = y1 - (x1*a);

            return new[] {b, a};
        }

        /// <summary>
        /// The Tuple contains the three coefficients for a quadratic polynome f(x) = ax^2 + bx + c
        /// </summary>
        public static double[] CalculateCoefficients(double[] x, double[] y)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "Both arrays have to be of same size");
            }

            int measurePoints = x.Length;
            double[] xy = new double[measurePoints];
            double[] xx = new double[measurePoints];
            double[] xxx = new double[measurePoints];
            double[] xxxx = new double[measurePoints];
            double[] yxx = new double[measurePoints];

            // initialize Arrays
            for (int i = 0; i < measurePoints; i++)
            {
                xy[i] = x[i] * y[i];
                xx[i] = x[i] * x[i];
                xxx[i] = x[i] * x[i] * x[i];
                xxxx[i] = x[i] * x[i] * x[i] * x[i];
                yxx[i] = x[i] * x[i] * y[i];
            }

            double avX = x.Average();
            double avY = y.Average();
            double avXy = xy.Average();
            double avXx = xx.Average();
            double avXxx = xxx.Average();
            double avXxxx = xxxx.Average();
            double avYxx = yxx.Average();

            double aNum = (avYxx - avY * avXx) * (avXx - avX * avX) - ( avXy - avY * avX ) * (avXxx - avX * avXx) ;
            double aDom = (avXxxx - avXx * avXx) * (avXx - avX * avX) - Math.Pow(avXxx - avX*avXx, 2);
            double a =  aNum / aDom;

            double bNum = avXy - avY*avX - a*(avXxx - avX*avXx);
            double bDom = avXx - avX*avX;
            double b = bNum / bDom;

            double c = avY - a*avXx - b * avX;

            return new []{c,b,a};
        }

        public static QuadraticFunction CalculateQuadraticFunction(IList<double> xData, IList<double> yData)
        {
            var function = new QuadraticFunction();
            function.Coefficients = CalculateCoefficients(xData, yData);
            function.Start = xData.FirstOrDefault();
            function.End = xData.LastOrDefault();
            return function;
        }

        /// <summary>
        /// Looks for the highest value of the given data.
        /// Therefore the dataset is smoothed and analyzed.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="values"></param>
        /// <returns>Index of the peak value. Returns -1 if theres no peak.</returns>
        public static double GetEstimatedPeak(IList<double> t, IList<IList<double>> values)
        {
            //double max = double.NegativeInfinity;
            //double maxX = double.NaN;
            int index = -1;

            // Get func-Aproximations for ALL Points
            var xFunctions = new List<PolynomialFunction>();
            var yFunctions = new List<PolynomialFunction>();
            var zFunctions = new List<PolynomialFunction>();
            //var functionGroup = new FunctionGroup(xFunctions, yFunctions, zFunctions);

            for (int i = 0; i <= t.Count - BeatshakeSettings.SamplePoints; i++)
            {
                int lastSamplePoint = i + BeatshakeSettings.SamplePoints - 1;
                var samplePoints = t.SubList(i, lastSamplePoint);
                var xFunction = new PolynomialFunction(samplePoints, values[0].SubList(i, lastSamplePoint));
                var yFunction = new PolynomialFunction(samplePoints, values[1].SubList(i, lastSamplePoint));
                var zFunction = new PolynomialFunction(samplePoints, values[2].SubList(i, lastSamplePoint));

                // Set start and Endpoints
                xFunction.Start = samplePoints[0];
                xFunction.End = samplePoints.Last();
                yFunction.Start = samplePoints[0];
                yFunction.End = samplePoints.Last();
                zFunction.Start = samplePoints[0];
                zFunction.End = samplePoints.Last();

                xFunctions.Add(xFunction);
                yFunctions.Add(yFunction);
                zFunctions.Add(zFunction);
            }

            // look for functions with highest derivation and get a prognose for the peak
            var biggestGrad = double.MinValue;
            
            // the gradient is at interval beginning or ending the greatest
            for (int i = 0; i < xFunctions.Count; i++)
            {
                var end = xFunctions[i].End;

                var gradX = xFunctions[i].GetGradient(end);
                var gradY = yFunctions[i].GetGradient(end);
                var gradZ = zFunctions[i].GetGradient(end);

                var absGrad = Math.Abs(gradX) + Math.Abs(gradY) + Math.Abs(gradZ);
                if (absGrad > biggestGrad)
                {
                    biggestGrad = absGrad;
                    index = i + BeatshakeSettings.SamplePoints; // we want the last sample point with the greatest gradient
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the index where the greatest absolute values are calculated
        /// </summary>
        /// <param name="values">All input value arrays have to be of same size.</param>
        /// <returns>-1 if no peak was detected</returns>
        public static int GetPeak(params double[][] values)
        {
            int dataSetCount = values.Count();
            int resultIndex = -1;

            // Test data for correctness
            if (dataSetCount == 0)
            {
                return resultIndex;
            }

            double maxVal = double.MinValue;
            double curVal;
            int dataSetLength = values.First().Count();

            // go through all indeces
            for (int i = 0; i < dataSetLength; i++)
            {
                curVal = 0;
                // take same index from the different data sets
                for (int j = 0; j < dataSetCount; j++) 
                {
                    curVal += Math.Abs(values[j][i]);
                }
                if (curVal > maxVal)
                {
                    maxVal = curVal;
                    resultIndex = i;
                }
            }

            return resultIndex;
        }


        public static bool IsMax(Tuple<double, double, double> coefficients)
        {
            return coefficients.Item1 <= 0;
        }

        public static bool IsMin(Tuple<double, double, double> coefficients)
        {
            return coefficients.Item1 >= 0;
        }


        public static bool AreFunctionsAlmostEqual(double integral, double tolerance, double range)
        {
            return (integral / range) < tolerance;
        }

        public static double GetIntegralDifference(Tuple<double, double, double> func1, Tuple<double, double, double> func2, double start, double end)
        {
            double integral = 0; // this integral means the speed difference
            int precision = BeatshakeSettings.IntegralPrecision;
            double delta = (end - start) / precision;
            for (double x = start; x < end; x += precision)
            {
                var diff = (func1.Item1 * x * x + func1.Item2 * x + func1.Item3) -
                           (func2.Item1 * x * x + func2.Item2 * x + func2.Item3);
                integral += Math.Abs(diff) * delta;
            }

            return integral;
        }

    }
}
