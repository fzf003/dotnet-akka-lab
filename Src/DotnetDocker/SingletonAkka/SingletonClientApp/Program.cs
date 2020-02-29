using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SingletonClientApp
{
    class Program
    {
        static Task Main(string[] args)
        {
            return new HostBuilder()
                                .ConfigureServices(services =>
                                {
                                    services.AddAkkaService(isdocker:true)
                                            .AddLogging();

                                }).ConfigureLogging(build =>
                                {
                                    build.AddConsole();
                                })
                               .RunConsoleAsync();
        }
    }
}
