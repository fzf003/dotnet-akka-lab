using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Text;
namespace Common
{
    public class FooActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new FooActor())
                                   .WithRouter(FromConfig.Instance);
        }

        readonly Cluster cluster;
        public FooActor()
        {
           cluster = Cluster.Get(Context.System);

            _log.Info($" Foo 地址上线 : {cluster.SelfAddress}, {Self.Path.ToStringWithAddress()}");

            
            this.Receive<Messages.Hello>(p => {

                Console.WriteLine($"{p.Message}");
             });
        }
    }
}
