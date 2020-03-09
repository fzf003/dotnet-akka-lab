using Akka.Actor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static Common.ProductActor;

namespace Common
{
    public  class ProductMasterActor : ReceiveActor
    {
        public static Props PropsFor()
        {
            return Props.Create(() => new ProductMasterActor());
        }
        public ProductMasterActor()
        {
            Receive<CreateProductCommand>(s =>
            {
                var orderbookActor = Context.Child(s.Id).GetOrElse(() => StartChild(s.Id));
                orderbookActor.Tell(s);
            });

            Receive<ChangeProductNameCommand>(s =>
            {
                var orderbookActor = Context.Child(s.Id).GetOrElse(() => StartChild(s.Id));
                orderbookActor.Tell(s);
            });

            this.Receive<PrintState>(s => {
                var orderbookActor = Context.Child(s.Id).GetOrElse(() => StartChild(s.Id));
                orderbookActor.Tell(s);
            });

            this.Receive<State>(p => {
                Console.WriteLine($" 当前状态:{JsonConvert.SerializeObject(p.CurrState)}");
            });
        }

        private IActorRef StartChild(string productId)
        {
            return Context.ActorOf(Props.Create(() => new ProductActor(productId)), productId);
        }
    }

}
