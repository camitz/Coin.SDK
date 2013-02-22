using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Coin.SDK
{
    public static class PropertyNameConventionConverter
    {
        public static string ToOauthStyle(string name)
        {
            var ss = new List<string>();
            var regex = new Regex(@"^([a-z]*)?([A-Z]+$|[A-Z][a-z]+|[A-Z]+(?=[A-Z]))*");
            MatchCollection matches = regex.Matches(name);

            if (!string.IsNullOrEmpty(matches[0].Groups[1].Value))
                ss.Add(matches[0].Groups[1].Value);

            ss.AddRange(from Capture s in matches[0].Groups[2].Captures select s.Value);

            if (matches[0].Groups[3].Success)
                ss.Add(matches[0].Groups[3].Value);

            return string.Join("_", ss.Select(x=>x.ToLowerInvariant()));
        }
    }
}