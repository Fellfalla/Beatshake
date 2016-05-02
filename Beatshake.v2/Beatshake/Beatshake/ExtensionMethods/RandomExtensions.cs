using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.ExtensionMethods
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random random, double min, double max)
        {
            return min + max * random.NextDouble();
        }
    }
}
