using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.ExtensionMethods
{
    public static class PrimitiveExtensions
    {
        public static bool IsAlmostEqual(this double a, double b, double tol)
        {
            return tol >= Math.Abs(a - b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="tol"></param>
        /// <returns>True if the abs of the give value is lower or equal <paramref name="tol"/></returns>
        public static bool IsAlmostZero(this double a, double tol = double.Epsilon)
        {
            if (tol < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tol), "Tolerance must be positive");
            }
            return a <= tol && a >= -tol;
        }

        public static double NextLower(this double value)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);
            if (value > 0)
                return BitConverter.Int64BitsToDouble(bits - 1);
            else if (value < 0)
                return BitConverter.Int64BitsToDouble(bits + 1);
            else
                return -double.Epsilon;
        }

        public static double NextGreater(this double value)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);
            if (value > 0)
                return BitConverter.Int64BitsToDouble(bits + 1);
            else if (value < 0)
                return BitConverter.Int64BitsToDouble(bits - 1);
            else
                return double.Epsilon;
        }

        public static double FastPower(this double value, uint exp)
        {
            double power = 1;
            for (int j = 0; j < exp; j++)
            {
                power *= value;
            }
            return power;
        }
    }
}
