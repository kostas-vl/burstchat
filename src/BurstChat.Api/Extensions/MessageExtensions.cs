using System;
using System.Collections.Generic;
using System.Linq;
using BurstChat.Domain.Schema.Chat;

namespace BurstChat.Api.Extensions;

public static class MessageExtensions
{
    public static List<Link> GetLinksFromContent(this Message message)
    {
        var words = message.Content.Split(" ");

        var links = words
            .Where(word => Uri.TryCreate(word, UriKind.Absolute, out _))
            .Select(uri => new Link { Url = uri, DateCreated = DateTime.UtcNow })
            .ToList();

        return links;
    }

    public static string RemoveLinksFromContent(this Message message)
    {
        var words = message.Content.Split(" ");

        var normilizedContent = words
            .Where(word => !Uri.TryCreate(word, UriKind.Absolute, out _))
            .ToList()
            .Aggregate(String.Empty, (current, next) => $"{current} {next}");

        return normilizedContent;
    }
}
