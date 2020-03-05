using Akka.Actor;
using Common;
using Microsoft.Extensions.Logging;
using ShardNode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Route_Node1
{
    public class AkkaHostedService : AkkaWorker
    {

        Akka.Cluster.Cluster cluster;
         public AkkaHostedService(ILoggerFactory loggerFactory, ActorSystem actorRefFactory) 
            : base(loggerFactory, actorRefFactory)
        {
           
            
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            cluster = Akka.Cluster.Cluster.Get(this.actorSystem);

            cluster.RegisterOnMemberUp(()=> {

                var foo = actorSystem.ActorOf(FooActor.Props(), "FooActor");
            });

            cluster.RegisterOnMemberUp(()=> {
            
            });
            
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            cluster.Leave(cluster.SelfAddress);

            return base.StopAsync(cancellationToken);
        }
    }
}
