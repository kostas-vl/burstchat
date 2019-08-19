using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurstChat.Api.Extensions;
using BurstChat.Api.Options;
using BurstChat.Api.Services.BCryptService;
using BurstChat.Api.Services.ChannelsService;
using BurstChat.Api.Services.IdentityServices;
using BurstChat.Api.Services.ModelValidationService;
using BurstChat.Api.Services.PrivateGroupMessaging;
using BurstChat.Api.Services.ServersService;
using BurstChat.Api.Services.UserService;
using BurstChat.Shared.Context;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace BurstChat.Api
{
    public class Startup
    {
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
        ///   Executes the neccessary configuration code for the BurstChat database context.
        /// </summary>
        /// <param name="options">The BurstChat database context options</param>
        private void ConfigureDatabaseContext(DbContextOptionsBuilder options)
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
        }

        /// <summary>
        ///   This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services collection to be used for the configuration</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<DatabaseOptions>(Configuration.GetSection("Database"))
                .Configure<AcceptedDomainsOptions>(Configuration.GetSection("AcceptedDomains"));

            services
                .AddSingleton<IBCryptService, BCryptProvider>();

            services
                .AddScoped<IChannelsService, ChannelsProvider>()
                .AddScoped<IPrivateGroupMessagingService, PrivateGroupMessagingProvider>()
                .AddScoped<IServersService, ServersProvider>()
                .AddScoped<IUserService, UserProvider>()
                .AddScoped<IProfileService, BurstChatProfileService>()
                .AddScoped<IResourceOwnerPasswordValidator, BurstChatResourceOwnerPasswordValidator>();

            services
                .AddTransient<IModelValidationService, ModelValidationProvider>();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddDbContext<BurstChatContext>(ConfigureDatabaseContext);

            services
                .AddIdentityServer()
                .AddBurstChatSigningCredentials(options =>
                {
                    var x509Configuration = Configuration
                        .GetSection("X509");

                    var path = x509Configuration
                        .GetValue<string>("Path");

                    var password = x509Configuration
                        .GetValue<string>("Password");

                    options.Path = path;
                    options.Password = password;
                })
                .AddConfigurationStore(options => options.ConfigureDbContext = ConfigureDatabaseContext)
                .AddOperationalStore(options => options.ConfigureDbContext = ConfigureDatabaseContext);

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

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Info { Title = "BurstChat API", Version = "v1" });
            });
        }

        /// <summary>
        ///   This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="application">The application builder to be used in the configuration</param>
        /// <param name="env">The hosting environment that the application is running</param>
        public void Configure(IApplicationBuilder application, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
                application.UseBurstChatDevelopmentResources(options =>
                {
                    Configuration
                        .GetSection("Secrets")
                        .Bind(options);
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            application.UseIdentityServer();

            application
                .UseCors("CorsPolicy")
                .UseMvc();
            
            application.UseSwagger();

            application.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "BurstChat V1");
            });

            application.UseStaticFiles();
        }
    }
}
