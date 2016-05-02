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
            return tol > Math.Abs(a - b);
        }
    }
}
