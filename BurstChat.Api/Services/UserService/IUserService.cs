using System;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Api.Services.UserService
{
    /// <summary>
    /// This interface represents a contract for all services that need to provide and manipulate user data.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// This method return a User instance based on the id provided.
        /// </summary>
        User Get(long id);
    }
}
