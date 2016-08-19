using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.ExtensionMethods
{
    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
            {
                throw new ArgumentException("End index must be greater than start index", nameof(endIndex));
            }

            var length = endIndex - startIndex + 1;
            T[] result = new T[length];
            Array.Copy(data, startIndex, result, 0, length);
            return result;
        }

        /// <summary>
        /// Instanciates the Array with <paramref name="value"/> in each entry
        /// Source: http://stackoverflow.com/a/1014015
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="value"></param>
        /// <returns>The same array as the input array</returns>
        public static T[] Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
            return arr;
        }

        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.Length == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        private class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
