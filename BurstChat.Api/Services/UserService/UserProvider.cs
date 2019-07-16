using System;
using System.Linq;
using BurstChat.Api.Errors;
using BurstChat.Api.Extensions;
using BurstChat.Api.Services.BCryptService;
using BurstChat.Shared.Context;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
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
        ///   This method return a User instance based on the id provided.
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
                    return new Failure<User, Error>(SystemErrors.Exception());
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
                var user = _burstChatContext
                    .Users
                    .FirstOrDefault(u => u.Email == email);

                if (user == null)
                    return new Failure<User, Error>(SystemErrors.Exception());

                var passwordIsValid = _bcryptService.VerifyHash(password, user.Password);
                if (!passwordIsValid)  
                    return new Failure<User, Error>(SystemErrors.Exception());

                return new Success<User, Error>(user);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<User, Error>(SystemErrors.Exception());
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
                var user = _burstChatContext
                    .Users
                    .Include(u => u.OneTimePasswords
                                   .Where(o => _bcryptService.VerifyHash(oneTimePass, o.OTP)))
                    .FirstOrDefault();

                if (user != null)
                {
                    var hashedPassword = _bcryptService.GenerateHash(password);

                    user.Password = hashedPassword;
                    _burstChatContext.SaveChanges();

                    return new Success<Unit, Error>(new Unit());
                }
                else
                    return new Failure<Unit, Error>(SystemErrors.Exception());
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }
    }
}
