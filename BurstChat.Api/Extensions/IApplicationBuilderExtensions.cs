using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.Extensions.Configuration;
using IdentityServer4.Models;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;
using IdentityResource = IdentityServer4.EntityFramework.Entities.IdentityResource;

namespace BurstChat.Api.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        ///     This method creates a Client instance for the web application of BurstChat, with all
        ///     the neccessary configuration.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        /// <param name="configuration">The application settings configuration</param>
        private static void AddDevelopmentWebClient(ConfigurationDbContext context, IConfiguration configuration)
        {
            var creationDate = DateTime.Now;
            var webClientId = "burstchat.web.client";
            var webClientSecret = configuration
                .GetSection("ClientSecrets")
                .GetValue<string>(webClientId);

            var webClient = new Client
            {
                ClientName = "BurstChat Web Client",
                ClientId = webClientId,
                RequirePkce = false,
                AllowPlainTextPkce = false,
                AllowAccessTokensViaBrowser = true,
                AllowRememberConsent = true,
                AccessTokenType = (int)AccessTokenType.Jwt,
                AllowOfflineAccess = true,
                Created = creationDate
            };

            webClient
                .ClientSecrets = new List<ClientSecret>
                {
                    new ClientSecret
                    {
                        Value = webClientSecret.Sha256(),
                        Description = "The secret of the BurstChat Web Client",
                        Created = creationDate
                    }
                };

            webClient
                .AllowedScopes = new List<ClientScope>
                {
                    new ClientScope { Scope = "openid" },
                    new ClientScope { Scope = "profile" },
                    new ClientScope { Scope = "burstchat.api" },
                    new ClientScope { Scope = "burstchat.signal" }
                };

            webClient
                .AllowedCorsOrigins = new List<ClientCorsOrigin>
                {
                    new ClientCorsOrigin { Origin = "http://localhost:4200" }
                };

            webClient
                .AllowedGrantTypes = new List<ClientGrantType>
                {
                    new ClientGrantType { GrantType = "password" }
                };

            webClient
                .RedirectUris = new List<ClientRedirectUri>
                {
                    new ClientRedirectUri { RedirectUri = @"http://localhost:4200/core/chat" }
                };

            webClient
                .PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>
                {
                    new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = @"http://localhost:4200/session/login" }
                };

            context
                .Clients
                .Add(webClient);

            context.SaveChanges();
        }

        /// <summary>
        ///     This method creates an Api Resource for Burst Chat api with the neccessary configuration.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        /// <param name="configuration">The application settings configuration</param>
        public static void AddDevelopmentApiResource(ConfigurationDbContext context, IConfiguration configuration)
        {
            var creationDate = DateTime.Now;
            var apiName = "burstchat.api";
            var apiSecret = configuration
                .GetSection("ApiSecrets")
                .GetValue<string>(apiName);

            var apiResource = new ApiResource
            {
                Created = creationDate,
                Name = apiName,
                Description = "The BurstChat API",
                Enabled = true,
            };

            apiResource
                .Secrets = new List<ApiSecret>
                {
                    new ApiSecret
                    {
                        Value = apiSecret.Sha256(),
                        Created = creationDate,
                    }
                };

            apiResource
                .Scopes = new List<ApiScope>
                {
                    new ApiScope 
                    {
                        Name = apiName,
                        Description = "The BurstChat API",
                        DisplayName = "BurstChat APi",
                        ShowInDiscoveryDocument = true,
                        Required = true
                    }
                };

            context
                .ApiResources
                .Add(apiResource);

            context.SaveChanges();
        }

        /// <summary>
        ///     This method creates an API Resource for the BurstChat Signal server with all the neccessary configuration.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        /// <param name="configuration">The application settings configuration</param>
        public static void AddDevelopmentSignalResource(ConfigurationDbContext context, IConfiguration configuration)
        {
            var creationDate = DateTime.Now;
            var signalName = "burstchat.signal";
            var signalSecret = configuration
                .GetSection("ApiSecrets")
                .GetValue<string>(signalName);

            var signalResource = new ApiResource
            {
                Created = creationDate,
                Name = signalName,
                Description = "The BurstChat Signal Server",
                Enabled = true
            };

            signalResource
                .Secrets = new List<ApiSecret>
                {
                    new ApiSecret
                    {
                        Value = signalSecret.Sha256(),
                        Created = creationDate,
                    }
                };

            signalResource
                .Scopes = new List<ApiScope>
                {
                    new ApiScope 
                    {
                        Name = signalName,
                        Description = "The BurstChat Signal Server",
                        DisplayName = "BurstChat Signal Server",
                        ShowInDiscoveryDocument = true,
                        Required = true
                    }
                };

            context
                .ApiResources
                .Add(signalResource);

            context.SaveChanges();
        }

        /// <summary>
        ///     This method creates the IdentityResource for the openid and profile scopes with all the neccessary configuration.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        /// <param name="configuration">The application settings configuration</param>
        private static void AddDevelopmentIdentityResources(ConfigurationDbContext context, IConfiguration configuration)
        {
            var creationDate = DateTime.Now;
            var openIdResource = new IdentityResource
            {
                Name = "openid",
                Description = "openid",
                DisplayName = "Open Id",
                ShowInDiscoveryDocument = true,
                Created = creationDate
            };

            var profileResource = new IdentityResource
            {
                Name = "profile",
                Description = "profile",
                DisplayName = "User Profile",
                ShowInDiscoveryDocument = true,
                Created = creationDate
            };

            context
                .IdentityResources
                .AddRange(new []{ openIdResource, profileResource });

            context.SaveChanges();
        }

        /// <summary>
        ///     This method will make all the neccessary changes to the identity database contextes in order to
        ///     ensure the existance of clients and api configuration.
        /// </summary>
        /// <param name="application">The application builder instance</param>
        /// <param name="configuration">The application settings configuration</param>
        public static void UseBurstChatDevelopmentResources(this IApplicationBuilder application, IConfiguration configuration)
        {
            var serviceScopeFactory = application
                .ApplicationServices
                .GetService<IServiceScopeFactory>();

            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var creationDate = DateTime.Now;

                serviceScope
                    .ServiceProvider
                    .GetRequiredService<PersistedGrantDbContext>()
                    .Database
                    .Migrate();

                var context = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>();

                context
                    .Database
                    .Migrate();

                var clients = context
                    .Clients
                    .ToList();

                var apis = context
                    .ApiResources
                    .ToList();

                var identities = context
                    .IdentityResources
                    .ToList();

                foreach (var client in clients)
                    context.Clients.Remove(client);

                foreach (var api in apis)
                    context.ApiResources.Remove(api);

                foreach (var identity in identities)
                    context.IdentityResources.Remove(identity);

                context.SaveChanges();

                AddDevelopmentWebClient(context, configuration);
                AddDevelopmentApiResource(context, configuration);
                AddDevelopmentSignalResource(context, configuration);
                AddDevelopmentIdentityResources(context, configuration);   
            }
        }
    }
}
