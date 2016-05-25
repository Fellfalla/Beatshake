using System;
using System.Collections.Generic;
using System.Linq;
using Beatshake.Core;
using Beatshake.ExtensionMethods;
using Xunit;
// ReSharper disable InconsistentNaming
// ReSharper disable JoinDeclarationAndInitializer

namespace Beatshake.Tests
{
    public class PolynomialFunctionTest
    {
        [Fact]
        public void GetGradientTest()
        {
            //init
            var function = new PolynomialFunction();

            // degree 0
            function.Coefficients = new double[] {3246};
            Assert.Equal(0, function.GetGradient(3214.124));

            // degree 1
            double secondCoefficient = 345.234;
            function.Coefficients = new double[] {3425, secondCoefficient};
            Assert.Equal(secondCoefficient, function.GetGradient(33526.124));

            // degree 2
            double c0 = 13526;
            double c1 = 32.23;
            double c2 = -124.23;
            function.Coefficients = new double[] {c0, c1, c2};
            double xValue = 42;
            Assert.Equal(xValue * 2 * c2 + c1, function.GetGradient(xValue));
        }

        /// <summary>
        /// This methid is only exact for 3 Sample points, otherwise
        /// interpolations beween points will occur
        /// </summary>
        /// <param name="pointCount"></param>
        /// <param name="peakValue"></param>
        /// <param name="peakPosition"></param>
        [Theory]
        [InlineData(3, 1, 1)]
        [InlineData(3, -1, 1)]
        [InlineData(3, -23, 1)]
        [InlineData(3, 1223.124, 1)]
        public void GetPeakTest(int pointCount, double peakValue, int peakPosition)
        {
            // Init testing context
            BeatshakeSettings.SamplePoints = pointCount;
            var timeSteps = DataGenerator.GetData(pointCount).ToList();
            var data = DataGenerator.GetZerosWithPeak(pointCount, peakValue, peakPosition);

            // Run Test
            var function = DataAnalyzer.CalculateQuadraticFunction(timeSteps, data.ToList());
            var peaks = function.GetPeaks().ToArray();
            Assert.Equal(1, peaks.Count());
            Assert.True(peaks.First().IsAlmostEqual(peakPosition, 0.000001));
        }

        [Fact]
        public void GetDerivationTest()
        {
            var function = new PolynomialFunction();
            var initialCoeffs = new double[] {0, 1, 0, 1};
            function.Coefficients = initialCoeffs;

            // smoke tests
            Assert.Equal(3, function.Degree);
            Assert.True(initialCoeffs.SequenceEqual(function.Coefficients));

            var functionD = function.GetDerivation();
            Assert.Equal(function.Degree-1, functionD.Degree);
            Assert.True(new double[] {1,0,3}.SequenceEqual(functionD.Coefficients));

            var functionDD = functionD.GetDerivation();
            Assert.Equal(functionD.Degree-1, functionDD.Degree);
            Assert.True(new double[] {0,6}.SequenceEqual(functionDD.Coefficients));

            var functionDDD = functionDD.GetDerivation();
            Assert.Equal(functionDD.Degree-1, functionDDD.Degree);
            Assert.True(new double[] {6}.SequenceEqual(functionDDD.Coefficients));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(35.156)]
        [InlineData(-1)]
        [InlineData(-15412.154)]
        public void GetValueAtTest(double x)
        {
            var function = new PolynomialFunction();

            // test const
            const double @const = 34.32;
            function.Coefficients = new double[] { @const };
            Assert.Equal(@const, function.ValueAt(x));

            // test linear
            function.Coefficients = new double[] {0, 1};
            Assert.Equal(x, function.ValueAt(x));

            // test quadratic
            function.Coefficients = new double[] {0,0,1};
            Assert.Equal(x*x, function.ValueAt(x));
        }

        [Theory]
        [InlineData(0,0)]
        [InlineData(0,1)]
        [InlineData(-1,1)]
        [InlineData(-1,0)]
        [InlineData(-55.156,513)]
        public void GetIntegralDifferenceTest(double start, double end)
        {
            double integral;
            double expected;
            double deviation;
            double tol = 0.005; // +-0.5% toleranced deviation
            BeatshakeSettings.IntegralPrecision = Math.Max((int) (end - start), 500);
            double c = 1;
            var zeroFunction = new PolynomialFunction();
            zeroFunction.Coefficients = new double[] {0};

            var function = new PolynomialFunction();

            // Test Constant
            function.Coefficients = new double[] { c };
            integral = function.GetIntegralDifference(zeroFunction, start, end);
            if (start.IsAlmostEqual(end, double.Epsilon))
            {
                Assert.Equal(0, integral, 5);
            }
            else
            {
                expected = Math.Abs(Math.Abs(end - start) * c);
                deviation = expected / integral - 1;
                Assert.InRange(deviation, -tol, tol); // 1% deviation
            }
          

            // Test Linear https://www.wolframalpha.com/input/?i=integral+abs%28x%29
            function.Coefficients = new double[] { 0, c };
            integral = function.GetIntegralDifference(zeroFunction, start, end);
            if (start.IsAlmostEqual(end, double.Epsilon))
            {
                Assert.Equal(0, integral, 5);
            }
            else {
                expected = 0.5 * (Math.Sign(end) * end * end - Math.Sign(start) * start * start);
                deviation = expected / integral - 1;
                Assert.InRange(deviation, -tol, tol); // 1% deviation
            }

            // Test quadratic
            function.Coefficients = new double[] { 0, 0, c };
            integral = function.GetIntegralDifference(zeroFunction, start, end);
            if (start.IsAlmostEqual(end, double.Epsilon))
            {
                Assert.Equal(0, integral, 5);
            }
            else {
                expected = (end.FastPower(3) - start.FastPower(3)) / 3;
                deviation = expected / integral - 1;
                Assert.InRange(deviation, -tol, tol); // 1% deviation

            }

            // todo: test function with higher degree than 2 (function up to degree of 2 are solved analytical)

        }

        [Fact]
        public void GetCoefficientTest()
        {
            double first = 1;
            double second = 2;
            double third = 3;
            var function = new PolynomialFunction();
            function.Coefficients = new double[] {first, second, third};

            Assert.Equal(first, function.GetCoefficient(0));
            Assert.Equal(second, function.GetCoefficient(1));
            Assert.Equal(third, function.GetCoefficient(2));
        }

    }
}