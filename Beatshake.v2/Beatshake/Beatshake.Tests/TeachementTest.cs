// <copyright file="TeachementTest.cs">Copyright ©  2014</copyright>

using System;
using System.Collections.Generic;
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

        [Theory]
        [InlineData(6,1,4)]
        public Teachement CreateTest(int pointCount, int peakValue, int peakPosition)
        {
            BeatshakeSettings.SamplePoints = pointCount;
            var timeSteps = DataGenerator.GetData(pointCount);
            IEnumerable<double> xVal, yval, zval;
            xVal = yval = zval = DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition);
            var teachement = Teachement.Create(timeSteps, xVal, yval, zval);
            return teachement;
        }

    }
}
