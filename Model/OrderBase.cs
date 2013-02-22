using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Coin.SDK.Model
{
    public interface IOrder
    {
        [Required]
        int? MerchantID { get; set; }

        [Required]
        string OrderRef { get; set; }

        bool Validate(out string[] messages);
        bool Validate();
        void FormatProperties(Action<string, object> action);
        string FormatProperties(Func<string, object, string> func);
        ISignedOrder ToSignedOrder();
    }

    public interface ISignedOrder : IBlankOrder
    {
        [Ignore]
        string Signature { get; }

        [Required(Message = "ConsumerKeyID must be specified. You can specify it in the *.Config file using the key CocoinConsumerSecretKey or by manually signing the order with the ConsumerKeyID and ConsumerSecretKey as arguments.")]
        string ConsumerKeyID { get; set; }

        [Required]
        string Nonce { get; set; }
        [Required]
        DateTime Timestamp { get; set; }

        string Sign();
        string Sign(string consumerKeyId, string consumerKeySecret);
        string Sign(string consumerKeySecret);
    }

    public interface IBlankOrder : IOrder
    {
        decimal? Amount { get; set; }

        [Email]
        string Email { get; set; }
    }

    public interface ISpecifiedOrder : IBlankOrder
    {
        [Required]
        decimal? Amount { get; set; }

        [Required]
        string Email { get; set; }
    }

    public abstract class OrderBase : IOrder
    {
        private int? _merchantID;
        public int? MerchantID
        {
            get {
                try
                {
                    return _merchantID ?? (_merchantID = Int32.Parse(ConfigurationManager.AppSettings[Constants.CocoinMerchantID]));
                }
                catch (Exception)
                {
                    return null;
                }

            }
            set { _merchantID = value; }
        }

        public string OrderRef { get; set; }


        public bool Validate()
        {
            return new OrderValidator().Validate(this);
        }

        public bool Validate(out string[] messages)
        {
            return new OrderValidator().Validate(this, out messages);
        }

        public void FormatProperties(Action<string, object> action)
        {
            foreach (
                var propertyInfo in
                    GetType().GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance))
            {
                if (HierarchicalAttributeGetter.GetAttributes<Ignore>(propertyInfo).Any())
                    continue;

                var value = propertyInfo.GetValue(this, null);
                if (value != null)
                {
                    var key = PropertyNameConventionConverter.ToOauthStyle(propertyInfo.Name);

                    if (HierarchicalAttributeGetter.GetAttributes<FormatAttribute>(propertyInfo).Any())
                        value = HierarchicalAttributeGetter.GetAttributes<FormatAttribute>(propertyInfo).Single().Format(value);

                    action(key, value);
                }
            }
        }

        public string FormatProperties(Func<string, object, string> func)
        {
            var s = string.Empty;
            FormatProperties((key, value) => { s += func(key, value); });
            return s;
        }

        public abstract ISignedOrder ToSignedOrder();
    }

}
