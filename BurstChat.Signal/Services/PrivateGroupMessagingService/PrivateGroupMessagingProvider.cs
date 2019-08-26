using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Users;
using BurstChat.Signal.Extensions;
using BurstChat.Signal.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BurstChat.Signal.Services.PrivateGroupMessaging
{
    /// <summary>
    ///   This class is a base implementation of the IPrivateGroupMessagingService interface.
    /// </summary>
    public class PrivateGroupMessagingProvider : IPrivateGroupMessagingService
    {
        private readonly ILogger<PrivateGroupMessagingProvider> _logger;
        private readonly AcceptedDomainsOptions _acceptedDomains;
      
        /// <summary>
        ///   Executes any neccessary start up code for the service.
        /// </summary>
        public PrivateGroupMessagingProvider(
            ILogger<PrivateGroupMessagingProvider> logger,
            IOptions<AcceptedDomainsOptions> acceptedDomains
        )
        {
            _logger = logger;
            _acceptedDomains = acceptedDomains.Value;
        }

        /// <summary>
        ///   This method will fetch information about a private group based on the provided id.
        /// </summary>
        /// <param name="groupId">The id of the private group</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<PrivateGroupMessage, Error>> GetPrivateGroupAsync(long groupId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{_acceptedDomains.BurstChatApiDomain}/api/groups/{groupId}";
                    var httpResponse = await client.GetAsync(url);

                    return await httpResponse.ParseBurstChatApiResponseAsync<PrivateGroupMessage>();
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<PrivateGroupMessage, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will fetch all messages of a private group based on the provided id.
        /// </summary>
        /// <param name="groupId">The id of the private group</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<IEnumerable<Message>, Error>> GetAllAsync(long groupId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{_acceptedDomains.BurstChatApiDomain}/api/groups/{groupId}";
                    var httpResponse = await client.GetAsync(url);

                    return await httpResponse.ParseBurstChatApiResponseAsync<IEnumerable<Message>>();
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<IEnumerable<Message>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will post a new message to a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> PostAsync(long groupId, Message message)
        {
            try
            {
                var jsonMessage = JsonConvert.SerializeObject(message);
                using (var client = new HttpClient())
                using (var requestContent = new StringContent(jsonMessage))
                {
                    var url = $"{_acceptedDomains.BurstChatApiDomain}/api/groups/{groupId}/messages";
                    var httpResponse = await client.PostAsync(url, requestContent);

                    return await httpResponse.ParseBurstChatApiResponseAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will edit an existing message of a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> PutAsync(long groupId, Message message)
        {
            try
            {
                var jsonMessage = JsonConvert.SerializeObject(message);
                using (var client = new HttpClient())
                using (var requestContent = new StringContent(jsonMessage))
                {
                    var url = $"{_acceptedDomains.BurstChatApiDomain}/groups/{groupId}/messages";
                    var httpResponse = await client.PutAsync(url, requestContent);

                    return await httpResponse.ParseBurstChatApiResponseAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete an existing message of a private group based on the provided group id
        ///   and message.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> DeleteAsync(long groupId, Message message)
        {
            try
            {
                var jsonMessage = JsonConvert.SerializeObject(message);
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                using (var requestContent = new StringContent(jsonMessage))
                {
                    request.Method = HttpMethod.Delete;
                    request.RequestUri = new Uri($"{_acceptedDomains.BurstChatApiDomain}/groups/{groupId}/messages");
                    request.Content = requestContent;
                    var httpResponse = await client.SendAsync(request);

                    return await httpResponse.ParseBurstChatApiResponseAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }
    }
}
