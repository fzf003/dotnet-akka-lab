using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Common.ProductActor;

namespace ClusterSharding_ClientNode
{
    public class AkkaHostedService : ShardNode.AkkaWorker
    {
        private readonly ILogger<AkkaHostedService> _logger;
  
        Akka.Cluster.Cluster cluster;
        public AkkaHostedService(ActorSystem actorSystem, ILoggerFactory loggerFactory)
            :base(loggerFactory,actorSystem)
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
              
            var proxy = ClusterSharding.Get(actorSystem).StartProxy(
                typeName: "fzf003",
                role: "ClusterShardingApp",
                messageExtractor: new MessageExtractor(10));

            var random = new Random();
 

            actorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), () =>
            {
                var Id = random.Next(1, 10).ToString();
                var createproduct = new CreateProductCommand(Id, string.Format("fzf-{0}", Id));
                var message = new ShardEnvelope(Id, createproduct);
                 Console.WriteLine(createproduct.ToString()) ;
                 proxy.Tell(message);
                 Console.WriteLine("打印:{0}",Id);
                 proxy.Tell(new ShardEnvelope(Id, new PrintState(Id)));




                /*var state = ClusterSharding.Get(actorSystem).ShardRegion("fzf003").Ask<CurrentShardRegionState>(GetShardRegionState.Instance).Result;
                foreach (var shard in state.Shards)
                    foreach (var entityId in shard.EntityIds)
                        Console.WriteLine($"fzf003/{shard.ShardId}/{entityId}");
                        */
                
            });


        }
    }


    /*public static class AkkaExtension
    {



        public static IServiceCollection AddAkkaService(this IServiceCollection services)
        {
            var config = HoconLoader.ParseConfig("app.conf");
            services.AddSingleton(ActorSystem.Create(config.GetString("akka.MaserServer"), config));
             services.AddHostedService<AkkaHostedService>();
            return services;
        }

        public static class HoconLoader
        {
            public static Config ParseConfig(string hoconPath)
            {
                return ConfigurationFactory.ParseString(File.ReadAllText(hoconPath));
            }
        }
    }*/
}
