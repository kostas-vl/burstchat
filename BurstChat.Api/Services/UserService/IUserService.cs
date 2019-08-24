using System;
using System.Collections.Generic;
using System.Security.Claims;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Api.Services.UserService
{
    /// <summary>
    /// This interface represents a contract for all services that need to provide and manipulate user data.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        ///   This method returns a User instance if the provided id belongs to one.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>An either monad of a User instance or an Error instance</returns>
        Either<User, Error> Get(long id);

        /// <summary>
        ///   This method returns a User instance if the provided email belongs to one.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>An either monad</returns>
        Either<User, Error> Get(string email);

        /// <summary>
        ///   Updates infomation about an existing user based on the user instance provided.
        ///   This method will update only non security specific information.
        ///   If the user needs to alter his existing password the ChangePassword method needs to be used.
        /// </summary>
        /// <param name="user">The user instance to be updated in the database</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Update(User user);

        /// <summary>
        ///   Deletes a registered user from the database based on the provided user instance.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Delete(long id);

        /// <summary>
        ///   This method will return all available subscribed servers of a user.
        /// <summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An either monad</returns>
        Either<IEnumerable<Server>, Error> GetSubscribedServers(long userId);

        /// <summary>
        ///   This method will return all private groups that the user with the provided user id
        ///   is part of.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An either monad</returns>
        Either<IEnumerable<PrivateGroupMessage>, Error> GetPrivateGroups(long userId);
   }
}
