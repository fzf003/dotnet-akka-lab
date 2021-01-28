using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubShardMessage
{
    public class PayLoad : ITradeEvent
    {
        public PayLoad(string body)
        {
            Body = body;
        }

        public string Body { get; }
    }
}
