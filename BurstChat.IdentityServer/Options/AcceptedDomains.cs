using System;
using System.Collections.Generic;

namespace BurstChat.IdentityServer.Options
{
    public class AcceptedDomainsOptions
    {
        /// <summary>
        ///   The various domains accepted by the cors policy.
        /// </summary>
        public IEnumerable<string> Cors { get; set; } = new string[0];
    }
}
