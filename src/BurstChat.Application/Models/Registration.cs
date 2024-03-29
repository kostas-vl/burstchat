using System;

namespace BurstChat.Application.Models
{
    /// <summary>
    /// This class contains all properties required for the registration of a new user.
    /// </summary>
    public class Registration
    {
        /// <summary>
        /// The Alpha invitation code to enable the registration process.
        /// </summary>
        public Guid AlphaInvitationCode { get; set; } = Guid.Empty;

        /// <summary>
        /// The name that is displayed on clients.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The email of the new user. It will be also his username for any authentication
        /// process.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The password of the new user.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The password confirmation to be used in the validation of the registration process.
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
