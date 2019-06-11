using System;
using System.Threading.Tasks;
using BurstChat.Shared.Models;

namespace BurstChat.Signal.Hubs.Chat
{
    /// <summary>
    /// This interface provides a contract for a class that represents all available methods that a client can consume
    /// in order to handle responses from the chat hub.
    /// </summary>
    public interface IChatClient
    {
        /// <summary>
        /// All users are informed for a new message based on the provided parameter.
        /// </summary>
        /// <param name="message">The message to be sent to connected users</param>
        /// <returns>A task instance</returns>
        Task MessageReceived(Message message);
    }
}