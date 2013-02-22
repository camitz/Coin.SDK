using System.Collections.Generic;

namespace Coin.SDK
{
    internal class MessageHandler
    {
        List<string> _messages = new List<string>();
        public void Add(string s, params object[] args)
        {
            _messages.Add(string.Format(s, args));
        }

        public string[] ToStringArray()
        {
            return _messages.ToArray();
        }

        public void AddRange(IEnumerable<string> ss)
        {
            _messages.AddRange(ss);
        }
    }
}