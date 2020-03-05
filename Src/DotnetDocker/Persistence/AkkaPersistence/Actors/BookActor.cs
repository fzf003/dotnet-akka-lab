using Akka.Actor;
using Akka.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPersistence.Actors
{
    public class BookActor : ReceivePersistentActor
    {
        #region 
        public class OrderbookSnapshot
        {
            public Book Snapshot { get; }

            public OrderbookSnapshot(Book book)
            {
                this.Snapshot = book;
            }
        }

        public class State
        {
            public Book CurrState { get; }

            public State()
                : this(new Book())
            {

            }
            public State(Book book)
            {
                this.CurrState = book;
            }
        }
        public class Book
        {
            public string BookId { get; }

            public string BookName { get; private set; }
            public Book()
                : this(string.Empty, string.Empty)
            {

            }
            public Book(string id, string name)
            {
                this.BookId = id;
                this.BookName = name;
            }

            public void ChangeName(string name)
            {
                this.BookName = name;
            }

            public override string ToString()
            {
                return $"BookId:{this.BookId}--BookName:{this.BookName}";
            }
        }
        #endregion

        #region Command
        public class CreateBookCommand
        {
            public string Name { get; }

            public string Id { get; }
            public CreateBookCommand(string id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }

        public class ChangeBookNameCommand
        {
            public string Id { get; }
            public string Name { get; }

            public ChangeBookNameCommand(string id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }

        public class PrintState
        {
            public string Id { get; }
            public PrintState(string id) {
                this.Id = id;
            }
        }
        #endregion

        #region Events
        public class CreateBookEvent
        {
            public string Name { get; }

            public string Id { get; }
            public CreateBookEvent(string id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }

        public class ChangeBookNameEvent
        {
            public string Id { get; }
            public string Name { get; }

            public ChangeBookNameEvent(string id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }

        #endregion

        public override string PersistenceId { get; }

        public static Props PropsFor(string Id)
        {
            return Props.Create(() => new BookActor(Id));
        }

        private State BookState;

        public const int SnapshotInterval = 10;
        public BookActor(string Id)
        {
            this.PersistenceId = $"book-{Id}";

            this.BookState = new State();

            Commands();

            Recovers();
        }

        private void Recovers()
        {
            Recover<SnapshotOffer>(offer =>
            {
                if (offer.Snapshot is OrderbookSnapshot orderBook)
                {
                    BookState = new State(orderBook.Snapshot);

                }
            });

            Recover<CreateBookEvent>(b =>
            {
                this.BookState = new State(new Book(b.Id, b.Name));
            });

            Recover<ChangeBookNameEvent>(a =>
            {
                this.BookState.CurrState.ChangeName(a.Name);
            });

        }

        public void Commands()
        {
            Command<SaveSnapshotSuccess>(s =>
            {
                Console.WriteLine("保存快照成功");
                DeleteSnapshots(new SnapshotSelectionCriteria(s.Metadata.SequenceNr - 1));
                DeleteMessages(s.Metadata.SequenceNr);
            });

            Command<CreateBookCommand>(p =>
            {
                this.Persist(new CreateBookEvent(p.Id, p.Name), @event =>
                     {

                         this.BookState = new State(new Book(@event.Id, @event.Name));

                         if (LastSequenceNr % SnapshotInterval == 0)
                         {
                             Console.WriteLine("快照:{0}", LastSequenceNr);
                             SaveSnapshot(new OrderbookSnapshot(this.BookState.CurrState));
                         }

                     });
            });

            Command<ChangeBookNameCommand>(p =>
            {
                this.Persist(new ChangeBookNameEvent(p.Id, p.Name), handler: @event =>
                 {
                     this.BookState.CurrState.ChangeName(@event.Name);

                     if (LastSequenceNr % SnapshotInterval == 0)
                     {
                         Console.WriteLine("快照:{0}", LastSequenceNr);
                         SaveSnapshot(new OrderbookSnapshot(this.BookState.CurrState));
                     }

                 });
            });


            Command<PrintState>(p => {
                Sender?.Tell(BookState);
            });


        }
    }
}
