namespace Coin.SDK.Signing
{
    public interface IKeyValueSigner
    {
        string Sign(string key);
        void Add(string key, object value);
    }
}