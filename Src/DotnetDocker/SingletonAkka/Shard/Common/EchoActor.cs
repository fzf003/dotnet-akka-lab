using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;

namespace Common
{
  
    public class SingleActor: ReceiveActor
    {
        public static Props CreateProps()
        {
            return Props.Create(() => new SingleActor());
        }

        public SingleActor()
        {
            Receive<Hello>(hello =>
            {
                Console.WriteLine("SingleActor:[{0}]===Hello-- {1}", this.Self, hello.Message);
                //Sender.Tell($"Response :{this.Self.Path.ToString()}--Hrello!");
            });
            this.ReceiveAny(p => {
                Console.WriteLine("单例 :{0}--{1}",p,this.Self.Path.ToString());
            });
        }
    }
    public class YouAcror:ReceiveActor
    {
        public static Props CreateProps()
        {
            return Props.Create(() => new YouAcror());
        }

          public YouAcror()
        {
            Receive<Hello>(hello =>
            {
                Console.WriteLine("[{0}]===Hello-- {1}", this.Self, hello.Message);
                //Sender.Tell($"Response :{this.Self.Path.ToString()}--Hrello!");
            });
        }

    }

 
    public class EchoActor : ReceiveActor
    {

        public static Props CreateProps()
        {
            return Props.Create(() => new EchoActor());
        }

        public EchoActor()
        {
            Receive<Hello>(hello =>
            {
                Console.WriteLine("[{0}]: {1}", this.Self, hello.Message);
                Sender.Tell($"Response :{this.Self.Path.ToString()}--Hrello!");
            });
        }
    }

    public class Hello
    {
      
        public Hello(string message)
        {
             
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class Start
    {
        public static Start Instance = new Start();

        private Start()
        {

        }
    }
}
