// <copyright file="TeachementTest.cs">Copyright ©  2014</copyright>
using System;
using System.Collections.Generic;
using Beatshake.Core;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Xunit;

namespace Beatshake.Core.Tests
{
    /// <summary>This class contains parameterized unit tests for Teachement</summary>
    [PexClass(typeof(Teachement))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    public partial class TeachementTest
    {
        /// <summary>Test stub for Create(Double[], Double[], Double[], Double[])</summary>
        [PexMethod]
        public Teachement CreateTest(
            double[] timesteps,
            double[] xValues,
            double[] yValues,
            double[] zValues
        )
        {
            Teachement result = Teachement.Create(timesteps, xValues, yValues, zValues);
            return result;
            // TODO: add assertions to method TeachementTest.CreateTest(Double[], Double[], Double[], Double[])
        }

        /// <summary>Test stub for Create(IEnumerable`1&lt;Double&gt;, IEnumerable`1&lt;Double&gt;, IEnumerable`1&lt;Double&gt;, IEnumerable`1&lt;Double&gt;)</summary>
        [PexMethod]
        public Teachement CreateTest01(
            IEnumerable<double> timesteps,
            IEnumerable<double> xValues,
            IEnumerable<double> yValues,
            IEnumerable<double> zValues
        )
        {
            Teachement result = Teachement.Create(timesteps, xValues, yValues, zValues);
            return result;
            // TODO: add assertions to method TeachementTest.CreateTest01(IEnumerable`1<Double>, IEnumerable`1<Double>, IEnumerable`1<Double>, IEnumerable`1<Double>)
        }

       [Fact]
       public void CreateTest()
        {
            var teachement = new Teachement();
        }
    }
}
