using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubShardMessage
{
    public class Pong
    {
       
        public Pong(string rsp,string replyaddress)
        {
            Rsp = rsp;
            ReplyAddress = replyaddress;
        }

        public string Rsp { get;   }

        public string ReplyAddress { get;  }
    }
}
