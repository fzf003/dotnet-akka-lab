using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveMessageQueue
{
    public interface IServiceGateway
    {
        Task<PayResult> Pay(string paytype);
    }

    public class DemoServiceGateway : IServiceGateway
    {
        readonly HttpClient httpClient;
        public DemoServiceGateway(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PayResult> Pay(string paytype)
        {
           var stream= await this.httpClient.GetStreamAsync("all.json");

            using (StreamContent streamContent = new StreamContent(stream))
            {
                return new PayResult($"{ paytype }---{await streamContent.ReadAsStringAsync()}");
            }

        }
    }

    public class PayResult
    {
        public string Body { get; }
        public PayResult(string body)
        {
            this.Body = body;

        }
    }
}
