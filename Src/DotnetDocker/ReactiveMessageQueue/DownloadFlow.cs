using Akka;
using Akka.Streams.Dsl;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveMessageQueue
{
    public static class DownloadFlow
    {
        public static Flow<string, PayResult, NotUsed> ProcessHtmlDownloadFor(
             int degreeOfParallelism, IServiceGateway serviceGateway)
        {
            return Flow.Create<string>()
                       .SelectAsync(degreeOfParallelism, doc => serviceGateway.Pay(doc).ContinueWith(tr=> {
                           if (tr.IsFaulted || tr.IsCanceled)
                               return new PayResult("erropr!!!");

                           return tr.Result;
                       }));
        }

 

        private static Func<Task<string>, PayResult> HtmlContinuationFunction()
        {
            return tr =>
            {
                // bad request, server error, or timeout
                if (tr.IsFaulted || tr.IsCanceled)
                    return new PayResult("Cancel");

                // 404
                if (string.IsNullOrEmpty(tr.Result))
                    return new PayResult("Error!!");

                return new PayResult(tr.Result);
            };
        }
    }
}
