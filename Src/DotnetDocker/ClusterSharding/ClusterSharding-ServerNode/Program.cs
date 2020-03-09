﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShardNode;
namespace ClusterSharding_ServerNode
{
    class Program
    {
        static Task Main(string[] args)
        {
            return new HostBuilder()
                                 .ConfigureServices(services =>
                                 {
                                     services.AddAkkaService("app.conf",false)
                                             .AddHostedService<AkkaHostedService>()
                                             .AddLogging();

                                 }).ConfigureLogging(build =>
                                 {
                                     build.AddConsole();
                                 })
                                .RunConsoleAsync();
        }
    }

   
}
