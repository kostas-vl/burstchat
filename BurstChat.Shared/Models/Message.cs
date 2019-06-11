using System;

namespace BurstChat.Shared.Models
{
    /// <summary>
    /// This class represents a chat message.
    /// </summary>
    public class Message
    {
        public string User;

        public string Content;

        public DateTime DatePosted;

        public bool Edited;
    }
}