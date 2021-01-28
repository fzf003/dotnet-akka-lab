using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClusterSharding_ServerNode
{
    public class AkkaHostedService : ShardNode.AkkaWorker
    {
        private readonly ILogger<AkkaHostedService> _logger;
  
        Akka.Cluster.Cluster cluster;
        public AkkaHostedService(ActorSystem actorSystem, ILoggerFactory loggerFactory)
            :base(loggerFactory, actorSystem)
        {
             this._logger = loggerFactory.CreateLogger<AkkaHostedService>();
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            cluster = Akka.Cluster.Cluster.Get(actorSystem);

            cluster.RegisterOnMemberUp(RegisterOnMemberUp);

            cluster.RegisterOnMemberRemoved(RegisterOnMemberRemoved);


            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {

            cluster.Leave(cluster.SelfAddress);
            
            return base.StopAsync(cancellationToken);
        }
       

         void RegisterOnMemberRemoved()
        {
            Console.WriteLine(cluster.SelfAddress + "离开.....");
        }
 
        public void RegisterOnMemberUp()
        {
            Console.WriteLine(cluster.SelfAddress + "上线.....");

            var sharding = ClusterSharding.Get(actorSystem);

             var shardRegion=sharding.Start(
                typeName: "fzf003",
                entityProps: ProductMasterActor.PropsFor(),
                settings: ClusterShardingSettings.Create(actorSystem),
                messageExtractor: new MessageExtractor(10));


  
         }
    }

 }
