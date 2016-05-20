using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.ExtensionMethods
{
    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int startIndex, int endIndex)
        {
            var length = endIndex - startIndex + 1;
            T[] result = new T[length];
            Array.Copy(data, startIndex, result, 0, length);
            return result;
        }

        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.Length == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        internal class ArrayTraverse
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
    public static class ListExtensions
    {
        public static IList<T> SubList<T>(this IList<T> data, int startIndex, int endIndex)
        {
            IList<T> result = new List<T>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                result.Add(data[i]);
            }
            
            return result;
        }

    }


}
