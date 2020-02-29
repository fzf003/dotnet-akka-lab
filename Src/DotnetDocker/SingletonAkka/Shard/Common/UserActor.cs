using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class UserActor: ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new UserActor());
        }
        readonly Cluster cluster;

        readonly IActorRef mediator;
        public UserActor()
        {
            cluster = Cluster.Get(Context.System);

            mediator = DistributedPubSub.Get(Context.System).Mediator;

            mediator.Tell(new Subscribe("fzf003", Self));

            //mediator.Tell(new Put(Self));

            ReadyHandle();
        }

        void ReadyHandle()
        {
            this.Receive<SubscribeAck>(p => {
                Console.WriteLine("订阅者:{0} 组名:{1} Topic:{2} ",
                    p.Subscribe.Ref,p.Subscribe.Group,p.Subscribe.Topic);
            });

            this.Receive<string>(p => {
                Console.WriteLine("定阅信息:{0}--Sender:{1}",p,this.Sender.Path.ToString());
            });

            this.ReceiveAny(p => {
                Console.WriteLine("死信息:{0}", p);
            });
        }

     
        protected override void PreStart()
        {
             base.PreStart();
        }

        protected override void PostStop()
        {
            // Unsubscribe
            // mediator.Tell(new Unsubscribe("fzf003", Self));
            mediator.Tell(new Remove(this.Self.Path.ToString()));
            base.PostStop();
        }
    }


    public class Publisher:ReceiveActor
    {
        readonly IActorRef mediator;
        public Publisher()
        {
            mediator = DistributedPubSub.Get(Context.System).Mediator;
            Ready();
        }

       void Ready()
        {
            this.Receive<string>(p => {
               // mediator.Tell(new Send(path: "/user/sub-1",message: p));
                mediator.Tell(new Publish("fzf003", p));
            });

            this.Receive<MyHello>(p => { });
        }

        public sealed class MyHello
        {

        }
    }

    
}
