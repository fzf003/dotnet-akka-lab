using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShardNode;
using Microsoft.Extensions.DependencyInjection;

namespace Route_Node1
{
    public static class AkkaHostExtensions
    {

        public static IHostBuilder UseAkka(
            this IHostBuilder hostBuilder, string filepath = "app.conf", Action<IServiceCollection> action = null)
        {
            hostBuilder.ConfigureServices(services => {
                services.AddAkkaService("app.conf");
                action(services);
            });

            return hostBuilder;
        }
    }
}
