using System;
using System.Threading.Tasks;
using BurstChat.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BurstChat.Signal.Hubs.Chat
{
    /// <summary>
    /// This class is a SignalR hub that is responsible for message of the delivered to the users of the chat.
    /// </summary>
    public class ChatHub : Hub<IChatClient>
    {
        private readonly ILogger<ChatHub> _logger;

        /// <summary>
        /// Executes any necessary start up code for the hub.
        /// </summary>
        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sends a new message to all users from the parameters provided.
        /// </summary>
        /// <param name="message">The message to be sent to connected users</param>
        /// <returns>A task instance</returns>
        public async Task SendMessage(Message message)
        {
            _logger.LogInformation($"New message sent with content: {message.Content}");
            await Clients.All.MessageReceived(message);
        }
    }
}