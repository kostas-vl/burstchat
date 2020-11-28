using System;
using System.Collections.Generic;
using System.Linq;
using BurstChat.Domain.Schema.Chat;

namespace BurstChat.Api.Extensions
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Queries the content of the provided message and returns a list of found links in it.
        /// </summary>
        /// <param name="message">The message instance of which the content will be queried</param>
        /// <returns>A list of links</returns>
        public static List<Link> GetLinksFromContent(this Message message)
        {
            var words = message
                .Content
                .Split(" ");

            var links = words
                .Where(word => Uri.TryCreate(word, UriKind.Absolute, out _))
                .Select(uri => new Link
                {
                    Url = uri,
                    DateCreated = DateTime.Now
                })
                .ToList();

            return links;
        }

        /// <summary>
        /// Queries the content of the provided message and returns a list of found links in it.
        /// </summary>
        /// <param name="message">The message instance of which the content will be queried</param>
        /// <returns>A list of links</returns>
        public static string RemoveLinksFromContent(this Message message)
        {
            var words = message
                .Content
                .Split(" ");

            var normilizedContent = words
                .Where(word => !Uri.TryCreate(word, UriKind.Absolute, out _))
                .ToList()
                .Aggregate(String.Empty, (current, next) => $"{current} {next}");

            return normilizedContent;
        }
    }
}
