using System.Linq;
using Beatshake.Core;
using Xunit;

namespace Beatshake.Tests
{
    public class UtilityTest
    {
        [Fact]
        public void MidnightFormulaTest()
        {
            double[] result;

            // x^2
            result = Utility.MidnightFormula(1, 0, 0).ToArray();
            Assert.Equal(1, result.Count());
            Assert.Equal(0, result[0]);

            // x^2 + 1
            result = Utility.MidnightFormula(1, 0, 1).ToArray();
            Assert.Equal(0, result.Count());

            // x^2 - 1
            result = Utility.MidnightFormula(1, 0, -1).ToArray();
            Assert.Equal(2, result.Count());
            Assert.True(result.Contains(1));
            Assert.True(result.Contains(-1));
        }
    }
}