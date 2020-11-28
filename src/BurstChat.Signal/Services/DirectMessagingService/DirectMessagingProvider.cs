using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Users;
using BurstChat.Signal.Services.ApiInteropService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Web;
using System.Text.Json;

namespace BurstChat.Signal.Services.DirectMessagingService
{
    /// <summary>
    ///   This class is the base implementation of the IDirectMessagingService interface.
    /// </summary>
    public class DirectMessagingProvider : IDirectMessagingService
    {
        private readonly ILogger<DirectMessagingProvider> _logger;
        private readonly BurstChatApiInteropService _apiInteropService;

        /// <summary>
        ///   Creates a new instance of DirectMessagingProvider.
        /// </summary>
        public DirectMessagingProvider(
            ILogger<DirectMessagingProvider> logger,
            BurstChatApiInteropService apiInteropService
        )
        {
            _logger = logger;
            _apiInteropService = apiInteropService;
        }

        /// <summary>
        ///   Fetches all available information about a direct messaging entry based on the provided
        ///   id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the target entry</param>
        /// <returns>An either monad</returns>
        public async Task<Either<DirectMessaging, Error>> GetAsync(HttpContext context, long directMessagingId)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = $"/api/direct/{directMessagingId}";

                return await _apiInteropService.SendAsync<DirectMessaging>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<DirectMessaging, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Fetches all available information about a direct messaging entry based on the provided
        ///   participants.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="firstParticipantId">The user id of the first participant</param>
        /// <param name="secondParticipantId">The user id of the seconad participant</param>
        /// <returns>An either monad</returns>
        public async Task<Either<DirectMessaging, Error>> GetAsync(HttpContext context, long firstParticipantId, long secondParticipantId)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = "/api/direct";
                var content = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string?, string?>("firstParticipantId", firstParticipantId.ToString()),
                    new KeyValuePair<string?, string?>("secondParticipantId", secondParticipantId.ToString())
                });

                return await _apiInteropService.SendAsync<DirectMessaging>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<DirectMessaging, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Creates a new direct messaging entry between two users based on the provided user ids.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessaging">The direct messaging instance to be added</param>
        /// <returns>An either monad</returns>
        public async Task<Either<DirectMessaging, Error>> PostAsync(HttpContext context, DirectMessaging directMessaging)
        {
            try
            {
                var method = HttpMethod.Post;
                var url = $"/api/direct";
                var jsonMessage = JsonSerializer.Serialize(directMessaging);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<DirectMessaging>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<DirectMessaging, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Removes a direct messaging entry based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the target entry</param>
        /// <returns>An either monad</returns>
        public async Task<Either<DirectMessaging, Error>> DeleteAsync(HttpContext context, long directMessagingId)
        {
            try
            {
                var method = HttpMethod.Delete;
                var url = $"/api/direct/{directMessagingId}";

                return await _apiInteropService.SendAsync<DirectMessaging>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<DirectMessaging, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Fetches all messages posted on a direct messaging entry.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="lastMessageId">The message id from which all the previous messages sent will be fetched</param>
        /// <returns>An either monad</returns>
        public async Task<Either<IEnumerable<Message>, Error>> GetMessagesAsync(HttpContext context,
                                                                                long directMessagingId,
                                                                                long? lastMessageId = null)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = $"/api/direct/{directMessagingId}/messages";
                if (lastMessageId is { })
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query[nameof(lastMessageId)] = lastMessageId.Value.ToString();
                    url += $"/?{query}";
                }

                return await _apiInteropService.SendAsync<IEnumerable<Message>>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<Message>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Inserts a new message on a direct messaging entry.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message to be inserted</param>
        /// <returns>An either monad</returns>
        public async Task<Either<Message, Error>> PostMessageAsync(HttpContext context, long directMessagingId, Message message)
        {
            try
            {
                var method = HttpMethod.Post;
                var url = $"/api/direct/{directMessagingId}/messages";
                var jsonMessage = JsonSerializer.Serialize(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<Message>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Updates a message on a direct messaging entry.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message to be updated</param>
        /// <returns>An either monad</returns>
        public async Task<Either<Message, Error>> PutMessageAsync(HttpContext context, long directMessagingId, Message message)
        {
            try
            {
                var method = HttpMethod.Put;
                var url = $"/api/direct/{directMessagingId}/messages";
                var jsonMessage = JsonSerializer.Serialize(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<Message>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   Removes a message from a direct messagin entry.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="directMessagingId">The id of the direct messaging entry</param>
        /// <param name="message">The message to be removed</param>
        /// <returns>An either monad</returns>
        public async Task<Either<Message, Error>> DeleteMessageAsync(HttpContext context, long directMessagingId, Message message)
        {
            try
            {
                var method = HttpMethod.Delete;
                var url = $"/api/direct/{directMessagingId}/messages";
                var jsonMessage = JsonSerializer.Serialize(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<Message>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }
    }
}
