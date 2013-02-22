using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Coin.SDK.Model;

namespace Coin.SDK
{
    public class OrderSigner
    {
        private readonly IKeyValueSigner _keyValueSigner;

        public OrderSigner(IKeyValueSigner keyValueSigner)
        {
            _keyValueSigner = keyValueSigner;
        }

        public string Sign(ISignedOrder order, string consumerKeySecret)
        {
            string[] messages;
            if (!order.Validate(out messages))
                throw new InvalidOperationException("Order did not validate: " + string.Join("\n", messages));

            order.FormatProperties((key, value) => _keyValueSigner.Add(key, value));

            return _keyValueSigner.Sign(consumerKeySecret);
        }
    }
}