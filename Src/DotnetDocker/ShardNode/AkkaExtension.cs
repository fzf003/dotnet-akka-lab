using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;
using System;
using System.IO;
using Akka.DependencyInjection;

namespace ShardNode
{
    public static class AkkaExtension
    {
        public static IServiceCollection AddAkkaService(this IServiceCollection services, string configfile, bool isdocker = false)
        {
            var config = HoconLoader.ParseConfig(configfile);
            if (isdocker)
                config = config.BootstrapFromDocker();

            services.AddSingleton(provider =>
            {
                var bootstrap = BootstrapSetup.Create().WithConfig(config);
                var di = ServiceProviderSetup.Create(provider);
                var actorSystemSetup = bootstrap.And(di);

                return ActorSystem.Create(config.GetString("akka.MaserServer"), actorSystemSetup).StartPbm();
            });
            return services;
        }


        private static ActorSystem StartPbm(this ActorSystem actorSystem)
        {
            var pbm = PetabridgeCmd.Get(actorSystem);
            pbm.RegisterCommandPalette(ClusterCommands.Instance);
            pbm.RegisterCommandPalette(RemoteCommands.Instance);
            pbm.RegisterCommandPalette(Petabridge.Cmd.Cluster.Sharding.ClusterShardingCommands.Instance);
            pbm.Start();
            return actorSystem;
        }


        public static IServiceCollection AddActorReference<TActor>(
          this IServiceCollection builder, IActorRef actorReference)
          where TActor : ActorBase
        {
            var actorRef = new ActorRefProvider<TActor>(actorReference);

            builder.AddSingleton<ActorRefProvider<TActor>>(actorRef);
            return builder;
        }

        public static IServiceCollection AddActorReference<TActor>(
           this IServiceCollection builder, Props actorProps)
           where TActor : ActorBase
        {

            builder.AddSingleton<ActorRefProvider<TActor>>(provider =>
            {
                var system = provider.GetService<ActorSystem>();

                var actorRef = new ActorRefProvider<TActor>(system.ActorOf(actorProps));

                return actorRef;
            });

            return builder;
        }

        public static IServiceCollection AddActorReference<TActor>(
          this IServiceCollection builder, Func<IServiceProvider, Props> actionProvider)
          where TActor : ActorBase
        {

            builder.AddSingleton<ActorRefProvider<TActor>>(provider =>
            {
                var system = provider.GetService<ActorSystem>();
                var actorProps = actionProvider(provider);
                var actorRef = new ActorRefProvider<TActor>(system.ActorOf(actorProps));

                return actorRef;
            });
            return builder;
        }




        public static class HoconLoader
        {
            public static Config ParseConfig(string hoconPath)
            {
                return ConfigurationFactory.ParseString(File.ReadAllText(hoconPath));
            }
        }
    }
}
