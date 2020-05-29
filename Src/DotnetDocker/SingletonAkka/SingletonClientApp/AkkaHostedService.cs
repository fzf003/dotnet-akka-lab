using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Common.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ShardNode;

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

            cluster.RegisterOnMemberUp(RegisterOnMemberRemoved);



            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {

           

            cluster.Leave(cluster.SelfAddress);

            return   CoordinatedShutdown.Get(ActorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
            //base.StopAsync(cancellationToken);
        }
 
        void RegisterOnMemberRemoved()
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
          
            //var proxy=GetClusterSingletonProxy();
            
           /* ActorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), () =>
            {
                 var message = new Hello(DateTime.Now.ToString());
                //proxy.Tell(message);
                this.clusterService.ResponseAsync(message)
                .ContinueWith(t => {
                   if(t.IsCompletedSuccessfully)
                    {
                        Console.WriteLine("HelloResponse:{0}",t.Result?.Message);
                    }
                });
                //Console.WriteLine("Send:{0}", message.Message);
                 
             });*/

        }
    }


    /*public static class AkkaExtension
    {
 
        public static IServiceCollection AddAkkaService(this IServiceCollection services,bool isdocker=false)
        {

            var config = HoconLoader.ParseConfig("app.conf");
            /*if(isdocker)
                config= config.BootstrapFromDocker();
            services.AddSingleton(ActorSystem.Create(config.GetString("akka.MaserServer"), config).StartPbm());
             services.AddHostedService<AkkaHostedService>();
            return services;
        }

        private static ActorSystem StartPbm(this ActorSystem actorSystem)
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
                return ConfigurationFactory.ParseString(File.ReadAllText(hoconPath));
            }
        }
    }*/
}
