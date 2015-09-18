using Coin.SDK.Services.Attributes;

namespace Coin.SDK.Model
{
    public interface ICompleteOrder : IBlankOrder
    {
        [Required]
        decimal? Amount { get; set; }

        bool? IsBlankAmount { get; }
        bool? IsBlankCustomer { get; }

        [Required]
        ICustomer Customer { get; set; }
    }
}