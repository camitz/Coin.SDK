using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK.Model
{
    public class OrderStatusModel
    {
        public int OrderID { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus OrderStatus { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus PaymentStatus { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OverdueStatus OverdueStatus { get; set; }

        public decimal Amount { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal AmountPaid { get; set; }

        public string OrderRef { get; set; }

        public DateTime CreatedTime { get; set; }

        public string ClearedTime { get { return "Not yet supported."; } }//todo

        public DateTime? AcceptedTime { get; set; }
    }
}