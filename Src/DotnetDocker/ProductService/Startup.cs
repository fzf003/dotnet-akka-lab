using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShardNode;
using Product.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Product.Application.Repository;
using Product.Infrastructure.Repository;
using Product.Infrastructure.Services;
using Product.Application.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Product.API.Actors;
using k8s.KubeConfigModels;
using Product.Application.Actors;
using System.IO;

namespace Product.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

         public void ConfigureServices(IServiceCollection services)
        {
            services.AddAkkaService("app.conf", false)
                    .AddHostedService<Worker>()
                    .AddHostedService<AkkaHostedService>()
                    .AddActorReference<ResponseActor>(ResponseActor.PropsFor())
                    .AddActorReference<ProductMasterActor>(ProductMasterActor.PropsFor());
           /* services.AddHealthChecksUI().AddInMemoryStorage();
            services.AddHealthChecks()
               .AddSqlServer(connectionString: Configuration["ConnectionStrings:Default"]);
               //.AddRedis($"{Configuration["Redis:Host"]},password={Configuration["Redis:Password"]}");
           */

            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductService, ProductService>();

            services.AddControllers();

            services.AddDbContext<ProductContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Defalut"), b =>
                {
                    b.EnableRetryOnFailure();
                });

            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product API", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Product API-V2", Version = "v2" });
            });


            services.AddLogging(cfg => cfg.AddConsole());
                
        }

         public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           // app.UseWelcomePage();
            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Product API V2");
            });

            app.UseEndpoints(endpoints =>
            {
                 endpoints.MapControllers();
                 endpoints.MapPut("Send",Process);

                endpoints.MapGet("/_proto/basket.proto", async ctx =>
                {
                    ctx.Response.ContentType = "text/plain";
                    
                    using var fs = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
                    using var sr = new StreamReader(fs);
                    while (!sr.EndOfStream)
                    {
                        var line = await sr.ReadLineAsync();
                        if (line != "/* >>" || line != "<< */")
                        {
                            await ctx.Response.WriteAsync(line+"\n");
                        }
                    }
                });
            });
        }

        Task Process(HttpContext httpContext)
        {
            Console.WriteLine(httpContext.Request.Method);

            httpContext.Response.WriteAsync(Guid.NewGuid().ToString("N"));
            return Task.CompletedTask;
        }
    }
}
