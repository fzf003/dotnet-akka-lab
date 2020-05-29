using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
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
            this._ClusterSingletonClient = ActorSystem.ActorOf(ClusterSingletonProxyService.ForProps(),"Proxy");
                //GetClusterSingletonProxy(ActorSystem);
        }

        public Task<HelloResponse> ResponseAsync(Hello message)
        {
            //this._ClusterSingletonClient.Tell(message);
            //return Task.FromResult(new HelloResponse(Guid.NewGuid().ToString()));
             return this._ClusterSingletonClient.Ask<HelloResponse>(message, TimeSpan.FromSeconds(5));
             
        }

        IActorRef GetClusterSingletonProxy(ActorSystem ActorSystem)
        {

            Props clusterSingletonProxyProps = ClusterSingletonProxy.Props(
                     singletonManagerPath: "/user/clusterSingletonManager",
                     settings: ClusterSingletonProxySettings.Create(ActorSystem));

            var proxy = ActorSystem.ActorOf(clusterSingletonProxyProps, name: "consumerProxy");

            return proxy;
        }


    }

    public class ClusterSingletonProxyService:ReceiveActor
    {
        public static Props ForProps()
        {
            return Props.Create<ClusterSingletonProxyService>();
               // .WithRouter(new Akka.Routing.RoundRobinPool(5));
        }

         IActorRef _ClusterSingletonClient;
        public ClusterSingletonProxyService()
        {

           /* Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                        TimeSpan.FromMilliseconds(1), Self, new Hello(DateTime.Now.ToString()), ActorRefs.NoSender);
           */
            Receive<Hello>( p => {
                
                this._ClusterSingletonClient.Ask<HelloResponse>(p).PipeTo(Sender, failure: (ex) =>
                {
                    return new HelloResponse(ex.Message);
                });

            });

            this.ReceiveAny(p => {
                Console.WriteLine(p);
                Sender?.Tell(new HelloResponse("该命令不支持"));
            });

            /* Receive<Terminated>(p => {
                //Context.Watch(_ClusterSingletonClient);
                Console.WriteLine("重新建立连接....");
                //_ClusterSingletonClient = GetClusterSingletonProxy(ActorSystem);
                //Context.Watch(_ClusterSingletonClient);
            });*/
        }

        protected override void PreStart()
        {
            this._ClusterSingletonClient = GetClusterSingletonProxy(Context.System);
            Context.Watch(this._ClusterSingletonClient);
            base.PreStart();
        }
        IActorRef GetClusterSingletonProxy(ActorSystem ActorSystem)
        {

            Props clusterSingletonProxyProps = ClusterSingletonProxy.Props(
                     singletonManagerPath: "/user/clusterSingletonManager",
                     settings: ClusterSingletonProxySettings.Create(ActorSystem));

            var proxy = ActorSystem.ActorOf(clusterSingletonProxyProps, name: "consumerProxy");

            return proxy;
        }
    }
}
