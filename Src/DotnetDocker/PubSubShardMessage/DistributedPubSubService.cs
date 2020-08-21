using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using System;
using System.Collections.Generic;
using System.Text;
using static Akka.Actor.ProviderSelection;

namespace PubSubShardMessage
{
    public interface ITradeEvent
    {
        string Body { get; }
    }

    public class PublishEvent : ITradeEvent
    {
        public string Body { get; }

        public PublishEvent(string body)
        {
            this.Body = body;
        }
    }


    public sealed class DistributedPubSubService 
    {
        private readonly IActorRef _mediator;

        public DistributedPubSubService(IActorRef mediator)
        {
            _mediator = mediator;
        }

        public void Publish(string topic, ITradeEvent @event,bool sendOneMessageToEachGroup=false)
        {
            _mediator.Tell(new Publish(topic, @event, sendOneMessageToEachGroup));
        }

        public void Subscribe(string topic,IActorRef subscribeprocess,string groupname=null)
        {
            _mediator.Tell(new Subscribe(topic, subscribeprocess, groupname));
        }

        public void UnSubscribe(string topic, IActorRef subscribeprocess, string groupname = null)
        {
            _mediator.Tell(new Unsubscribe(topic, subscribeprocess, groupname));
        }

        public void Put(IActorRef  process)
        {
            _mediator.Tell(new Put(process));
        }

    

        public void Send(string actorpath,ITradeEvent @event,bool localAffinity=false)
        {
            _mediator.Tell(new Send(actorpath, @event,localAffinity));
        }

        public static DistributedPubSubService For(ActorSystem sys)
        {
            var mediator = DistributedPubSub.Get(sys).Mediator;
            return new DistributedPubSubService(mediator);
        }
    }
}
