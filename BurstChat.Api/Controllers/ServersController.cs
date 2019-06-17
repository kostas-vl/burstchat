using System;
using System.Collections.Generic;
using BurstChat.Shared.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    /// This controllers exposes endpoints for listing, subscribing and leaving servers.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("/api/servers")]
    public class ServersController : ControllerBase
    {
        private readonly ILogger<ServersController> _logger;
        private readonly BurstChatContext _burstChatContext;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// </summary>
        public ServersController(
            ILogger<ServersController> logger,
            BurstChatContext burstChatContext
        )
        {
            _logger = logger;
            _burstChatContext = burstChatContext;
        }

        /// <summary>
        /// Fetches all the currently subscribed servers.
        /// </summary>
        /// <returns>A enumerable of servers</returns>
        public IEnumerable<object> GetSubcribed()
        {
            return new object[0];
        }
    }
}
