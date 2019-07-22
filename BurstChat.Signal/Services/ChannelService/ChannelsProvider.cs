using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Chat;
using BurstChat.Signal.Extensions;
using BurstChat.Signal.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BurstChat.Signal.Services.ChannelsService
{
    /// <summary>
    ///   This class is the base implementation of the IChannelsService interface.
    /// </summary>
    public class ChannelsProvider : IChannelsService
    {
        private readonly ILogger<ChannelsProvider> _logger;
        private readonly AcceptedDomainsOptions _acceptedDomains;

        /// <summary>
        ///   Executes any neccessary start up code for the service.
        /// </summary>
        public ChannelsProvider(
            ILogger<ChannelsProvider> logger,
            IOptions<AcceptedDomainsOptions> acceptedDomains
        )
        {
            _logger = logger;
            _acceptedDomains = acceptedDomains.Value;
        }

        /// <summary>
        ///   This method will fetch all messages of a channels based on the provided id.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<IEnumerable<Message>, Error>> GetAllAsync(int channelId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{_acceptedDomains.BurstChatApiDomain}/api/channels/{channelId}/messages";
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
        ///   This method will post a new message to a channel based on the provided channel id
        ///   and message.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be posted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> PostAsync(int channelId, Message message)
        {
            try
            {
                var jsonMessage = JsonConvert.SerializeObject(message);
                using (var client = new HttpClient())
                using (var requestContent = new StringContent(jsonMessage))
                {
                    var url = $"{_acceptedDomains.BurstChatApiDomain}/api/channels/{channelId}/messages";
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
        ///  This method will edit an existing message of a channel based on the provided channel id 
        ///  and message.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be edited</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> PutAsync(int channelId, Message message)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            using (var client = new HttpClient())
            using (var requestContent = new StringContent(jsonMessage))
            {
                var url = $"{_acceptedDomains.BurstChatApiDomain}/api/channels/{channelId}/messages";
                var httpResponse = await client.PutAsync(url, requestContent);

                return await httpResponse.ParseBurstChatApiResponseAsync();
            }
        }

        /// <summary>
        ///   This method will delete an existing message from a channel based on the provided channel id
        ///   and message.
        /// </summary>
        /// <param name="channelId">The id of the channel</param>
        /// <param name="message">The message to be deleted</param>
        /// <returns>A task that encapsulates an either monad</returns>
        public async Task<Either<Unit, Error>> DeleteAsync(int channelId, Message message)
        {
            try
            {
                var jsonMessage = JsonConvert.SerializeObject(message);
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                using (var requestContent = new StringContent(jsonMessage))
                {
                    request.Method = HttpMethod.Delete;
                    request.RequestUri = new Uri($"{_acceptedDomains.BurstChatApiDomain}/api/channels/{channelId}/messages");
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
