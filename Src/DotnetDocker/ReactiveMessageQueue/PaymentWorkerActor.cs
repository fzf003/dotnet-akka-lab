using Akka.Actor;
using Akka.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveMessageQueue
{
    public class PaymentWorkerActor : ReceiveActor
    {
        public class StartMessage
        {
            public string Body { get; }
            public StartMessage(string body)
            {
                this.Body = body;
            }
        }
        public static Props CreateProps(ActorSystem actorSystem)
        {
            return ServiceProvider.For(actorSystem).Props<PaymentWorkerActor>();
        }

        private readonly IServiceGateway _paymentGateway;

        public PaymentWorkerActor(IServiceGateway paymentGateway)
        {
            _paymentGateway = paymentGateway;

            Context.SetReceiveTimeout(TimeSpan.FromSeconds(1));
 
            Ready();
        }

        void Ready()
        {
            Receive<string>(HandleSendPayment);

            Receive<PayResult>(HandlePaymentReceipt);
        }

      

        void Stop()
        {
            this.Receive<StartMessage>(p => {
                Console.WriteLine("Start:"+p.Body);
               
            });

            this.Receive<string>(p => {
                Console.WriteLine("Stop:{0}",p);
               //this.Become(Ready);
            });
            Console.WriteLine("停止...");
        }

        private void HandlePaymentReceipt(PayResult message)
        {
            Console.WriteLine($"{message.Body}");
        }

        private void HandleSendPayment(string message)
        {
            Console.WriteLine("Payment:{0}",message);
       
            _paymentGateway.Pay(message).PipeTo(Self, Sender);
        }
    }
}
