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

        public bool Validate(IBlankOrder order)
        {
            string[] messages;
            return Validate(order, out messages);
        }

        public bool Validate(IBlankOrder order, out string[] messages)
        {
            _failed = ValidateProperties(order, _messages);

            var blankOrder = order as IBlankOrder;

            if (!(order.Is<ICompleteOrder>()))
            {
                if (blankOrder.IsBlankAmount.HasValue && !blankOrder.IsBlankAmount.Value && !blankOrder.Amount.HasValue)
                {
                    _failed = true;
                    _messages.Add("IsBlankAmount is false but Amount is not set.");
                }

                if (blankOrder.IsBlankCustomer.HasValue && !blankOrder.IsBlankCustomer.Value && blankOrder.Customer == null)
                {
                    _failed = true;
                    _messages.Add("IsBlankCustomer is false but Customer is not set.");
                }

                if (blankOrder.IsBlankAmount.HasValue && !blankOrder.IsBlankAmount.Value && blankOrder.IsBlankCustomer.HasValue && !blankOrder.IsBlankCustomer.Value)
                {
                    _failed = true;
                    _messages.Add("Overspecified IBlankOrder. By fixing both Amount and Customer (IsBlankAmount and IsBlankCustomer are both false) you have overspecified your blank order." +
                        " You should wrap this order in a CompleteOrder.");
                }
            }

            messages = _messages.ToStringArray();

            return !_failed;
        }

        private static bool ValidateProperties(object specimen, MessageHandler _messages)
        {
            var failed = false;
            foreach (var propertyInfo in specimen.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                IEnumerable<ValidationAttribute> t;
                if ((t = HierarchicalAttributeGetter.GetAttributes<ValidationAttribute>(propertyInfo)).Any())
                {
                    foreach (var validationAttribute in t)
                    {
                        string message;
                        if (!validationAttribute.Validate(propertyInfo, specimen, out message))
                        {
                            failed = true;
                            _messages.Add(message);
                        }
                    }
                }
                if (!(propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsValueType || Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null || propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType == typeof(Decimal)))
                {
                    var o = propertyInfo.GetValue(specimen, null);
                    if (o != null)
                        failed = ValidateProperties(o, _messages) || failed;
                }

            }
            return failed;
        }
    }
}