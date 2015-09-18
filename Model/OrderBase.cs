using System;
using System.Collections.Generic;
using System.Configuration;
using Coin.SDK.Services;

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
        public string GroupRef { get; set; }


        public void FormatProperties(Action<string, object> action)
        {
            PropertyFormatter.FormatProperties(this, action);
        }

        public string FormatProperties(Func<string, object, string> func, string separator = "")
        {
            var s = new List<string>();
            FormatProperties((key, value) => { s.Add(func(key, value)); });
            return string.Join(separator, s);
        }

    }

}
