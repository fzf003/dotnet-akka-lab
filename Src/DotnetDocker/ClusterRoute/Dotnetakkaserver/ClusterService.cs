using Akka.Actor;
using Akka.Routing;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnetakkaserver
{
    public interface IClusterService
    {
        Task<List<HelloResponse>> ResponseAsync(Hello message,TimeSpan? timeout=null);
    }

    public class ClusterService : IClusterService
    {
        readonly IActorRef _ClusterSingletonClient;
        readonly ActorSystem _ActorSystem;
        readonly ClusterProxy _clusterProxy;
        public ClusterService(ActorSystem ActorSystem, ClusterProxy clusterProxy)
        {
            this._clusterProxy = clusterProxy;
            this._ActorSystem = ActorSystem;
            this._ClusterSingletonClient = GetClusterSingletonProxy(_clusterProxy);
        }
       

        public Task<List<HelloResponse>> ResponseAsync(Hello message, TimeSpan? timeout = null)
        {
            return this._ClusterSingletonClient.Ask<List<HelloResponse>>(message, timeout);
        }

         IActorRef GetClusterSingletonProxy(ClusterProxy clusterProxy)
        {
             
            return
                clusterProxy.ClusterClient;
                //_ActorSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "PubActor");
        }


    }

    public class ClusterProxy
    {
         
        public IActorRef ArtileClient {
            get;set;
        }

        public IActorRef ClusterClient
        {
            get;
            set;
        }
    }
}
