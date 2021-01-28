namespace Dotnetakkaserver 
{
    using Akka.Actor;
    using Akka.Pattern;
    using Akka.Routing;
    using Common.Messages;
    using Common.Services;
    using Microsoft.Extensions.Hosting; 
    using Microsoft.Extensions.Logging;
    using ShardNode;
    using System; 
    using System.Threading; 
    using System.Threading.Tasks;
    using Akka.DependencyInjection;

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
                
                var articlecient = actorSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "artileClient");
                this._clusterProxy.ArtileClient = articlecient;

                this.logger.LogInformation("上线:{0}",foorouter.Path.Address.ToString());
                this.logger.LogInformation("上线:{0}", articlecient.Path.Address.ToString());
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