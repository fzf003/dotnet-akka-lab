using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPersistence.Actors
{
    /// <summary>
    /// 持续查询当前实体事件
    /// </summary>
    public class BookPersistenceQuery:ReceiveActor
    { 
        public class Start
        {
            public static Start Instance = new Start();
            private Start() { }

        }
        public static Props PropsFor(string bookid)
        {
            return Props.Create(() => new BookPersistenceQuery(bookid));
        }

        public BookPersistenceQuery(string bookid)
        {
            var actorSystem = Context.System;

            this.Receive<Start>(p => {

                var reader = actorSystem.ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
              

                reader.EventsByPersistenceId($"book-{bookid}", 0, long.MaxValue)
                      .RunForeach(p =>
                      {
                          Console.WriteLine($"读取当前Actor:book-{bookid} 事件:{JsonConvert.SerializeObject(p.Event)}");
                      }, actorSystem.Materializer());
            });
        }
    }
}
