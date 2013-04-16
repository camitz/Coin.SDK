using System;
using System.Configuration;
using Coin.SDK.Model;
using Newtonsoft.Json;

namespace Coin.SDK
{
    public class SignedOrder : OrderDecorator, ISignedOrder
    {
        private bool _isSigned;
        private string _signature;
        private string _nonce;
        private DateTime? _now;
        private DateTime? _timestamp;
        private string _consumerKey;
        private string _consumerSecret;

        public SignedOrder(IBlankOrder order)
            : base(order)
        {
        }

        public string Signature
        {
            get
            {
                if (!_isSigned)
                {
                    Sign();
                }

                return _signature;
            }
        }

        public string ConsumerKey
        {
            get
            {
                var order = GetWrappedOrder<SignedOrder>();

                if (order != null)
                    return order.ConsumerKey;

                return _consumerKey;
            }
            set
            {
                var order = GetWrappedOrder<SignedOrder>();

                if (order != null)
                    order.ConsumerKey = value;

                _consumerKey = value;
            }
        }

        public string ConsumerSecret
        {
            get
            {
                var order = GetWrappedOrder<SignedOrder>();

                if (order != null)
                    return order.ConsumerSecret;

                return _consumerSecret;
            }
            set
            {
                var order = GetWrappedOrder<SignedOrder>();

                if (order != null)
                    order.ConsumerSecret = value;

                _consumerSecret = value;
            }
        }

        public string Sign()
        {
            return Sign(null, ConfigurationManager.AppSettings[Constants.CocoinConsumerSecret]);
        }

        public string Sign(string consumerKeySecret)
        {
            return Sign(null, consumerKeySecret);
        }

        public string Sign(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey ?? ConsumerKey ?? ConfigurationManager.AppSettings[Constants.CocoinConsumerKey];
            consumerSecret = consumerSecret ?? ConsumerSecret ?? ConfigurationManager.AppSettings[Constants.CocoinConsumerSecret];

            _signature = new OrderSigner(new KeyValueSigner()).Sign(this, consumerSecret);
            _isSigned = true;

            return _signature;
        }

        public string Nonce
        {
            get
            {
                var order = GetWrappedOrder<SignedOrder>();

                if (order != null)
                    return order.Nonce;

                return _nonce ?? (_nonce = (Now.Ticks ^ MerchantID ^ new Random().Next()).ToString());
            }
            set
            {
                var order = GetWrappedOrder<SignedOrder>();

                if (order != null)
                    order.Nonce = value;

                _nonce = value;
            }
        }

        [Ignore]
        public DateTime Now
        {
            get { return (DateTime)(_now ?? (_now = DateTime.Now)); }
            internal set { _now = value; }
        }

        [UnixTimestamp]
        [JsonConverter(typeof(UnixTimestampJsonConverter))]
        public DateTime Timestamp
        {
            get
            {
                var order = GetWrappedOrder<SignedOrder>();

                if (order != null)
                    return order.Timestamp;

                return _timestamp ?? Now;
            }
            set
            {
                var order = GetWrappedOrder<SignedOrder>();

                if (order != null)
                    order.Timestamp = value;

                _timestamp = value;
            }
        }


    }
}