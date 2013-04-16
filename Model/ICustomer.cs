namespace Coin.SDK.Model
{
    public interface ICustomer
    {
        bool? IsBlankCustomer { get; set; }
    }


    public class CustomerByEmail : ICustomer
    {
        public CustomerByEmail(string email, bool? isBlankCustomer=null)
        {
            Email = email;
            IsBlankCustomer = isBlankCustomer;
        }


        public CustomerByEmail()
        {
        }

        [Email]
        public string Email { get; set; }

        [Ignore]
        public bool? IsBlankCustomer { get; set; }
    }

    class CustomerByFacebook : ICustomer
    {
        public long FacebookID { get; set; }

        [Ignore]
        public bool? IsBlankCustomer { get; set; }
    }

}