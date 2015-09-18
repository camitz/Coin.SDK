using System;
using System.Configuration;

namespace Coin.SDK
{
    public static class APIClientFactory
    {
        public static OrderClient CreateOrderClient()
        {
            return CreateOrderClient(null);
        }

        public static OrderClient CreateOrderClient(string accessToken = null)
        {
            int? merchantID = null;
            try
            {
                merchantID = Int32.Parse(ConfigurationManager.AppSettings[Constants.CocoinMerchantID]);
            }
            catch (Exception)
            {

            }

            return new OrderClient(merchantID,
                                   ConfigurationManager.AppSettings[Constants.CocoinApiBaseUrl] ?? Constants.CocoinDefaultApiBaseUrl,
                                   accessToken);
        }

        public static OrderClient CreateOrderClient(int merchantID, string apiBaseUri=null)
        {
            return new OrderClient(merchantID,
                                   apiBaseUri ?? ConfigurationManager.AppSettings[Constants.CocoinApiBaseUrl] ?? Constants.CocoinDefaultApiBaseUrl, null);
        }

    }
}