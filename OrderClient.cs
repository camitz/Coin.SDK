using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Coin.SDK.Model;

namespace Coin.SDK
{
    public class OrderClient
    {
        private readonly int? _merchantID;
        private readonly string _keyID;
        private readonly string _secret;
        private HttpClient _client;

        public OrderClient(int? merchantID, string keyID, string secret, string siteUrl)
        {
            _merchantID = merchantID;
            _keyID = keyID;
            _secret = secret;

            _client = new HttpClient { BaseAddress = new Uri(siteUrl) };

            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public Task<OrderResponseModel> PutOrderAsync(IOrder order)
        {
            var signedOrder = order.ToSignedOrder();

            order.MerchantID = _merchantID ?? order.MerchantID;

            signedOrder.Sign(_keyID, _secret);


            return Task.Factory.StartNew(() => _client
                .PutAsJsonAsync("api/Order", order)
                                                   .ContinueWith(x =>
                                                                     {
                                                                         if (x.Result.IsSuccessStatusCode)
                                                                         {
                                                                             return x.Result.Content.ReadAsAsync<OrderResponseModel>().ContinueWith(y => y.Result).Result;
                                                                         }

                                                                         throw new CocoinAPIException(x.Result.StatusCode, x.Result.ReasonPhrase);
                                                                     })).Result;
        }

        public OrderResponseModel PutOrder(IOrder order)
        {
            try
            {
                return PutOrderAsync(order).Result;
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        public OrderStatusModel GetOrder(int orderID)
        {
            try
            {
                return GetOrderAsync(orderID).Result;
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        private Task<OrderStatusModel> GetOrderAsync(int orderID)
        {
            return Task.Factory.StartNew(() => _client.GetAsync("api/Order/" + orderID).ContinueWith(x =>
            {
                if (x.Result.IsSuccessStatusCode)
                {
                    return x.Result.Content.ReadAsAsync<OrderStatusModel>().ContinueWith(y => y.Result).Result;
                }

                throw new CocoinAPIException(x.Result.StatusCode, x.Result.ReasonPhrase);
            }).Result);

        }
    }

    public class CocoinAPIException : Exception
    {
        public CocoinAPIException(HttpStatusCode statusCode, string reasonPhrase)
            : base(string.Format("The request returned with a {0} and the message: {1}", statusCode, reasonPhrase))
        {

        }
    }
}