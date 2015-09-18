using Coin.SDK.Signing;

namespace Coin.SDK.Model
{
    public interface IAdditionalMerchant
    {
        [IncludeInSignature]
        decimal Amount { get; set; }

        [IncludeInSignature]
        int MerchantID { get; set; }
    }

    public class AdditionalMerchant : IAdditionalMerchant
    {
        public decimal Amount { get; set; }

        public int MerchantID { get; set; }
    }
}