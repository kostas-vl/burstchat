using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using BurstChat.IdentityServer.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BurstChat.IdentityServer.Extensions
{
    /// <summary>
    ///   This class extends an IIdentityServerBuilder instance with new methods.
    /// </summary>
    public static class IIdentityServerBuilderExtensions
    {
        /// <summary>
        ///   This method will extend the IIdentityServerBuilder instance by adding the proper signing credentials for the
        ///   identity server based on a specific certificate file.
        /// </summary>
        /// <param name="identityServerBuilder">The identity server builder instance</param>
        /// <returns>The modified identity server builder instance</returns>
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
}
