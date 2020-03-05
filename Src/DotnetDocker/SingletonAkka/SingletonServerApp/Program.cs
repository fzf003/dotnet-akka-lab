using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.Singleton;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ShardNode;
namespace SingletonServerApp
{
    class Program
    {
        static  Task Main(string[] args)
        {
           
            return  new HostBuilder()
                                .ConfigureServices(services =>
                                {
                                    services.AddAkkaService("app.conf",isdocker:false)
                                            .AddHostedService<AkkaHostedService>()
                                            .AddLogging();

                                })
                               .RunConsoleAsync();
        }

      
    }
}
