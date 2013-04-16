using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coin.SDK.Model
{
    public static class PropertyFormatter
    {
        internal static void FormatProperties(object specimen, Action<string, object> action)
        {
            var properties = new List<PropertyInfoItem>();
            CollectProperties(specimen, properties);

            foreach (var propertyInfoItem in properties.OrderBy(x=>x.level).Distinct(new EqualityComparer<PropertyInfoItem>((x,y)=>x.Key==y.Key)))
            {
                var key = propertyInfoItem.Key;
                var propertyInfo = propertyInfoItem.PropertyInfo;

                if (HierarchicalAttributeGetter.GetAttributes<Ignore>(propertyInfo).Any())
                    continue;

                if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsValueType ||
                    Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null ||
                    propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType == typeof(Decimal))
                {
                    var value = propertyInfo.GetValue(propertyInfoItem.specimen, null);
                    if (value != null)
                    {
                        if (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                            value = value.ToString().ToLower();
                        else if (HierarchicalAttributeGetter.GetAttributes<FormatAttribute>(propertyInfo).Any())
                            value =
                                HierarchicalAttributeGetter.GetAttributes<FormatAttribute>(propertyInfo).Single().Format
                                    (value);

                        action(key, value);
                    }
                }
            }
        }

        private static void CollectProperties(object specimen, IList<PropertyInfoItem> properties, int level = 0)
        {
            foreach (
                var propertyInfo in
                    specimen.GetType().GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
                )
            {
                if (HierarchicalAttributeGetter.GetAttributes<Ignore>(propertyInfo).Any())
                    continue;

                if (
                    !(propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsValueType ||
                      Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null ||
                      propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType == typeof(Decimal)))
                {
                    var o = propertyInfo.GetValue(specimen, null);
                    if (o != null)
                        CollectProperties(o, properties);
                }
                else
                {
                    var key = PropertyNameConventionConverter.ToOauthStyle(propertyInfo.Name);

                    properties.Add(new PropertyInfoItem { level = level, Key = key, PropertyInfo = propertyInfo, specimen = specimen });
                }
            }
        }

        private struct PropertyInfoItem
        {
            internal int level;
            internal string Key;
            internal PropertyInfo PropertyInfo;
            internal object specimen;
        }

        public class EqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;

            public EqualityComparer(Func<T, T, bool> comparer)
            {
                if (comparer == null)
                    throw new ArgumentNullException("comparer");

                _comparer = comparer;
            }


            public bool Equals(T x, T y)
            {
                return _comparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                return 0;
            }
        }
    }
}