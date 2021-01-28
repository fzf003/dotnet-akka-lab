using System;
using Akka.Actor;
using Akka.DependencyInjection;

namespace Common
{
    public class SingleActor : ReceiveActor
    {
        public static Props CreateProps(ActorSystem actorSystem)
        {
            return ServiceProvider.For(actorSystem).Props<SingleActor>();
            // Props.Create(() => new SingleActor());
        }

        public SingleActor()
        {
            Receive<Messages.Hello>(hello =>
            {
                Sender?.Tell(new Messages.HelloResponse( string.Format("SingleActor:[{0}]===Hello-- {1}", this.Self, hello.Message)));
                Console.WriteLine("SingleActor:[{0}]===Hello-- {1}", this.Self, hello.Message);
            });

            ReceiveAny(p => {
                Console.WriteLine("单例 :{0}--{1}", p, this.Self.Path.ToString());
            });
        }
    }
}
