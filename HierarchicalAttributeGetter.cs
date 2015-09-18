using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coin.SDK
{
    public class HierarchicalAttributeGetter
    {
        public static IEnumerable<T> GetAttributes<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            var l = new List<T>();
            l.AddRange(Attribute.GetCustomAttributes(propertyInfo, typeof (T), true) as IEnumerable<T>);

            foreach (var @interface in propertyInfo.ReflectedType.GetInterfaces())
            {
                var p = @interface.GetProperty(propertyInfo.Name);
                if(p!=null)
                {
                    l.AddRange(Attribute.GetCustomAttributes(p, typeof(T), true) as IEnumerable<T>);
                }
            }

            return l;
        }

        public static IEnumerable<Attribute> GetAttributes(PropertyInfo propertyInfo, Type includeAttributeType)
        {
            var l = new List<Attribute>();
            l.AddRange(Attribute.GetCustomAttributes(propertyInfo, includeAttributeType, true));

            foreach (var @interface in propertyInfo.ReflectedType.GetInterfaces())
            {
                var p = @interface.GetProperty(propertyInfo.Name);
                if (p != null)
                {
                    l.AddRange(Attribute.GetCustomAttributes(p, includeAttributeType, true));
                }
            }

            return l;
        }
    }
}