using System;
using BurstChat.Application;
using BurstChat.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace BurstChat.Api
{
    public class Startup
    {
        private readonly static ILoggerFactory BurstChatContextLogger = LoggerFactory.Create(builder => builder.AddConsole());

        public IConfiguration Configuration { get; }

        /// <summary>
        ///   Creates an instance of Startup.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance of the application</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        ///   This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services collection to be used for the configuration</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddApplication()
                .AddInfrastructure(Configuration);

            services.AddMvc();
            services.AddControllers();
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "BurstChat API", Version = "v1" });
            });
        }

        /// <summary>
        ///   This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="application">The application builder to be used in the configuration</param>
        /// <param name="env">The hosting environment that the application is running</param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }

            application
                .UseStaticFiles()
                .UseRouting()
                .UseCors(Infrastructure.DependencyInjection.CorsPolicyName)
                .UseAuthentication()
                .UseAuthorization()
                .UseSwagger()
                .UseSwaggerUI(config =>
                {
                    config.SwaggerEndpoint("/swagger/v1/swagger.json", "BurstChat API V1");
                })
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .Use(async (context, next) =>
                {
                    var path = context?.Request?.Path;
                    if (path?.Value?.IndexOf("/api", StringComparison.InvariantCulture) == -1)
                    {
                        context?.Response?.Redirect("index.html");
                        return;
                    }
                    await next();
                });
        }
    }
}
