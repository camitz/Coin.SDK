using Coin.SDK.Signing;

namespace Coin.SDK.Model
{
    public interface IOrderItem
    {
        [IncludeInSignature]
        string Description { get; set; }
        [IncludeInSignature]
        int? Quantity { get; set; }
        [IncludeInSignature]
        decimal? Price { get; set; }
        [IncludeInSignature]
        decimal Total { get; set; }
    }

    public class OrderItem : IOrderItem
    {
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal Total { get; set; }
    }
}