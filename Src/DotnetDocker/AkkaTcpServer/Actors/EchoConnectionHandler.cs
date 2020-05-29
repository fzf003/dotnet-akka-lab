using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AkkaTcpServer.Actors
{
    public class EchoConnectionHandler : ReceiveActor
    {
        private readonly IActorRef _connection;
        private readonly ILoggingAdapter logger = Context.GetLogger();
        public EchoConnectionHandler(EndPoint remote, IActorRef connection)
        {
            _connection = connection;
            ///监控接入的连接
            Context.Watch(connection);

            Receive<Tcp.Received>(received =>
            {
                var text = Encoding.UTF8.GetString(received.Data.ToArray()).Trim();
                logger.Info("接收 '{0}' 远程地址 [{1}]--{2}", text, remote, Sender.Path.Address.ToString());
                if (text == "exit")
                    Context.Stop(Self);
                else
                    Sender.Tell(Tcp.Write.Create(received.Data));
            });

            Receive<Tcp.ConnectionClosed>(closed =>
            {
                logger.Info("关闭远程连接 [{0}] ", remote);
                Context.Stop(Self);
            });

            Receive<Terminated>(terminated =>
            {
                logger.Info("远程连接已关闭 [{0}] ", remote);
                Context.Stop(Self);
            });
        }

 
        protected override void PostStop()
        {
            base.PostStop();

            _connection.Tell(Tcp.Close.Instance);
        }
    }
}
