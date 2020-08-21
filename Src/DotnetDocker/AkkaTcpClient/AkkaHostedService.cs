using Akka.Actor;
using AkkaTcpClient.Actors;
using Microsoft.Extensions.Logging;
using ShardNode;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AkkaTcpClient.Actors.ClientService;

namespace AkkaTcpClient
{
    public class AkkaHostedService : AkkaWorker
    {
        Akka.Cluster.Cluster cluster;

        IActorRef clientserviceref;
        public AkkaHostedService(ILoggerFactory loggerFactory, ActorSystem actorRefFactory)
            : base(loggerFactory, actorRefFactory)
        {

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            cluster = Akka.Cluster.Cluster.Get(this.actorSystem);

            cluster.RegisterOnMemberUp(() => {

                clientserviceref = actorSystem.ActorOf(Props.Create(() => new ClientService()), "echo-client");

                actorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100), () =>
                {
                    clientserviceref.Tell(new SendMessage(Guid.NewGuid().ToString("N")));
                });

            });

            cluster.RegisterOnMemberRemoved(() =>
            {
                clientserviceref.Tell(StopMessage.Instance);
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
