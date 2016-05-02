using System;
using Beatshake.Core;
using Beatshake.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beatshake.Tests
{
    [TestClass]
    public class DataAnalyzerTests
    {
        [TestMethod]
        public void CalculateCoefficientsTest()
        {
            // those data come from the PDF example where the formulars come from
            double[] X = new[] {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0};
            double[] Y = {0.38, 1.15, 2.71, 3.92, 5.93, 8.56, 11.24};
            var checkResult = Tuple.Create(0.19642857, 0.23642857, -0.03285714);

            var coefficients = DataAnalyzer.CalculateCoefficients(X, Y);
            
            Assert.IsTrue(coefficients.Item1.IsAlmostEqual(checkResult.Item1, 0.0000001), string.Format("first: {0}\nsecond:{1}", coefficients.Item1, checkResult.Item1));
            Assert.IsTrue(coefficients.Item2.IsAlmostEqual(checkResult.Item2, 0.0000001), string.Format("first: {0}\nsecond:{1}", coefficients.Item2, checkResult.Item2));
            Assert.IsTrue(coefficients.Item3.IsAlmostEqual(checkResult.Item3, 0.0000001), string.Format("first: {0}\nsecond:{1}", coefficients.Item3, checkResult.Item3));
        }


    }

    
}
