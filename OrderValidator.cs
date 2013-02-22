using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coin.SDK.Model;

namespace Coin.SDK
{
    public class OrderValidator
    {
        private bool _failed;
        private readonly MessageHandler _messages = new MessageHandler();
        private readonly HierarchicalAttributeGetter _hierarchicalAttributeGetter = new HierarchicalAttributeGetter();

        public HierarchicalAttributeGetter HierarchicalAttributeGetter
        {
            get { return _hierarchicalAttributeGetter; }
        }

        public bool Validate(IOrder order)
        {
            string[] messages;
            return Validate(order, out messages);
        }

        public bool Validate(IOrder order, out string[] messages)
        {
            foreach (var propertyInfo in order.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                IEnumerable<ValidationAttribute> t;
                if ((t = HierarchicalAttributeGetter.GetAttributes<ValidationAttribute>(propertyInfo)).Any())
                {
                    foreach (var validationAttribute in t)
                    {
                        string message;
                        if(!validationAttribute.Validate(propertyInfo, order, out message))
                        {
                            _failed = true;
                            _messages.Add(message);
                            
                        }
                    }
                }
            }

            if (!(order is ISpecifiedOrder))
            {
                var o = order as IBlankOrder;
                if (o.Amount.HasValue && !string.IsNullOrEmpty(o.Email))
                {
                    _failed = true;
                    _messages.Add("Overspecified IBlankOrder. By setting both Amount and Email the IBlankOrder has become overspecified. You should use SpecifiedOrder or SignedSpecifiedOrder.");
                }
            }

            messages = _messages.ToStringArray();

            return !_failed;
        }
    }
}