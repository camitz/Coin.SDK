using System;

namespace Coin.SDK.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormattingPrefix : Attribute
    {
        private readonly string _prefix;

        public FormattingPrefix(string prefix)
        {
            _prefix = prefix;
        }

        public string Value
        {
            get { return _prefix; }
        }
    }
}