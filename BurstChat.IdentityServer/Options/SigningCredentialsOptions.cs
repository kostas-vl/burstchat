using System;

namespace BurstChat.IdentityServer.Options
{
    /// <summary>
    ///   This class represents various options that can be configured for the certificate used by the Identity Server
    ///   Signing Credentials store.
    /// </summary>
    public class SigningCredentialsOptions
    {
        /// <summary>
        ///   The path to the certificate file.
        /// </summary>
        public string Path
        {
            get; set;
        }

        /// <summary>
        ///   The password for the certificate.
        /// </summary>
        public string Password
        {
            get; set;
        }
    }
}