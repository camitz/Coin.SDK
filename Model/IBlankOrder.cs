using System;
using System.Collections.Generic;
using Coin.SDK.Services.Attributes;
using Coin.SDK.Signing;
using Newtonsoft.Json;

namespace Coin.SDK.Model
{
    public interface IBlankOrder : IOrder
    {
        [IncludeInSignature]
        decimal? Amount { get; set; }
        [IncludeInSignature]
        decimal? Tax { get; set; }
        [IncludeInSignature]
        decimal? Subtotal { get; set; }

        [IncludeInSignature]
        bool? IsBlankAmount { get; set; }

        [IncludeInSignature]
        bool? IsBlankCustomer { get; set; }

        [IncludeInSignature]
        bool? IsInvoiceOrder { get; set; }

        [IncludeInSignature]
        bool? IsInteractive { get; set; }

        [IncludeInSignature]
        bool? DisableReminderFee { get; set; }

        [FormattingPrefix("Customer")]
        [IncludeInSignature]
        ICustomer Customer { get; set; }

        [IncludeInSignature]
        string Description { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IncludeInSignature]
        string ReturnUrl { get; set; }//todo: security strict domain control

        [ShortDateTimeString]
        [IncludeInSignature]
        DateTime? DueDate { get; set; }

        [IncludeInSignature]
        int? AcceptWithin { get; set; }

        [IgnoreForFormatting]
        [JsonIgnore]
        bool HasAcceptLimit { get; }

        [IgnoreForFormatting]
        [JsonIgnore]
        DateTime? AcceptBefore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IncludeInSignature]
        IList<IAdditionalMerchant> AdditionalMerchants { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IncludeInSignature]
        IList<IOrderItem> OrderItems { get; }

        [IgnoreForFormatting]
        [JsonIgnore]
        decimal? SuggestedAmount { get; set; }

        [IgnoreForFormatting]
        [JsonIgnore]
        ICustomer SuggestedCustomer { get; set; }

        [IgnoreForFormatting]
        [JsonIgnore]
        decimal? SpecifiedAmount { get; set; }

        [IgnoreForFormatting]
        [JsonIgnore]
        ICustomer SpecifiedCustomer { get; set; }

        [IgnoreForFormatting]
        [JsonIgnore]
        string CocoinCreateOrderUrl { get; }



        bool Is<T>() where T : class, IBlankOrder;

        bool Validate(out string[] messages);
        bool Validate();
        IDictionary<string,string> FormatPropertiesForRequest();
    }
}