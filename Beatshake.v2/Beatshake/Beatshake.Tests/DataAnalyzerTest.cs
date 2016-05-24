using System;
using System.Collections.Generic;
using System.Linq;
using Beatshake.Core;
using Beatshake.ExtensionMethods;
using Xunit;

namespace Beatshake.Tests
{
    public class DataAnalyzerTest
    {
        [Theory]
        [InlineData(6, 1, 4)]
        [InlineData(6, -1, 4)]
        public void GetMultifunctionalPeakTest(int pointCount, int peakValue, int peakPosition)
        {
            // Init testing context
            BeatshakeSettings.SamplePoints = pointCount;
            var timeSteps = DataGenerator.GetData(pointCount).ToList();
            IEnumerable<double> xVal, yval, zval;
            xVal = yval = zval = DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition);
            var data = new List<IList<double>>() {xVal.ToList(), yval.ToList(), zval.ToList()}.ToList();

            // Run Test
            var peak = DataAnalyzer.GetPeak(timeSteps, data);
            Assert.Equal(peakPosition, peak);
        }



        [Fact]
        public void CalculateCoefficientsTest()
        {
            // those data come from the PDF example where the formulars come from
            double[] X = new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0 };
            double[] Y = { 0.38, 1.15, 2.71, 3.92, 5.93, 8.56, 11.24 };
            var checkResult = Tuple.Create(0.19642857, 0.23642857, -0.03285714);

            var coefficients = DataAnalyzer.CalculateCoefficients(X, Y);

            Assert.True(coefficients.Item1.IsAlmostEqual(checkResult.Item1, 0.0000001), string.Format("first: {0}\nsecond:{1}", coefficients.Item1, checkResult.Item1));
            Assert.True(coefficients.Item2.IsAlmostEqual(checkResult.Item2, 0.0000001), string.Format("first: {0}\nsecond:{1}", coefficients.Item2, checkResult.Item2));
            Assert.True(coefficients.Item3.IsAlmostEqual(checkResult.Item3, 0.0000001), string.Format("first: {0}\nsecond:{1}", coefficients.Item3, checkResult.Item3));

        }

    }

    public class UtilityTest
    {
        [Fact]
        public void MidnightFormulaTest()
        {
            double[] result;

            // x^2
            result = Utility.MidnightFormula(1, 0, 0).ToArray();
            Assert.Equal(1, result.Count());
            Assert.Equal(0, result[0]);

            // x^2 + 1
            result = Utility.MidnightFormula(1, 0, 1).ToArray();
            Assert.Equal(0, result.Count());

            // x^2 - 1
            result = Utility.MidnightFormula(1, 0, -1).ToArray();
            Assert.Equal(2, result.Count());
            Assert.True(result.Contains(1));
            Assert.True(result.Contains(-1));
        }
    }

    public class QuadraticFunctionTest
    {
        [Fact]
        public void CoefficientTest()
        {
            var func = new QuadraticFunction();
            double c0 = 0;
            double c1 = 1;
            double c2 = 2;
            func.Coefficients = new Tuple<double, double, double>(c2,c1,c0);

            Assert.Equal(c0, func.C);
            Assert.Equal(c1, func.B);
            Assert.Equal(c2, func.A);

            Assert.Equal(c0, ((PolynomialFunction)func).Coefficients[0]);
            Assert.Equal(c1, ((PolynomialFunction)func).Coefficients[1]);
            Assert.Equal(c2, ((PolynomialFunction)func).Coefficients[2]);

        }
    }
}
