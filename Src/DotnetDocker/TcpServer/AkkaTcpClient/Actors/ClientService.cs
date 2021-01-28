using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AkkaTcpClient.Actors
{
    public class ClientService : ReceiveActor
    {
        private IActorRef connectionActor;
        public IStash Stash { get; set; }
        private readonly ILoggingAdapter logger = Context.GetLogger();
        readonly IPAddress _iPAddress;
        readonly int _prot;

        public ClientService() : this(IPAddress.Any, 9988) { }
        public ClientService(IPAddress ipAddress,int prot)
        {
            this._iPAddress = ipAddress;
            this._prot = prot;
            Receive<StartMessage>(_ => HandleStart());
            Receive<StopMessage>(_ => HandleStop());
            Receive<SendMessage>(msg=>WriterHandle(msg));
            Receive<Tcp.Connected>(m => HandleConnected(m));
            Receive<Tcp.Received>(m => HandleReceived(m));
            Receive<Terminated>(p => {
                logger.Info("释放..{0}" , p.ActorRef.Path.ToString());
                Context.Stop(Self);
            });
            this.ReceiveAny(o => {
                Console.WriteLine(o);
                Unhandled(o);
            });
        }

        protected override void PreStart()
        {
            Self.Tell(StartMessage.Instance);
            base.PreStart();
        }

        private void HandleStart()
        {
            logger.Info($"远程接入链接:{Sender.Path.Address.ToString()}");
            Context.System.Tcp().Tell(new Tcp.Connect(new IPEndPoint(this._iPAddress, this._prot)));
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

        void WriterHandle(SendMessage message)
        {
            connectionActor?.Tell(Tcp.Write.Create(ByteString.FromString(message.Message)));
        }

        #region 消息协定
        public class StopMessage
        {
            private StopMessage() { }

            public static StopMessage Instance = new StopMessage();

        }

        public class SendMessage
        {
            readonly string _message;
            public SendMessage(string message)
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

        public class StartMessage
        {
            private StartMessage() { }

            public static StartMessage Instance = new StartMessage();
        }

        #endregion
    }


}
