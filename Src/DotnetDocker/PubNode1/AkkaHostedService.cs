using Akka.Actor;
using Akka.Cluster.Discovery;
using Akka.Cluster.Tools.Client;
using Microsoft.Extensions.Logging;
using PubSubShardMessage;
using ShardNode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PubNode1
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
        public override   Task StartAsync(CancellationToken cancellationToken)
        {

            cluster = Akka.Cluster.Cluster.Get(ActorSystem);

            cluster.RegisterOnMemberUp(RegisterOnMemberUp);

            cluster.RegisterOnMemberUp(RegisterOnMemberRemoved);

            ClusterDiscovery.Join(ActorSystem);

            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            cluster.Leave(cluster.SelfAddress);

            await base.StopAsync(cancellationToken);

            //CoordinatedShutdown.Get(ActorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }

        void RegisterOnMemberRemoved()
        {
            Console.WriteLine(cluster.SelfAddress + "离开.....");
        }


        public void RegisterOnMemberUp()
        {
            Console.WriteLine(cluster.SelfAddress + "上线.....");

            var publisher = actorSystem.ActorOf<PublisherActor>("publisher");
            //actorSystem.ActorOf<SubscriberActor>("sub");
             var pubeventclient= DistributedPubSubService.For(actorSystem);
             ClusterClientReceptionist.Get(actorSystem).RegisterService(publisher);
            
             /* ActorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), () =>
              {
                  // Console.WriteLine("ddd");
                  publisher.Ask<Pong>(new Ping(Guid.NewGuid().ToString("N")))
                           .ContinueWith(t => {
                               Console.WriteLine(t.Result.Rsp+"--"+t.Result.ReplyAddress);
                           });

                 //pubeventclient.Publish("fzf007", new PayLoad(Guid.NewGuid().ToString("N")),true);


                 // pubeventclient.Publish("fzf003", new PublishEvent(Guid.NewGuid().ToString("N")),true);

              });*/

        }
    }

}
