using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public class FunctionGroup
    {

        public FunctionGroup()
        {
            
        }

        public FunctionGroup(params PolynomialFunction[] functions)
        {
            Functions = functions.ToList();
        }

        public List<PolynomialFunction> Functions { get; } = new List<PolynomialFunction>(3) {null,null,null};

        public void AddFunction(PolynomialFunction function)
        {
            Functions.Add(function);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <returns>true if the function is known and was removed, otherwise false</returns>
        public bool RemoveFunction(PolynomialFunction function)
        {

            var index = Functions.IndexOf(function);
            if (index == -1) 
            {
                // function is not contained
                return false;
            }
            else
            {
                Functions.RemoveAt(index);
                return true;
            }
        }

        public double GetPeak()
        {
            return 0;
        }

        public void PeakNormalizeDownTo(FunctionGroup other)
        {
            AssertDimensionalEquality(other, nameof(other));
            // the question is: normalize down all seperately or all with same amount
            for (int i = 0; i < Functions.Count; i++)
            {
                var peakVal = Functions[i].Peaks.Max();
                var desiredPeak = Math.Abs(other.Functions[i].Peaks.Max());
                if (Math.Abs(peakVal) > desiredPeak) // only scale down
                {
                    Functions[i] = Functions[i].GetPeakNormalizedFunction(desiredPeak*Math.Sign(peakVal)); // Do not swap sign
                }
            }
        }



        public double GetDifferenceIntegral(double start, double end, params PolynomialFunction[] others)
        {
            return GetDifferenceIntegral(start, end, new FunctionGroup(others));
        }

        public double GetDifferenceIntegral (double start, double end, FunctionGroup other)
        {
            AssertDimensionalEquality(other, nameof(other));
            double error = 0;

            for (int i = 0; i < Functions.Count; i++)
            {
                error += Functions[i].GetAbsIntegralDifference(other.Functions[i], start, end);
            }

            return error;
        }

        private void AssertDimensionalEquality(FunctionGroup other, string nameOf)
        {
            if (other.Functions.Count != Functions.Count)
            {
                throw new ArgumentOutOfRangeException(nameOf, "Count of functions must not differ!");
            }
        }
    }
}
