using System;

namespace Coin.SDK.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class FormatAttribute : Attribute
    {
        public abstract string Format(object o);
    }
}