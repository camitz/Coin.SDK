using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK.Model
{
    public class CancelResponseModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseState Status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderState OrderState { get; set; }
        public int OrderID { get; set; }
        public string Message { get; set; }


        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        public string AccessToken { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}