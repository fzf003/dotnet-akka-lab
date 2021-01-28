using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;
using ShardNode;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SeedNode
{
    public class AkkaHostedService : AkkaWorker
    {
        private readonly ILogger<AkkaHostedService> _logger;
        private ActorSystem ActorSystem { get; }

        IActorRef actorRef = Nobody.Instance;

        Akka.Cluster.Cluster cluster;
        public AkkaHostedService(ILoggerFactory loggerfactory, ActorSystem actorSystem)
            : base(loggerfactory, actorSystem)
        {
            this.ActorSystem = actorSystem;

            this._logger = loggerfactory.CreateLogger<AkkaHostedService>();


        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {

            cluster = Akka.Cluster.Cluster.Get(ActorSystem);

            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            cluster.Leave(cluster.SelfAddress);

            await base.StopAsync(cancellationToken);
        }


    }
}

