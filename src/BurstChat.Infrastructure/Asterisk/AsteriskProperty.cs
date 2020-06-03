namespace BurstChat.Infrastructure.Asterisk
{
    /// <summary>
    /// This class represents a property that can be returned by calling the Asterisk ARI
    /// for a pjsip endpoint, aor and auth configuration.
    /// </summary>
    public class AsteriskProperty
    {
        /// <summary>
        /// The name of the configuration property.
        /// </summary>
        public string Attribute { get; set; } = string.Empty;

        /// <summary>
        /// The value of the configuration property.
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}
