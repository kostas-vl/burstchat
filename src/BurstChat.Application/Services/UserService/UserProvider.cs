using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using BurstChat.Application.Errors;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Services.BCryptService;
using BurstChat.Application.Monads;
using BurstChat.Application.Models;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.UserService
{
    /// <summary>
    /// This class is the base implementation of the IUserService.
    /// </summary>
    public class UserProvider : IUserService
    {
        private readonly ILogger<UserProvider> _logger;
        private readonly IBurstChatContext _burstChatContext;
        private readonly IBCryptService _bcryptService;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        ///
        /// Exceptions:
        ///     ArgumentNullException: When any of the parameters is null.
        /// </summary>
        public UserProvider(
            ILogger<UserProvider> logger,
            IBurstChatContext burstChatContext,
            IBCryptService bcryptService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _burstChatContext = burstChatContext ?? throw new ArgumentNullException(nameof(burstChatContext));
            _bcryptService = bcryptService ?? throw new ArgumentNullException(nameof(bcryptService));
        }

        /// <summary>
        /// Will check if the provided alpha invitation code exists in the database and is valid.
        /// </summary>
        /// <param name="alphaInvitationCode">The alpha invitation code instance</param>
        /// <returns>An either monad</returns>
        private Either<Unit, Error> AlphaInvitationCodeExists(Guid alphaInvitationCode)
        {
            try
            {
                var codeExists = _burstChatContext
                    .AlphaInvitations
                    .Any(a => a.Code == alphaInvitationCode
                              && a.DateExpired >= DateTime.UtcNow);

                return codeExists
                    ? new Success<Unit, Error>(new Unit())
                    : new Failure<Unit, Error>(AlphaInvitationErrors.AlphaInvitationCodeIsNotValid());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method returns a User instance if the provided id belongs to one.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>An either monad of a User instance or an Error instance</returns>
        public Either<User, Error> Get(long id)
        {
            try
            {
                _logger.LogInformation($"New user request with id: {id}");

                var user = _burstChatContext
                    .Users
                    .FirstOrDefault(u => u.Id == id);

                return user is { }
                    ? new Success<User, Error>(user)
                    : new Failure<User, Error>(UserErrors.UserNotFound());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<User, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method returns a User instance if the provided email belongs to one.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>An either monad</returns>
        public Either<User, Error> Get(string email)
        {
            try
            {
                var user = _burstChatContext
                    .Users
                    .FirstOrDefault(u => u.Email == email);

                return user is { }
                    ? new Success<User, Error>(user)
                    : new Failure<User, Error>(UserErrors.UserNotFound());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<User, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Registers a new user based on the provided parameters.
        /// </summary>
        /// <param name="alphaInvitationCode">The alpha invitation code</param>
        /// <param name="email">The email of the new user</param>
        /// <param name="name">The name of the new user</param>
        /// <param name="password">The password of the new user</param>
        /// <returns>An either monad</returns>
        public Either<User, Error> Insert(Guid alphaInvitationCode, string email, string name, string password)
        {
            try
            {
                if (Get(email) is Success<User, Error>)
                    return new Failure<User, Error>(UserErrors.UserAlreadyExists());

                return AlphaInvitationCodeExists(alphaInvitationCode).Bind(_ =>
                {
                    var hashedPassword = _bcryptService.GenerateHash(password);

                    var user = new User
                    {
                        Email = email,
                        Name = name,
                        Password = hashedPassword,
                        DateCreated = DateTime.UtcNow
                    };

                    _burstChatContext.Users.Add(user);
                    _burstChatContext.SaveChanges();

                    return new Success<User, Error>(user);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<User, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Updates infomation about an existing user based on the user instance provided.
        /// </summary>
        /// <param name="user">The user instance to be updated in the database</param>
        /// <returns>An either monad</returns>
        public Either<User, Error> Update(User user)
        {
            try
            {
                if (user is { })
                {
                    var userId = user.Id;

                    return Get(userId).Bind(storedUser =>
                    {
                        storedUser.Email = user.Email;
                        storedUser.Name = user.Name;
                        storedUser.Avatar = user.Avatar;

                        _burstChatContext.SaveChanges();
                        return new Success<User, Error>(storedUser);
                    });
                }
                else
                    return new Failure<User, Error>(UserErrors.UserNotFound());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<User, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Deletes a registered user from the database based on the provided user instance.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Delete(long id)
        {
            try
            {
                return Get(id).Bind(user =>
                {
                    _burstChatContext.Users.Remove(user);
                    return new Success<Unit, Error>(new Unit());
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will return all available subscribed servers of a user.
        /// <summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Server>, Error> GetSubscriptions(long userId)
        {
            try
            {
                var servers = _burstChatContext
                    .Servers
                    .Include(server => server.Subscriptions)
                    .Where(server => server.Subscriptions
                                           .Any(subscription => subscription.UserId == userId))
                    .ToList();

                return new Success<IEnumerable<Server>, Error>(servers);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<Server>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will return all private group that the user with the provided user id
        /// is part of.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<PrivateGroup>, Error> GetPrivateGroups(long userId)
        {
            try
            {
                return Get(userId).Bind(user =>
                {
                    var group = _burstChatContext
                        .PrivateGroups
                        .Include(pmg => pmg.Users
                                            .Where(u => u.Id == user.Id))
                        .Include(pmg => pmg.Messages)
                        .ToList();

                    return new Success<IEnumerable<PrivateGroup>, Error>(group);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<PrivateGroup>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will return all direct messaging that the user is part of.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<DirectMessaging>, Error> GetDirectMessaging(long userId)
        {
            try
            {
                return Get(userId).Bind(user =>
                {
                    var directMessaging = _burstChatContext
                        .DirectMessaging
                        .Where(d => d.FirstParticipantUserId == user.Id
                                    || d.SecondParticipantUserId == user.Id)
                        .ToList();

                    return new Success<IEnumerable<DirectMessaging>, Error>(directMessaging);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<DirectMessaging>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will validate the provided email and password in order to
        /// find any user that has registered these credentials.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>An either monad</returns>
        public Either<User, Error> Validate(string email, string password)
        {
            try
            {
                return Get(email).Bind<User>(user =>
                {
                    return _bcryptService.VerifyHash(password, user.Password)
                        ? new Success<User, Error>(user)
                        : new Failure<User, Error>(UserErrors.UserPasswordDidNotMatch());
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<User, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will validate the provided email and if a user is registered with it
        /// a new one time password will be generated for him and sent via email.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>An either monad</returns>
        public Either<string, Error> IssueOneTimePassword(string email)
        {
            try
            {
                var user = _burstChatContext
                    .Users
                    .Include(u => u.OneTimePasswords)
                    .FirstOrDefault(u => u.Email == email);

                if (user is null)
                    return new Failure<string, Error>(UserErrors.UserNotFound());

                var dateCreated = DateTime.UtcNow;
                // TODO: Implement a better solution for one time pass generation.
                var timedOneTimePass = Guid.NewGuid().ToString();
                var oneTimePassword = new OneTimePassword
                {
                    OTP = _bcryptService.GenerateHash(timedOneTimePass),
                    DateCreated = dateCreated,
                    ExpirationDate = dateCreated.AddMinutes(15)
                };

                user.OneTimePasswords.Add(oneTimePassword);

                _burstChatContext.SaveChanges();

                return new Success<string, Error>(timedOneTimePass);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<string, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Changes the current hashed password of the user to the one provided.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="oneTimePass">The one time password of the user</param>
        /// <param name="password">The string value of the password that will be hashed</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> ChangePassword(string email, string oneTimePass, string password)
        {
            try
            {
                var user = _burstChatContext
                    .Users
                    .FirstOrDefault(u => u.Email == email);

                if (user is { })
                {
                    var otp = _burstChatContext
                        .Users
                        .Where(u => u.Id == user.Id)
                        .Include(u => u.OneTimePasswords)
                        .Select(u => u.OneTimePasswords
                                      .Where(o => o.ExpirationDate >= DateTime.UtcNow))
                        .AsEnumerable()
                        .SelectMany(_ => _)
                        .FirstOrDefault(o => _bcryptService.VerifyHash(oneTimePass, o.OTP));

                    if (otp is { })
                    {
                        var hashedPassword = _bcryptService.GenerateHash(password);

                        user.Password = hashedPassword;
                        _burstChatContext.SaveChanges();

                        return new Success<Unit, Error>(new Unit());
                    }
                }

                return new Failure<Unit, Error>(UserErrors.UserOneTimePasswordInvalid());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Fetches all invitations sent to a user based on the provided id.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Invitation>, Error> GetInvitations(long userId)
        {
            try
            {
                var invitations = _burstChatContext
                    .Invitations
                    .Include(i => i.Server)
                    .Include(i => i.User)
                    .Where(i => i.UserId == userId && !i.Accepted && !i.Declined)
                    .ToList();

                return new Success<IEnumerable<Invitation>, Error>(invitations);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<Invitation>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will updated the Accepted or Declined propery of an existing invitation based on the instance
        /// provided.
        /// </summary>
        /// <param name="data">The server invitation to be updated</param>
        /// <returns>An either monad</returns>
        public Either<Invitation, Error> UpdateInvitation(long userId, UpdateInvitation data)
        {
            try
            {
                var storedInvitation = _burstChatContext
                    .Invitations
                    .Include(i => i.Server)
                    .Include(i => i.User)
                    .FirstOrDefault(i => i.Id == data.InvitationId && i.UserId == userId);

                if (storedInvitation == null)
                    return new Failure<Invitation, Error>(UserErrors.CouldNotUpdateInvitation());

                storedInvitation.Accepted = data.Accepted;
                storedInvitation.Declined = !data.Accepted;
                storedInvitation.DateUpdated = DateTime.UtcNow;

                if (storedInvitation.Accepted)
                {
                    var subscription = new Subscription
                    {
                        ServerId = storedInvitation.ServerId,
                        UserId = storedInvitation.UserId,
                    };

                    _burstChatContext
                        .Subscriptions
                        .Add(subscription);
                }

                _burstChatContext.SaveChanges();

                return new Success<Invitation, Error>(storedInvitation);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Invitation, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will fetch all appropriate user claims based on the provided instance.
        /// </summary>
        /// <param name="user">The user instance</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Claim>, Error> GetClaims(User user)
        {
            if (user == null)
                return new Failure<IEnumerable<Claim>, Error>(UserErrors.UserNotFound());

            var claims = new Claim[]
            {
                new Claim("email", user.Email),
                new Claim("sub", user.Id.ToString()),
                new Claim("username", user.Name)
            };

            return new Success<IEnumerable<Claim>, Error>(claims);
        }
    }
}
