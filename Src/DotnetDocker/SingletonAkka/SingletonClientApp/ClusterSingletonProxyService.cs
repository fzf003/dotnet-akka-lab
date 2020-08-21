using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SingletonClientApp
{
    public class ClusterSingletonProxyService : ReceiveActor
    {
        public static Props ForProps()
        {
            return Props.Create<ClusterSingletonProxyService>();
                        //.WithRouter(new Akka.Routing.RoundRobinPool(5));
        }

        IActorRef _ClusterSingletonClient;
        public ClusterSingletonProxyService()
        {


            Receive<Hello>(p => {

                _ClusterSingletonClient
                .Ask<HelloResponse>(p)
                .PipeTo(Sender, failure: (ex) =>
                {
                    return new HelloResponse(ex.Message);
                });

            });

            this.ReceiveAny(p => {
                Console.WriteLine(p);
                Sender?.Tell(new HelloResponse("该命令不支持"));
            });


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
                     singletonManagerPath: "/user/singletonManager",
                     settings: ClusterSingletonProxySettings.Create(ActorSystem));

            var proxy = ActorSystem.ActorOf(clusterSingletonProxyProps, name: "consumerProxy");

            return proxy;
        }
    }
}
