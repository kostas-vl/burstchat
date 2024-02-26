using System;
using System.Collections.Generic;
using System.Linq;
using BurstChat.Domain.Schema.Alpha;
using BurstChat.IdentityServer.Options;
using BurstChat.Infrastructure.Options;
using BurstChat.Infrastructure.Persistence;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;
using ApiScope = IdentityServer4.EntityFramework.Entities.ApiScope;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using IdentityResource = IdentityServer4.EntityFramework.Entities.IdentityResource;

namespace BurstChat.IdentityServer.Extensions;

public static class IApplicationBuilderExtensions
{
    private static void AddDevelopmentWebClient(
        ConfigurationDbContext? context,
        IdentityResourcesOptions identityResourcesOptions
    )
    {
        if (context == null)
            return;

        var creationDate = DateTime.UtcNow;
        var webClientId = "burstchat.web.client";

        identityResourcesOptions.ClientSecrets.TryGetValue(webClientId, out var webClientSecret);

        identityResourcesOptions.ClientRedirectUris.TryGetValue(
            webClientId,
            out var webClientRedirectUris
        );

        identityResourcesOptions.ClientCorsOrigins.TryGetValue(
            webClientId,
            out var webClientCorsOrigins
        );

        identityResourcesOptions.ClientPostLogoutRedirectUris.TryGetValue(
            webClientId,
            out var webClientPostLogoutRedirectUris
        );

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

        webClient.ClientSecrets = new List<ClientSecret>
        {
            new()
            {
                Value = webClientSecret.Sha256(),
                Description = "The secret of the BurstChat Web Client",
                Created = creationDate
            }
        };

        webClient.AllowedScopes = new List<ClientScope>
        {
            new() { Scope = "openid" },
            new() { Scope = "profile" },
            new() { Scope = "burstchat.api" },
            new() { Scope = "burstchat.signal" },
            new() { Scope = "offline_access" }
        };

        webClient.AllowedCorsOrigins = (webClientCorsOrigins ?? Enumerable.Empty<string>())
            .Select(entry => new ClientCorsOrigin { Origin = entry })
            .ToList();

        webClient.AllowedGrantTypes = new List<ClientGrantType>
        {
            new ClientGrantType { GrantType = "password" }
        };

        webClient.RedirectUris = (webClientRedirectUris ?? Enumerable.Empty<string>())
            .Select(entry => new ClientRedirectUri { RedirectUri = entry })
            .ToList();

        webClient.PostLogoutRedirectUris = (
            webClientPostLogoutRedirectUris ?? Enumerable.Empty<string>()
        )
            .Select(entry => new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = entry })
            .ToList();

        context.Clients.Add(webClient);
    }

    public static void AddDevelopmentApiResource(
        ConfigurationDbContext? context,
        IdentityResourcesOptions identityResourcesOptions
    )
    {
        if (context == null)
            return;

        var creationDate = DateTime.UtcNow;
        var apiName = "burstchat.api";
        var apiSecret = identityResourcesOptions.ApiSecrets[apiName];

        var apiResource = new ApiResource
        {
            Created = creationDate,
            Name = apiName,
            Description = "The BurstChat API",
            Enabled = true,
        };

        apiResource.Secrets = new List<ApiResourceSecret>
        {
            new() { Value = apiSecret.Sha256(), Created = creationDate, }
        };

        apiResource.Scopes = new List<ApiResourceScope> { new() { Scope = apiName, } };

        context.ApiResources.Add(apiResource);
    }

    public static void AddDevelopmentSignalResource(
        ConfigurationDbContext? context,
        IdentityResourcesOptions identityResourcesOptions
    )
    {
        if (context == null)
            return;

        var creationDate = DateTime.UtcNow;
        var signalName = "burstchat.signal";
        var signalSecret = identityResourcesOptions.ApiSecrets[signalName];

        var signalResource = new ApiResource
        {
            Created = creationDate,
            Name = signalName,
            Description = "The BurstChat Signal Server",
            Enabled = true
        };

        signalResource.Secrets = new List<ApiResourceSecret>
        {
            new() { Value = signalSecret.Sha256(), Created = creationDate, }
        };

        signalResource.Scopes = new List<ApiResourceScope> { new() { Scope = signalName, } };

        context.ApiResources.Add(signalResource);
    }

    private static void AddDevelopmeApiScopes(ConfigurationDbContext? context)
    {
        if (context == null)
            return;

        var scopes = new List<ApiScope>
        {
            new() { Name = "burstchat.api", DisplayName = "The BurstChat Api scope" },
            new() { Name = "burstchat.signal", DisplayName = "The BurstChat Signal scope" }
        };

        context.ApiScopes.AddRange(scopes);
    }

    private static void AddDevelopmentIdentityResources(ConfigurationDbContext? context)
    {
        if (context == null)
            return;

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

        context.IdentityResources.AddRange(new[] { openIdResource, profileResource });
    }

    public static void UseBurstChatDevelopmentResources(
        this IApplicationBuilder application,
        Action<IdentityResourcesOptions> secretsCallback
    )
    {
        var identityResourcesOptions = new IdentityResourcesOptions();
        secretsCallback(identityResourcesOptions);

        var serviceScopeFactory =
            application.ApplicationServices.GetService<IServiceScopeFactory>();

        using var serviceScope = serviceScopeFactory?.CreateScope();
        var creationDate = DateTime.UtcNow;

        serviceScope
            ?.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
            .Database.Migrate();

        var context = serviceScope?.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        context?.Database.Migrate();

        var clients = context?.Clients.ToList() ?? Enumerable.Empty<Client>();

        var apis = context?.ApiResources.ToList() ?? Enumerable.Empty<ApiResource>();

        var scopes = context?.ApiScopes.ToList() ?? Enumerable.Empty<ApiScope>();

        var identities =
            context?.IdentityResources.ToList() ?? Enumerable.Empty<IdentityResource>();

        foreach (var client in clients)
            context?.Clients.Remove(client);

        foreach (var api in apis)
            context?.ApiResources.Remove(api);

        foreach (var scope in scopes)
            context?.ApiScopes.Remove(scope);

        foreach (var identity in identities)
            context?.IdentityResources.Remove(identity);

        AddDevelopmentWebClient(context, identityResourcesOptions);
        AddDevelopmentApiResource(context, identityResourcesOptions);
        AddDevelopmentSignalResource(context, identityResourcesOptions);
        AddDevelopmeApiScopes(context);
        AddDevelopmentIdentityResources(context);

        context?.SaveChanges();
    }

    public static void UseAlphaInvitationCodes(
        this IApplicationBuilder application,
        Action<AlphaInvitationCodesOptions> alphaInvitationsCallback
    )
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

            var serviceScopeFactory =
                application.ApplicationServices.GetService<IServiceScopeFactory>();

            using var serviceScope = serviceScopeFactory?.CreateScope();

            var context = serviceScope?.ServiceProvider.GetRequiredService<BurstChatContext>();

            var alphaInvitationCodes = context?.AlphaInvitations.ToList();

            foreach (var code in alphaInvitationCodes ?? Enumerable.Empty<AlphaInvitation>())
                context?.AlphaInvitations.Remove(code);

            context?.AlphaInvitations.AddRange(options.AlphaCodes);

            context?.SaveChanges();
        }
    }
}
