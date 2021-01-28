using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.DependencyInjection;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SingletonClientApp
{
    public interface IClusterService
    {
        Task<HelloResponse> ResponseAsync(Hello message);
    }

    public class ClusterService: IClusterService
    {
        readonly IActorRef _ClusterSingletonClient;

        public ClusterService(ActorSystem ActorSystem)
        {
            
            this._ClusterSingletonClient = ActorSystem.ActorOf(ClusterSingletonProxyService.ForProps(ActorSystem),"Proxy");
        }

        public Task<HelloResponse> ResponseAsync(Hello message)
        {
              return this._ClusterSingletonClient.Ask<HelloResponse>(message, TimeSpan.FromSeconds(5));
        }
 
    }

   
}
