using System;
using System.Linq;
using Beatshake.ExtensionMethods;
using Xunit;
using Ploeh.AutoFixture.Xunit;

namespace Beatshake.Tests
{
    public class ExtensionMethodsTest
    {
        [Fact]
        public void SubArrayTest()
        {
            var array = new int[] {0, 1, 2, 3, 4, 5};
            var subarray = new int[] {1, 2, 3};

            // test all element
            Assert.True(array.SubArray(0, array.Length -1 ).SequenceEqual(array));

            // test multiple elements element
            Assert.True(array.SubArray(1,3).SequenceEqual(subarray));

            // test one element
            Assert.True(array.SubArray(0,0).SequenceEqual(new[] {0}));

            Assert.ThrowsAny<ArgumentException>(() => array.SubArray(3, 2));
            Assert.ThrowsAny<ArgumentException>(() => array.SubArray(123, 125));
            Assert.ThrowsAny<ArgumentException>(() => array.SubArray(-4, -2));
        }

        [Theory]
        [InlineData(-12.4)]
        [InlineData(213525)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-112124)]
        public void IsAlmostEqualTest(double value1)
        {
            double tol = 0.1;
            double inTolLower = value1 - tol;
            double inTolGreater = value1 + tol;
            double outTolLower = value1 - tol - double.Epsilon;
            double outTolGreater = value1 + tol + double.Epsilon;
      
            // test to itself
            Assert.True(value1.IsAlmostEqual(value1, tol));
            Assert.True(value1.IsAlmostEqual(value1, double.Epsilon));

            // test minimal inside tolerance border
            Assert.True(value1.IsAlmostEqual(inTolLower, tol + double.Epsilon));
            Assert.True(value1.IsAlmostEqual(inTolGreater, tol + double.Epsilon));

            // test on tolerance border
            Assert.True(value1.IsAlmostEqual(inTolLower, tol));
            Assert.True(value1.IsAlmostEqual(inTolGreater, tol));

            // test out of tolerance border
            Assert.False(value1.IsAlmostEqual(outTolLower, tol));
            Assert.False(value1.IsAlmostEqual(outTolGreater, tol));

        }

        [Fact]
        public void NextGreaterDoubleTest()
        {
            // test 0
            double zero = 0;
            Assert.True(zero.NextGreater() > zero);
            Assert.False(zero.NextGreater() <= zero);

            Random random = new Random();
            double value = random.NextDouble();
            Assert.True(value.NextGreater() > value);
            Assert.False(value.NextGreater() <= value);
        }

        [Fact]
        public void NextLowerDoubleTest()
        {
            // test 0
            double zero = 0;
            Assert.True(zero.NextLower() > zero);
            Assert.False(zero.NextLower() <= zero);

            Random random = new Random();
            double value = random.NextDouble();
            Assert.True(value.NextLower() > value);
            Assert.False(value.NextLower() <= value);
        }
    }

}
