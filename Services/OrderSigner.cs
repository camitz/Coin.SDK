using System;
using Coin.SDK.Model;
using Coin.SDK.Signing;

namespace Coin.SDK.Services
{
    public class OrderSigner
    {
        private readonly Signer _signer;

        public OrderSigner()
        {
            _signer = new Signer(new KeyValueSigner());
        }

        public OrderSigner(IKeyValueSigner keyValueSigner)
        {
            _signer = new Signer(keyValueSigner);
        }

        public string SignOrder(SignedOrder order, string secret)
        {
            string[] messages;
            if (!order.Validate(out messages))
                throw new InvalidOperationException("Order did not validate: " + string.Join("\n", messages));

            return _signer.Sign(order, secret);
        }

    }
}