using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK
{
    public class RequiredAttribute : ValidationAttribute
    {
        public override bool Validate(PropertyInfo propertyInfo, object o, out string message)
        {
            var value = propertyInfo.GetValue(o, null);
            message = null;
            if (value != null)
                return true;

            message = Message ?? string.Format("{0} is a required property for {1}.", propertyInfo.Name, o.GetType().Name);
            return false;
        }
    }

    public class Email : FormatValidationAttribute
    {
        public override bool Validate(PropertyInfo propertyInfo, object o, out string message)
        {
            message = null;
            var regex = new Regex(@"[\w-\.]+@([\w-]+\.)+[\w-]+");

            var value = propertyInfo.GetValue(o, null);
            if (value == null)
                return true;

            if (regex.IsMatch(value.ToString()))
                return true;

            message = Message ?? string.Format("{0} is malformed in {1}.", propertyInfo.Name, o.GetType().Name);
            return false;
        }
    }

    public abstract class FormatValidationAttribute : ValidationAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ValidationAttribute : Attribute
    {
        public string Message { get; set; }
        public abstract bool Validate(PropertyInfo propertyInfo, object o, out string message);
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class Ignore : Attribute
    {
    }

    public abstract class DateTimeFormatAttribute : FormatAttribute
    {
        public string Format(object o, Func<DateTime, string> func)
        {
            DateTime d;
            if (o is DateTime?)
            {
                if (!((DateTime?)o).HasValue)
                    return null;
                d = ((DateTime?)o).Value;
            }
            else
                d = (DateTime)o;

            return func(d);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UnixTimestamp : DateTimeFormatAttribute
    {
        public override string Format(object o)
        {
            return Format(o, d => d.ToUnixTime().ToString(CultureInfo.InvariantCulture));
        }
    }

    public class UnixTimestampJsonConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string value2;
            if (value is DateTime)
            {
                value2 = ((DateTime)value).ToUnixTime().ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                throw new NotImplementedException();
            }
            writer.WriteValue(value2);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ShortDateTimeString : DateTimeFormatAttribute
    {
        public override string Format(object o)
        {
            //var c = new IsoDateTimeConverter();//todo iso
            return Format(o, d => d.ToShortDateString().ToString(CultureInfo.InvariantCulture));
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class FormatAttribute : Attribute
    {
        public abstract string Format(object o);
    }
}