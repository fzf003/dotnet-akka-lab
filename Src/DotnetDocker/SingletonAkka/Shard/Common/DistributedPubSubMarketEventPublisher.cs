using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Publish = Akka.Cluster.Tools.PublishSubscribe.Publish;

namespace Common
{
    public interface IMarketEventPublisher
    {
        void Publish(string topicName, object @event);
        void Send(string actorpath, object message);

    }
    public sealed class DistributedPubSubMarketEventPublisher : IMarketEventPublisher
    {
        private readonly IActorRef _mediator;

        public DistributedPubSubMarketEventPublisher(IActorRef mediator)
        {
            _mediator = mediator;
        }

        public void Publish(string topicName, object @event)
        {
          
            _mediator.Tell(new Publish(topicName, @event));

            
        }

        public void Send(string actorpath,object message)
        {
            _mediator.Tell(new Send(actorpath, message));
        }



        public static DistributedPubSubMarketEventPublisher For(ActorSystem sys)
        {
            var mediator = DistributedPubSub.Get(sys).Mediator;
            return new DistributedPubSubMarketEventPublisher(mediator);
        }
    }
}
