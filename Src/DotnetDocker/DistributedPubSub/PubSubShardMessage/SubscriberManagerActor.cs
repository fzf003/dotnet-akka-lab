using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubShardMessage
{
    public class SubscriberManagerActor:ReceiveActor
    {
        readonly MarketEventSubscription subscriptionManager;

        readonly DistributedPubSubService distributedPubSub;

        readonly string groupname;
        public SubscriberManagerActor(string groupname)
        {
            this.groupname = groupname;

            //this.subscriptionManager = MarketEventSubscription.For(Context.System);

            //this.subscriptionManager.Subscribe("fzf007", Self,this.groupname);
            this.distributedPubSub = DistributedPubSubService.For(Context.System);

            distributedPubSub.Subscribe("fzf007", Self,this.groupname);
            Receive<SubscribeAck>(p =>
            {
               
                Console.WriteLine("Path:{0}",p.Subscribe);
            });

            Receive<ITradeEvent>(p => {

                //Console.WriteLine(Sender.Path);
                Console.WriteLine($"GroupName:{this.groupname}{this.Self.Path}--{p.Body}");
               // Sender.Tell($"Repy:{Guid.NewGuid().ToString("N")}");

            });
        }

        protected override void PostStop()
        {
            subscriptionManager.Unsubscribe("fzf007", Self, this.groupname);
            Console.WriteLine("停止.....");
            base.PostStop();
        }
    }
}
