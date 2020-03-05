using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using ShardNode;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaPersistence
{
    class Program
    {
        static Task Main(string[] args)
        {

            return new HostBuilder()
                            .ConfigureServices(services =>
                            {
                                services.AddAkkaService("App.conf")
                                        .AddHostedService<AkkaHostedService>()
                                        .AddLogging();
                            }).ConfigureLogging(build =>
                            {
                                build.AddConsole();
                            })
                            .RunConsoleAsync();
        }
    }
}
