using Coin.SDK.Services.Attributes;

namespace Coin.SDK.Model
{
    public interface IInvoiceOrder : IBlankOrder
    {
        [Required]
        decimal? Amount { get; set; }

        bool? IsBlankAmount { get; }
        bool? IsBlankCustomer { get; }
        bool IsInvoice { get; }

        ICustomer Customer { get; set; }
    }
}