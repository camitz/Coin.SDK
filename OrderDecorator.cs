using System;
using Coin.SDK.Model;

namespace Coin.SDK
{
    public abstract class OrderDecorator : IBlankOrder
    {
        protected OrderDecorator(IBlankOrder order)
        {
            Order = order;
        }

        public IBlankOrder Order { get; set; }

        public int? MerchantID { get { return Order.MerchantID; } set { Order.MerchantID = value; } }
        public string OrderRef { get { return Order.OrderRef; } set { Order.OrderRef = value; } }
        public decimal? Amount { get { return Order.Amount; } set { Order.Amount = value; } }
        public virtual bool? IsBlankAmount { get { return Order.IsBlankAmount; } set { Order.IsBlankAmount = value; } }
        public virtual bool? IsBlankCustomer { get { return Order.IsBlankCustomer; } set { Order.IsBlankCustomer = value; } }
        public ICustomer Customer { get { return Order.Customer; } set { Order.Customer = value; } }
        public string ReturnUrl { get { return Order.ReturnUrl; } set { Order.ReturnUrl = value; } }
        public DateTime? DueDate { get { return Order.DueDate; } set { Order.DueDate = value; } }

        public T GetWrappedOrder<T>() where T : class, IBlankOrder
        {
            var order = Order;
            while (order != null && !(order is T) && order is OrderDecorator)
            {
                order = ((OrderDecorator)order).Order;
            }
            var o2 = order as T;
            return o2;
        }

        public bool Is<T>() where T: class, IBlankOrder
        {
            if (this is T)
                return true;

            return GetWrappedOrder<T>() != null;
        }

        public bool Validate(out string[] messages)
        {
            return new OrderValidator().Validate(this, out messages);
        }

        public bool Validate()
        {
            return Order.Validate();
        }

        public void FormatProperties(Action<string, object> action)
        {
            PropertyFormatter.FormatProperties(this, action);
        }

        public string FormatProperties(Func<string, object, string> func)
        {
            var s = string.Empty;
            FormatProperties((key, value) => { s += func(key, value); });
            return s;
        }


    }


}