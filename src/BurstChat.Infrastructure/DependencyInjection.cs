using System;
using System.Linq;
using BurstChat.Application.Interfaces;
using BurstChat.Infrastructure.Extensions;
using BurstChat.Infrastructure.Options;
using BurstChat.Infrastructure.Persistence;
using BurstChat.Infrastructure.Services.AsteriskService;
using BurstChat.Infrastructure.Services.EmailService;
using BurstChat.Infrastructure.Services.ProfileService;
using BurstChat.Infrastructure.Services.ResourceOwnerPasswordValidator;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

namespace BurstChat.Infrastructure
{
    /// <summary>
    /// A static class that contains various extension methods for helping make dependency injection
    /// configurable with less code.
    /// </summary>
    public static class DependencyInjection
    {
        private static readonly string BurstChatMigrations = typeof(BurstChatContext).Assembly.FullName;

        private static readonly string ConfigurationDbMigrations = typeof(BurstChatContext).Assembly.FullName;

        private static readonly string PersistedGrantDbMigrations = typeof(BurstChatContext).Assembly.FullName;

        public static readonly string CorsPolicyName = "CorsPolicy";

        /// <summary>
        /// This method returns a configuration action for any database context
        /// used in the BurstChat applications.
        /// </summary>
        /// <param name="migrationsPath">The path to the migrations directory</param>
        /// <param name="options">The appropriate database options instance</param>
        /// <returns>A configuration action</returns>
        private static Action<DbContextOptionsBuilder> ConfigureDatabaseContext(string migrationsPath, IConfigurationSection section)
        {
            var databaseOptions = new DatabaseOptions();

            section.Bind(databaseOptions);

            return optionsBuilder =>
            {
                switch (databaseOptions.Provider)
                {
                    case "npgsql":
                        optionsBuilder
                            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                            .UseNpgsql(databaseOptions.ConnectionString, dbContextOptions =>
                            {
                                dbContextOptions.MigrationsAssembly(migrationsPath);
                            });
                        break;
                    default:
                        break;
                }
            };
        }

        /// <summary>
        /// Makes any neccessary configurations for the services of a BurstChat application.
        /// </summary>
        /// <param name="services">The aspnet core services collection instance</param>
        /// <param name="configuration">The aspnet core configuration instance</param>
        /// <returns>The modified services collection instance</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<DatabaseOptions>(configuration.GetSection("Database"))
                .Configure<AcceptedDomainsOptions>(configuration.GetSection("AcceptedDomains"));

            services
                .AddScoped<IAsteriskService>(injection =>
                {
                    var logger = injection
                        .GetRequiredService<ILogger<AsteriskProvider>>();
                    var options = configuration
                        .GetSection("Asterisk.Database")
                        .Get<DatabaseOptions>();
                    return new AsteriskProvider(logger, options);
                })
                .AddScoped<IBurstChatContext>(provider => provider.GetService<BurstChatContext>());

            services.AddAuthorization();

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options => configuration.GetSection("AccessTokenValidation").Bind(options));

            var dbBuilderCallback = ConfigureDatabaseContext(BurstChatMigrations, configuration.GetSection("Database"));

            services.AddDbContext<BurstChatContext>(dbBuilderCallback);

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    var acceptedDomains = configuration
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

            return services;
        }

        /// <summary>
        /// Makes any neccessary configurations for the services of the BurstChat identity server.
        /// </summary>
        /// <param name="services">The aspnet core services collection instance</param>
        /// <param name="configuration">The aspnet core configuration instance</param>
        /// <returns>The modified services collection instance</returns>
        public static IServiceCollection AddIdentityServerInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<AcceptedDomainsOptions>(configuration.GetSection("AcceptedDomains"))
                .Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"))
                .Configure<AlphaInvitationCodesOptions>(configuration.GetSection("AlphaCodes"));

            services.AddSingleton<IEmailService, EmailProvider>();

            services
                .AddScoped<IAsteriskService>(injection =>
                {
                    var logger = injection
                        .GetRequiredService<ILogger<AsteriskProvider>>();
                    var options = configuration
                        .GetSection("Asterisk.Database")
                        .Get<DatabaseOptions>();
                    return new AsteriskProvider(logger, options);
                })
                .AddScoped<IProfileService, BurstChatProfileService>()
                .AddScoped<IResourceOwnerPasswordValidator, BurstChatResourceOwnerPasswordValidator>()
                .AddScoped<IBurstChatContext>(provider => provider.GetService<BurstChatContext>());

            var dbBuilderCallback = ConfigureDatabaseContext(BurstChatMigrations, configuration.GetSection("Database"));

            services.AddDbContext<BurstChatContext>(dbBuilderCallback);

            services
                .AddIdentityServer(options =>
                {
                    var host = Environment
                        .GetEnvironmentVariable(EnvironmentVariables.BURST_CHAT_IDENTITY_HOST)
                        ?? "localhost";

                    var port = Environment
                        .GetEnvironmentVariable(EnvironmentVariables.BURST_CHAT_IDENTITY_PORT)
                        ?? "5002";

                    var domain = $"http://{host}:{port}";

                    options.IssuerUri = domain;
                })
                .AddBurstChatSigningCredentials(options => configuration.GetSection("X509").Bind(options))
                .AddConfigurationStore(options =>
                {
                    var path = ConfigurationDbMigrations;
                    var section = configuration.GetSection("IdentityServer.Database");
                    options.ConfigureDbContext = ConfigureDatabaseContext(path, section);
                })
                .AddOperationalStore(options =>
                {
                    var path = PersistedGrantDbMigrations;
                    var section = configuration.GetSection("IdentityServer.Database");
                    options.ConfigureDbContext = ConfigureDatabaseContext(path, section);
                });

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    var acceptedDomains = configuration
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

            return services;
        }
    }
}
