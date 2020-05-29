using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Mongodb.MessageQueue.Exceptions
{
    [Serializable]
    public class UniqueMessageIdViolationException : Exception
    {
        
        public string Id { get; }

      
        public UniqueMessageIdViolationException(string id) : base($"无法发送ID: {id} 因为已存在具有该ID的消息")
        {
            Id = id;
        }

        protected UniqueMessageIdViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
