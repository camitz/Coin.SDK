using System;

namespace Coin.SDK.Services.Attributes
{
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
}