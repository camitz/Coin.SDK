using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Coin.SDK
{
    public interface IKeyValueSigner
    {
        string Sign(string key);
        void Add(string key, object value);
    }


    public class KeyValueSigner : IKeyValueSigner
    {
        private Dictionary<string, object> _stringParts = new Dictionary<string, object>();

        public KeyValueSigner()
        {
        }

        public string Sign(string key)
        {
            string s = string.Join("&", _stringParts
                                            .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                                            .ThenBy(x => x.Value.ToString(), StringComparer.OrdinalIgnoreCase)
                                            .Select(x => string.Format(CultureInfo.InvariantCulture, "{0}={1}", x.Key, x.Value)));

            byte[] keyByte = new ASCIIEncoding().GetBytes(key);

            var hmacsha256 = new HMACSHA256(keyByte);

            return
                HttpUtility.UrlEncode(
                    Convert.ToBase64String(
                        hmacsha256.ComputeHash(new UTF8Encoding().GetBytes(s))));
        }

        public void Add(string key, object value)
        {
            _stringParts.Add(key, value);
        }

        public void Reset()
        {
            _stringParts.Clear();
        }
    }
}