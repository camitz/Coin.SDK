using System;
using System.Globalization;

namespace Coin.SDK.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UnixTimestamp : DateTimeFormatAttribute
    {
        public override string Format(object o)
        {
            return Format(o, d => d.ToUnixTime().ToString(CultureInfo.InvariantCulture));
        }
    }
}