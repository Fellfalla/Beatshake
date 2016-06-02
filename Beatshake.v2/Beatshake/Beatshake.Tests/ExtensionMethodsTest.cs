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
            double outTolLower = value1 - tol.NextLower();
            double outTolGreater = value1 + tol.NextGreater();
            double roundingErrorTol = 0.000000001;

            // test to itself
            Assert.True(value1.IsAlmostEqual(value1, tol));
            Assert.True(value1.IsAlmostEqual(value1, double.Epsilon)); 

            // test minimal inside tolerance border
            Assert.True(value1.IsAlmostEqual(inTolLower, tol + roundingErrorTol));
            Assert.True(value1.IsAlmostEqual(inTolGreater, tol + roundingErrorTol));

            // test on tolerance border
            // Assert.True(value1.IsAlmostEqual(inTolLower, tol + roundingErrorTol));
            // Assert.True(value1.IsAlmostEqual(inTolGreater, tol + roundingErrorTol));

            // test out of tolerance border
            Assert.False(value1.IsAlmostEqual(outTolLower, tol - roundingErrorTol));
            Assert.False(value1.IsAlmostEqual(outTolGreater, tol - roundingErrorTol));

        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-double.Epsilon)]
        [InlineData(double.Epsilon)]
        public void NextGreaterDoubleTest(double value)
        {
            Assert.True(value.NextGreater() > value);
            Assert.False(value.NextGreater() <= value);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-double.Epsilon)]
        [InlineData(double.Epsilon)]
        public void NextLowerDoubleTest(double value)
        {
            Assert.True(value.NextLower() < value);
            Assert.False(value.NextLower() >= value);
        }

        [Fact]
        public void PowerDoubleTest()
        {
            double value = 2;
            Assert.Equal(1,value.FastPower(0));
            Assert.Equal(2,value.FastPower(1));
            Assert.Equal(4,value.FastPower(2));
            Assert.Equal(8,value.FastPower(3));
            Assert.Equal(16,value.FastPower(4));
        }

        [Fact]
        public void IsAlmostZeroTest()
        {
            double a = 0;
            Assert.True(a.IsAlmostZero());
            Assert.True(a.IsAlmostZero(Double.Epsilon));
            Assert.True(a.IsAlmostZero(Double.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => a.IsAlmostZero(-1));

            a = 1;
            Assert.False(a.IsAlmostZero());
            Assert.False(a.IsAlmostZero(0.9));
            Assert.True(a.IsAlmostZero(1));
            Assert.True(a.IsAlmostZero(10));
            Assert.True(a.IsAlmostZero(Double.MaxValue));
        }
    }

}
