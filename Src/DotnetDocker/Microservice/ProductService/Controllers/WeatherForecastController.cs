using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Product.API.Actors;
using ShardNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : Controller
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        readonly static HttpClient httpClient = new HttpClient();

        readonly ActorRefProvider<ResponseActor> _response;

        public WeatherForecastController(ActorRefProvider<ResponseActor> actorSystem)
        {
            this._response = actorSystem;
        }

        [HttpGet]
        public async Task<string> Get()
        {

            return await _response.Ask<string>(string.Empty);
            //var responseref = Response<string>.CreateResponse();

           // _actorSystem.EventStream.Publish(responseref);

            //await Task.Delay(2000);

            //return await responseref.GetResponseAsync();
        }

        [HttpGet]
        [Route("{Id}/Job")]
        public async Task<IEnumerable<WeatherForecast>> GetService(Guid Id)
        {
            // Making an http call here to serve as an example of
            // how dependency calls will be captured and treated
            // automatically as child of incoming request.
           // var res = await httpClient.GetStringAsync("http://www.baidu.com");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }


    public class WeatherForecastService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray());
        }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
