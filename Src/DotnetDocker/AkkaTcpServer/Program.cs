using Akka.Actor;
using AkkaTcpServer.Actors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;
using   ShardNode;

namespace AkkaTcpServer
{
    class Program
    {
        static  Task Main(string[] args)
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
