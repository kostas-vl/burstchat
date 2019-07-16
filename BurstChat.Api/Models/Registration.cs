using System;

namespace BurstChat.Api.Models
{
    /// <summary>
    ///   This class contains all properties required for the registration of a new user.
    /// </summary>
    public class Registration
    {
        /// <summary>
        ///   The email of the new user. It will be also his username for any authentication
        ///   process.
        /// </summary>
        public string Email
        {
            get; set;
        }

        /// <summary>
        ///   The password of the new user.
        /// </summary>
        public string Password
        {
            get; set;
        }

        /// <summary>
        ///   The password confirmation to be used in the validation of the registration process.
        /// </summary>
        public string ConfirmPassword
        {
            get; set;
        }
    }
}
