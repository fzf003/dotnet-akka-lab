using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Common.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ShardNode;
using Akka.Cluster.Discovery;

namespace SingletonClientApp
{
    public class AkkaHostedService : AkkaWorker
    {
        private readonly ILogger<AkkaHostedService> _logger;
        private ActorSystem ActorSystem { get; }

        readonly IClusterService clusterService;

        private Akka.Cluster.Cluster cluster;
        public AkkaHostedService(ActorSystem actorSystem, ILoggerFactory loggerFactory, IClusterService clusterService)
            :base(loggerFactory, actorSystem)
        {
            this.ActorSystem = actorSystem;

            this.clusterService = clusterService;

            this._logger = loggerFactory.CreateLogger<AkkaHostedService>();
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
           
            cluster = Akka.Cluster.Cluster.Get(ActorSystem);

            cluster.RegisterOnMemberUp(RegisterOnMemberUp);

            cluster.RegisterOnMemberRemoved(RegisterOnMemberRemoved);

            ClusterDiscovery.Join(ActorSystem);

            return base.StartAsync(cancellationToken);
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
             
            ActorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), () =>
            {
                var message = new Hello(Guid.NewGuid().ToString("N"));
                this.clusterService.ResponseAsync(message)
                                   .ContinueWith(t =>
                                   {
                                       if (t.IsCompletedSuccessfully)
                                       {
                                           Console.WriteLine("HelloResponse:{0}", t.Result?.Message);
                                       }
                                   });

            });

        }
    }



}
