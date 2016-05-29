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
        public static Teachement Create(double[] timesteps, double[] xValues, double[] yValues, double[] zValues, bool throwOnThinData = false)
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
                if (throwOnThinData)
                {
                    throw new InsufficientDataException();
                }
                else
                {
                    startIndex = 0;
                }
            }

            teachement.XCurve = new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                xValues.SubArray(startIndex, endIndex));
            teachement.YCurve = new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                yValues.SubArray(startIndex, endIndex));
            teachement.ZCurve = new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                zValues.SubArray(startIndex, endIndex));


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
        public static Teachement Create(IEnumerable<double> timesteps, IEnumerable<double> xValues, IEnumerable<double> yValues, IEnumerable<double> zValues, bool throwOnThinData = false)
        {
                return Create(timesteps.ToArray(), xValues.ToArray(), yValues.ToArray(), zValues.ToArray(), throwOnThinData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="normalized">If true, the functions are scaled to maxValues of 1 in the highest peak through <see cref="PolynomialFunction.GetNormalizedFunction"/></param>
        /// <param name="functions"></param>
        /// <returns></returns>
        public double GetError(double start, double end, bool normalized = true, params PolynomialFunction[] functions)
        {
            PolynomialFunction tFunc1;
            PolynomialFunction tFunc2;
            PolynomialFunction tFunc3;
            PolynomialFunction otherFunc1;
            PolynomialFunction otherFunc2;
            PolynomialFunction otherFunc3;

            if (normalized)
            {
                tFunc1      = XCurve.GetNormalizedFunction();
                tFunc2      = YCurve.GetNormalizedFunction();
                tFunc3      = ZCurve.GetNormalizedFunction();
                otherFunc1  = functions[0].GetNormalizedFunction();
                otherFunc2  = functions[1].GetNormalizedFunction();
                otherFunc3  = functions[2].GetNormalizedFunction();
            }
            else
            {
                tFunc1 = XCurve;
                tFunc2 = YCurve;
                tFunc3 = ZCurve;
                otherFunc1 = functions[0];
                otherFunc2 = functions[1];
                otherFunc3 = functions[2];
            }

            double error = 0;
            for (int i = 0; i < 3; i++)
            {
                error =     tFunc1.GetAbsIntegralDifference(   otherFunc1  , start, end)   ;
                error +=    tFunc2.GetAbsIntegralDifference(  otherFunc2  , start, end)    ;
                error +=    tFunc3.GetAbsIntegralDifference(  otherFunc3  , start, end)    ;
            }

            return error;
        }

        public bool FitsDataSet(double tolerance, double end, double start = 0, bool normalized = true, params PolynomialFunction[] functions)
        {
            var difference = GetError(end, start, normalized, functions);
            if (DataAnalyzer.AreFunctionsAlmostEqual(difference, tolerance, end - start))
            {
                return true;
            }
            return false;
        }
    }
}
