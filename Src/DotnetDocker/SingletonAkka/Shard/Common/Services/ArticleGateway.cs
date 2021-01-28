using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface IArticleGateway
    {
        Task<ArticleWorkerActor.PaymentResponse> QueryAsync(string AccountNO,string path, CancellationToken cancellationToken = default);
    }

    public class ArticleGateway : IArticleGateway
    {
        readonly HttpClient httpClient;
        
        public ArticleGateway(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ArticleWorkerActor.PaymentResponse> QueryAsync(string AccountNO, string path, CancellationToken cancellationToken=default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, path);
             
            var message=await this.httpClient.SendAsync(request,cancellationToken);
            if (message.IsSuccessStatusCode)
            {
               var responseBody= await message.Content.ReadAsStringAsync();

                return new ArticleWorkerActor.PaymentResponse(accountNO: AccountNO, path: path, message: responseBody);
            }
            return new ArticleWorkerActor.PaymentResponse(accountNO: AccountNO, path: path, message:"Error！！！" );
        }
    }
}