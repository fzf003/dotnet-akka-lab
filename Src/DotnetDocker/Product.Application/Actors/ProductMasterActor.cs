using Akka.Actor;
using Product.Application.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.Application.Actors
{
    public sealed class ProductMasterActor : ReceiveActor
    {
        public static Props PropsFor()
        {
            return Props.Create<ProductMasterActor>();
        }
        public ProductMasterActor()
        {
            Receive<CreateProductDraftCommand>(s =>
            {
                var orderbookActor = Context.Child(s.ProductDraft.ProductId.ToString()).GetOrElse(() => StartChild(s.ProductDraft.ProductId.ToString()));
                orderbookActor.Forward(s);
            });
        }

        private IActorRef StartChild(string productid)
        {
            return Context.ActorOf(ProductActor.PropsFor(productid), productid);
        }
    }
}
