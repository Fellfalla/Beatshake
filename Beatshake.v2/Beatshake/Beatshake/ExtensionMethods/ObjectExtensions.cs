using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.ExtensionMethods
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Parses the class the the attribute <see cref="DefaultValueAttribute"/> and assigns the attribute to the marked properties
        /// </summary>
        /// <param name="target"></param>
        public static void AssignDefaultValueAttributes(this object target)
        {
            foreach (PropertyInfo property in target.GetType().GetRuntimeProperties())
            {
                var myAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
                
                if (myAttribute != null && property.CanWrite)
                {
                    property.SetValue(target, Convert.ChangeType(myAttribute.Value, property.PropertyType));
                }
            }
        }
    }
}
