using Beatshake.Core;
using Xunit;

namespace Beatshake.Tests
{
    public class QuadraticFunctionTest
    {
        [Fact]
        public void CoefficientTest()
        {
            var func = new QuadraticFunction();
            double c = 0;
            double b = 1;
            double a = 2;
            func.Coefficients = new double[] {c,b,a};

            Assert.Equal(c, func.C);
            Assert.Equal(b, func.B);
            Assert.Equal(a, func.A);

            Assert.Equal(c, ((PolynomialFunction)func).Coefficients[0]);
            Assert.Equal(b, ((PolynomialFunction)func).Coefficients[1]);
            Assert.Equal(a, ((PolynomialFunction)func).Coefficients[2]);

        }
    }
}