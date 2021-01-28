using Akka.Actor;
using Akka.Cluster.Tools.Client;
using Akka.Configuration;
using PubSubShardMessage;
using System;
using System.IO;

namespace PubClusterClientApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var config = ConfigurationFactory.ParseString(File.ReadAllText("app.conf"));

            ActorSystem system = ActorSystem.Create(config.GetString("akka.MaserServer"), config);

            var client = system.ActorOf<ClusterClientSendActor>();


            system.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1), () =>
            {
                client.Tell(new PubSubShardMessage.Ping($"{DateTime.Now.ToString()}-{Guid.NewGuid().ToString("N")}"));
            });


            Console.ReadKey();


        }
    }

   
}
