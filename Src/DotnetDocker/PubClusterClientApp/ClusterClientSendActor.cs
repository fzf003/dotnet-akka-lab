using Akka.Actor;
using Akka.Cluster.Tools.Client;
using PubSubShardMessage;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

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
           
            clusterClient = Context.ActorOf(ClusterClient.Props(ClusterClientSettings.Create(Context.System)),"ClusterClientActor");

            this.Receive<Ping>(p =>
            {
                clusterClient.Ask<Pong>(new ClusterClient.Send("/user/publisher", p, localAffinity: false))
                             .PipeTo(Self);
            });


            this.Receive<Pong>(p => {

                Console.WriteLine(p.Rsp+"--"+p.ReplyAddress);
            });

          
            

        }
    }
}
