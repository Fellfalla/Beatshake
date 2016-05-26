using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

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

            private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

            public static bool IsPrimitive(this Type type)
            {
            
                if (type == typeof(string)) return true;
                return (type.GetTypeInfo().IsValueType & type.GetTypeInfo().IsPrimitive);
            }

            public static object Copy(this object originalObject)
            {
                return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
            }
            private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
            {
                if (originalObject == null) return null;
                var typeToReflect = originalObject.GetType();
                if (IsPrimitive(typeToReflect)) return originalObject;
                if (visited.ContainsKey(originalObject)) return visited[originalObject];
                if (typeof(Delegate).GetTypeInfo().IsAssignableFrom(typeToReflect.GetTypeInfo())) return null;
                var cloneObject = CloneMethod.Invoke(originalObject, null);
                if (typeToReflect.IsArray)
                {
                    var arrayType = typeToReflect.GetElementType();
                    if (IsPrimitive(arrayType) == false)
                    {
                        Array clonedArray = (Array)cloneObject;
                        clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                    }

                }
                visited.Add(originalObject, cloneObject);
                CopyFields(originalObject, visited, cloneObject, typeToReflect);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
                return cloneObject;
            }

            private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
            {
                if (typeToReflect.GetTypeInfo().BaseType != null)
                {
                    RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.GetTypeInfo().BaseType);
                    CopyFields(originalObject, visited, cloneObject, typeToReflect.GetTypeInfo().BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
                }
            }

            private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
            {
                foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
                {
                    if (filter != null && filter(fieldInfo) == false) continue;
                    if (IsPrimitive(fieldInfo.FieldType)) continue;
                    try
                    {
                        var originalFieldValue = fieldInfo.GetValue(originalObject);
                        var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                        fieldInfo.SetValue(cloneObject, clonedFieldValue);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    
                }
            }
            public static T Copy<T>(this T original)
            {
                return (T)Copy((object)original);
            }
        }

        public class ReferenceEqualityComparer : EqualityComparer<object>
        {
            public override bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }
            public override int GetHashCode(object obj)
            {
                if (obj == null) return 0;
                return obj.GetHashCode();
            }
        }
}
