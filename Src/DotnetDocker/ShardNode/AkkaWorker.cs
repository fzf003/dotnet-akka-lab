using Akka.Actor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShardNode
{
    public abstract class AkkaWorker : IHostedService
    {
        private readonly ILogger<AkkaWorker> _logger;

        protected ActorSystem actorSystem { get; }

        public AkkaWorker(ILoggerFactory loggerFactory, ActorSystem actorRefFactory)
        {
            this._logger = loggerFactory.CreateLogger<AkkaWorker>();

            this.actorSystem = actorRefFactory;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
 
            return actorSystem.Terminate();
        }
    }
}
