using Coin.SDK.Model;

namespace Coin.SDK
{

    public interface ICompleteOrder : IBlankOrder
    {
        [Required]
        decimal? Amount { get; set; }

        bool? IsBlankAmount { get; }
        bool? IsBlankCustomer { get; }

        [Required]
        ICustomer Customer { get; set; }
    }

    public class CompleteOrder : OrderDecorator, ICompleteOrder
    {
        public CompleteOrder(IBlankOrder order) : base(order)
        {
        }

        public override bool? IsBlankAmount { get { return false; } }
        public override bool? IsBlankCustomer { get { return false; } }

       
    }
}