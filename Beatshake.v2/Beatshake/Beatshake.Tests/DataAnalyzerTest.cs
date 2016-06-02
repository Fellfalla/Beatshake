using System;
using System.Collections.Generic;
using System.Linq;
using Beatshake.Core;
using Xunit;

namespace Beatshake.Tests
{
    public class DataAnalyzerTest
    {

        /// <summary>
        /// This tests if the evaluation of multiple polynomial functions return the expected peaks
        /// </summary>
        /// <param name="pointCount"></param>
        /// <param name="peakValue"></param>
        /// <param name="peakPosition"></param>
        [Theory]
        [InlineData(3, 1, 1)]
        [InlineData(3, -1, 1)]
        [InlineData(3, 253.32, 1)]
        [InlineData(3, -235, 1)]
        [InlineData(3, 1, 0)]
        [InlineData(3, -1, 0)]
        [InlineData(3, 1, 2)]
        [InlineData(3, -1, 2)]
        public void GetPeakTest(int pointCount, int peakValue, int peakPosition)
        {
            // Init testing context
            BeatshakeSettings.SamplePoints = pointCount;
            IEnumerable<double> xVal, yval, zval;
            xVal = yval = zval = DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition);

            // Run Test
            var peak = DataAnalyzer.GetPeak(xVal.ToArray(), yval.ToArray(), zval.ToArray());
            Assert.Equal(peakPosition, peak);
        }



        [Fact]
        public void CalculateCoefficientsTest()
        {
            // those data come from the PDF example where the formulars come from
            double[] x = new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0 };
            double[] y = { 0.38, 1.15, 2.71, 3.92, 5.93, 8.56, 11.24 };
            var checkResult = Tuple.Create(0.19642857, 0.23642857, -0.03285714);
            var coefficients = DataAnalyzer.CalculateCoefficients(x, y);

            Assert.Equal(checkResult.Item1, coefficients[2], 8);
            Assert.Equal(checkResult.Item2, coefficients[1], 8);
            Assert.Equal(checkResult.Item3, coefficients[0], 8);

            // low data count shall not throw error
            x = y = DataGenerator.GetData(2).ToArray();
            DataAnalyzer.CalculateCoefficients(x, y);
            x = y = DataGenerator.GetData(1).ToArray();
            DataAnalyzer.CalculateCoefficients(x, y);
        }

        [Theory]
        [InlineData(0,1,1,5,1,4)]
        [InlineData(0,1,0,0,0,0)]
        [InlineData(0,1,5,1,5,-4)]
        public void LinearInterpolationTest(double x1, double x2, double y1, double y2, double expect1, double expect2)
        {

            var coeffs = DataAnalyzer.LinearInterpolation(x1, x2, y1, y2);
            Assert.Equal(expect1, coeffs[0]);
            Assert.Equal(expect2, coeffs[1]);


        }

    }
}
