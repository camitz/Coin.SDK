using System;
using Coin.SDK.Model;
using Newtonsoft.Json.Converters;

namespace Coin.SDK
{
    public class Order : OrderBase, IBlankOrder
    {
        private DateTime? _dueDate;
        private bool? _isBlankAmount;
        private bool? _isBlankCustomer;
        private ICustomer _customer;


        public decimal? Amount { get; set; }

        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                if (value.HasValue)
                    _dueDate = value.Value.Date;
            }
        }

        public bool Is<T>() where T : class, IBlankOrder
        {
            return this is T;
        }

        public virtual bool? IsBlankAmount
        {
            get { return _isBlankAmount; }
            set { _isBlankAmount = value; }
        }

        public virtual bool? IsBlankCustomer
        {
            get { return _isBlankCustomer; }
            set { _isBlankCustomer = value; }
        }

        public ICustomer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                IsBlankCustomer = value.IsBlankCustomer;
            }
        }

        public string ReturnUrl { get; set; }

        [Ignore]
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


        public bool Validate()
        {
            return new OrderValidator().Validate(this);
        }

        public bool Validate(out string[] messages)
        {
            return new OrderValidator().Validate(this, out messages);
        }

    }
}