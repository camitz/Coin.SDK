using System;
using Coin.SDK.Services.Attributes;

namespace Coin.SDK.Signing
{
    public interface ISignedObject
    {
        [IgnoreForFormatting]
        string Signature { get; }

        [IncludeInSignature]
        string ConsumerKey { get;  }

        [IncludeInSignature]
        string Nonce { get; }

        [IncludeInSignature]
        [UnixTimestamp]
        DateTime Timestamp { get; }
    }
}