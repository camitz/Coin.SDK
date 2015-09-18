using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK.Model
{
    public class OrderStateModel
    {
        public int OrderID { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderState OrderState { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentState PaymentState { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OverdueState OverdueState { get; set; }

        public decimal Amount { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal AmountPaid { get; set; }

        public string OrderRef { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? ClearedTime { get; set; }

        public DateTime? AcceptedTime { get; set; }
        //public string CustomerRedirectUrl { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string InvoiceUrl { get; set; }
        public string PaymentCode { get; set; }
    }
}