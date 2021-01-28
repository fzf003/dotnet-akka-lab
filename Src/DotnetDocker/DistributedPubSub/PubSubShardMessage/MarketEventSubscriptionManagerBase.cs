using Akka.Actor;
using System.Threading.Tasks;

namespace PubSubShardMessage
{


    public abstract class MarketEventSubscriptionManagerBase 
    {
       
        public abstract Task Subscribe(string topic,IActorRef subscriber, string group = null);

        public abstract Task Unsubscribe(string topic, IActorRef subscriber, string group = null);

        
    }


}