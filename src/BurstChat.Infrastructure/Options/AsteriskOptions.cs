using System;

namespace BurstChat.Infrastructure.Options
{
    /// <summary>
    /// Contains configuration properties for communicating with an Asterisk instance.
    public class AsteriskOptions
    {
        /// <summary>
        /// The domain to which the Asterisk instance is accessible.
        /// </summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// The username with which a remote request will be executed.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The password with which a remote request will be executed.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
