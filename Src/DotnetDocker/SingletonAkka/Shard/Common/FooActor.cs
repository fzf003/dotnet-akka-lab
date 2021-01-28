using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.Routing;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
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

                _log.Info($"{p.Message}");
                Sender?.Tell(Enumerable.Range(1, 3)
                       .Select(x => new HelloResponse($"{Self.Path.ToString()}-{p.Message}"))
                       .ToList());

                //Sender?.Tell(new HelloResponse($"{Self.Path.ToString()}-{p.Message}"));

             });
        }
    }
}
