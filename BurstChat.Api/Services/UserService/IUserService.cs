using System;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Users;

namespace BurstChat.Api.Services.UserService
{
    /// <summary>
    /// This interface represents a contract for all services that need to provide and manipulate user data.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        ///   Registers a new user based on the provided instance.
        /// </summary>
        /// <param name="user">The user instance to be registered</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Insert(User user);

        /// <summary>
        ///   Updates infomation about an existing user based on the user instance provided.
        ///   This method will update only non security specific information.
        ///   If the user needs to alter his existing password the ChangePassword method needs to be used.
        /// </summary>
        /// <param name="user">The user instance to be updated in the database</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Update(User user);

        /// <summary>
        ///   This method return a User instance based on the id provided.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>An either monad of a User instance or an Error instance</returns>
        Either<User, Error> Select(long id);

        /// <summary>
        ///   This method fetches the instance of a registered user based on the provided email.
        /// </summary>
        /// <param name="email">The email that will be used for the search</param>
        /// <returns>An either monad</returns>
        Either<User, Error> SelectByEmail(string email);

        /// <summary>
        ///   Deletes a registered user from the database based on the provided user instance.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> Delete(long id);

        /// <summary>
        ///   This method will validate the provided email and password in order to
        ///   find any user that has registered these credentials.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>An either monad</returns>
        Either<User, Error> Validate(string email, string password);

        /// <summary>
        ///   Changes the current hashed password of the user to the one provided.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <param name="password">The string value of the password that will be hashed</param>
        /// <returns>An either monad</returns>
        Either<Unit, Error> ChangePassword(long id, string password);
    }
}
