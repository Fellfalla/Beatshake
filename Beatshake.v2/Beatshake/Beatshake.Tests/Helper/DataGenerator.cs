using System.Collections.Generic;

namespace Beatshake.Tests
{
    public static class DataGenerator
    {
        public static IEnumerable<double> GetRaisingData(int count, int step = 1, int start = 0)
        {
            int dataCount = 0;
            for (int i = start; dataCount < count; i += step)
            {
                yield return i;
                dataCount++;
            }
        }

        public static IEnumerable<double> GetZerosWithPeak(int count, int peakValue, int peakPosition)
        {
            for (int i = 0; i < count; i++)
            {
                if (i == peakPosition)
                {
                    yield return peakValue;
                }
                else
                {
                    yield return 0;
                }
            }
        }
    }
}