﻿using Akka.Actor;
using Akka.Routing;
using Common.Messages;
using Microsoft.Extensions.Logging;
using ShardNode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Route_Node2
{
    public class AkkaHostedService : AkkaWorker
    {
        Akka.Cluster.Cluster cluster;
        public AkkaHostedService(ILoggerFactory loggerFactory, ActorSystem actorRefFactory) 
            : base(loggerFactory, actorRefFactory)
        {
            
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            cluster = Akka.Cluster.Cluster.Get(this.actorSystem);

            cluster.RegisterOnMemberUp(() => {

                var foorouter = actorSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "PubActor");

                this.actorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(1000), () => {

                    foorouter.Ask<HelloResponse>(new Hello(Guid.NewGuid().ToString()))
                    .ContinueWith(tr => {
                        if (tr.IsCompletedSuccessfully)
                            Console.WriteLine($"Response：{tr.Result?.Message}");
                    });

                });
            });

            cluster.RegisterOnMemberUp(() => {

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
