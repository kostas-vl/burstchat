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
   }
}
