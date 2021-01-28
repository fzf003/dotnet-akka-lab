using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AkkaTcpServer.Actors
{
    public class EchoService : ReceiveActor
    {
        private readonly IActorRef _manager = Context.System.Tcp();
        private IActorRef _tcpListener;
        private IActorRef _stopCommander;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EchoService(EndPoint endpoint)
        {
            _manager.Tell(new Tcp.Bind(Self, endpoint));

            Receive<Tcp.Bound>(bond => {
                Console.WriteLine($"绑定TCP监听连接:{Sender}");
                 _tcpListener = Sender;
            });

            Receive<Tcp.Connected>(connected =>
            {
                logger.Info("远程连接 {0} 已接入", connected.RemoteAddress);
                var handler = Context.ActorOf(Props.Create(() => new EchoConnectionHandler(connected.RemoteAddress, Sender)));
                Sender.Tell(new Tcp.Register(handler));
            });

            
            Receive<StopServer>(_ =>
            {
                logger.Info("关闭连接:{0}",Sender.Path.ToString());
                _stopCommander = Sender;
                _tcpListener?.Tell(Tcp.Unbind.Instance);
            });
            // Report that close completed
            Receive<Tcp.Unbound>(_ => {
                Console.WriteLine("解除绑定");
                _stopCommander?.Tell("Done");
            });
        }

        public class StopServer {
            private StopServer() { }
            public static StopServer Instance = new StopServer();
        }
    }
}
