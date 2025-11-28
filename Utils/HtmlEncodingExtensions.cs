using System.Net;
using System.Collections;
using System.Reflection;

namespace KartverketRegister.Utils
{
    public static class HtmlEncodingExtensions
    {
        public static T HtmlEncodeStrings<T>(this T obj)
        {
            if (obj == null)
                return obj;

            HashSet<object> visited = new HashSet<object>();
            EncodeObject(obj, visited);
            return obj;
        }

        private static void EncodeObject(object obj, HashSet<object> visited)
        {
            if (obj == null)
                return;

            Type type = obj.GetType();

            // Prevent infinite loops (circular refs)
            if (!type.IsValueType)
            {
                if (visited.Contains(obj))
                    return;

                visited.Add(obj);
            }

            // Handle collections/lists
            if (obj is IEnumerable enumerable && obj is not string)
            {
                foreach (object item in enumerable)
                {
                    EncodeObject(item, visited);
                }
                return;
            }

            // Handle properties
            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                Type propType = prop.PropertyType;
                object value = prop.GetValue(obj);

                if (value == null)
                    continue;

                // Encode string
                if (propType == typeof(string))
                {
                    prop.SetValue(obj, WebUtility.HtmlEncode((string)value));
                }
                // Recurse into nested objects
                else if (!propType.IsPrimitive &&
                         !propType.IsEnum &&
                         !propType.FullName.StartsWith("System.") &&
                         !propType.IsValueType)
                {
                    EncodeObject(value, visited);
                }
            }
        }
    }

}
