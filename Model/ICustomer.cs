using Coin.SDK.Signing;
using Newtonsoft.Json;

namespace Coin.SDK.Model
{
    public interface ICustomer
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IncludeInSignature]
        string Firstname { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IncludeInSignature]
        string Lastname { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IncludeInSignature]
        string Phonenumber { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IncludeInSignature]
        string Address { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [IncludeInSignature]
        string SocialIdNumber { get; set; }
    }
}