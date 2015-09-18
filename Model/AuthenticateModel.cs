using System;
using System.Configuration;
using System.Globalization;
using Coin.SDK.Services.Attributes;
using Coin.SDK.Signing;
using Newtonsoft.Json;

namespace Coin.SDK.Model
{
    public class AuthenticateModel : ISignedObject
    {
        private DateTime? _now;
        private string _nonce;
        private bool _isSigned;
        private string _signature;
        private DateTime? _timestamp;

        public string ConsumerKey { get; set; }

        [IgnoreForFormatting]
        [JsonIgnore]
        public string ConsumerSecret { get; set; }

        public string Nonce
        {
            get
            {
                return _nonce ??
                       (_nonce = (Now.Ticks ^ MerchantID ^ new Random().Next()).ToString(CultureInfo.InvariantCulture));
            }
            set { _nonce = value; }
        }

        [IncludeInSignature]
        public int MerchantID { get; set; }

        [IgnoreForFormatting]
        private DateTime Now
        {
            get { return (DateTime) (_now ?? (_now = DateTime.UtcNow)); }
            set { _now = value; }
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
            consumerSecret = consumerSecret ??
                             ConsumerSecret ?? ConfigurationManager.AppSettings[Constants.CocoinConsumerSecret];

            _signature = new Signer(new KeyValueSigner()).Sign(this, consumerSecret);
            _isSigned = true;

            return _signature;
        }

        [JsonConverter(typeof (UnixTimestampJsonConverter))]
        public DateTime Timestamp
        {
            get { return _timestamp ?? Now; }
            set { _timestamp = value; }
        }
    }
}