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
                return -double.Epsilon;
        }
    }
}
