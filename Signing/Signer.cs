using System;
using Coin.SDK.Services;

namespace Coin.SDK.Signing
{
    public class Signer
    {
        private readonly IKeyValueSigner _keyValueSigner;

        public Signer(IKeyValueSigner keyValueSigner)
        {
            _keyValueSigner = keyValueSigner;
        }

        public string Sign(object o, string secret)
        {
            PropertyFormatter.FormatProperties(o, (key, value) => _keyValueSigner.Add(key, value), typeof(IncludeInSignature));

            return _keyValueSigner.Sign(secret);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IncludeInSignature : Attribute
    {
    }

}