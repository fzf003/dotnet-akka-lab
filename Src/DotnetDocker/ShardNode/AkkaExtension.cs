using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;
using System;
using System.IO;

namespace ShardNode
{
    public static class AkkaExtension
    {
        public static IServiceCollection AddAkkaService(this IServiceCollection services,string  configfile,bool isdocker=false)
        {
            var config = HoconLoader.ParseConfig(configfile);
            if (isdocker)
                config = config.BootstrapFromDocker();
            services.AddSingleton(ActorSystem.Create(config.GetString("akka.MaserServer"), config).StartPbm());
            return services;
        }

        private static ActorSystem StartPbm(this ActorSystem actorSystem)
        {
            var pbm = PetabridgeCmd.Get(actorSystem);
            pbm.RegisterCommandPalette(ClusterCommands.Instance);
            pbm.RegisterCommandPalette(RemoteCommands.Instance);
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
    }
}
