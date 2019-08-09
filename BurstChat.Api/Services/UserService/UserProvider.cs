using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using BurstChat.Api.Errors;
using BurstChat.Api.Extensions;
using BurstChat.Api.Services.BCryptService;
using BurstChat.Shared.Context;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Shared.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Services.UserService
{
    /// <summary>
    /// This class is the base implementation of the IUserService.
    /// </summary>
    public class UserProvider : IUserService
    {
        private readonly ILogger<UserProvider> _logger;
        private readonly BurstChatContext _burstChatContext;
        private readonly IBCryptService _bcryptService;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// </summary>
        public UserProvider(
            ILogger<UserProvider> logger,
            BurstChatContext burstChatContext,
            IBCryptService bcryptService
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
            _bcryptService = bcryptService;
        }

        /// <summary>
        ///   This method returns a User instance if the provided id belongs to one.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>An either monad of a User instance or an Error instance</returns>
        public Either<User, Error> Get(long id)
        {
            try
            {
                var user = _burstChatContext
                    .Users
                    .FirstOrDefault(u => u.Id == id);

                if (user != null)
                    return new Success<User, Error>(user);
                else
                    return new Failure<User, Error>(UserErrors.UserNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<User, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method returns a User instance if the provided email belongs to one.
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

                if (user != null)
                    return new Success<User, Error>(user);
                else
                    return new Failure<User, Error>(UserErrors.UserNotFound());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<User, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Registers a new user based on the provided parameters.
        /// </summary>
        /// <param name="email">The email of the new user</param>
        /// <param name="password">The password of the new user</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Insert(string email, string password)
        {
            try
            {
                if (Get(email) is Success<User, Error>)
                    return new Failure<Unit, Error>(UserErrors.UserAlreadyExists());

                var hashedPassword = _bcryptService.GenerateHash(password);

                var user = new User
                {
                    Email = email,
                    Name = string.Empty,
                    Password = hashedPassword,
                    DateCreated = DateTime.Now
                };

                _burstChatContext.Users.Add(user);
                _burstChatContext.SaveChanges();

                return new Success<Unit, Error>(new Unit());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Updates infomation about an existing user based on the user instance provided.
        /// </summary>
        /// <param name="user">The user instance to be updated in the database</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Update(User user)
        {
            try
            {
                var userId = user?.Id ?? default(long);

                return Get(userId)
                    .Bind(storedUser =>
                    {
                        storedUser.Email = user.Email;
                        storedUser.Name = user.Name;

                        _burstChatContext.SaveChanges();
                        return new Success<Unit, Error>(new Unit());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Deletes a registered user from the database based on the provided user instance.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> Delete(long id)
        {
            try
            {
                return Get(id)
                    .Bind(user => 
                    {
                        _burstChatContext.Remove(user);
                        return new Success<Unit, Error>(new Unit());
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will validate the provided email and password in order to
        ///   find any user that has registered these credentials.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>An either monad</returns>
        public Either<User, Error> Validate(string email, string password)
        {
            try
            {

                return Get(email)
                    .Bind<User>(user => 
                    {
                        var passwordIsValid = _bcryptService.VerifyHash(password, user.Password);

                        if (!passwordIsValid)  
                            return new Failure<User, Error>(UserErrors.UserPasswordDidNotMatch());

                        return new Success<User, Error>(user);
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<User, Error>(SystemErrors.Exception());
            }           
        }

        /// <summary>
        ///   This method will validate the provided email and if a user is registered with it
        ///   a new one time password will be generated for him and sent via email.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> IssueOneTimePassword(string email)
        {
            try
            {
                var user = _burstChatContext
                    .Users
                    .Include(u => u.OneTimePasswords)
                    .FirstOrDefault(u => u.Email == email);

                if (user == null)
                    return new Failure<Unit, Error>(UserErrors.UserNotFound());

                var dateCreated = DateTime.Now;
                var timedOneTimePass = "111111";
                var oneTimePassword = new OneTimePassword
                {
                    OTP = _bcryptService.GenerateHash(timedOneTimePass),
                    DateCreated = dateCreated,
                    ExpirationDate = dateCreated.AddMinutes(15) 
                };

                user.OneTimePasswords
                    .Add(oneTimePassword);

                _burstChatContext.SaveChanges();

                return new Success<Unit, Error>(new Unit());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Changes the current hashed password of the user to the one provided.
        /// </summary>
        /// <param name="oneTimePass">The one time password of the user</param>
        /// <param name="password">The string value of the password that will be hashed</param>
        /// <returns>An either monad</returns>
        public Either<Unit, Error> ChangePassword(string oneTimePass, string password)
        {
            try
            {
                var hashedOneTImePass = _bcryptService.GenerateHash(oneTimePass);

                var users = _burstChatContext
                    .Users
                    .Include(u => u.OneTimePasswords);

                var user = users
                    .FirstOrDefault(u => u.OneTimePasswords
                                          .Where(o => o.ExpirationDate >= DateTime.Now)
                                          .Any(o => _bcryptService.VerifyHash(o.OTP, oneTimePass)));

                if (user != null)
                {
                    var oneTimePassword = user
                        .OneTimePasswords
                        .FirstOrDefault(o => _bcryptService.VerifyHash(o.OTP, hashedOneTImePass));

                    if (oneTimePassword.ExpirationDate < DateTime.Now)
                        return new Failure<Unit, Error>(UserErrors.UserOneTimePasswordExpired());

                    var hashedPassword = _bcryptService.GenerateHash(password);

                    user.Password = hashedPassword;
                    _burstChatContext.SaveChanges();

                    return new Success<Unit, Error>(new Unit());
                }
                else
                    return new Failure<Unit, Error>(UserErrors.UserOneTimePasswordInvalid());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will return all available subscribed servers of a user.
        /// <summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Server>, Error> GetSubscribedServers(long userId)
        {
            try 
            {
                var servers = _burstChatContext
                    .Servers
                    .Include(server => server.SubscribedUsers
                                             .Where(subscription => subscription.UserId == userId))
                    .ToList();

                return new Success<IEnumerable<Server>, Error>(servers);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<IEnumerable<Server>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will return all private group that the user with the provided user id
        ///   is part of.
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<PrivateGroupMessage>, Error> GetPrivateGroups(long userId)
        {
            try
            {
                return Get(userId)
                    .Bind(user => 
                    {
                        var group = _burstChatContext
                            .PrivateGroupMessage
                            .Include(pmg => pmg.Users
                                               .Where(u => u.Id == user.Id))
                            .Include(pmg => pmg.Messages)
                            .ToList();

                        return new Success<IEnumerable<PrivateGroupMessage>, Error>(group);
                    });
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<IEnumerable<PrivateGroupMessage>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will fetch all appropriate user claims based on the provided instance.
        /// </summary>
        /// <param name="user">The user instance</param>
        /// <returns>An either monad</returns>
        public Either<IEnumerable<Claim>, Error> GetClaims(User user)
        {
            if (user == null)
                return new Failure<IEnumerable<Claim>, Error>(null);

            var claims = new Claim[]
            {
                new Claim("email", user.Email),
                new Claim("username", user.Name)
            };

            return new Success<IEnumerable<Claim>, Error>(claims);
        }
    }
}
