using System;
using System.Globalization;

namespace Coin.SDK.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ShortDateTimeString : DateTimeFormatAttribute
    {
        public override string Format(object o)
        {
            //var c = new IsoDateTimeConverter();//todo iso
            return Format(o, d => d.ToShortDateString().ToString(CultureInfo.InvariantCulture));
        }

    }
}