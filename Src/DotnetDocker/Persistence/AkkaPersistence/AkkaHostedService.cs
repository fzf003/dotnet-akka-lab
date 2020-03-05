using Akka.Actor;
using Microsoft.Extensions.Logging;
using ShardNode;
using System;
using System.Threading;
using System.Threading.Tasks;
using static AkkaPersistence.Actors.BookActor;
using static AkkaPersistence.Actors.BookPersistenceQuery;

namespace AkkaPersistence
{


    public class AkkaHostedService : AkkaWorker
    {
        private readonly ILogger<AkkaHostedService> _logger;


        public AkkaHostedService(ILoggerFactory loggerFactory, ActorSystem actorRefFactory)
             : base(loggerFactory, actorRefFactory)
        {
            this._logger = loggerFactory.CreateLogger<AkkaHostedService>();

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var bookmaster = actorSystem.ActorOf<BookMasterActor>("bookmaster");
            
            var bookid = "003";

            var bookquery= actorSystem.ActorOf(Actors.BookPersistenceQuery.PropsFor(bookid),"bookquery");

            bookquery.Tell(Start.Instance);

            actorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.Zero, TimeSpan.FromSeconds(1), () =>
            {
                bookmaster.Tell(new CreateBookCommand(bookid, Guid.NewGuid().ToString("N")));
            });

            actorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.Zero, TimeSpan.FromSeconds(5), () =>
            {
                bookmaster.Tell(new CreateBookCommand(bookid, $"HZH-{DateTime.Now.ToString()}"));
            });

            actorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.Zero, TimeSpan.FromSeconds(3), () =>
            {
                bookmaster.Tell(new PrintState(bookid));
            });
 
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {

            return actorSystem.Terminate();
        }
    }
}
