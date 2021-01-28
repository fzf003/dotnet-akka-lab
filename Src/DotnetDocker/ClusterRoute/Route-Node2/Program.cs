using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using ShardNode;
using Microsoft.Extensions.DependencyInjection;

namespace Route_Node2
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
                            .Build().RunAsync();
        }
    }
}
