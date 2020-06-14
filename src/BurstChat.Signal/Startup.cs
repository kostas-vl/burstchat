using BurstChat.Application;
using BurstChat.Infrastructure;
using BurstChat.Signal.Options;
using BurstChat.Signal.Hubs.Chat;
using BurstChat.Signal.Services.ChannelsService;
using BurstChat.Signal.Services.DirectMessagingService;
using BurstChat.Signal.Services.PrivateGroupMessaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BurstChat.Signal.Services.ApiInteropService;
using BurstChat.Signal.Services.InvitationsService;
using BurstChat.Signal.Services.ServerService;
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

            services
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services
                .AddSignalR();

            services
                .AddScoped<IInvitationsService, InvitationsProvider>()
                .AddScoped<IServerService, ServerProvider>()
                .AddScoped<IPrivateGroupMessagingService, PrivateGroupMessagingProvider>()
                .AddScoped<IChannelsService, ChannelsProvider>()
                .AddScoped<IDirectMessagingService, DirectMessagingProvider>();

            services
                .AddHttpContextAccessor();

            services
                .AddHttpClient<BurstChatApiInteropService>();
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
