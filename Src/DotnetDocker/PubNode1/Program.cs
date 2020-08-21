using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using ShardNode;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace PubNode1
{
    class Program
    {
        static Task Main(string[] args)
        {
            return new HostBuilder()
                              .ConfigureServices(services =>
                              {
                                  services.AddAkkaService("app.conf", isdocker: false)
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
