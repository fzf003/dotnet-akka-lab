using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SeedNode
{
    public class AkkaHostedService : BackgroundService
    {
        private readonly ILogger<AkkaHostedService> _logger;
        private ActorSystem ActorSystem { get; }
      
        IActorRef actorRef = Nobody.Instance;

        Akka.Cluster.Cluster cluster;
        public AkkaHostedService(ActorSystem actorSystem, ILogger<AkkaHostedService> logger)
        {
            this.ActorSystem = actorSystem;

            this._logger = logger;

             
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {

            cluster = Akka.Cluster.Cluster.Get(ActorSystem);
 
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {

            cluster.Leave(cluster.SelfAddress);

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
 }

