using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Akka;
using ShardNode;
using Product.API.Actors;

namespace Product.API
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        readonly ActorSystem _actorSystem;

        readonly ActorRefProvider<ResponseActor> _actorRefProvider;

        public Worker(ILogger<Worker> logger, ActorSystem actorSystem, ActorRefProvider<ResponseActor> actorRefProvider)
        {
            _logger = logger;

            _actorSystem = actorSystem;

            _actorRefProvider = actorRefProvider;


        }

        protected override   Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /* while (!stoppingToken.IsCancellationRequested)
             {
                 _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                 await Task.Delay(1000, stoppingToken);
             }*/

            return Task.CompletedTask;
        }
    }




  



}
