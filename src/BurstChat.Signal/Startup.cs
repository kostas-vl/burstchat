using BurstChat.Application;
using BurstChat.Infrastructure;
using BurstChat.Signal.Options;
using BurstChat.Signal.Hubs.Chat;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using DependencyInjection = BurstChat.Infrastructure.DependencyInjection;

namespace BurstChat.Signal
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        /// <summary>
        ///   Creates a new instance of Startup.
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddApplication()
                .AddInfrastructure(Configuration);

            services.Configure<ApiDomainOptions>(options =>
            {
                options.BurstChatApiDomain = Configuration.GetValue<string>("BurstChatApiDomain");
            });

            services.AddControllers();
            services.AddSignalR();
            services.AddHttpContextAccessor();
       }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            application.UseAuthentication();

            application.UseRouting();
            application.UseCors(DependencyInjection.CorsPolicyName);
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
