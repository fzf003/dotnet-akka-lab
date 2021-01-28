using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using Akka.Routing;
using System;


namespace PubSubShardMessage
{
    public class PublisherActor : ReceiveActor
    {

        public static Props PropsFor()
        {
            return Props.Create<PublisherActor>()
                        .WithRouter(new RoundRobinPool(5));
        }
 
        private readonly ILoggingAdapter _log = Context.GetLogger();

        DistributedPubSubService pubsubservice;

        public PublisherActor()
        {
            pubsubservice= DistributedPubSubService.For(Context.System);
 
            this.Receive<PubSubShardMessage.Ping>(Handle);
 
            Receive<ITradeEvent>(Handler);

           
        }

        protected override void PostStop()
        {
             
            base.PostStop();
        }
        private void Handler(ITradeEvent tradeEvent)
        {
            pubsubservice.Publish("fzf003", tradeEvent,true);
       
        }

       

        private void Handle(PubSubShardMessage.Ping message)
        {
           Console.WriteLine("收到:{0}---{1}",message.Msg,Sender.Path);
            Self.Tell(new PayLoad(message.Msg));
            Sender?.Tell(new Pong($"Pong:{Guid.NewGuid().ToString("N")}",DateTime.Now.ToString()));
        }

     
 
    }

    
}
