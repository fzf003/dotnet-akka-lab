using Akka.Actor;
using Newtonsoft.Json;
using System;

namespace AkkaPersistence
{
    public sealed class BookMasterActor : ReceiveActor
    {
        public BookMasterActor()
        {
            Receive<Actors.BookActor.CreateBookCommand>(s =>
            {
                var orderbookActor = Context.Child(s.Id).GetOrElse(() => StartChild(s.Id));
                orderbookActor.Tell(s);
            });

            Receive<Actors.BookActor.ChangeBookNameCommand>(s =>
            {
                var orderbookActor = Context.Child(s.Id).GetOrElse(() => StartChild(s.Id));
                orderbookActor.Tell(s);
            });

            this.Receive<Actors.BookActor.PrintState>(s => {
                var orderbookActor = Context.Child(s.Id).GetOrElse(() => StartChild(s.Id));
                orderbookActor.Tell(s);
            });

            this.Receive<Actors.BookActor.State>(p => {
                Console.WriteLine($" 当前状态:{JsonConvert.SerializeObject( p.CurrState)}");
            });
        }

        private IActorRef StartChild(string bookId)
        {
            return Context.ActorOf(Props.Create(() => new Actors.BookActor(bookId)), bookId);
        }
    }
}
