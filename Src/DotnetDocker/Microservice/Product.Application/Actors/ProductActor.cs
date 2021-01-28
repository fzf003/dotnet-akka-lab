using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Product.Application.Commands;
using Product.Application.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductInfo = Product.Infrastructure.Domain;
namespace Product.Application.Actors
{
    public class ProductActor : ReceivePersistentActor
    {
        ILoggingAdapter logger= Context.GetLogger();
        public override string PersistenceId { get; }

        Product.Infrastructure.Domain.ProductInfo productState;
        public ProductActor(string productid)
        {
            
            this.PersistenceId = productid; 
        }

        public static Props PropsFor(string productid)
        {
            return Props.Create<ProductActor>(productid);
        }


        private void Commands()
        {
            this.Command<CreateProductDraftCommand>(ProcessCommand);
        }

        private void ProcessCommand(CreateProductDraftCommand productDraftCommand)
        {
            var @event = new CreateProductEvent()
            {
                ProductDraft = productDraftCommand.ProductDraft
            };

            this.Persist(@event, e => {

                Sender?.Tell(e);
                logger.Info("发布事件:{0}",Newtonsoft.Json.JsonConvert.SerializeObject(@event));
                Context.System.EventStream.Publish(e);
            });
        }





    }
}
