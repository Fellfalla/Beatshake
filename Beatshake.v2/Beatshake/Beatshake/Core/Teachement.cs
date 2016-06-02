using System;
using System.Collections.Generic;
using System.Linq;
using Beatshake.ExtensionMethods;

namespace Beatshake.Core
{
    public class Teachement : FunctionGroup
    {
        public PolynomialFunction XCurve
        {
            get { return Functions[0]; }
            set { Functions[0] = value; }
        }
        public PolynomialFunction YCurve
        {
            get { return Functions[1]; }
            set { Functions[1] = value; }
        }
        public PolynomialFunction ZCurve
        {
            get { return Functions[2]; }
            set { Functions[2] = value; }
        }

        /// <summary>
        /// <exception cref="InsufficientDataException">Thrown if a peak is to close to the start of the data</exception>
        /// </summary>
        /// <param name="timesteps"></param>
        /// <param name="throwOnThinData"></param>
        /// <param name="valueArrays"></param>
        /// <returns></returns>
        public static Teachement Create(double[] timesteps, bool throwOnThinData = false, params IList<double>[] valueArrays)
        {
            var teachement = new Teachement();

            IList<double> xValues = valueArrays[0];
            IList<double> yValues = valueArrays[1];
            IList<double> zValues = valueArrays[2];
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
            teachement.Functions.Clear();
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                xValues.SubList(startIndex, endIndex)));
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                yValues.SubList(startIndex, endIndex)));
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                zValues.SubList(startIndex, endIndex)));


            return teachement;
        }

        ///// <summary>
        ///// <seealso cref="Create(double[],double[],double[],double[])"/>
        ///// </summary>
        ///// <param name="timesteps"></param>
        ///// <param name="xValues"></param>
        ///// <param name="yValues"></param>
        ///// <param name="zValues"></param>
        ///// <returns></returns>
        //public static Teachement Create(IEnumerable<double> timesteps, bool throwOnThinData = false, params IEnumerable<double>[] valueArrays)
        //{
        //        return Create(timesteps.ToArray(), throwOnThinData, valueArrays.Select(doubles => doubles.ToArray()).ToArray());
        //}


        public bool FitsDataSet(double tolerance, double start, double end, ComparisonStrategy strategy, FunctionGroup functions)
        {
            switch (strategy)
            {
                case ComparisonStrategy.Absolute: throw new NotImplementedException();
                case ComparisonStrategy.CoefficientNormalized: throw new NotImplementedException();
                case ComparisonStrategy.PeakNormalized:
                    functions.PeakNormalizeDownTo(this);
                    var difference = GetDifferenceIntegral(end, start, functions);
                    if (DataAnalyzer.AreFunctionsAlmostEqual(difference, tolerance, end - start))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
            return false;
        }
    }
}
