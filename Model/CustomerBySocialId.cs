using Coin.SDK.Services.Attributes;
using Coin.SDK.Signing;

namespace Coin.SDK.Model
{
    public class CustomerBySocialId : ICustomer
    {
        private string _email;

        public CustomerBySocialId(string socialId)
        {
            SocialIdNumber = socialId;
        }


        public CustomerBySocialId()
        {
        }

        [Email]
        [IncludeInSignature]
        public string Email { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }

        [IncludeInSignature]
        public string SocialIdNumber { get; set; }
    }
}