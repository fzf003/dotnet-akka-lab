namespace Dotnetakkaserver {
    using Microsoft.Extensions.Hosting; 
    using Microsoft.Extensions.Logging; 
    using System; 
    using System.Threading; 
    using System.Threading.Tasks; 

    public class Worker:BackgroundService {
        protected override  Task ExecuteAsync(CancellationToken stoppingToken) {

            return Task.CompletedTask;
        }
    }
}