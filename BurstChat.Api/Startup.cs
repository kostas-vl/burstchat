﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurstChat.Api.Options;
using BurstChat.Api.Services.ChannelsService;
using BurstChat.Api.Services.PrivateGroupsService;
using BurstChat.Api.Services.DirectMessagingService;
using BurstChat.Api.Services.ServersService;
using BurstChat.Shared.Context;
using BurstChat.Shared.Services.BCryptService;
using BurstChat.Shared.Services.ModelValidationService;
using BurstChat.Shared.Services.UserService;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api
{
    public class Startup
    {
        private readonly static ILoggerFactory BurstChatContextLogger = LoggerFactory.Create(builder => builder.AddConsole());

        public IConfiguration Configuration
        { 
            get;
        }

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
            IdentityModelEventSource.ShowPII = true;
            
            services
                .Configure<DatabaseOptions>(Configuration.GetSection("Database"))
                .Configure<AcceptedDomainsOptions>(Configuration.GetSection("AcceptedDomains"));

            services
                .AddSingleton<IBCryptService, BCryptProvider>();

            services
                .AddScoped<IChannelsService, ChannelsProvider>()
                .AddScoped<IPrivateGroupsService, PrivateGroupsProvider>()
                .AddScoped<IDirectMessagingService, DirectMessagingProvider>()
                .AddScoped<IServersService, ServersProvider>()
                .AddScoped<IModelValidationService, ModelValidationProvider>()
                .AddScoped<IUserService, UserProvider>();

            services
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services
                .AddAuthorization();

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options => Configuration.GetSection("AccessTokenValidation").Bind(options));

            services
                .AddDbContext<BurstChatContext>(options =>
                {
                    var databaseOptions = new DatabaseOptions();

                    Configuration
                        .GetSection("Database")
                        .Bind(databaseOptions);

                    switch (databaseOptions.Provider)
                    {
                        case "npgsql":
                            options
                                .UseLoggerFactory(BurstChatContextLogger)
                                .UseNpgsql(databaseOptions.ConnectionString, dbContextOptions =>
                                {
                                    dbContextOptions.MigrationsAssembly("BurstChat.Api");
                                });
                            break;
                        case "sqlserver":
                            options
                                .UseLoggerFactory(BurstChatContextLogger)
                                .UseSqlServer(databaseOptions.ConnectionString, dbContextOptions =>
                                {
                                    dbContextOptions.MigrationsAssembly("BurstChat.Api");
                                });
                            break;
                        default:
                            break;
                    }
                });

            services
                .AddCors(options =>
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

            // services.AddSwaggerGen(config =>
            // {
            //     config.SwaggerDoc("v1", new Info { Title = "BurstChat API", Version = "v1" });
            // });
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
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            application.UseStaticFiles();
            application.UseRouting();
            application.UseCors("CorsPolicy");
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseEndpoints(endpoints => 
            {
                endpoints.MapControllers();
            });
        }
    }
}
