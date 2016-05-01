using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            Random random = new Random();
            var array = enumerable as T[] ?? enumerable.ToArray();
            return array.ElementAtOrDefault(random.Next(0, array.Length));
        }

    }
}
