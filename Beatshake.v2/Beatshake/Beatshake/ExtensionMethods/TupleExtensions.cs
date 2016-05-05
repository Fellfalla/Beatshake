using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.ExtensionMethods
{
    public static class TupleExtensions
    {
        public static bool IsInTolerance(this Tuple<double, double, double> tuple1, Tuple<double, double, double> tuple2, double tol1, double tol2, double tol3)
        {
            return tuple1.Item1.IsAlmostEqual(tuple2.Item1, tol1) &&
                   tuple1.Item2.IsAlmostEqual(tuple2.Item2, tol2) &&
                   tuple1.Item3.IsAlmostEqual(tuple2.Item3, tol3);
        }

        /// <summary>
        /// True if the precentage of deviation is lower than tol
        /// </summary>
        /// <param name="tuple1"></param>
        /// <param name="tuple2"></param>
        /// <param name="deviationTol"></param>
        /// <returns></returns>
        public static bool IsInPercentageTolerance(this Tuple<double, double, double> tuple1, Tuple<double, double, double> tuple2, double deviationTol)
        {
            return tuple1.Item1.IsAlmostEqual(tuple2.Item1, tuple1.Item1 * deviationTol) &&
                   tuple1.Item2.IsAlmostEqual(tuple2.Item2, tuple1.Item2 * deviationTol) &&
                   tuple1.Item3.IsAlmostEqual(tuple2.Item3, tuple1.Item3 * deviationTol);
        }
    }
}
