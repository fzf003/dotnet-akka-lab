using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using System;
using System.Threading.Tasks;
namespace PubSubShardMessage
{
    public class MarketEventSubscription : MarketEventSubscriptionManagerBase
    {

        private readonly IActorRef _mediator;
        public MarketEventSubscription(IActorRef mediator)
        {
            _mediator = mediator;
        }

        public override async Task Subscribe(string topic, IActorRef subscriber,string group=null)
        {

            await _mediator.Ask<SubscribeAck>(new Subscribe(topic, subscriber), TimeSpan.FromSeconds(3))
                           .ContinueWith(t =>
                            {
                                Console.WriteLine("SS:{0}", t.Result);
                            });
        }

        public override async Task Unsubscribe(string topic,  IActorRef subscriber, string group = null)
        {
            await _mediator.Ask<UnsubscribeAck>(new Unsubscribe(topic, subscriber, group));
        }


        public static MarketEventSubscription For(ActorSystem sys)
        {
            var mediator = DistributedPubSub.Get(sys).Mediator;
            return new MarketEventSubscription(mediator);
        }
    }



   


    /*public class MakeSubscribeAck
    {
        public MakeSubscribeAck(string topic, ITradeEvent[] events)
        {
            Topic = topic;
            Events = events;
        }

        public string Topic { get; }

        public ITradeEvent[] Events { get; }
    }

    public class MarketUnsubscribeAck
    {
        public MarketUnsubscribeAck(string topic, ITradeEvent[] events)
        {
            Topic = topic;
            Events = events;
        }

        public string Topic { get; }

        public ITradeEvent[] Events { get; }
    }*/

}
