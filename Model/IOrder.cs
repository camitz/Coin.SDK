using System;

namespace Coin.SDK.Model
{
    public interface IOrder
    {
        [Required]
        int? MerchantID { get; set; }

        [Required]
        string OrderRef { get; set; }

        void FormatProperties(Action<string, object> action);
        string FormatProperties(Func<string, object, string> func);
    }

    public interface IBlankOrder : IOrder
    {
        decimal? Amount { get; set; }

        bool? IsBlankAmount { get; set; }
        bool? IsBlankCustomer { get; set; }

        ICustomer Customer { get; set; }

        string ReturnUrl { get; set; }//todo: security strict domain control

        [ShortDateTimeString]
        DateTime? DueDate { get; set; }

        bool Is<T>() where T : class, IBlankOrder;

        bool Validate(out string[] messages);
        bool Validate();
    }


    public interface ISignedOrder : IBlankOrder
    {
        [Ignore]
        string Signature { get; }

        [Required(Message = "ConsumerKey must be specified. You can specify it in the *.Config file using the key CocoinConsumerSecret or by manually signing the order with the ConsumerKey and ConsumerSecret as arguments.")]
        string ConsumerKey { get; set; }

        [Required]
        string Nonce { get; set; }
        [Required]
        DateTime Timestamp { get; set; }

        [Ignore]
        string ConsumerSecret { get; set; }

        string Sign();
        string Sign(string consumerKey, string consumerSecret);
        string Sign(string consumerKeySecret);
    }


}