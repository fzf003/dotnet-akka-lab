using Akka.Actor;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using ShardNode;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace AkkaTcpClient
{
    class Program
    {
        static Task Main(string[] args)
        {

            return new HostBuilder()
                         .ConfigureServices(services =>
                         {
                             services.AddAkkaService("app.conf")
                                     .AddHostedService<AkkaHostedService>()
                                     .AddLogging();

                         })
                        .RunConsoleAsync();



           /* using (var system = ActorSystem.Create("echo-client-system"))
            {
                var port = 9001;

                

                var actor = system.ActorOf(Props.Create(() => new ClientService(IPAddress.Loopback, port)), "echo-client");
                        
                for (; ; )
                {
                    system.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(1000), () => {
                        actor.Tell(new SendMessage(Guid.NewGuid().ToString("N")));
                    });
                    
                    Console.ReadKey();
                    // actor.Tell(StartMessage.Instance);
                    //actor.Tell(StopMessage.Instance);
                }

            }*/
        }
    }
}
