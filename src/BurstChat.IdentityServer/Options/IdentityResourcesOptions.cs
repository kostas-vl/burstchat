using System.Collections.Generic;

namespace BurstChat.IdentityServer.Options;

public class IdentityResourcesOptions
{
    public IDictionary<string, string> ClientSecrets { get; set; } =
        new Dictionary<string, string>();

    public IDictionary<string, IEnumerable<string>> ClientRedirectUris { get; set; } =
        new Dictionary<string, IEnumerable<string>>();

    public IDictionary<string, IEnumerable<string>> ClientCorsOrigins { get; set; } =
        new Dictionary<string, IEnumerable<string>>();

    public IDictionary<string, IEnumerable<string>> ClientPostLogoutRedirectUris { get; set; } =
        new Dictionary<string, IEnumerable<string>>();

    public IDictionary<string, string> ApiSecrets { get; set; } = new Dictionary<string, string>();
}
