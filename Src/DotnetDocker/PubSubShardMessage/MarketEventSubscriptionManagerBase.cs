using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PubSubShardMessage
{
     

    public abstract class MarketEventSubscriptionManagerBase 
    {
       
        public abstract Task Subscribe(string topic,IActorRef subscriber, string group = null);

        public abstract Task Unsubscribe(string topic, IActorRef subscriber, string group = null);

        
    }


}