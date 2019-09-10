using System;
using System.Net.Http;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Extensions;
using BurstChat.Shared.Monads;
using BurstChat.Shared.Schema.Servers;
using BurstChat.Signal.Services.ApiInteropService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal.Services.ServerService
{
    /// <summary>
    ///     This class is the based implementation of the IServerService interface.
    /// </summary>
    public class ServerProvider : IServerService
    {
        private readonly ILogger<ServerProvider> _logger;
        private readonly BurstChatApiInteropService _apiInteropService;

        /// <summary>
        ///     Creates a new instance of ServerProvider.
        /// </summary>
        public ServerProvider(
            ILogger<ServerProvider> logger,
            BurstChatApiInteropService apiInteropService
        )
        {
            _logger = logger;
            _apiInteropService = apiInteropService;
        }

        /// <summary>
        ///     Fetches information about a server based on the provided parameters.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <param name="serverId">The id of the target server</param>
        /// <returns>A task of an either monad</returns>
        public async Task<Either<Server, Error>> GetAsync(HttpContext context, int serverId)
        {
            try
            {
                var method = HttpMethod.Get;
                var url = $"/api/servers/{serverId}";

                return await _apiInteropService.SendAsync<Server>(context, method, url);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                return new Failure<Server, Error>(SystemErrors.Exception());
            }
        }
    }
}