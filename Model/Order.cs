using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Runtime.InteropServices;
using Coin.SDK.Services.Attributes;

namespace Coin.SDK.Model
{
    [ComVisible(true)]
    public class Order : OrderBase, IBlankOrder
    {
        private DateTime? _dueDate;
        private bool? _isInvoiceOrder;
        private List<IAdditionalMerchant> _additionalMerchants;
        private List<IOrderItem> _orderItems;
        private DateTime? _acceptBefore;
        private int? _acceptWithin;


        public decimal? Amount { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Subtotal { get; set; }

        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                if (value.HasValue)
                    _dueDate = value.Value.Date;
            }
        }

        public int? AcceptWithin
        {
            get { return _acceptWithin; }
            set { _acceptWithin = value; }
        }

        public bool HasAcceptLimit { get { return !AcceptWithin.HasValue && AcceptWithin.Value > 0 || AcceptBefore.HasValue; } }
        public DateTime? AcceptBefore
        {
            get
            {
                if (_acceptBefore.HasValue)
                    return _acceptBefore;

                if (_acceptWithin.HasValue && _acceptWithin.Value <= 0) 
                    return null;

                return DateTime.UtcNow.AddMinutes(_acceptWithin.Value);
            }
            set
            {
                _acceptBefore = value;
                if (value.HasValue)
                    _acceptWithin = (value.Value - DateTime.UtcNow).Minutes;
            }
        }

        public bool Is<T>() where T : class, IBlankOrder
        {
            return this is T;
        }

        public virtual bool? IsBlankAmount { get; set; }

        public virtual bool? IsBlankCustomer { get; set; }

        public bool? IsInvoiceOrder
        {
            get { return _isInvoiceOrder; }
            set
            {
                if (value.HasValue && value.Value)
                    IsBlankAmount = false;
                _isInvoiceOrder = value;
            }
        }

        public virtual bool? IsInteractive { get; set; }
        public virtual bool? DisableReminderFee { get; set; }
        public ICustomer Customer { get; set; }

        public string Description { get; set; }

        public string ReturnUrl { get; set; }

        [IgnoreForFormatting]
        public decimal? SuggestedAmount
        {
            get
            {
                if (!(IsBlankAmount.HasValue && IsBlankAmount.Value))
                    return null;

                return Amount;
            }
            set
            {
                IsBlankAmount = true;
                Amount = value;
            }
        }

        [IgnoreForFormatting]
        public ICustomer SuggestedCustomer
        {
            get
            {
                if (!(IsBlankCustomer.HasValue && IsBlankCustomer.Value))
                    return null;

                return Customer;
            }
            set
            {
                IsBlankCustomer = true;
                Customer = value;
            }
        }

        [IgnoreForFormatting]
        public decimal? SpecifiedAmount
        {
            get
            {
                if (IsBlankAmount.HasValue && IsBlankAmount.Value)
                    return null;

                return Amount;
            }
            set
            {
                IsBlankAmount = false;
                Amount = value;
            }
        }

        [IgnoreForFormatting]
        public ICustomer SpecifiedCustomer
        {
            get
            {
                if (IsBlankCustomer.HasValue && IsBlankCustomer.Value)
                    return null;

                return Customer;
            }
            set
            {
                IsBlankCustomer = false;
                Customer = value;
            }
        }

        public IList<IAdditionalMerchant> AdditionalMerchants
        {
            get { return _additionalMerchants ?? (_additionalMerchants = new List<IAdditionalMerchant>()); }
        }

        public IList<IOrderItem> OrderItems
        {
            get { return _orderItems ?? (_orderItems = new List<IOrderItem>()); }
        }






        public IDictionary<string, string> FormatPropertiesForRequest()
        {
            var data = new Dictionary<string, string>();
            FormatProperties((key, value) => data.Add(key, string.Format(CultureInfo.InvariantCulture, "{0}: '{1}'", key, value)));

            return data;
        }

        public string CocoinCreateOrderUrl
        {
            get
            {
                var url = new UriBuilder(ConfigurationManager.AppSettings["CocoinBaseUrl"] ?? "https://www.cocoin.com")
                {
                    Path = "Order/Create",
                    Query = FormatProperties((key, value) => string.Format(CultureInfo.InvariantCulture, "{0}={1}", key, value), "&")
                };

                return url.ToString();
            }
        }



        public bool Validate()
        {
            string[] messages;
            return Validate(out messages);
        }

        public bool Validate(out string[] messages)
        {
            return new OrderValidator().Validate(this, out messages);
        }

    }
}