using System;
using System.Configuration;

namespace Coin.SDK
{
    public static class APIClientFactory
    {
        public static OrderClient CreateOrderClient()
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
                                   ConfigurationManager.AppSettings[Constants.CocoinConsumerKeyID],
                                   ConfigurationManager.AppSettings[Constants.CocoinConsumerSecretKey],
                                   ConfigurationManager.AppSettings[Constants.CocoinApiUrl] ?? "https://www.cocoin.com");
        }
    }
}