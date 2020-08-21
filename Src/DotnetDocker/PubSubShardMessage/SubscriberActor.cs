using Akka.Actor;
using Akka.Cluster.Tools.Client;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubShardMessage
{
    public class SubscriberActor : ReceiveActor
    {

        private readonly ILoggingAdapter _log = Context.GetLogger();

        string topicname = "fzf003";

        string groupname = "fzfgroup";

        DistributedPubSubService pubSubServicemediator;

        public SubscriberActor()
        {
            pubSubServicemediator = DistributedPubSubService.For(Context.System);

            pubSubServicemediator.Subscribe(topicname, Self, groupname);
  
            Receive<SubscribeAck>(_ => Handle(_));

            Receive<Ping>(_ => Handle(_));

            Receive<ITradeEvent>(p => {

                Console.WriteLine($"Group:{groupname}收到事件:{p.Body}--{this.Sender.Path}");
            });
        }

        protected override void PostStop()
        {
            pubSubServicemediator.UnSubscribe(topicname, Self, groupname);
            base.PostStop();

        }

        private void Handle(SubscribeAck msg)
        {
            if (msg.Subscribe.Topic.Equals(topicname)
                && msg.Subscribe.Ref.Equals(Self)
               && msg.Subscribe.Group.Equals(groupname))
            {
                _log.Info($" Group:{groupname} Recevied message : {msg}, Sender: {Sender}");
            }
        }

        private void Handle(Ping msg)
        {
            _log.Info($"接收到消息 : {msg.Msg}, 来自: {Sender}");
        }
    }
}
