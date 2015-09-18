using Coin.SDK.Signing;

namespace Coin.SDK.Model
{
    public class CustomerByFacebook : ICustomer
    {
        public CustomerByFacebook(string facebookID)
        {
            FacebookID = facebookID;
        }

        [IncludeInSignature]
        public string FacebookID { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public string SocialIdNumber { get; set; }
    }
}