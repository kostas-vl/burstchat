using System;
using System.Linq;
using System.Collections.Generic;
using BurstChat.Application.Errors;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Monads;
using BurstChat.Application.Extensions;
using BurstChat.Application.Services.ServersService;
using BurstChat.Domain.Schema.Chat;
using BurstChat.Domain.Schema.Servers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.ChannelsService;

public class ChannelsProvider : IChannelsService
{
    private readonly ILogger<ChannelsProvider> _logger;
    private readonly IBurstChatContext _burstChatContext;
    private readonly IServersService _serversService;

    public ChannelsProvider(
        ILogger<ChannelsProvider> logger,
        IBurstChatContext burstChatContext,
        IServersService serversService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _burstChatContext = burstChatContext ?? throw new ArgumentNullException(nameof(burstChatContext));
        _serversService = serversService ?? throw new ArgumentNullException(nameof(serversService));
    }


    private Result<Server> GetServer(long userId, int channelId) => _burstChatContext
        .Map(bc => bc
            .Servers
            .Include(s => s.Channels)
            .Include(s => s.Subscriptions)
            .AsQueryable()
            .FirstOrDefault(s =>
                s.Subscriptions.Any(sub => sub.UserId == userId)
                && s.Channels.Any(c => c.Id == channelId))
        )
        .And(server => server?.Ok() ?? ChannelErrors.ChannelNotFound)
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Channel> Get(long userId, int channelId) => Get(userId, channelId)
        .Map(_ => _burstChatContext.Channels.FirstOrDefault(c => c.Id == channelId))
        .And(channel => channel is Channel { IsPublic: true } ? channel.Ok() : ChannelErrors.ChannelNotFound)
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Channel> Insert(long userId, int serverId, Channel channel) => _serversService
        .Get(userId, serverId)
        .Map(server =>
        {
            var channelEntry = new Channel
            {
                Name = channel.Name,
                IsPublic = channel.IsPublic,
                DateCreated = DateTime.UtcNow
            };
            server.Channels.Add(channelEntry);
            _burstChatContext.SaveChanges();
            return channelEntry;
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Channel> Update(long userId, Channel channel) => channel
        .And(channel => channel?.Ok() ?? ChannelErrors.ChannelNotFound)
        .And(channel => Get(userId, channel.Id))
        .Map(channelEntry =>
        {
            channelEntry.Name = channel.Name;
            channelEntry.IsPublic = channel.IsPublic;
            _burstChatContext.SaveChanges();
            return channelEntry;
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Channel> Delete(long userId, int channelId) => Get(userId, channelId)
        .Map(channel =>
        {
            _burstChatContext.Channels.Remove(channel);
            _burstChatContext.SaveChanges();
            return channel;
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<IEnumerable<Message>> GetMessages(
        long userId, int channelId, string? searchTerm = null, long? lastMessageId = null
    ) => GetServer(userId, channelId)
        .And(_ => Get(userId, channelId))
        .Map(_ => _burstChatContext
            .Channels
            .Include(c => c.Messages)
            .ThenInclude(m => m.User)
            .Include(c => c.Messages)
            .ThenInclude(m => m.Links)
            .Where(c => c.Id == channelId)
            .Select(c => c.Messages
                          .Where(m => m.Id < (lastMessageId ?? long.MaxValue)
                                      && (searchTerm == null || m.Content.Contains(searchTerm)))
                          .OrderByDescending(m => m.Id)
                          .Take(100))
            .ToList()
            .SelectMany(_ => _)
            .OrderBy(m => m.Id)
            .ToList()
            .AsEnumerable()
        )
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Message> InsertMessage(long userId, int channelId, Message message) => Get(userId, channelId)
        .And(channel =>
        {
            var user = _burstChatContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user is null || message is null)
                return ChannelErrors.ChannelNotFound;

            var newMessage = new Message
            {
                User = user,
                Links = message.GetLinksFromContent(),
                Content = message.RemoveLinksFromContent(),
                Edited = false,
                DatePosted = DateTime.UtcNow
            };

            channel.Messages.Add(newMessage);
            _burstChatContext.SaveChanges();
            return newMessage.Ok();
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Message> UpdateMessage(long userId, int channelId, Message message) => message
        .And(m => m?.Ok() ?? ChannelErrors.ChannelMessageNotFound)
        .And(m =>
        {
            var entries = _burstChatContext
                .Channels
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .Include(c => c.Messages)
                .ThenInclude(m => m.Links)
                .Where(c => c.Id == channelId)
                .Select(c => c.Messages.FirstOrDefault(m => m.Id == message.Id))
                .ToList();

            if (entries.Count != 1) return ChannelErrors.ChannelMessageNotFound;

            var entry = entries.First()!;
            entry.Links = message.GetLinksFromContent();
            entry.Content = message.RemoveLinksFromContent();
            entry.Edited = true;
            _burstChatContext.SaveChanges();
            return entry.Ok();
        })
        .InspectErr(e => _logger.LogError(e.Message));


    public Result<Message> DeleteMessage(long userId, int channelId, long messageId) => _burstChatContext
        .Map(bc => bc.Channels.Include(c => c.Messages.Where(m => m.Id == messageId)).First(c => c.Id == channelId))
        .And(channel => channel.Messages.Any() ? channel.Ok() : ChannelErrors.ChannelMessageNotFound)
        .Map(channel =>
        {
            var message = channel.Messages.First();
            channel.Messages.Remove(message);
            _burstChatContext.SaveChanges();
            return message;
        })
        .InspectErr(e => _logger.LogError(e.Message));
}
