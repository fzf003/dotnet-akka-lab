using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messages
{
    public class Hello
    {

        public Hello(string message)
        {

            Message = message;
        }

        public string Message { get; private set; }
    }
}
