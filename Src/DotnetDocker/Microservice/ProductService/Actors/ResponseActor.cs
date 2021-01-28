using Akka;
using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Event;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Akka.Util.Internal;
using System.IO;

namespace Product.API.Actors
{

    public class RequestActor: ReceiveActor
    {
        public static Props PropsFor()
        {
            return Props.Create<RequestActor>();
        }

        public RequestActor() {
            
        }
    }

    public class ResponseActor:ReceiveActor
    {
     
            public static Props PropsFor()
            {
                return Props.Create<ResponseActor>();
            }

            public ResponseActor()
            {
              Context.System.EventStream.Subscribe<Response<string>>(Self);

            }

            protected override bool AroundReceive(Receive receive, object message)
            {
           
                return message.Match().With<Response<string>>(p => {
                    Console.WriteLine("回复:{0}", p);
                    //p.SetSuccess(Guid.NewGuid().ToString("N"));
                }).With<string>(p => {
                  
                    Sender?.Tell(Guid.NewGuid().ToString("N")+"---"+Self.Path.ToString()+"--"+Context.Parent.Path.ToString());
                })
                .WasHandled;

                //base.AroundReceive(receive, message);
            }
    }

    public class Response<T>
    {
        private TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
 
        public void SetSuccess(T success)
        {
             taskCompletionSource.SetResult(success);
        }

        public void SetError(Exception  error)
        {
             
            taskCompletionSource.SetException(error);
        }

        public Task<T> GetResponseAsync()
        {
            return taskCompletionSource.Task;
        }



        public static Response<T> CreateResponse()
        {
            return new Response<T>();
        }
    }
}
