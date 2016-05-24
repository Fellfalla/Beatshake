using Beatshake.Core;
using Xunit;

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
    }
}