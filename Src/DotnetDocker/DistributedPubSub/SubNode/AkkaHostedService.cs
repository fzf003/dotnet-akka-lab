using Akka.Actor;
using Akka.Cluster.Discovery;
using Microsoft.Extensions.Logging;
using PubSubShardMessage;
using ShardNode;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace SubNode
{
    public class AkkaHostedService : AkkaWorker
    {
        private readonly ILogger<AkkaHostedService> _logger;
        private ActorSystem ActorSystem { get; }

        private Akka.Cluster.Cluster cluster;
        public AkkaHostedService(ActorSystem actorSystem, ILoggerFactory loggerFactory)
            : base(loggerFactory, actorSystem)
        {
            this.ActorSystem = actorSystem;
 
            this._logger = loggerFactory.CreateLogger<AkkaHostedService>();
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            cluster = Akka.Cluster.Cluster.Get(ActorSystem);

            cluster.RegisterOnMemberUp(RegisterOnMemberUp);

            cluster.RegisterOnMemberRemoved(RegisterOnMemberRemoved);
            ClusterDiscovery.Get(ActorSystem);
           

            await ClusterDiscovery.JoinAsync(ActorSystem);
           
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            cluster.Leave(cluster.SelfAddress);

            await base.StopAsync(cancellationToken);

            //CoordinatedShutdown.Get(ActorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }

        void RegisterOnMemberRemoved()
        {
            Console.WriteLine(cluster.SelfAddress + "离开.....");
        }


        public void RegisterOnMemberUp()
        {
            Console.WriteLine(cluster.SelfAddress + "上线.....");


             actorSystem.ActorOf<SubscriberActor>("sub1");
            actorSystem.ActorOf<SubscriberActor>("sub2");
            ;
             //actorSystem.ActorOf(Props.Create<SubscriberManagerActor>("g1"),"sub1");
             //actorSystem.ActorOf(Props.Create<SubscriberManagerActor>("g2"),"sub2");
        }
    }

}
