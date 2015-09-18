using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK.Services.Attributes
{
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
}