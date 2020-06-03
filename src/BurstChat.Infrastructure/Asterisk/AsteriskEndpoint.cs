namespace BurstChat.Infrastructure.Asterisk
{
    /// <summary>
    /// This class contains information about an Asterisk pjsip endpoint, that
    /// can be used to connect to Asterisk using a websocket connection
    /// from a browser.
    /// </summary>
    public class AsteriskEndpoint
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
