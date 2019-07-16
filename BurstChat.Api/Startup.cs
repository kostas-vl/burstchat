using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurstChat.Api.Services.BCryptService;
using BurstChat.Api.Services.ChannelsService;
using BurstChat.Api.Services.ModelValidationService;
using BurstChat.Api.Services.ServersService;
using BurstChat.Api.Services.UserService;
using BurstChat.Shared.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BurstChat.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IBCryptService, BCryptProvider>();

            services
                .AddScoped<IChannelsService, ChannelsProvider>()
                .AddScoped<IServersService, ServersProvider>()
                .AddScoped<IUserService, UserProvider>();

            services
                .AddTransient<IModelValidationService, ModelValidationProvider>();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddDbContext<BurstChatContext>(options =>
                {
                    var databaseConfiguration = Configuration.GetSection("Database");
                    var provider = databaseConfiguration["Provider"];
                    var connection = databaseConfiguration["ConnectionString"];
                    switch (provider)
                    {
                        case "sqlite":
                            options.UseSqlite(connection, dbContextOptions => 
                            {
                                dbContextOptions.MigrationsAssembly("BurstChat.Api");
                            });
                            break;
                        case "sqlserver":
                            options.UseSqlServer(connection, dbContextOptions =>
                            {
                                dbContextOptions.MigrationsAssembly("BurstChat.Api");
                            });
                            break;
                        default:
                            break;
                    }
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder application, IHostingEnvironment env)
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

            application
                .UseMvc();
        }
    }
}
