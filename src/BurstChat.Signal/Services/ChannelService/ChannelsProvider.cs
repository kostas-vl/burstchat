using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using Microsoft.Extensions.Logging;
using BurstChat.Signal.Services.ApiInteropService;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Web;
using System.Text.Json;

namespace BurstChat.Signal.Services.ChannelsService
{
    /// <summary>
    /// This class is the base implementation of the IChannelsService interface.
    /// </summary>
    public class ChannelsProvider : IChannelsService
    {
        private readonly ILogger<ChannelsProvider> _logger;
        private readonly BurstChatApiInteropService _apiInteropService;

        /// <summary>
        /// Executes any neccessary start up code for the service.
        ///
        /// Exceptions:
        ///     ArgumentNullException: When any parameter is null.
        /// </summary>
        public ChannelsProvider(
            ILogger<ChannelsProvider> logger,
            BurstChatApiInteropService apiInteropService
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiInteropService = apiInteropService ?? throw new ArgumentNullException(nameof(apiInteropService));
        }

        /// <summary>
        /// This method will fetch information about a channel based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Channel, Error>> GetAsync(HttpContext context, int channelId)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = $"api/channels/{channelId}";

                return await _apiInteropService.SendAsync<Channel>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will invoke a call to the BurstChat API for the creation of a new channel.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="serverId">The id of the channel</param>
        /// <param name="channel">The instance of the new channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Channel, Error>> PostAsync(HttpContext context, int serverId, Channel channel)
        {
            try
            {
                var method = HttpMethod.Post;
                var url = "api/channels";
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["serverId"] = serverId.ToString();
                url += $"/?{query}";
                var jsonMessage = JsonSerializer.Serialize(channel);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<Channel>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will invoke a call to the BurstChat API inorder to update information about
        /// a channel.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channel">The updated information of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Channel, Error>> PutAsync(HttpContext context, Channel channel)
        {
            try
            {
                var method = HttpMethod.Put;
                var url = "api/channels";
                var jsonMessage = JsonSerializer.Serialize(channel);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<Channel>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will invoke a call to the BurstChat API for the deletion of a channel.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Channel, Error>> DeleteAsync(HttpContext context, int channelId)
        {
            try
            {
                var method = HttpMethod.Delete;
                var url = $"api/channels/{channelId}";

                return await _apiInteropService.SendAsync<Channel>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }


        /// <summary>
        /// This method will fetch all messages of a channels based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="searchTerm">A term that needs to be present on all returned messages</param>
        /// <param name="lastMessageId">The id of the interval message for the rest</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<IEnumerable<Message>, Error>> GetMessagesAsync(HttpContext context,
                                                                                int channelId,
                                                                                string? searchTerm = null,
                                                                                long? lastMessageId = null)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = $"api/channels/{channelId}/messages";
                var query = HttpUtility.ParseQueryString(string.Empty);

                if (lastMessageId is not null)
                    query[nameof(lastMessageId)] = lastMessageId.ToString();

                if (searchTerm is not null)
                    query[nameof(searchTerm)] = searchTerm;

                if (query.Count > 0)
                    url += $"/?{query}";

                return await _apiInteropService.SendAsync<IEnumerable<Message>>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new Failure<IEnumerable<Message>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// This method will post a new message to a channel based on the provided channel id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Message, Error>> PostMessageAsync(HttpContext context, int channelId, Message message)
        {
            try
            {
                var method = HttpMethod.Post;
                var url = $"api/channels/{channelId}/messages";
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
        /// This method will edit an existing message of a channel based on the provided channel id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Message, Error>> PutMessageAsync(HttpContext context, int channelId, Message message)
        {
            try
            {
                var method = HttpMethod.Put;
                var url = $"api/channels/{channelId}/messages";
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
        /// This method will delete an existing message from a channel based on the provided channel id
        /// and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Message, Error>> DeleteMessageAsync(HttpContext context, int channelId, Message message)
        {
            try
            {
                var method = HttpMethod.Delete;
                var url = $"api/channels/{channelId}/messages";
                var jsonMessage = JsonSerializer.Serialize(message.Id);
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
