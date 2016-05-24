using System.Collections.Generic;

namespace Beatshake.Tests
{
    public static class DataGenerator
    {
        /// <summary>
        /// This function returns a array with <paramref name="count"></paramref> values 
        /// beginning at <paramref name="start"/> and by <paramref name="step"></paramref> increasing values
        /// </summary>
        /// <param name="count">The number of values in the returne enumerable</param>
        /// <param name="step">Difference between Two neighboured values</param>
        /// <param name="start">Value of the first element</param>
        /// <returns></returns>
        public static IEnumerable<double> GetData(int count, int step = 1, int start = 0)
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