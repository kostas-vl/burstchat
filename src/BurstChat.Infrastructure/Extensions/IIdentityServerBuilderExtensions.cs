using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using BurstChat.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BurstChat.Infrastructure.Extensions;

public static class IIdentityServerBuilderExtensions
{
    public static IIdentityServerBuilder AddBurstChatSigningCredentials(this IIdentityServerBuilder identityServerBuilder, Action<SigningCredentialsOptions> callback)
    {
        var options = new SigningCredentialsOptions();
        callback(options);

        var certificateData = File.ReadAllBytes(options.Path);
        var x509 = new X509Certificate2(certificateData, options.Password);

        identityServerBuilder.AddSigningCredential(x509);

        return identityServerBuilder;
    }

}
