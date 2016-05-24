using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public static class Utility
    {
        public static List<double> NormalizeTimeStamps(List<long> timestamps)
        {
            return timestamps.Select(l => (double) (l -  timestamps.First())).ToList();
        } 

        public static List<double> NormalizeTimeStamps(List<double> timestamps)
        {
            return timestamps.Select(l => (l -  timestamps.First())).ToList();
        }

        public static IEnumerable<double> MidnightFormula(double a, double b, double c)
        {
            var radicand = b * b - 4 * a * c; //b^2 -4ac
            var dominator = 2 * a;
            if (radicand < 0)
            {
                yield break;
            }
            else if (Math.Abs(radicand) <= double.Epsilon)
            {
                yield return -b / dominator;
            }
            else
            {
                var sqrtResult = Math.Sqrt(radicand);
                yield return (-b + sqrtResult) / dominator;
                yield return (-b - sqrtResult) / dominator;
            }
            yield break;
        } 

    }
}
