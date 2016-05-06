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
}
