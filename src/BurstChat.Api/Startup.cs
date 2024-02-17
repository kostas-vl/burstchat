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

namespace BurstChat.Api;

public class Startup
{
    private readonly static ILoggerFactory BurstChatContextLogger = LoggerFactory.Create(builder => builder.AddConsole());

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

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
