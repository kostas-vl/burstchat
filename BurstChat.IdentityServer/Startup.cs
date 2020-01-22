using System;
using System.Linq;
using BurstChat.IdentityServer.Extensions;
using BurstChat.IdentityServer.Options;
using BurstChat.IdentityServer.Services;
using BurstChat.Shared.Context;
using BurstChat.Shared.Services.BCryptService;
using BurstChat.Shared.Services.UserService;
using BurstChat.Shared.Services.ModelValidationService;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BurstChat.Shared.Services.EmailService;
using BurstChat.Shared.Options;

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
                    case "npgsql":
                        options.UseNpgsql(databaseOptions.ConnectionString, dbContextOptions =>
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
                .Configure<AcceptedDomainsOptions>(Configuration.GetSection("AcceptedDomains"))
                .Configure<SmtpOptions>(Configuration.GetSection("SmtpOptions"));

            services
                .AddSingleton<IBCryptService, BCryptProvider>()
                .AddSingleton<IEmailService, EmailProvider>();

            services
                .AddScoped<IUserService, UserProvider>()
                .AddScoped<IProfileService, BurstChatProfileService>()
                .AddScoped<IResourceOwnerPasswordValidator, BurstChatResourceOwnerPasswordValidator>();

            services
                .AddTransient<IModelValidationService, ModelValidationProvider>();

            services
                .AddDbContext<BurstChatContext>(ConfigureDatabaseContext("BurstChat.Api"));

            services
                .AddIdentityServer(options =>
                {
                    options.IssuerUri = "http://localhost:5002";
                    options.PublicOrigin = "http://localhost:5002";
                })
                .AddBurstChatSigningCredentials(options => Configuration.GetSection("X509").Bind(options))
                .AddConfigurationStore(options => options.ConfigureDbContext = ConfigureDatabaseContext("IdentityServer.ConfigurationDbContext"))
                .AddOperationalStore(options => options.ConfigureDbContext = ConfigureDatabaseContext("IdentityServer.PersistedGrantDbContext"));

            services
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    var acceptedDomains = Configuration
                        .GetSection("AcceptedDomains")
                        .Get<string[]>();
                    if (acceptedDomains != null && acceptedDomains.Count() > 0)
                    {
                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithOrigins(acceptedDomains);
                    }
                });
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
                application.UseBurstChatDevelopmentResources(options => Configuration.GetSection("Secrets").Bind(options));
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            application
                .UseRouting()
                .UseCors("CorsPolicy")
                .UseIdentityServer()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
