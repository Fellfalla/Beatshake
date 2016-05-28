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

            // degree 3
            double a = 6;
            double b = 5;
            double c = 4;
            double d = 3;
            function.Coefficients = new double[] {d, c, b, a};
            xValue = 13.13;
            // f'(x) = 3ax^2 + 2bx + c
            Assert.Equal(xValue * xValue * 3 * a + b * 2 * xValue + c, function.GetGradient(xValue), 10);

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

        [Theory]
        [InlineData(new double[] { })]
        [InlineData(new double[] { 1 })]
        [InlineData(new double[] { 1,-1 })]
        [InlineData(new double[] { 1,-1 ,1})]
        [InlineData(new double[] { 1,-1 ,1,1})]
        [InlineData(new double[] { 1,-1 ,1,1,1})]
        public void GetPeaksTest(double[] coefficients)
        {
            var function = new PolynomialFunction();
            function.Coefficients = coefficients;
            var peaks = function.GetPeaks();
            var peakCount = Math.Max(coefficients.Length - 2, 0);
            Assert.Equal(peakCount, peaks.Count());
        }

        [Fact]
        public void ConstructorTest()
        {
            // those data come from the PDF example where the formulars come from
            double[] X = new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0 };
            double[] Y = { 0.38, 1.15, 2.71, 3.92, 5.93, 8.56, 11.24 };
            var checkResult = Tuple.Create(0.19642857, 0.23642857, -0.03285714);

            PolynomialFunction function = new PolynomialFunction(X,Y);

            Assert.Equal(checkResult.Item1, function.GetCoefficient(2), 8);
            Assert.Equal(checkResult.Item2, function.GetCoefficient(1), 8);
            Assert.Equal(checkResult.Item3, function.GetCoefficient(0), 8);

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

            // Get some empty Derivations
            functionDDD.GetDerivation().GetDerivation().GetDerivation();
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

        [Fact]
        public void NormalizeTest()
        {
            var function = new PolynomialFunction();
            PolynomialFunction normalizedFunction;

            // 0 degree
            function.Coefficients = new double[] {5};
            normalizedFunction = function.GetNormalizedFunction();
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[0]);
            Assert.Equal(1, normalizedFunction.Coefficients[0]);
            Assert.Equal(1, normalizedFunction.ValueAt(4645.51));
            // 0 degree negative
            function.Coefficients = new double[] { -5 };
            normalizedFunction = function.GetNormalizedFunction();
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[0]);
            Assert.Equal(-1, normalizedFunction.Coefficients[0]);
            Assert.Equal(-1, normalizedFunction.ValueAt(4645.51));

            // 1 degree
            function.Coefficients = new double[] { 5, 5 };
            normalizedFunction = function.GetNormalizedFunction();
            Assert.Equal(0.5, normalizedFunction.Coefficients[0]);
            Assert.Equal(0.5, normalizedFunction.Coefficients[1]);
            // 1 degree negative
            function.Coefficients = new double[] { 5, -5 };
            normalizedFunction = function.GetNormalizedFunction();
            Assert.Equal(0.5, normalizedFunction.Coefficients[0]);
            Assert.Equal(-0.5, normalizedFunction.Coefficients[1]);

            // 2 degree
            function.Coefficients = new double[] { 5, 5, 5 };
            normalizedFunction = function.GetNormalizedFunction();
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[0]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[1]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[2]);
            Assert.Equal(1d/3, normalizedFunction.Coefficients[0], 10);
            Assert.Equal(1d/3, normalizedFunction.Coefficients[1], 10);
            Assert.Equal(1d/3, normalizedFunction.Coefficients[2], 10);
            // 2 degree negative
            function.Coefficients = new double[] { 5, 5, -5 };
            normalizedFunction = function.GetNormalizedFunction();
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[0]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[1]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[2]);
            Assert.Equal(1d/3, normalizedFunction.Coefficients[0], 10);
            Assert.Equal(1d/3, normalizedFunction.Coefficients[1], 10);
            Assert.Equal(-1d/3, normalizedFunction.Coefficients[2], 10);

            // all 0
            function.Coefficients = new double[] {0,0,0};
            normalizedFunction = function.GetNormalizedFunction();
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[0]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[1]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[2]);
            Assert.Equal(0, normalizedFunction.Coefficients[0], 10);
            Assert.Equal(0, normalizedFunction.Coefficients[1], 10);
            Assert.Equal(0, normalizedFunction.Coefficients[2], 10);

            // very small coefficients
            function.Coefficients = new double[] { 0.000001, 0, 0.000001 };
            normalizedFunction = function.GetNormalizedFunction();
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[0]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[1]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[2]);
            Assert.Equal(0.5, normalizedFunction.Coefficients[0], 10);
            Assert.Equal(0, normalizedFunction.Coefficients[1], 10);
            Assert.Equal(0.5, normalizedFunction.Coefficients[2], 10);

            // very different coefficients
            function.Coefficients = new double[] { 0.00000001, 0.000000001, 1000000000000 };
            normalizedFunction = function.GetNormalizedFunction();
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[0]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[1]);
            Assert.NotEqual(double.NaN, normalizedFunction.Coefficients[2]);
            Assert.Equal(0, normalizedFunction.Coefficients[0], 7);
            Assert.Equal(0, normalizedFunction.Coefficients[1], 7);
            Assert.Equal(1, normalizedFunction.Coefficients[2], 7);

        }

        [Fact]
        public void PeakNormalizedTest()
        {
            var function = new PolynomialFunction();
            PolynomialFunction normalizedFunction;

            // 0 degree
            function.Coefficients = new double[] {5};
            normalizedFunction = function.GetPeakNormalizedFunction();
            Assert.Equal(1, normalizedFunction.Coefficients[0]);
            Assert.Equal(1, normalizedFunction.ValueAt(4645.51));

            // 1 degree
            //function.Coefficients = new double[] { 5, 5 };
            //normalizedFunction = function.GetPeakNormalizedFunction();
            //Assert.Equal(0.5, normalizedFunction.Coefficients[0]);
            //Assert.Equal(0.5, normalizedFunction.Coefficients[1]);

            // 2 degree
            function.Coefficients = new double[] { 5, -5, 5 };
            normalizedFunction = function.GetPeakNormalizedFunction();
            Assert.Equal(1, normalizedFunction.ValueAt(0), 10);

        }

    }
}