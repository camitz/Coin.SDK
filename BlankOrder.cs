using System;
using System.Configuration;
using Coin.SDK.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coin.SDK
{
    public class BlankOrder : OrderBase, IBlankOrder
    {
        private DateTime? _dueDate;

        public override ISignedOrder ToSignedOrder()
        {
            var signedOrder = this as ISignedOrder;
            if (signedOrder == null)
            {
                signedOrder = new SignedBlankOrder
                                  {
                                      Amount = Amount,
                                      Email = Email,
                                      OrderRef = OrderRef,
                                      MerchantID = MerchantID,
                                      DueDate = DueDate
                                  };
            }

            return signedOrder;
        }

        public decimal? Amount { get; set; }
        public string Email { get; set; }

        [ShortDateTimeString]
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                if (value.HasValue)
                    _dueDate = value.Value.Date;
            }
        }
    }

    public class SignedBlankOrder : BlankOrder, ISignedOrder
    {
        private bool _isSigned;
        private string _signature;
        private string _nonce;
        private DateTime? _now;
        private DateTime? _timestamp;

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

        public string ConsumerKeyID { get; set; }

        public string Sign()
        {
            return Sign(null, ConfigurationManager.AppSettings[Constants.CocoinConsumerSecretKey]);
        }

        public string Sign(string consumerKeySecret)
        {
            return Sign(null, consumerKeySecret);
        }

        public string Sign(string consumerKeyId, string consumerKeySecret)
        {
            ConsumerKeyID = consumerKeyId ?? ConsumerKeyID ?? ConfigurationManager.AppSettings[Constants.CocoinConsumerSecretKey];

            _signature = new OrderSigner(new KeyValueSigner()).Sign(this, consumerKeySecret);
            _isSigned = true;

            return _signature;
        }

        public string Nonce
        {
            get { return _nonce ?? (_nonce = (Now.Ticks ^ MerchantID ^ new Random().Next()).ToString()); }
            set { _nonce = value; }
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
            get { return _timestamp ?? Now; }
            set { _timestamp = value; }
        }
    }

    public class SpecifiedOrder : BlankOrder, ISpecifiedOrder
    {

    }

    public class SignedSpecifiedOrder : SignedBlankOrder, ISpecifiedOrder
    {
    }

    public static class DateTimeExtensions
    {

        public static DateTime FromUnixTime(this long s)
        {
            var unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return unixRef.AddSeconds(s);
        }

        public static long ToUnixTime(this DateTime d)
        {
            var unixRef = new DateTime(1970, 1, 1, 0, 0, 0);

            return (d.Ticks - unixRef.Ticks) / 10000000;
        }
    }
}