using System;
using Akka.Actor;
namespace Common
{
    public class SingleActor : ReceiveActor
    {
        public static Props CreateProps()
        {
            return Props.Create(() => new SingleActor());
        }

        public SingleActor()
        {
            Receive<Messages.Hello>(hello =>
            {
                Console.WriteLine("SingleActor:[{0}]===Hello-- {1}", this.Self, hello.Message);
            });

            ReceiveAny(p => {
                Console.WriteLine("单例 :{0}--{1}", p, this.Self.Path.ToString());
            });
        }
    }
}
