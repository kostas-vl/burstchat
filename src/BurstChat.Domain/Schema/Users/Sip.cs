namespace BurstChat.Domain.Schema.Users
{
    /// <summary>
    /// This class represents the sip configuration of a user.
    /// </summary>
    public class Sip
    {
        /// <summary>
        /// The id of the entry.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The sip username.
        /// </summary>
        public string Username { get; set; } = string.Empty;
    }
}
