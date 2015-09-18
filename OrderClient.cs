using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Coin.SDK.Model;
using log4net;

namespace Coin.SDK
{
    public class OrderClient
    {
        private readonly int? _merchantID;
        private readonly string _siteUrl;

        public string AccessToken
        {
            get { return _accessToken; }
            private set
            {
                _accessToken = value;
            }
        }

        private readonly ILog _log = LogManager.GetLogger(typeof(OrderClient));
        private string _accessToken;


        private OrderClient(int? merchantID, string siteUrl)
        {
            _merchantID = merchantID;
            _siteUrl = siteUrl;
        }

        internal OrderClient(int? merchantID, string siteUrl, string accessToken)
            : this(merchantID, siteUrl)
        {
            _accessToken = accessToken;
        }

        public async Task<OrderResponseModel> PostOrderAsync(IBlankOrder order)
        {
            //var signedOrder = new SignedOrder(order);

            order.MerchantID = _merchantID ?? order.MerchantID;

            //signedOrder.Sign(_keyID, _secret);

            var _client = GetHttpClient();


            var p2 = await _client
                .PostAsJsonAsync("api/order", order);

            if (p2.IsSuccessStatusCode)
            {
                try
                {
                    return await p2.Content.ReadAsAsync<OrderResponseModel>().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    var s = p2.Content.ReadAsStringAsync().Result;
                    throw new CocoinAPIException(HttpStatusCode.InternalServerError, "Could not deserialize response: " + s);
                }
            }


            var s2 = await p2.Content.ReadAsStringAsync();

            throw new CocoinAPIException(p2.StatusCode, "Unknown error. Content: " + s2);
        }

        private HttpClient GetHttpClient()
        {
            var _client = new HttpClient
                          {
                              BaseAddress = new Uri(_siteUrl)
                          };


            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(_accessToken))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("AccessToken", _accessToken);
            return _client;
        }


        public async Task<OrderStateModel> GetOrderAsync(int orderID)
        {
            var _client = GetHttpClient();

            var p2 = await _client.GetAsync("api/order/" + orderID);
            if (p2.IsSuccessStatusCode)
                return await p2.Content.ReadAsAsync<OrderStateModel>().ConfigureAwait(false);

            throw new CocoinAPIException(p2.StatusCode, p2.ReasonPhrase);
        }

        public async Task<ICollection<OrderStateModel>> GetOrdersAsync(int? offset=null, int? count=null)
        {
            var uri = "api/orders/"+_merchantID;

            var parms = new Dictionary<string, string>
                        {
                            {"offset", Convert.ToString(offset)},
                            {"count", Convert.ToString(count)}
                        };

            if (parms.Values.Any(x => !string.IsNullOrEmpty(x)))
                uri = string.Format("{0}?{1}", uri, string.Join("&", parms.Select(x => string.Format("{0}={1}", x.Key, x.Value))));

            var _client = GetHttpClient();

            _client.Timeout = TimeSpan.FromMinutes(10);

            var p2 = await _client.GetAsync(uri);
            if (p2.IsSuccessStatusCode)

                return await p2.Content.ReadAsAsync<ICollection<OrderStateModel>>().ConfigureAwait(false);

            throw new CocoinAPIException(p2.StatusCode, p2.ReasonPhrase);
        }

        public async Task<string> AuthenticateAsync(AuthenticateModel authenticateModel)
        {
            authenticateModel.MerchantID = _merchantID ?? authenticateModel.MerchantID;

            var _client = GetHttpClient();

            var p2 = await _client
                .PostAsJsonAsync("api/authenticate", authenticateModel);

            if (p2.IsSuccessStatusCode)
            {
                try
                {
                    var t = await p2.Content.ReadAsAsync<AccessTokenModel>().ConfigureAwait(false);
                    AccessToken = t.AccessToken;
                    return t.AccessToken;
                }
                catch (Exception e)
                {
                    var s = p2.Content.ReadAsStringAsync().Result;
                    throw new CocoinAPIException(HttpStatusCode.InternalServerError, "Could not deserialize response: " + s);
                }
            }

            var s2 = await p2.Content.ReadAsStringAsync().ConfigureAwait(false);

            throw new CocoinAPIException(p2.StatusCode, "Unknown error. Content: " + s2);
        }

        public Task<string> AuthenticateAsync()
        {
            if (!_merchantID.HasValue)
                throw new NullReferenceException("MerchantID");

            var authenticateModel = new AuthenticateModel
                                    {
                                        MerchantID = _merchantID.Value
                                    };

            authenticateModel.Sign();

            return AuthenticateAsync(authenticateModel);
        }

        public async Task<OrderStateModel> AcceptAsync(int orderID)
        {
            var _client = GetHttpClient();

            var p2 = await _client.PutAsync("api/order/accept/" + orderID, null);
            if (p2.IsSuccessStatusCode)
                return await p2.Content.ReadAsAsync<OrderStateModel>().ConfigureAwait(false);

            throw new CocoinAPIException(p2.StatusCode, p2.ReasonPhrase);
        }

        public async Task<CancelResponseModel>CancelOrderAsync(int orderID)
        {
            var _client = GetHttpClient();

            var p2 = await _client.PutAsync("api/order/cancel/" + orderID, null);
            if (p2.IsSuccessStatusCode)
                return await p2.Content.ReadAsAsync<CancelResponseModel>().ConfigureAwait(false);

            throw new CocoinAPIException(p2.StatusCode, p2.ReasonPhrase);
        }
    }
}