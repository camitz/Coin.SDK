using Coin.SDK.Services.Attributes;
using Coin.SDK.Signing;

namespace Coin.SDK.Model
{
    public interface IOrder
    {
        [Required]
        [IncludeInSignature]
        int? MerchantID { get; set; }

        [Required]
        [IncludeInSignature]
        string OrderRef { get; set; }

        [IncludeInSignature]
        string GroupRef { get; set; }

        //void FormatProperties(Action<string, object> action);
        //string FormatProperties(Func<string, object, string> func, string separator = "");
    }
}