using System;
using System.Configuration;
using System.Globalization;
using Coin.SDK.Services;
using Coin.SDK.Services.Attributes;
using Coin.SDK.Signing;
using Newtonsoft.Json;

namespace Coin.SDK.Model
{
    public class SignedOrder : OrderDecorator, ISignedOrder, ISignedObject
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

            _signature = new OrderSigner().SignOrder(this, consumerSecret);
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

        [IgnoreForFormatting]
        [JsonIgnore]
        public DateTime Now
        {
            get { return (DateTime)(_now ?? (_now = DateTime.UtcNow)); }
            internal set { _now = value; }
        }

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

       

        public override System.Collections.Generic.IDictionary<string, string> FormatPropertiesForRequest()
        {
            var data = base.FormatPropertiesForRequest();

            data.Add("signature", string.Format(CultureInfo.InvariantCulture, "{0}: '{1}'", "signature", Signature));

            return data;
        }

        [IgnoreForFormatting]
        public override string CocoinCreateOrderUrl
        {
            get { return string.Format("{0}&signature={1}", base.CocoinCreateOrderUrl, Signature); }
        }
    }
}