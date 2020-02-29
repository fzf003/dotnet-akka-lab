using Akka.Actor;
using Akka.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using Akka;
namespace Common
{
    public class Print
    {
        public static Print Instance = new Print();
        private Print() { }
    }

        
public interface ICreateCommand
    {
           string BookId { get;   }

          string BookName { get;   }

          decimal Price { get;  }
    }
    public class CreateBookCommand : ICreateCommand
    {
        public string BookId { get; private set; }

        public string BookName { get; private set; }

        public decimal Price { get; private set; }

        public CreateBookCommand(string Id,string Name ,decimal price)
        {
            this.BookId = Id;
            this.BookName = Name;
            this.Price = price;
        }


    }

    public class BookInfo
    {
        public string BookId { get; private set; }

        public string BookName { get; private set; }

        public decimal Price { get; private set; }

        public BookInfo(string Id, string Name, decimal price)
        {
            this.BookId = Id;
            this.BookName = Name;
            this.Price = price;
        }
    }
    public sealed class OrderBookMasterActor : ReceiveActor
    {
        public static Props PropsFor()
        {
            return Props.Create(() => new OrderBookMasterActor());
        }
        public OrderBookMasterActor()
        {
 

            Receive<ICreateCommand>(s =>
            {
                var orderbookActor = Context.Child(s.BookId).GetOrElse(() => StartChild(s.BookId));
                orderbookActor.Forward(s);
            });

            this.Receive<Print>(s => {
               // var orderbookActor = Context.Child(s).GetOrElse(() => StartChild(s.BookId));
                //orderbookActor.Forward(s);
            });
           
        }

        private IActorRef StartChild(string bookId)
        {
            return Context.ActorOf(OrderBookActor.PropsFor(bookId));
        }
    }
    public class OrderBookActor : ReceivePersistentActor
    {
       
        public class OrderbookSnapshot
        {
            public OrderbookSnapshot(string Id, string name, decimal price)
            {
                this.BookName = name;
                this.BookId = Id;
                this.Price = price;
            }
            public string BookId { get; }

            public string BookName { get; }

            public decimal Price { get; }
        }
        public class CreateBookEvent
        {
            public CreateBookEvent(string Id,string name,decimal price)
            {
                this.BookName = name;
                this.BookId = Id;
                this.Price = price;
            }

           public string BookId { get; }

            public string BookName { get; }

            public decimal Price { get; }
        }

        public override string PersistenceId => this.id;

        public static Props PropsFor(string Id)
        {
            return Props.Create(() => new OrderBookActor(Id));
        }

        readonly string id;

        private readonly long SnapshotInterval = 10;

        BookInfo bookInfo { get;  set; }
        public OrderBookActor(string Id)
        {
            this.id = Id;
            Recovers();
            Commands();
            //this.bookInfo = new BookInfo(string.Empty, string.Empty, 0);
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), this.Self, Print.Instance,this.Self);
           base.PreStart();
        }

        private void Recovers()
        {
            Recover<SnapshotOffer>(offer =>
            {
                offer.Snapshot.Match()
                     .With<OrderbookSnapshot>(p => {
                         this.bookInfo = new BookInfo(p.BookId, p.BookName, p.Price);
                     });
            });

            this.Recover<CreateBookEvent>(p => {
                this.bookInfo = new BookInfo(p.BookId, p.BookName, p.Price);
            });
        }


        private void Commands()
        {
            this.Command<ICreateCommand>(p => {
                ProcessAsk(p);
            });

            this.Command<Print>(p => {
                Console.WriteLine("Print:{0}",Newtonsoft.Json.JsonConvert.SerializeObject(this.bookInfo));
            });
        }

        void ProcessAsk(ICreateCommand command)
        {
            this.PersistAsync(new CreateBookEvent(command.BookId,command.BookName,command.Price), @event => {
                Console.WriteLine($"创建Book:{@event.BookId}--{@event.BookName}--{@event.Price}");
                this.bookInfo = new BookInfo(@event.BookId, @event.BookName, @event.Price);
                PersistSnapshot();
            });
        }

        void PersistSnapshot()
        {
            if (LastSequenceNr % SnapshotInterval == 0)
            {
                SaveSnapshot(this.bookInfo);
            }
        }


    }
}
