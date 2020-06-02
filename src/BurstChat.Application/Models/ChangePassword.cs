using System;

namespace BurstChat.Application.Models
{
    /// <summary>
    /// This class contains properties for change password operation of a user.
    /// </summary>
    public class ChangePassword
    {
        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The one time password provided to the user for the operation.
        /// </summary>
        public string OneTimePassword { get; set; } = string.Empty;

        /// <summary>
        /// The new password of the user.
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// The confirmation of the new password.
        /// </summary>
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
