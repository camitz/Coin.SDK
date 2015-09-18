using System.Reflection;
using System.Text.RegularExpressions;

namespace Coin.SDK.Services.Attributes
{
    public class Email : FormatValidationAttribute
    {
        public override bool Validate(PropertyInfo propertyInfo, object o, out string message)
        {
            message = null;
            var regex = new Regex(@"[\w-\.]+@([\w-]+\.)+[\w-]+");

            var value = propertyInfo.GetValue(o, null);
            if (value == null)
                return true;

            if (regex.IsMatch(value.ToString()))
                return true;

            message = Message ?? string.Format("{0} is malformed in {1}.", propertyInfo.Name, o.GetType().Name);
            return false;
        }
    }
}