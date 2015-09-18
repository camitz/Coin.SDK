
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK.Model
{
    public class OrderResponseModel
    {
        public string CustomerRedirectUrl { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseState State { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderState OrderState { get; set; }
        public int OrderID { get; set; }
        public string Message { get; set; }

        public string OrderRef { get; set; }

        public string AccessToken { get; set; }
    }


    public enum ResponseState
    {
        Success
    }

    public enum OverdueState
    {
        NotOverdue,
        OverduePaymentMade,
        Overdue
    }

    public enum PaymentState
    {
        NoPaymentMade,
        PaidInFull,
        PartialPayment
    }

    public enum OrderState
    {
        Created,
        PendingSpecification,
        PendingAccept,
        Accepted,
        Canceled,
    }
}