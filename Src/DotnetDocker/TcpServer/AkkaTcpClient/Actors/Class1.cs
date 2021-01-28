using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System.Net;

namespace AkkaTcpClient.Actors
{
    public class UpdClientService : ReceiveActor
    {
        private IActorRef connectionActor;
        public IStash Stash { get; set; }
        private readonly ILoggingAdapter logger = Context.GetLogger();
        readonly IPAddress _iPAddress;
        readonly int _prot;

        public UpdClientService() : this(IPAddress.Any, 9988) { }
        public UpdClientService(IPAddress ipAddress, int prot)
        {
            this._iPAddress = ipAddress;
            this._prot = prot;
            Receive<UpdStartMessage>(_ => HandleStart());
            Receive<UpdStopMessage>(_ => HandleStop());
            Receive<UpdSendMessage>(msg => WriterHandle(msg));
            Receive<Akka.IO.UdpConnected>(m => HandleConnected(m));
            Receive<Akka.IO.Udp.Received>(m => HandleReceived(m));
            Receive<Terminated>(p => {
                logger.Info("释放..{0}", p.ActorRef.Path.ToString());
                Context.Stop(Self);
            });
            this.ReceiveAny(o => {
                Console.WriteLine(o);
                Unhandled(o);
            });
        }

        private void HandleConnected(UdpConnected m)
        {
            throw new NotImplementedException();
        }

        private void HandleReceived(Udp.Received m)
        {
            throw new NotImplementedException();
        }

        protected override void PreStart()
        {
            Self.Tell(UpdStartMessage.Instance);
            base.PreStart();
        }

        private void HandleStart()
        {

            logger.Info($"远程接入链接:{Sender.Path.Address.ToString()}");
           // Context.System.Udp().Tell(new Akka.IO.Udp.Connect(new IPEndPoint(this._iPAddress, this._prot)));
        }

        private void HandleStop()
        {
            connectionActor?.Tell(Tcp.Close.Instance);
            Context.Stop(Self);
            logger.Info($"停止{Sender.Path.Address.ToString()}");
        }

        void HandleConnected(Tcp.Connected m)
        {
            logger.Info("tcp.connected,remote:{0}--{1}--sender:{2}", m?.RemoteAddress?.ToString(), m?.LocalAddress?.ToString(), Sender?.Path.Address.ToString());

            connectionActor = Sender;
            Context.Watch(connectionActor);
           
            Sender.Tell(new Tcp.Register(Self));

        }

        void HandleReceived(Tcp.Received m)
        {
            logger.Info($"接收信息: {m.Data}--{Sender.Path.Address.ToString()}");
        }

        void WriterHandle(UpdSendMessage message)
        {
          
            connectionActor?.Tell(Tcp.Write.Create(ByteString.FromString(message.Message)));
        }

        #region 消息协定
        public class UpdStopMessage
        {
            private UpdStopMessage() { }

            public static UpdStopMessage Instance = new UpdStopMessage();

        }

        public class UpdSendMessage
        {
            readonly string _message;
            public UpdSendMessage(string message)
            {
                this._message = message;
            }

            public string Message
            {
                get
                {
                    return this._message;
                }
            }
        }

        public class UpdStartMessage
        {
            private UpdStartMessage() { }

            public static UpdStartMessage Instance = new UpdStartMessage();
        }

        #endregion
    }
}
