using System;
using Coin.SDK.Services.Attributes;
using Newtonsoft.Json;

namespace Coin.SDK.Model
{
    public interface ISignedOrder : IBlankOrder
    {
        [IgnoreForFormatting]
        string Signature { get; }

        [Required(Message = "ConsumerKey must be specified. You can specify it in the *.Config file using the key CocoinConsumerSecret or by manually signing the order with the ConsumerKey and ConsumerSecret as arguments.")]
        string ConsumerKey { get; set; }

        [Required]
        string Nonce { get; set; }

        [Required]
        DateTime Timestamp { get; set; }

        [IgnoreForFormatting]
        [JsonIgnore]
        string ConsumerSecret { get; set; }

        string Sign();
        string Sign(string consumerKey, string consumerSecret);
        string Sign(string consumerKeySecret);
    }
}