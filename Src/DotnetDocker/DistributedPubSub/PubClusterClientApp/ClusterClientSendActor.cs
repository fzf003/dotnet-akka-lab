using Akka.Actor;
using Akka.Cluster.Tools.Client;
using PubSubShardMessage;
using System;

namespace PubClusterClientApp
{
    public class ClusterClientSendActor : ReceiveActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new ClusterClientSendActor());
        }

        IActorRef clusterClient;

        public ClusterClientSendActor()
        {
           
            clusterClient =  
            //Context.ActorOf(ClusterClient.Props(ClusterClientSettings.Create(Context.System)), "ClusterClientActor");

            Context.ActorOf(ClusterClient.Props(ClusterClientSettings.Create(Context.System)),"ClusterClientActor");

            this.Receive<Ping>(p =>
            {
                clusterClient.Ask<Pong>(new ClusterClient.Send("/user/publisher", p))
                             .PipeTo(Self);
            });


            this.Receive<Pong>(p => {

                Console.WriteLine(p.Rsp+"--"+p.ReplyAddress);
            });

            this.Receive<ITradeEvent>(@event => {
                var message=new ClusterClient.Publish("/user/publisher", @event);
                this.clusterClient.Tell(message);
            });
          
            

        }


    }
}
