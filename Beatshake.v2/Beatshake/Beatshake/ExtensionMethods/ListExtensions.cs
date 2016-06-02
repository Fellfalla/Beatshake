using System.Collections.Generic;

namespace Beatshake.ExtensionMethods
{
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