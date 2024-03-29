namespace BurstChat.Application.Models
{
    /// <summary>
    /// This class contains properties for representing a user name and password of a user.
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// The email of the user that is also his username.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
