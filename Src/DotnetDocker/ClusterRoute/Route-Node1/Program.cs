using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using ShardNode;
using Microsoft.Extensions.DependencyInjection;

namespace Route_Node1
{
    class Program
    {
        static Task Main(string[] args)
        {
            return new HostBuilder()
                              .ConfigureServices(services =>
                              {
                                  services.AddAkkaService("app.conf", isdocker: true)
                                          .AddHostedService<AkkaHostedService>()
                                          .AddLogging();

                              })
                             .RunConsoleAsync();
        }
    }
}
