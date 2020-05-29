using Mongodb.MessageQueue.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mongodb.MessageQueue.Model
{
    public class ReceivedMessage : Message
    {
        readonly Func<Task> _ack;
        readonly Func<Task> _nack;
        readonly Func<Task> _renew;

        
        public ReceivedMessage(Dictionary<string, string> headers, byte[] body, Func<Task> ack, Func<Task> nack, Func<Task> renew, int deliveryCount) : base(headers, body)
        {
            DeliveryCount = deliveryCount;
            _ack = ack ?? throw new ArgumentNullException(nameof(ack));
            _nack = nack ?? throw new ArgumentNullException(nameof(nack));
            _renew = renew ?? throw new ArgumentNullException(nameof(renew));

            if (!headers.TryGetValue(Fields.MessageId, out var messageId))
            {
                throw new ArgumentException($"ReceivedMessage 必须要求该消息头: '{Fields.MessageId}' 存在");
            }

            MessageId = messageId;
        }

       
        public string MessageId { get; }

      
        public int DeliveryCount { get; }

     
        public Task Ack() => _ack();

       
        public Task Nack() => _nack();

         
        public Task Renew() => _renew();
    }
}
