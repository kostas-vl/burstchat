using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Services.ApiInteropService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BurstChat.Signal.Services.PrivateGroupMessaging
{
    /// <summary>
    ///   This class is a base implementation of the IPrivateGroupMessagingService interface.
    /// </summary>
    public class PrivateGroupMessagingProvider : IPrivateGroupMessagingService
    {
        private readonly ILogger<PrivateGroupMessagingProvider> _logger;
        private readonly BurstChatApiInteropService _apiInteropService;

        /// <summary>
        ///   Executes any neccessary start up code for the service.
        /// </summary>
        public PrivateGroupMessagingProvider(
            ILogger<PrivateGroupMessagingProvider> logger,
            BurstChatApiInteropService apiInteropService
        )
        {
            _logger = logger;
            _apiInteropService = apiInteropService;
        }

        /// <summary>
        ///   This method will fetch information about a private group based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the private group</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<PrivateGroup, Error>> GetPrivateGroupAsync(HttpContext context, long groupId)
        {
            try
            {
                var url = $"/api/groups/{groupId}";
                var method = HttpMethod.Get;

                return await _apiInteropService.SendAsync<PrivateGroup>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<PrivateGroup, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will fetch all messages of a private group based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the private group</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<IEnumerable<Message>, Error>> GetAllAsync(HttpContext context, long groupId)
        {
            try
            {
                var url = $"/api/groups/{groupId}";
                var method = HttpMethod.Get;

                return await _apiInteropService.SendAsync<IEnumerable<Message>>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<Message>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will post a new message to a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> PostAsync(HttpContext context, long groupId, Message message)
        {
            try
            {
                var url = $"/api/groups/{groupId}/messages";
                var method = HttpMethod.Post;
                var jsonMessage = JsonConvert.SerializeObject(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will edit an existing message of a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> PutAsync(HttpContext context, long groupId, Message message)
        {
            try
            {
                var method = HttpMethod.Put;
                var url = $"/api/groups/{groupId}/messages";
                var jsonMessage = JsonConvert.SerializeObject(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete an existing message of a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> DeleteAsync(HttpContext context, long groupId, Message message)
        {
            try
            {
                var method = HttpMethod.Delete;
                var url = $"/api/groups/{groupId}/messages";
                var jsonMessage = JsonConvert.SerializeObject(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }
    }
}
