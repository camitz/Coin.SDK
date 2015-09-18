using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Coin.SDK.Services.Attributes;

namespace Coin.SDK.Services
{
    public static class PropertyFormatter
    {
        public static void FormatProperties(object specimen, Action<string, object> action, Type includeAttributeType = null)
        {
            var properties = new List<PropertyInfoItem>();
            CollectProperties(specimen, properties);

            foreach (var propertyInfoItem in properties.OrderBy(x => x.level).Distinct(new EqualityComparer<PropertyInfoItem>((x, y) => x.Key == y.Key)))
            {
                var key = propertyInfoItem.Key;
                var propertyInfo = propertyInfoItem.PropertyInfo;

                try
                {
                    if (HierarchicalAttributeGetter.GetAttributes<IgnoreForFormatting>(propertyInfo).Any())
                        continue;

                    if (includeAttributeType != null && !HierarchicalAttributeGetter.GetAttributes(propertyInfo, includeAttributeType).Any())
                        continue;

                    if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsValueType ||
                        Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null ||
                        propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType == typeof(Decimal))
                    {
                        var value = propertyInfo.GetValue(propertyInfoItem.specimen, null);

                        if (value is decimal)
                            value = ((decimal)value).ToString(CultureInfo.InvariantCulture);

                        if (value != null)
                        {
                            if (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                                value = value.ToString().ToLower();
                            else if (HierarchicalAttributeGetter.GetAttributes<FormatAttribute>(propertyInfo).Any())
                                value = HierarchicalAttributeGetter.GetAttributes<FormatAttribute>(propertyInfo).Single().Format(value);

                            action(key, value);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new PropertyFormatterException(string.Format("Exception caught while formatting {0} of a {1}.", key, specimen.GetType().Name), e);
                }
            }
        }

        private static void CollectProperties(object specimen, IList<PropertyInfoItem> properties, int level = 0, string prefix = null)
        {
            if (specimen is IEnumerable)
            {
                var collection = specimen as IEnumerable;
                var i = 0;
                foreach (var item in collection)
                    CollectProperties(item, properties, level + 1, prefix + "[" + i++ + "]");

                return;
            }

            foreach (var propertyInfo in
                    specimen.GetType().GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance))
            {
                if (HierarchicalAttributeGetter.GetAttributes<IgnoreForFormatting>(propertyInfo).Any())
                    continue;

                if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsValueType ||
                    Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null ||
                    propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType == typeof(Decimal))
                {
                    var key = PropertyNameConventionConverter.ToOauthStyle((prefix ?? string.Empty) + propertyInfo.Name);

                    properties.Add(new PropertyInfoItem { level = level, Key = key, PropertyInfo = propertyInfo, specimen = specimen });
                }
                else
                {
                    var o = propertyInfo.GetValue(specimen, null);
                    if (o != null)
                    {
                        var prefixes = HierarchicalAttributeGetter.GetAttributes<FormattingPrefix>(propertyInfo);
                        string propertyPrefix = null;
                        if (prefixes.Any())
                            propertyPrefix = prefixes.FirstOrDefault().Value;
                        else if (o is IEnumerable)
                            propertyPrefix = propertyInfo.Name;

                        CollectProperties(o, properties, level + 1, prefix + propertyPrefix);
                    }
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

    public class PropertyFormatterException : InvalidOperationException
    {
        public PropertyFormatterException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}