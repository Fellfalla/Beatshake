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
        [InlineData(6,1,4,4)]
        public Teachement CreateTest(int pointCount, int peakValue, int peakPosition, int samplePoints)
        {
            BeatshakeSettings.SamplePoints = samplePoints;
            var timeSteps = DataGenerator.GetData(pointCount);
            IEnumerable<double> xVal, yval, zval;
            xVal = yval = zval = DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition);
            var teachement = Teachement.Create(timeSteps, xVal, yval, zval);
            return teachement;
        }
        
        [Theory]
        [InlineData(true,   1,          3, 1,       2)]
        [InlineData(true,   0.1,        3, 1,       2)]
        [InlineData(true,   0.01,       3, 1,       2)]
        [InlineData(true,   0.0001,     3, 1,       2)]
        [InlineData(true,   0.0001,     3, 346,     2)] // Methods should normalize
        [InlineData(false,  0,          6, 1,       4)]
        [InlineData(true,  0.3,        6, 1,       4)]
        [InlineData(true,  0.3,        6, 1,       4)]
        [InlineData(true,  0.3,        6, 124,       4)]
        public void TeachementCalculateErrorTest(bool shallFit, double maxError, int pointCount, int peakValue, int peakPosition)
        {
            int beatshakeSamplePoints = peakPosition;
            var teachement = CreateTest(pointCount, peakValue, peakPosition, beatshakeSamplePoints);
            var timeSteps = DataGenerator.GetData(pointCount).ToArray();
            var first = new PolynomialFunction(timeSteps, DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition).ToArray());
            var second = new PolynomialFunction(timeSteps, DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition).ToArray());
            var third = new PolynomialFunction(timeSteps, DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition).ToArray());
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
