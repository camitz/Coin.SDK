using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using Coin.SDK.Services;
using Coin.SDK.Services.Attributes;
using Newtonsoft.Json;

namespace Coin.SDK.Model
{
    public abstract class OrderDecorator : IBlankOrder
    {
        protected OrderDecorator(IBlankOrder order)
        {
            Order = order;
        }

        [JsonIgnore]
        public IBlankOrder Order { get; set; }

        public int? MerchantID { get { return Order.MerchantID; } set { Order.MerchantID = value; } }
        public string OrderRef { get { return Order.OrderRef; } set { Order.OrderRef = value; } }
        public string GroupRef { get { return Order.GroupRef; } set { Order.GroupRef = value; } }
        public decimal? Amount { get { return Order.Amount; } set { Order.Amount = value; } }
        public decimal? Tax { get { return Order.Tax; } set { Order.Subtotal = value; } }
        public decimal? Subtotal { get { return Order.Subtotal; } set { Order.Subtotal = value; } }
        public ICustomer Customer { get { return Order.Customer; } set { Order.Customer = value; } }
        public string Description { get { return Order.Description; } set { Order.Description = value; } }
        public virtual bool? IsBlankAmount { get { return Order.IsBlankAmount; } set { Order.IsBlankAmount = value; } }
        public virtual bool? IsBlankCustomer { get { return Order.IsBlankCustomer; } set { Order.IsBlankCustomer = value; } }
        public virtual bool? IsInvoiceOrder { get { return Order.IsInvoiceOrder; } set { Order.IsInvoiceOrder = value; } }
        public virtual bool? IsInteractive { get { return Order.IsInteractive; } set { Order.IsInteractive = value; } }
        public virtual bool? DisableReminderFee { get { return Order.DisableReminderFee; } set { Order.DisableReminderFee = value; } }
        public string ReturnUrl { get { return Order.ReturnUrl; } set { Order.ReturnUrl = value; } }
        public DateTime? DueDate { get { return Order.DueDate; } set { Order.DueDate = value; } }

        public virtual int? AcceptWithin { get { return Order.AcceptWithin; } set { Order.AcceptWithin = value; } }

        public virtual decimal? SuggestedAmount { get { return Order.SuggestedAmount; } set { Order.SuggestedAmount = value; } }
        public virtual ICustomer SuggestedCustomer { get { return Order.SuggestedCustomer; } set { Order.SuggestedCustomer = value; } }
        public virtual decimal? SpecifiedAmount { get { return Order.SpecifiedAmount; } set { Order.SpecifiedAmount = value; } }
        public virtual ICustomer SpecifiedCustomer { get { return Order.SpecifiedCustomer; } set { Order.SpecifiedCustomer = value; } }

        public virtual bool HasAcceptLimit { get { return Order.HasAcceptLimit; } }
        public virtual DateTime? AcceptBefore { get { return Order.AcceptBefore; } set { Order.AcceptBefore = value; } }

        public virtual IList<IAdditionalMerchant> AdditionalMerchants { get { return Order.AdditionalMerchants; } }
        public virtual IList<IOrderItem> OrderItems { get { return Order.OrderItems; } }

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

        public bool Is<T>() where T : class, IBlankOrder
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

        public virtual IDictionary<string, string> FormatPropertiesForRequest()
        {
            var data = new Dictionary<string, string>();
            FormatProperties((key, value) => data.Add(key, string.Format(CultureInfo.InvariantCulture, "{0}: '{1}'", key, value)));

            return data;
        }

        public void FormatProperties(Action<string, object> action)
        {
            PropertyFormatter.FormatProperties(this, action);
        }

        public string FormatProperties(Func<string, object, string> func, string separator = "")
        {
            var s = new List<string>();
            FormatProperties((key, value) => s.Add(func(key, value)));
            return string.Join(separator, s);
        }

        [IgnoreForFormatting]
        public virtual string CocoinCreateOrderUrl
        {
            get
            {
                var url = new UriBuilder(ConfigurationManager.AppSettings["CocoinBaseUrl"] ?? "https://www.cocoin.com")
                {
                    Path = (IsInvoiceOrder.HasValue && IsInvoiceOrder.Value ? "Invoice" : "Order") + "/Create",
                    Query = FormatProperties((key, value) => string.Format(CultureInfo.InvariantCulture, "{0}={1}", key, value), "&")
                };

                return url.ToString().Replace("\n", "%20%0A");
            }
        }
    }


}