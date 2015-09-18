using System;

namespace Coin.SDK.Model
{
    public class InvoiceOrder : OrderDecorator, IInvoiceOrder
    {
        public InvoiceOrder(IBlankOrder order)
            : base(order)
        {
        }

        public override bool? IsBlankAmount { get { return false; } }
        public override bool? IsBlankCustomer { get { return true; } }
        public bool IsInvoice { get { return true; } }

        public override decimal? SuggestedAmount
        {
            get { throw new InvalidOperationException("Amount must be specified for InvoiceOrder."); }
            set { throw new InvalidOperationException("Amount must be specified for InvoiceOrder."); }
        }
      
    }
}