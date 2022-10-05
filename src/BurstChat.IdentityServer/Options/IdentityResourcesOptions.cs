using System.Collections.Generic;

namespace BurstChat.IdentityServer.Options
{
    /// <summary>
    /// This class contains all options that can be provided for various secrets about the available clients
    /// and apis for the BurstChat Identity Server.
    /// </summary>
    public class IdentityResourcesOptions
    {
        /// <summary>
        /// Contains a dictionary with the clients and their secret.
        /// </summary>
        public IDictionary<string, string> ClientSecrets { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Contains a dictionary with the clients and all of their redirect uris.
        /// </summary>
        public IDictionary<string, IEnumerable<string>> ClientRedirectUris { get; set; } = new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// Contains a dictionary with the clients and all of their cors origins.
        /// </summary>
        public IDictionary<string, IEnumerable<string>> ClientCorsOrigins { get; set; } = new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// Contains a dictionary with the clients and all of their post logout redirect uris.
        public IDictionary<string, IEnumerable<string>> ClientPostLogoutRedirectUris { get; set; } = new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// Contains a dictionary with the apis and their secret.
        /// </summary>
        public IDictionary<string, string> ApiSecrets { get; set; } = new Dictionary<string, string>();
    }
}
