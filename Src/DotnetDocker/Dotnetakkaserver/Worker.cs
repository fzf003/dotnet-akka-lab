namespace Dotnetakkaserver {
    using Akka.Actor;
    using Akka.Routing;
    using Common.Messages;
    using Microsoft.Extensions.Hosting; 
    using Microsoft.Extensions.Logging;
    using ShardNode;
    using System; 
    using System.Threading; 
    using System.Threading.Tasks; 

    /*public class Worker:BackgroundService {
        protected override  Task ExecuteAsync(CancellationToken stoppingToken) {

            return Task.CompletedTask;
        }
    }*/


    public class AkkaHostedService : AkkaWorker
    {
        Akka.Cluster.Cluster cluster;

        readonly ClusterProxy _clusterProxy;
        readonly ILogger<AkkaHostedService> logger;
        public AkkaHostedService(ILoggerFactory loggerFactory, ActorSystem actorRefFactory, ClusterProxy clusterProxy)
            : base(loggerFactory, actorRefFactory)
        {
            this._clusterProxy = clusterProxy;
            this.logger = loggerFactory.CreateLogger<AkkaHostedService>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            cluster = Akka.Cluster.Cluster.Get(this.actorSystem);

            cluster.RegisterOnMemberUp(() => {
                
                var foorouter = actorSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "PubActor");
                this._clusterProxy.ClusterClient = foorouter;
                this.logger.LogInformation("ÉÏÏß:{0}",foorouter.Path.Address.ToString());
                
            });

            cluster.RegisterOnMemberUp(() => {

            });


            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            cluster.Leave(cluster.SelfAddress);

            return CoordinatedShutdown.Get(actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}