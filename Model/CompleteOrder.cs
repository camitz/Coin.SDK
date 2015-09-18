using System;

namespace Coin.SDK.Model
{
    public class CompleteOrder : OrderDecorator, ICompleteOrder
    {
        public CompleteOrder(IBlankOrder order)
            : base(order)
        {
        }

        public override bool? IsBlankAmount { get { return false; } }
        public override bool? IsBlankCustomer { get { return false; } }

        public override decimal? SuggestedAmount
        {
            get { throw new InvalidOperationException("Amount and Customer must be specified for CompleteOrder."); }
            set { throw new InvalidOperationException("Amount and Customer must be specified for CompleteOrder."); }
        }

        public override ICustomer SuggestedCustomer
        {
            get { throw new InvalidOperationException("Amount and Customer must be specified for CompleteOrder."); }
            set { throw new InvalidOperationException("Amount and Customer must be specified for CompleteOrder."); }
        }
    }
}