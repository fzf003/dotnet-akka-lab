using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.Singleton;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static SingletonServerApp.AkkaExtension;

namespace SingletonServerApp
{
    class Program
    {
        static  Task Main(string[] args)
        {
            Console.WriteLine("hello World");
            
            return  new HostBuilder()
                                .ConfigureServices(services =>
                                {
                                    services.AddAkkaService(isdocker:true)
                                            .AddLogging();

                                })
                               .RunConsoleAsync();
        }

      
    }
}
