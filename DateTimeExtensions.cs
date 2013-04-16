using System;

namespace Coin.SDK
{
    public static class DateTimeExtensions
    {

        public static DateTime FromUnixTime(this long s)
        {
            var unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return unixRef.AddSeconds(s);
        }

        public static long ToUnixTime(this DateTime d)
        {
            var unixRef = new DateTime(1970, 1, 1, 0, 0, 0);

            return (d.Ticks - unixRef.Ticks) / 10000000;
        }
    }
}