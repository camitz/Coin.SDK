using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK.Model
{
    public class OrderResponseModel
    {
        public string CustomerRedirectUrl { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseStatus Status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus OrderStatus { get; set; }
        public int OrderID { get; set; }
        public string Message { get; set; }

        public string OrderRef { get; set; }
    }


    public enum ResponseStatus
    {
        Success
    }

    public enum OverdueStatus
    {
        NotOverdue,
        OverduePayment,
        Overdue
    }

    public enum PaymentStatus
    {
        NoPaymentMade,
        PaidInFull,
        PartialPayment
    }

    public enum OrderStatus
    {
        Created,
        PendingSpecification,
        PendingAccept,
        Accepted,
        Cancelled,
    }
}