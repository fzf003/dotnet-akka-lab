using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Event;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Services
{
    
    public class ArticleWorkerActor : ReceiveActor
    {
        public static Props For(ActorSystem actorSystem)
        {
             return ServiceProvider.For(actorSystem).Props<ArticleWorkerActor>()
                                  .WithRouter(FromConfig.Instance);
        }

        public class SendPaymentMessage
        {
            public SendPaymentMessage(string accountNO, string path)
            {
                AccountNO = accountNO;
                Path = path;
            }

            public string AccountNO { get; }
            public string Path { get; }
        }

        public class PaymentResponse
        {
           
            public PaymentResponse(string accountNO, string path,string message)
            {
                AccountNO = accountNO;
                Path = path;
                Message = message;
            }

            public string AccountNO { get; }

            public string Path { get; set; }

            public string Message { get; }
        }

        ILoggingAdapter logging = Context.GetLogger();

        private readonly IArticleGateway  _articleGateway;

        public ArticleWorkerActor(IArticleGateway articleGateway)
        {
            _articleGateway = articleGateway;

            Receive<SendPaymentMessage>(HandleSendPayment);

            Receive<PaymentResponse>(HandlePaymentReceipt);
        }

        private void HandlePaymentReceipt(PaymentResponse message)
        {
            message.Path = $"{this.Self.Path}--{message.Path}--HashCode:{this._articleGateway.GetHashCode()}";
            logging.Info($"回复处理:{message.Message}");
            Sender.Tell(message);
        }

        private void HandleSendPayment(SendPaymentMessage message)
        {
            logging.Info($"请求处理:{message.AccountNO}");
            _articleGateway.QueryAsync(message.AccountNO, message.Path).PipeTo(Self, Sender);
        }
    }

    
}
