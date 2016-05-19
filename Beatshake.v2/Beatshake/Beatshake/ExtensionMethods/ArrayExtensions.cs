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
