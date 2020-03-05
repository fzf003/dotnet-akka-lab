using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dotnetakkaserver {
    class Program {
        static Task Main (string[] args) {
 
            return new HostBuilder ()
                .ConfigureServices (services => {
                    services
                        .AddHostedService<Worker> ();
                })
                .RunConsoleAsync ();
        }
    }
}