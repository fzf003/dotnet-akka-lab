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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SingletonServerApp
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
            Console.WriteLine("执行......");

            cluster.RegisterOnMemberUp(RegisterOnMemberUp);

            cluster.RegisterOnMemberUp(RegisterOnMemberRemoved);

            return Task.CompletedTask;
        }

        public virtual void RegisterOnMemberRemoved()
        {
            Console.WriteLine(cluster.SelfAddress + "离开.....");
        }


        IActorRef GetClusterSingletonProxy()
        {
            Props clusterSingletonProxyProps = ClusterSingletonProxy.Props(
                     singletonManagerPath: "/user/clusterSingletonManager",
                     settings: ClusterSingletonProxySettings.Create(ActorSystem));

            var proxy = ActorSystem.ActorOf(clusterSingletonProxyProps, name: "consumerProxy");

            return proxy;
        }

        public void RegisterOnMemberUp()
        {
            Console.WriteLine(cluster.SelfAddress + "上线.....");
  
            var clusterSingletonManagerProps =ClusterSingletonManager.Props(
                                     singletonProps: SingleActor.CreateProps().WithRouter(new Akka.Routing.RoundRobinPool(5)),
                                     terminationMessage: PoisonPill.Instance,
                                     settings: ClusterSingletonManagerSettings.Create(ActorSystem));
                                     

              var singlet= ActorSystem.ActorOf(clusterSingletonManagerProps,name: "clusterSingletonManager");
               Console.WriteLine("clusterSingletonManager:{0}", singlet.Path.ToString());

          //  var proxy= GetClusterSingletonProxy();
            
           /* ActorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), () =>
            {
                 var message = new Hello(DateTime.Now.ToString());
                proxy.Tell(message);
                Console.WriteLine("Send:{0}", message.Message);
                 
                // proxy.Tell(new Hello(DateTime.Now.ToString()));
            });*/
         }
    }


    public static class AkkaExtension
    {
 
        public static IServiceCollection AddAkkaService(this IServiceCollection services,bool isdocker=false)
        {
            var config = HoconLoader.ParseConfig("app.conf");
            if (isdocker)
               config= config.BootstrapFromDocker();
            services.AddSingleton(ActorSystem.Create(config.GetString("akka.MaserServer"), config).StartPbm());
             services.AddHostedService<AkkaHostedService>();
            return services;
        }

        public static ActorSystem StartPbm(this ActorSystem actorSystem)
        {
            var pbm = PetabridgeCmd.Get(actorSystem);
            pbm.RegisterCommandPalette(ClusterCommands.Instance);
            //pbm.RegisterCommandPalette(RemoteCommands.Instance);
            pbm.Start();
            return actorSystem;
        }

        public static class HoconLoader
        {
            public static Config ParseConfig(string hoconPath)
            {
                var str=File.ReadAllText(hoconPath);
                return ConfigurationFactory.ParseString(str);
            }
        }
    }
}
