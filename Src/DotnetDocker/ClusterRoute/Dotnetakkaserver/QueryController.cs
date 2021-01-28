using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Common.Services.ArticleWorkerActor;

namespace Dotnetakkaserver
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        readonly IClusterService _clusterService;

        readonly ClusterProxy _clusterProxy;

        public QueryController(IClusterService clusterService, ClusterProxy clusterProxy)
        {
            this._clusterService = clusterService;
            _clusterProxy = clusterProxy;
        }


        [HttpGet("{Id}")]
        public   async Task<List<HelloResponse>> QueryResponseAsync(string Id="oop")
        {
            return await _clusterService.ResponseAsync(new Hello(Id),TimeSpan.FromSeconds(1)).ConfigureAwait(false);
        }

        [HttpGet("/userName/{userName}/Path/{path}")]
        public Task<PaymentResponse> QueryArtileAsync(string userName="fzf003",string path="siji.json")
        {
            return this._clusterProxy.ArtileClient.Ask<PaymentResponse>(new SendPaymentMessage(userName,path));
        }
    }
}
