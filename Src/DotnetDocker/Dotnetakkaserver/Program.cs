using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dotnetakkaserver {
    class Program {
        static Task Main(string[] args)
        {

            return new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel()
                              .UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                   // services.AddHostedService<Worker>();
                })
                .RunConsoleAsync();
        }
    }
}