using System;
using System.Collections.Generic;

namespace BurstChat.Signal.Options
{
    /// <summary>
    ///   Contains all settings associated with domains that can interact with the SignalR application.
    /// </summary>
    public class AcceptedDomainsOptions
    {
        /// <summary>
        ///   The accepted cors domains.
        /// </summary>
        public IEnumerable<string> Cors { get; set; } = new string[0];

        /// <summary>
        ///   The burst chat api domain url.
        /// </summary>
        public string BurstChatApiDomain { get; set; } = string.Empty;
    }
}
