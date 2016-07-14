using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Beatshake.ExtensionMethods
{
    public static class VisualElementExtensions
    {
        public static T FindParentOfType<T>(this Element element) where T:class
        {
            var current = element;
            while (current != null)
            {
                T parent = current.Parent as T;

                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    current = current.Parent;
                }
            }
            return null;
        }
    }
}
