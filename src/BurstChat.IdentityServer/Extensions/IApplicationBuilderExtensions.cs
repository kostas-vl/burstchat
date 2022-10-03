using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Infrastructure.Options;
using BurstChat.Infrastructure.Persistence;
using BurstChat.IdentityServer.Options;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;

using Client = IdentityServer4.EntityFramework.Entities.Client;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;
using ApiScope = IdentityServer4.EntityFramework.Entities.ApiScope;
using IdentityResource = IdentityServer4.EntityFramework.Entities.IdentityResource;
using BurstChat.Domain.Schema.Alpha;

namespace BurstChat.IdentityServer.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        /// This method creates a Client instance for the web application of BurstChat, with all
        /// the neccessary configuration.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        /// <param name="configuration">The application settings configuration</param>
        private static void AddDevelopmentWebClient(ConfigurationDbContext? context, IdentitySecretsOptions identitySecretsOptions)
        {
            if (context == null) return;

            var creationDate = DateTime.UtcNow;
            var webClientId = "burstchat.web.client";
            var webClientSecret = identitySecretsOptions
                .ClientSecrets[webClientId];

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
                    new()
                    {
                        Value = webClientSecret.Sha256(),
                        Description = "The secret of the BurstChat Web Client",
                        Created = creationDate
                    }
                };

            webClient
                .AllowedScopes = new List<ClientScope>
                {
                    new() { Scope = "openid" },
                    new() { Scope = "profile" },
                    new() { Scope = "burstchat.api" },
                    new() { Scope = "burstchat.signal" },
                    new() { Scope = "offline_access" }
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
        }

        /// <summary>
        /// This method creates an Api Resource for Burst Chat api with the neccessary configuration.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        /// <param name="configuration">The application settings configuration</param>
        public static void AddDevelopmentApiResource(ConfigurationDbContext? context, IdentitySecretsOptions identitySecretsOptions)
        {
            if (context == null) return;

            var creationDate = DateTime.UtcNow;
            var apiName = "burstchat.api";
            var apiSecret = identitySecretsOptions
                .ApiSecrets[apiName];

            var apiResource = new ApiResource
            {
                Created = creationDate,
                Name = apiName,
                Description = "The BurstChat API",
                Enabled = true,
            };

            apiResource
                .Secrets = new List<ApiResourceSecret>
                {
                    new()
                    {
                        Value = apiSecret.Sha256(),
                        Created = creationDate,
                    }
                };

            apiResource
                .Scopes = new List<ApiResourceScope>
                {
                    new()
                    {
                        Scope = apiName,
                    }
                };

            context
                .ApiResources
                .Add(apiResource);
        }

        /// <summary>
        /// This method creates an API Resource for the BurstChat Signal server with all the neccessary configuration.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        /// <param name="configuration">The application settings configuration</param>
        public static void AddDevelopmentSignalResource(ConfigurationDbContext? context, IdentitySecretsOptions identitySecretsOptions)
        {
            if (context == null) return;

            var creationDate = DateTime.UtcNow;
            var signalName = "burstchat.signal";
            var signalSecret = identitySecretsOptions
                .ApiSecrets[signalName];

            var signalResource = new ApiResource
            {
                Created = creationDate,
                Name = signalName,
                Description = "The BurstChat Signal Server",
                Enabled = true
            };

            signalResource
                .Secrets = new List<ApiResourceSecret>
                {
                    new()
                    {
                        Value = signalSecret.Sha256(),
                        Created = creationDate,
                    }
                };

            signalResource
                .Scopes = new List<ApiResourceScope>
                {
                    new()
                    {
                        Scope = signalName,
                    }
                };

            context
                .ApiResources
                .Add(signalResource);
        }

        /// <summary>
        /// This method creates the ApiScopes for the Api and Signal projects.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        private static void AddDevelopmeApiScopes(ConfigurationDbContext? context)
        {
            if (context == null) return;

            var scopes = new List<ApiScope>
            {
                new() { Name = "burstchat.api", DisplayName = "The BurstChat Api scope" },
                new() { Name = "burstchat.signal", DisplayName = "The BurstChat Signal scope" }
            };

            context
                .ApiScopes
                .AddRange(scopes);
        }

        /// <summary>
        /// This method creates the IdentityResource for the openid and profile scopes with all the neccessary configuration.
        /// </summary>
        /// <param name="context">The configuration database context</param>
        private static void AddDevelopmentIdentityResources(ConfigurationDbContext? context)
        {
            if (context == null) return;

            var creationDate = DateTime.UtcNow;
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
                .AddRange(new [] { openIdResource, profileResource });
        }

        /// <summary>
        /// This method will make all the neccessary changes to the identity database contextes in order to
        /// ensure the existance of clients and api configuration.
        /// </summary>
        /// <param name="application">The application builder instance</param>
        /// <param name="secretsCallback">The callback that will populate the identity secrets options</param>
        public static void UseBurstChatDevelopmentResources(this IApplicationBuilder application, Action<IdentitySecretsOptions> secretsCallback)
        {
            var identitySecretsOptions = new IdentitySecretsOptions();
            secretsCallback(identitySecretsOptions);

            var serviceScopeFactory = application
                .ApplicationServices
                .GetService<IServiceScopeFactory>();

            using var serviceScope = serviceScopeFactory?.CreateScope();
            var creationDate = DateTime.UtcNow;

            serviceScope?
                .ServiceProvider
                .GetRequiredService<PersistedGrantDbContext>()
                .Database
                .Migrate();

            var context = serviceScope?
                .ServiceProvider
                .GetRequiredService<ConfigurationDbContext>();

            context?.Database.Migrate();

            var clients = context?
                .Clients
                .ToList()
                ?? Enumerable.Empty<Client>();

            var apis = context?
                .ApiResources
                .ToList()
                ?? Enumerable.Empty<ApiResource>();

            var scopes = context?
                .ApiScopes
                .ToList()
                ?? Enumerable.Empty<ApiScope>();

            var identities = context?
                .IdentityResources
                .ToList()
                ?? Enumerable.Empty<IdentityResource>();

            foreach (var client in clients)
                context?.Clients.Remove(client);

            foreach (var api in apis)
                context?.ApiResources.Remove(api);

            foreach (var scope in scopes)
                context?.ApiScopes.Remove(scope);

            foreach (var identity in identities)
                context?.IdentityResources.Remove(identity);

            AddDevelopmentWebClient(context, identitySecretsOptions);
            AddDevelopmentApiResource(context, identitySecretsOptions);
            AddDevelopmentSignalResource(context, identitySecretsOptions);
            AddDevelopmeApiScopes(context);
            AddDevelopmentIdentityResources(context);

            context?.SaveChanges();
        }

        /// <summary>
        /// Register alpha invitation codes based on the configuration action provided.
        /// </summary>
        /// <param name="application">The application builder instance</param>
        /// <param name="alphaInvitationsCallback">The invitations configuration callback</param>
        public static void UseAlphaInvitationCodes(this IApplicationBuilder application, Action<AlphaInvitationCodesOptions> alphaInvitationsCallback)
        {
            var options = new AlphaInvitationCodesOptions();
            alphaInvitationsCallback(options);

            if (options?.AlphaCodes is not null)
            {
                // Npgsql 6 introduces changes to datetimes with timezones so each datetimes
                // will be passed as a UTC datetime.
                foreach (var code in options.AlphaCodes)
                {
                    code.DateCreated = code.DateCreated.ToUniversalTime();
                    code.DateExpired = code.DateExpired.ToUniversalTime();
                }

                var serviceScopeFactory = application
                    .ApplicationServices
                    .GetService<IServiceScopeFactory>();

                using var serviceScope = serviceScopeFactory?.CreateScope();

                var context = serviceScope?
                    .ServiceProvider
                    .GetRequiredService<BurstChatContext>();

                var alphaInvitationCodes = context?
                    .AlphaInvitations
                    .ToList();

                foreach (var code in alphaInvitationCodes ?? Enumerable.Empty<AlphaInvitation>())
                    context?.AlphaInvitations.Remove(code);

                context?.AlphaInvitations.AddRange(options.AlphaCodes);

                context?.SaveChanges();
            }
        }
    }
}
