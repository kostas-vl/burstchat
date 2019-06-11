using System;
using BurstChat.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurstChat.Api.Controllers
{
    /// <summary>
    /// This class represents a RESTful controller that exposes methods for information about the chat.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("/api/chat")]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;

        /// <summary>
        /// Executes any necessary start up code for the controller.
        /// </summary>
        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Fetches the current messages of the chat. It will return a maximum of 300 messages.
        /// </summary>
        /// <returns>An implementation of an IActionResult</returns>
        public IActionResult GetLatest()
        {
            return Ok(new Message[0]);
        }
    }
}