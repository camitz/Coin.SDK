using Coin.SDK.Services.Attributes;
using Coin.SDK.Signing;

namespace Coin.SDK.Model
{
    public class CustomerByEmail : ICustomer
    {
        private string _email;

        public CustomerByEmail(string email)
        {
            Email = email;
        }


        public CustomerByEmail()
        {
        }

        [Email]
        [IncludeInSignature]
        public string Email
        {
            get { return _email; }
            set { _email = string.IsNullOrEmpty(value) ? value : value.Trim().ToLowerInvariant(); }
        }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public string SocialIdNumber { get; set; }
    }
}