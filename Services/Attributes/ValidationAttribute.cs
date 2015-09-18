using System;
using System.Reflection;

namespace Coin.SDK.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ValidationAttribute : Attribute
    {
        public string Message { get; set; }
        public abstract bool Validate(PropertyInfo propertyInfo, object o, out string message);
    }
}