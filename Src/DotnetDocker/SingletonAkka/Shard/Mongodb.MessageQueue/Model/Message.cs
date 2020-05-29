using System;
using System.Collections.Generic;
using System.Text;

namespace Mongodb.MessageQueue.Model
{
    public class Message
    {
         
        public Dictionary<string, string> Headers { get; }

         
        public byte[] Body { get; }

        
        public Message(byte[] body) : this(new Dictionary<string, string>(), body)
        {
        }

        
        public Message(Dictionary<string, string> headers, byte[] body)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }
    }
}
