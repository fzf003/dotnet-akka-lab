using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ShardNode;
namespace SingletonClientApp
{
    class Program
    {
        static Task Main(string[] args)
        {
           
            return new HostBuilder()
                                .ConfigureServices(services =>
                                {
                                    services.AddAkkaService("app.conf", isdocker:false)
                                            .AddHostedService<AkkaHostedService>()
                                            .AddSingleton<IClusterService, ClusterService>()
                                            .AddLogging();
                                    

                                }).ConfigureLogging(build =>
                                {
                                    build.AddConsole();
                                })
                               .RunConsoleAsync();
        }
    }
}
