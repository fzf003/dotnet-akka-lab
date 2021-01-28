using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using ShardNode;
using Microsoft.Extensions.DependencyInjection;
using Akka.DI.Extensions.DependencyInjection;
using Akka.Actor;
using Common;
using Common.Services;
using Microsoft.Extensions.Logging;
namespace Route_Node1
{
    class Program
    {
        static Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .UseAkka(action: services =>
                {
                    services.AddHostedService<AkkaHostedService>()
                            .AddLogging(l=>l.AddConsole())
                            .AddHttpClient<IArticleGateway, ArticleGateway>(options =>
                            {
                                options.BaseAddress = new Uri("http://v1.jinrishici.com");
                            });
                });


            return hostBuilder.Build().RunAsync();
        }

         
    }

    
}
