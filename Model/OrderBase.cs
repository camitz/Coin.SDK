using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Coin.SDK.Model
{

    public abstract class OrderBase : IOrder
    {
        private int? _merchantID;

        public int? MerchantID
        {
            get {
                try
                {
                    return _merchantID ?? (_merchantID = Int32.Parse(ConfigurationManager.AppSettings[Constants.CocoinMerchantID]));
                }
                catch (Exception)
                {
                    return null;
                }

            }
            set { _merchantID = value; }
        }

        public string OrderRef { get; set; }


        public void FormatProperties(Action<string, object> action)
        {
            PropertyFormatter.FormatProperties(this, action);
        }

        public string FormatProperties(Func<string, object, string> func)
        {
            var s = string.Empty;
            FormatProperties((key, value) => { s += func(key, value); });
            return s;
        }

    }

}
