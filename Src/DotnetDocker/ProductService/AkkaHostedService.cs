using Akka.Actor;
using Microsoft.Extensions.Logging;
using ShardNode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Product.API
{
    public class AkkaHostedService : AkkaWorker
    {
        private readonly ILogger<AkkaHostedService> _logger;

        Akka.Cluster.Cluster cluster;
        public AkkaHostedService(ActorSystem actorSystem, ILoggerFactory loggerFactory)
            : base(loggerFactory, actorSystem)
        {
            _logger = loggerFactory.CreateLogger<AkkaHostedService>();
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            cluster = Akka.Cluster.Cluster.Get(actorSystem);

            cluster.RegisterOnMemberUp(RegisterOnMemberUp);

            cluster.RegisterOnMemberRemoved(RegisterOnMemberRemoved);

            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

              cluster.Leave(cluster.SelfAddress);

              await base.StopAsync(cancellationToken);
        }


        void RegisterOnMemberRemoved()
        {
            _logger.LogInformation($"{cluster.SelfAddress }下线.....");
        }

        public void RegisterOnMemberUp()
        {
            _logger.LogInformation($"{cluster.SelfAddress}上线.....");

        }
    }
}
