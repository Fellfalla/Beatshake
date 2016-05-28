// <copyright file="TeachementTest.cs">Copyright ©  2014</copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Beatshake.Core;
using Xunit;
using Xunit.Sdk;

namespace Beatshake.Tests
{
    /// <summary>This class contains parameterized unit tests for Teachement</summary>
    public class TeachementTest
    {
       [Fact]
       public void SmokeTest()
        {
            var teachement = new Teachement();
        }

        //[Fact]
        //public Teachement CreateTest()
        //{
        //} 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointCount"></param>
        /// <param name="peakValue"></param>
        /// <param name="peakPosition"></param>
        /// <param name="samplePoints">Samplepoints should no be greater than <paramref name="peakPosition"/></param>
        /// <returns></returns>
        [Theory]
        [InlineData(6,  1,  4,  4)]
        [InlineData(3,  1,  1,  3)]
        public Teachement CreateTest(int pointCount, double peakValue, int peakPosition, int samplePoints)
        {
            BeatshakeSettings.SamplePoints = samplePoints;
            var timeSteps = DataGenerator.GetData(pointCount);
            IEnumerable<double> xVal, yval, zval;
            xVal = yval = zval = DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition);
            var teachement = Teachement.Create(timeSteps, xVal, yval, zval);
            NoNaNsInTeachement(teachement);
            return teachement;
        }

        private void NoNaNsInTeachement(Teachement teachement)
        {
            Assert.NotEqual(double.NaN, teachement.XCurve.Coefficients[0]);
            Assert.NotEqual(double.NaN, teachement.XCurve.Coefficients[1]);
            Assert.NotEqual(double.NaN, teachement.XCurve.Coefficients[2]);
            Assert.NotEqual(double.NaN, teachement.YCurve.Coefficients[0]);
            Assert.NotEqual(double.NaN, teachement.YCurve.Coefficients[1]);
            Assert.NotEqual(double.NaN, teachement.YCurve.Coefficients[2]);
            Assert.NotEqual(double.NaN, teachement.ZCurve.Coefficients[0]);
            Assert.NotEqual(double.NaN, teachement.ZCurve.Coefficients[1]);
            Assert.NotEqual(double.NaN, teachement.ZCurve.Coefficients[2]);
        }

        [Theory]
        //          fits    maxError    points  peak    peakPos
        // data with peak at last pos shall fit
        [InlineData(true,   1,          3,      1,      2)]
        [InlineData(true,   0.1,        3,      1,      2)]
        [InlineData(true,   0.01,       3,      1,      2)]
        [InlineData(true,   0.0001,     3,      1,      2)]
        [InlineData(true,   0.0001,     3,      -1,     2)]
        [InlineData(true,   0.0001,     3,      346,    2)] // Methods should normalize 
        [InlineData(true,   0.5,        8,      0.1,    7)]
        [InlineData(true,   0.03,       8,      1,      7)]
        [InlineData(true,   0.03,       8,      1123,   7)]
        [InlineData(true,   0.3,        6,      1,      5)]
        [InlineData(true,   0.3,        6,      -1,     5)]
        [InlineData(true,   0.3,        6,      124,    5)]
        [InlineData(true,   0.5,       15,      1123,   14)]

        // data with peak not at last pos shall not fit
        [InlineData(false,  0,          6,      1,      4)]
        [InlineData(false,  0.3,        6,      1,      4)]
        [InlineData(false,  0.3,        6,      1,      4)]
        [InlineData(false,  0.3,        6,      124,    4)]
        [InlineData(false,  0.5,        15,     1123,   7)]
        [InlineData(false,   1,          3,      1,      1)]
        [InlineData(false,   0.1,        3,      1,      1)]
        [InlineData(false,   0.01,       3,      1,      1)]
        [InlineData(false,   0.0001,     3,      1,      1)]
        [InlineData(false,   0.0001,     3,      346,    1)] // Methods should normalize
        public void TeachementCalculateErrorTest(bool shallFit, double maxError, int pointCount, double peakValue, int peakPosition)
        {
            // initialize environment settings
            int beatshakeSamplePoints = peakPosition;

            // Create teachement from input data (peak values will be last position)
            var teachement = CreateTest(pointCount, peakValue, peakPosition, beatshakeSamplePoints);

            // Create actual functions from input data
            var timeSteps = DataGenerator.GetData(pointCount).ToArray();
            var first = new PolynomialFunction(timeSteps, DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition).ToArray());
            var second = new PolynomialFunction(timeSteps, DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition).ToArray());
            var third = new PolynomialFunction(timeSteps, DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition).ToArray());

            // get the error
            var error = teachement.GetError(0, timeSteps.Last(), true, first, second, third);
            if (shallFit)
            {

                Assert.InRange(error, 0, maxError);
            }            
            else
            {
                Assert.NotInRange(error, 0, maxError);
            }
        }

    }
}
