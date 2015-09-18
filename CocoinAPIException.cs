using System;
using System.Net;

namespace Coin.SDK
{
    public class CocoinAPIException : Exception
    {
        public CocoinAPIException(HttpStatusCode statusCode, string reasonPhrase)
            : base(string.Format("The request returned with a {0} and the message: {1}", statusCode, reasonPhrase))
        {

        }
    }
}