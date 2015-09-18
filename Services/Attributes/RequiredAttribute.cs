using System.Reflection;

namespace Coin.SDK.Services.Attributes
{
    public class RequiredAttribute : ValidationAttribute
    {
        public override bool Validate(PropertyInfo propertyInfo, object o, out string message)
        {
            var value = propertyInfo.GetValue(o, null);
            message = null;
            if (value != null)
                return true;

            message = Message ?? string.Format("{0} is a required property for {1}.", propertyInfo.Name, o.GetType().Name);
            return false;
        }
    }
}