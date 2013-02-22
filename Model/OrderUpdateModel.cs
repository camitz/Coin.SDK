using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK.Model
{
    public class OrderUpdateModel
    {
        public int OrderID { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseStatus Status { get; set; }

        public string Message { get; set; }
    }
}