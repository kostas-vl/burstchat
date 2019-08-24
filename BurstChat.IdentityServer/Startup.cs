using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurstChat.IdentityServer.Extensions;
using BurstChat.IdentityServer.Options;
using BurstChat.IdentityServer.Services;
using BurstChat.Shared.Context;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Services.BCryptService;
using BurstChat.Shared.Services.UserService;
using BurstChat.Shared.Services.ModelValidationService;
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

namespace BurstChat.IdentityServer
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        ///   This method will return the proper action that can configure any database context assosiated with the project
        ///   based on the configuration key provided.
        /// </summary>
        /// <param name="databaseContextName">The target database context that is also a configuration key</param>
        /// <returns>An action</returns>
        public Action<DbContextOptionsBuilder> ConfigureDatabaseContext(string databaseContextName)
        {
            var databaseOptions = new DatabaseOptions();

            Configuration
                .GetSection(databaseContextName)
                .Bind(databaseOptions);

            return (options) => 
            {
                switch (databaseOptions.Provider)
                {
                    case "sqlite":
                        options.UseSqlite(databaseOptions.ConnectionString, dbContextOptions =>
                        {
                            dbContextOptions.MigrationsAssembly(databaseOptions.MigrationsAssembly);
                        });
                        break;

                    case "sqlserver":
                        options.UseSqlServer(databaseOptions.ConnectionString, dbContextOptions =>
                        {
                            dbContextOptions.MigrationsAssembly(databaseOptions.MigrationsAssembly);
                        });
                        break;

                    default:
                        break;
                }
            };
        }

        /// <summary>
        ///   This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services collection to be used for the configuration</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IBCryptService, BCryptProvider>();

            services
                .AddScoped<IUserService, UserProvider>()
                .AddScoped<IProfileService, BurstChatProfileService>()
                .AddScoped<IResourceOwnerPasswordValidator, BurstChatResourceOwnerPasswordValidator>();

            services
                .AddTransient<IModelValidationService, ModelValidationProvider>();

            services
                .AddDbContext<BurstChatContext>(ConfigureDatabaseContext("BurstChat.Api"));

            services
                .AddIdentityServer()
                .AddBurstChatSigningCredentials(options => Configuration.GetSection("X509").Bind(options))
                .AddConfigurationStore(options => options.ConfigureDbContext = ConfigureDatabaseContext("IdentityServer.ConfigurationDbContext"))
                .AddOperationalStore(options => options.ConfigureDbContext = ConfigureDatabaseContext("IdentityServer.PersistedGrantDbContext"));

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

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                application.UseBurstChatDevelopmentResources(options => Configuration.GetSection("Secrets").Bind(options));
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
        }
    }
}
