using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class FccActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new FccActor());
                                 //  .WithRouter(FromConfig.Instance);
        }

        public FccActor()
        {
            Cluster cluster = Cluster.Get(Context.System);
            _log.Info($">>> Foo Address : {cluster.SelfAddress}, {Self.Path.ToStringWithAddress()}");

            
            this.Receive<Hello>(p => {

                Console.WriteLine("kfk Tell:"+p.Message);
             });
        }
    }
}
