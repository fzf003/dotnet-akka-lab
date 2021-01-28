using Akka.Actor;
using AkkaTcpServer.Actors;
using Microsoft.Extensions.Logging;
using ShardNode;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaTcpServer
{
    public class AkkaHostedService : AkkaWorker
    {
        Akka.Cluster.Cluster cluster;

        IActorRef listenerechoserice;
        public AkkaHostedService(ILoggerFactory loggerFactory, ActorSystem actorRefFactory)
            : base(loggerFactory, actorRefFactory)
        {

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            cluster = Akka.Cluster.Cluster.Get(this.actorSystem);

            cluster.RegisterOnMemberUp(() => {
               
                listenerechoserice = actorSystem.ActorOf(Props.Create(() => new EchoService(new IPEndPoint(IPAddress.Any, 9001))), "echo-service");

            });

            cluster.RegisterOnMemberRemoved(() => {
                listenerechoserice.Ask(EchoService.StopServer.Instance)
                                  .ContinueWith(t =>
                                  {
                                     if (t.IsCompletedSuccessfully)
                                        Console.WriteLine($"服务已停止:{t.Result}");
                                   }).ConfigureAwait(false);
            });


            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
           

            cluster.Leave(cluster.SelfAddress);

            return base.StopAsync(cancellationToken);
        }
    }
}
