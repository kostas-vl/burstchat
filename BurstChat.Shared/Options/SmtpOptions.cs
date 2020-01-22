namespace BurstChat.Shared.Options
{
    /// <summary>
    /// This class contains all properties necessary for connecting to a remote smtp server.
    /// </summary>
    public class SmtpOptions
    {
        /// <summary>
        /// The host address of the target smtp server.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The port of the target smtp server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The username with which the connection to the server will be established.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The password with which the connection to the server will be established.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The email address of the sender.
        /// </summary>
        public string Sender { get; set; } = string.Empty;
    }
}