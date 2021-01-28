using System;

namespace PubSubShardMessage
{
   
    public class Ping
    {
        
        public Ping(string msg)
        {
            Msg = msg;
        }

        public string Msg { get; }
    }
}
