using Akka.Actor;
using Akka.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
 

    public class ProductActor : ReceivePersistentActor
    {
        #region Message
        public class ProductSnapshot
        {
            public Product Snapshot { get; }

            public ProductSnapshot(Product order)
            {
                this.Snapshot = order;
            }
        }

        public class State
        {
            public Product CurrState { get; }

            public State()
                : this(new Product())
            {

            }
            public State(Product product)
            {
                this.CurrState = product;
            }
        }
        public class Product
        {
            public string ProductId { get; }

            public string ProductName { get; private set; }
            public Product()
                : this(string.Empty, string.Empty)
            {

            }
            public Product(string id, string name)
            {
                this.ProductId = id;
                this.ProductName = name;
            }

            public void ChangeName(string name)
            {
                this.ProductName = name;
            }

            public override string ToString()
            {
                return $"ProductId:{this.ProductId}--ProductName:{this.ProductName}";
            }
        }


        #endregion

        #region Command
        public class CreateProductCommand
        {
            public string Name { get; }

            public string Id { get; }
            public CreateProductCommand(string id, string name)
            {
                this.Id = id;
                this.Name = name;
            }

            public override string ToString()
            {
                return $"Id:{this.Id }--Name:{Name}";
            }
        }

        public class ChangeProductNameCommand
        {
            public string Id { get; }
            public string Name { get; }

            public ChangeProductNameCommand(string id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }

        public class PrintState
        {
            public string Id { get; }
            public PrintState(string id)
            {
                this.Id = id;
            }
        }
        #endregion

        #region Events
        public class CreateProductEvent
        {
            public string Name { get; }

            public string Id { get; }
            public CreateProductEvent(string id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }

        public class ChangeProductNameEvent
        {
            public string Id { get; }
            public string Name { get; }

            public ChangeProductNameEvent(string id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }

        #endregion

       public override string PersistenceId { get; }

        public static Props PropsFor(string Id)
        {
            return Props.Create(() => new ProductActor(Id));
        }

        private State state;

        

        public const int SnapshotInterval = 10;
        public ProductActor(string Id)
        {
            this.PersistenceId = $"product-{Id}";

            this.state = new State();

            Commands();

            Recovers();
        }

        private void Recovers()
        {
            Recover<SnapshotOffer>(offer =>
            {
                if (offer.Snapshot is ProductSnapshot productBook)
                {
                    Console.WriteLine("快照恢复:{0}名称:{1}", productBook.Snapshot.ProductId, productBook.Snapshot.ProductName);

                    state = new State(productBook.Snapshot);
                }
            });

            Recover<CreateProductEvent>(b =>
            {
                Console.WriteLine("事件恢复:{0}名称:{1}",b.Id,b.Name);
                this.state = new State(new Product(b.Id, b.Name));
            });

            Recover<ChangeProductNameEvent>(a =>
            {
                this.state.CurrState.ChangeName(a.Name);
            });

        }

        public void Commands()
        {
            Command<SaveSnapshotSuccess>(s =>
            {
                Console.WriteLine("保存快照成功");
                ///删除多余的事件
               /* DeleteSnapshots(new SnapshotSelectionCriteria(s.Metadata.SequenceNr - 1));
                DeleteMessages(s.Metadata.SequenceNr);*/
            });

            Command<CreateProductCommand>(p =>
            {
                this.Persist(new CreateProductEvent(p.Id, p.Name), @event =>
                {
                    this.state = new State(new Product(@event.Id, @event.Name));

                    Console.WriteLine($"创建:{@event.Id}--{ @event.Name}");
                    if (LastSequenceNr % SnapshotInterval == 0)
                    {
                        Console.WriteLine("快照:{0}", LastSequenceNr);
                        SaveSnapshot(new ProductSnapshot(this.state.CurrState));
                    }

                });
            });

            Command<ChangeProductNameCommand>(p =>
            {
                this.Persist(new ChangeProductNameEvent(p.Id, p.Name), handler: @event =>
                {
                    this.state.CurrState.ChangeName(@event.Name);

                    if (LastSequenceNr % SnapshotInterval == 0)
                    {
                        Console.WriteLine("快照:{0}", LastSequenceNr);
                        SaveSnapshot(new ProductSnapshot(this.state.CurrState));
                    }

                });
            });


            Command<PrintState>(p => {
                Sender?.Tell(this.state);
            });


        }
    }
}
