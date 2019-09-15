using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Shared.Schema.Servers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BurstChat.Signal.Services.ApiInteropService;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace BurstChat.Signal.Services.ChannelsService
{
    /// <summary>
    ///   This class is the base implementation of the IChannelsService interface.
    /// </summary>
    public class ChannelsProvider : IChannelsService
    {
        private readonly ILogger<ChannelsProvider> _logger;
        private readonly BurstChatApiInteropService _apiInteropService;

        /// <summary>
        ///   Executes any neccessary start up code for the service.
        /// </summary>
        public ChannelsProvider(
            ILogger<ChannelsProvider> logger,
            BurstChatApiInteropService apiInteropService
        )
        {
            _logger = logger;
            _apiInteropService = apiInteropService;
        }

        /// <summary>
        ///   This method will fetch information about a channel based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Channel, Error>> GetChannelAsync(HttpContext context, int channelId)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = $"api/channels/{channelId}";

                return await _apiInteropService.SendAsync<Channel>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Channel, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will fetch all messages of a channels based on the provided id.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<IEnumerable<Message>, Error>> GetAllAsync(HttpContext context, int channelId)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = $"api/channels/{channelId}/messages";

                return await _apiInteropService.SendAsync<IEnumerable<Message>>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<IEnumerable<Message>, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will post a new message to a channel based on the provided channel id
        ///   and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Message, Error>> PostAsync(HttpContext context, int channelId, Message message)
        {
            try
            {
                var method = HttpMethod.Post;
                var url = $"api/channels/{channelId}/messages";
                var jsonMessage = JsonConvert.SerializeObject(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync<Message>(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Message, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///  This method will edit an existing message of a channel based on the provided channel id 
        ///  and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> PutAsync(HttpContext context, int channelId, Message message)
        {
            try
            {
                var method = HttpMethod.Put;
                var url = $"api/channels/{channelId}/messages";
                var jsonMessage = JsonConvert.SerializeObject(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        ///   This method will delete an existing message from a channel based on the provided channel id
        ///   and message.
        /// </summary>
        /// <param name="context">The http context of the current request</param>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> DeleteAsync(HttpContext context, int channelId, Message message)
        {
            try
            {
                var method = HttpMethod.Delete;
                var url = $"api/channels/{channelId}/messages";
                var jsonMessage = JsonConvert.SerializeObject(message);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                return await _apiInteropService.SendAsync(context, method, url, content);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }
    }
}
