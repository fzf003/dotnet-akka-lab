using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnetakkaserver
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        readonly IClusterService _clusterService;
        public QueryController(IClusterService clusterService)
        {
            this._clusterService = clusterService;
        }

        [HttpGet]
        public   async Task<List<HelloResponse>> QueryResponseAsync()
        {
            return await _clusterService.ResponseAsync(new Hello(Guid.NewGuid().ToString()), TimeSpan.FromSeconds(5)).ConfigureAwait(false);
         
        }
    }
}
