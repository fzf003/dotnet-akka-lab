using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ShardNode;

namespace SeedNode
{
    class Program
    {
        static Task Main(string[] args)
        {
            return new HostBuilder()
                                .ConfigureServices(services =>
                                {
                                    services.AddAkkaService("app.conf",false)
                                            .AddHostedService<AkkaHostedService>();
                                 })
                                .RunConsoleAsync();

        }
    }
}
