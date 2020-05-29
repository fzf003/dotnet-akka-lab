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



         
        }
    }
}
