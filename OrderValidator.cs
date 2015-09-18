using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coin.SDK.Model;
using Coin.SDK.Services.Attributes;

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

            if (!(order.Is<ICompleteOrder>()))
            {
                if (order.IsBlankAmount.HasValue && !order.IsBlankAmount.Value && !order.Amount.HasValue)
                {
                    _failed = true;
                    _messages.Add("IsBlankAmount is false but Amount is not set.");
                }

                if (order.IsBlankCustomer.HasValue && !order.IsBlankCustomer.Value && order.Customer == null)
                {
                    _failed = true;
                    _messages.Add("IsBlankCustomer is false but Customer is not set.");
                }

                //Skipping the edit, only accept.
                if (order.IsBlankCustomer.HasValue && !order.IsBlankCustomer.Value &&
                    order.IsInvoiceOrder.HasValue && order.IsInvoiceOrder.Value &&
                    (!order.IsInteractive.HasValue || order.IsInteractive.Value))
                {
                    _failed = true;
                    _messages.Add(
                        "IsBlankCustomer is false but IsInteractive is true (or not set). In InvoiceMode (IsInvoice = true), this combination is not supported currently. If the contrary is important for your business, please contact support@cocoin.com.");
                }

                if ((!order.IsBlankCustomer.HasValue || order.IsBlankCustomer.Value) && order.IsInteractive.HasValue &&
                    !order.IsInteractive.Value)
                {
                    _failed = true;
                    _messages.Add(
                        "IsBlankCustomer is true (or not set) and IsInteractive is also false. This is not a valid combination.");
                }

                if ((!order.IsBlankAmount.HasValue || order.IsBlankAmount.Value) && order.IsInvoiceOrder.HasValue &&
                    order.IsInvoiceOrder.Value)
                {
                    _failed = true;
                    _messages.Add(
                        "IsBlankAmount is true (or not set) but IsInvoiceOrder is true. Currently, in invoice mode, customers cannot change the amount at time of payment. You must set IsBlankAmount to false. If the contrary is important for your business, please contact support@cocoin.com.");
                }

                if ((!order.IsBlankAmount.HasValue || order.IsBlankAmount.Value) &&
                    (order.Subtotal.HasValue || order.Tax.HasValue))
                {
                    _failed = true;
                    _messages.Add(
                        "IsBlankAmount is true (or not set) and also either Tax or Subtotal. Currently, these properties cannot be set in blank mode. If the contrary is important for your business, please contact support@cocoin.com.");
                }

                if (order.IsBlankAmount.HasValue && !order.IsBlankAmount.Value && order.IsBlankCustomer.HasValue &&
                    !order.IsBlankCustomer.Value)
                {
                    _failed = true;
                    _messages.Add(
                        "Overspecified IBlankOrder. By fixing both Amount and Customer (IsBlankAmount and IsBlankCustomer are both false) you have overspecified your blank order." +
                        " You should wrap this order in a CompleteOrder.");
                }

                if ((!order.IsInteractive.HasValue || order.IsInteractive.Value) && order.Customer is CustomerBySocialId)
                {
                    _failed = true;
                    _messages.Add(
                        "Customers specified by social id number is currently only permitted in the non-interactive method.");
                }
            }
            else
            {
                if ((!order.IsInteractive.HasValue || order.IsInteractive.Value) && order.Customer is CustomerBySocialId)
                {
                    _failed = true;
                    _messages.Add(
                        "Customers specified by social id number is currently only permitted in the non-interactive method.");
                }
                
            }

            messages = _messages.ToStringArray();

            return !_failed;
        }

        private static bool ValidateProperties(object specimen, MessageHandler _messages)
        {
            var failed = false;

            if (specimen is IEnumerable)
            {
                var collection = specimen as IEnumerable;
                foreach (var item in collection)
                {
                    failed = ValidateProperties(item, _messages) || failed;
                }

                return failed;
            }

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
                    if (!HierarchicalAttributeGetter.GetAttributes<IgnoreForFormatting>(propertyInfo).Any())
                    {
                        var o = propertyInfo.GetValue(specimen, null);
                        if (o != null)
                            failed = ValidateProperties(o, _messages) || failed;

                    }
                }

            }
            return failed;
        }
    }
}