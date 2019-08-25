﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurstChat.Signal.Options;
using BurstChat.Signal.Hubs.Chat;
using BurstChat.Signal.Services.ChannelsService;
using BurstChat.Signal.Services.PrivateGroupMessaging;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                .Configure<AcceptedDomainsOptions>(Configuration.GetSection("AcceptedDomains"));

            services
                .AddScoped<IPrivateGroupMessagingService, PrivateGroupMessagingProvider>()
                .AddScoped<IChannelsService, ChannelsProvider>();

            services.AddSignalR();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetSection("AccessTokenValidation:Authority").Get<string>();
                    options.ApiName = Configuration.GetSection("AccessTokenValidation:ApiName").Get<string>();
                    options.ApiSecret = Configuration.GetSection("AccessTokenValidation:ApiSecret").Get<string>();
                    options.RequireHttpsMetadata = false;
                });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    var acceptedDomains = Configuration
                        .GetSection("AcceptedDomains:Cors")
                        .Get<string[]>();
                    if (acceptedDomains != null && acceptedDomains.Count() > 0)
                    {
                        builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(acceptedDomains)
                            .AllowCredentials();
                    }
                });
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

            application.UseAuthentication();

            application
                .UseCors("CorsPolicy")
                .UseSignalR(options =>
                {
                    options.MapHub<ChatHub>("/chat");
                })
                .UseMvc();
        }
    }
}
