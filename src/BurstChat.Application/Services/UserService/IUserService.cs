using System;
using System.Collections.Generic;
using System.Security.Claims;
using BurstChat.Application.Monads;
using BurstChat.Application.Models;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;

namespace BurstChat.Application.Services.UserService;

/// <summary>
/// This interface represents a contract for all services that need to provide and manipulate user data.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// This method returns a User instance if the provided id belongs to one.
    /// </summary>
    /// <param name="id">The id of the user</param>
    /// <returns>An either monad of a User instance or an Error instance</returns>
    Result<User> Get(long id);

    /// <summary>
    /// This method returns a User instance if the provided email belongs to one.
    /// </summary>
    /// <param name="email">The email of the user</param>
    /// <returns>An either monad</returns>
    Result<User> Get(string email);

    /// <summary>
    /// Registers a new user based on the provided parameters.
    /// </summary>
    /// <param name="alphaInvitationCode">The alpha invitation code</param>
    /// <param name="email">The email of the new user</param>
    /// <param name="name">The name of the new user</param>
    /// <param name="password">The password of the new user</param>
    /// <returns>An either monad</returns>
    Result<User> Insert(Guid alphaInvitationCode, string email, string name, string password);

    /// <summary>
    /// Updates infomation about an existing user based on the user instance provided.
    /// This method will update only non security specific information.
    /// If the user needs to alter his existing password the ChangePassword method needs to be used.
    /// </summary>
    /// <param name="user">The user instance to be updated in the database</param>
    /// <returns>An either monad</returns>
    Result<User> Update(User user);

    /// <summary>
    /// Deletes a registered user from the database based on the provided user id.
    /// </summary>
    /// <param name="id">The id of the user</param>
    /// <returns>An either monad</returns>
    Result<Unit> Delete(long id);

    /// <summary>
    /// This method will return all available subscribed servers of a user.
    /// <summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>An either monad</returns>
    Result<IEnumerable<Server>> GetSubscriptions(long userId);

    /// <summary>
    /// This method will return all private groups that the user is part of.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>An either monad</returns>
    Result<IEnumerable<PrivateGroup>> GetPrivateGroups(long userId);

    /// <summary>
    /// This method will return all direct messaging that the user is part of.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>An either monad</returns>
    Result<IEnumerable<DirectMessaging>> GetDirectMessaging(long userId);

    /// <summary>
    /// This method will validate the provided email and password in order to
    /// find any user that has registered these credentials.
    /// </summary>
    /// <param name="email">The email of the user</param>
    /// <param name="password">The password of the user</param>
    /// <returns>An either monad</returns>
    Result<User> Validate(string email, string password);

    /// <summary>
    /// This method will validate the provided email and if a user is registered with it
    /// a new one time password will be generated for him and sent via email.
    /// </summary>
    /// <param name="email">The email of the user</param>
    /// <returns>An either monad</returns>
    Result<string> IssueOneTimePassword(string email);

    /// <summary>
    /// Changes the current hashed password of the user to the one provided.
    /// </summary>
    /// <param name="email">The email of the user</param>
    /// <param name="oneTimePass">The one time password of the user</param>
    /// <param name="password">The string value of the password that will be hashed</param>
    /// <returns>An either monad</returns>
    Result<Unit> ChangePassword(string email, string oneTimePass, string password);

    /// <summary>
    /// Fetches all invitations sent to a user based on the provided id.
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>An either monad</returns>
    Result<IEnumerable<Invitation>> GetInvitations(long userId);

    /// <summary>
    /// This method will updated the Accepted or Declined propery of an existing invitation based on the instance
    /// provided.
    /// </summary>
    /// <param name="userId">The user id that updated the invitation</param>
    /// <param name="data">The server invitation to be updated</param>
    /// <returns>An either monad</returns>
    Result<Invitation> UpdateInvitation(long userId, UpdateInvitation data);

    /// <summary>
    /// This method will fetch all appropriate user claims based on the provided instance.
    /// </summary>
    /// <param name="user">The user instance</param>
    /// <returns>An either monad</returns>
    Result<IEnumerable<Claim>> GetClaims(User user);
}
