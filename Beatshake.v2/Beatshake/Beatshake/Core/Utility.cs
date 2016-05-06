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

    }
}
