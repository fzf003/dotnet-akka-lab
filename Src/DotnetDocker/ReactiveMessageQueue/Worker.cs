using Akka.Actor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.DI.Core;
using Akka.Cluster;

namespace ReactiveMessageQueue
{
    public class Worker : ShardNode.AkkaWorker
    {
         readonly ILogger<Worker> logger;
        readonly IServiceGateway serviceGateway;
          Cluster cluster;
        public Worker(ILoggerFactory loggerFactory, ActorSystem actorRefFactory, IServiceGateway serviceGateway) : base(loggerFactory, actorRefFactory)
        {
            logger = loggerFactory.CreateLogger<Worker>();
            this.serviceGateway = serviceGateway;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            cluster = Cluster.Get(this.actorSystem);

            cluster.RegisterOnMemberUp(() => {

               var client= this.actorSystem.ActorOf(PaymentWorkerActor.CreateProps(this.actorSystem),"client");

                this.actorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), () => {
                    client.Tell(Guid.NewGuid().ToString("N"));
                });

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