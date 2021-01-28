using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
namespace PubSubShardMessage
{
    public class EchoService:ReceiveActor
    {
        public EchoService()
        {
            this.Receive<PubSubShardMessage.Ping>(Handle);

        }

        private void Handle(Ping obj)
        {
            Sender?.Tell(new Pong(Guid.NewGuid().ToString(),Self.Path.ToString()));
        }
    }
}
