using System;
using System.Collections.Generic;
using System.Linq;
using Beatshake.ExtensionMethods;

namespace Beatshake.Core
{
    /// <summary>
    /// This exception is thrown if data are not sufficient to perform the requested action.
    /// There might
    /// </summary>
    public  class InsufficientDataException : ArgumentException { }

    public class Teachement
    {
        public PolynomialFunction XCurve;
        public PolynomialFunction YCurve;
        public PolynomialFunction ZCurve;

        /// <summary>
        /// <exception cref="InsufficientDataException">Thrown if a peak is to close to the start of the data</exception>
        /// </summary>
        /// <param name="timesteps"></param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="zValues"></param>
        /// <returns></returns>
        public static Teachement Create(double[] timesteps, double[] xValues, double[] yValues, double[] zValues)
        {
            var teachement = new Teachement();

            // Get point with highest absolute Acceleration
            var endIndex = DataAnalyzer.GetPeak(xValues , yValues, zValues);
            int startIndex = endIndex - BeatshakeSettings.SamplePoints;

            if (endIndex == -1)
            {
                throw new InvalidOperationException("No peak value could be detected.");
            }
            if (startIndex < 0)
            {
                throw new InsufficientDataException();
            }

            teachement.XCurve = new PolynomialFunction(timesteps.SubArray(startIndex, endIndex), xValues.SubArray(startIndex, endIndex));
            teachement.YCurve = new PolynomialFunction(timesteps.SubArray(startIndex, endIndex), yValues.SubArray(startIndex, endIndex));
            teachement.ZCurve = new PolynomialFunction(timesteps.SubArray(startIndex, endIndex), zValues.SubArray(startIndex, endIndex));

            return teachement;
        }

        /// <summary>
        /// <seealso cref="Create(double[],double[],double[],double[])"/>
        /// </summary>
        /// <param name="timesteps"></param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="zValues"></param>
        /// <returns></returns>
        public static Teachement Create(IEnumerable<double> timesteps, IEnumerable<double> xValues, IEnumerable<double> yValues, IEnumerable<double> zValues)
        {
                return Create(timesteps.ToArray(), xValues.ToArray(), yValues.ToArray(), zValues.ToArray());
        }
    }
}
