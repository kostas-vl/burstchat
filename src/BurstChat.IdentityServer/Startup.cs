using BurstChat.Application;
using BurstChat.Infrastructure;
using BurstChat.IdentityServer.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BurstChat.IdentityServer;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddApplication()
            .AddIdentityServerInfrastructure(Configuration);

        services
            .AddControllers()
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
    }

    public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            application.UseDeveloperExceptionPage();
            application.UseBurstChatDevelopmentResources(options => Configuration.GetSection("DevelopmentResources").Bind(options));
            application.UseAlphaInvitationCodes(options => Configuration.GetSection("Invitations").Bind(options));
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            application.UseHsts();
        }

        application
            .UseRouting()
            .UseCors(Infrastructure.DependencyInjection.CorsPolicyName)
            .UseIdentityServer()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
    }
}
