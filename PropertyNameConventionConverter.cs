using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Coin.SDK
{
    public static class PropertyNameConventionConverter
    {
        public static string ToOauthStyle(string name)
        {
            var input = new string(name.Reverse().ToArray());
            var ss = new List<string>();

            Regex myRegex = new Regex(@"[a-z]+[A-Z]?|[A-Z]+|\d+", RegexOptions.None);
            string strTargetString = input;

            MatchCollection matchCollection = myRegex.Matches(strTargetString);
            foreach (Match myMatch in matchCollection)
            {
                if (myMatch.Success)
                {
                   ss.Add(myMatch.Value);
                }
            }


            string output = string.Join("_", ss.Select(x => Regex.Replace(new string(x.ToLowerInvariant().Reverse().ToArray()), @"[_\[\]]", "")).Reverse());
            return output;
        }
    }
}