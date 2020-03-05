using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;
using System;
using System.Threading;
using System.Threading.Tasks;
using ShardNode;

namespace SingletonServerApp
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
        public override Task StartAsync(CancellationToken cancellationToken)
        {

            cluster = Akka.Cluster.Cluster.Get(ActorSystem);

            cluster.RegisterOnMemberUp(RegisterOnMemberUp);

            cluster.RegisterOnMemberUp(RegisterOnMemberRemoved);



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

            var clusterSingletonManagerProps = ClusterSingletonManager.Props(
                                     singletonProps: SingleActor.CreateProps().WithRouter(new Akka.Routing.RoundRobinPool(5)),
                                     terminationMessage: PoisonPill.Instance,
                                     settings: ClusterSingletonManagerSettings.Create(ActorSystem));


            var singlet = ActorSystem.ActorOf(clusterSingletonManagerProps, name: "clusterSingletonManager");
            Console.WriteLine("clusterSingletonManager:{0}", singlet.Path.ToString());



        }
    }
}
