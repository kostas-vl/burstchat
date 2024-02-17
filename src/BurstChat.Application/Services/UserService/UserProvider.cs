using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BurstChat.Application.Errors;
using BurstChat.Application.Interfaces;
using BurstChat.Application.Models;
using BurstChat.Application.Monads;
using BurstChat.Application.Services.BCryptService;
using BurstChat.Domain.Schema.Servers;
using BurstChat.Domain.Schema.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurstChat.Application.Services.UserService;

public class UserProvider : IUserService
{
    private readonly ILogger<UserProvider> _logger;
    private readonly IBurstChatContext _burstChatContext;
    private readonly IBCryptService _bcryptService;

    public UserProvider(
        ILogger<UserProvider> logger,
        IBurstChatContext burstChatContext,
        IBCryptService bcryptService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _burstChatContext =
            burstChatContext ?? throw new ArgumentNullException(nameof(burstChatContext));
        _bcryptService = bcryptService ?? throw new ArgumentNullException(nameof(bcryptService));
    }

    private Result<Unit> AlphaInvitationCodeExists(Guid alphaInvitationCode) =>
        _burstChatContext
            .Map(bc =>
                bc.AlphaInvitations.Any(a =>
                    a.Code == alphaInvitationCode && a.DateExpired >= DateTime.UtcNow
                )
            )
            .And(codeExists =>
                codeExists ? Unit.Ok : AlphaInvitationErrors.AlphaInvitationCodeIsNotValid
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<User> Get(long id) =>
        _logger
            .Inspect(logger => logger.LogInformation($"New user request with id: {id}"))
            .Map(_ => _burstChatContext.Users.FirstOrDefault(u => u.Id == id))
            .And(user => user?.Ok() ?? UserErrors.UserNotFound)
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<User> Get(string email) =>
        _burstChatContext
            .Map(bc => bc.Users.FirstOrDefault(u => u.Email == email))
            .And(u => u?.Ok() ?? UserErrors.UserNotFound)
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<User> Insert(
        Guid alphaInvitationCode,
        string email,
        string name,
        string password
    ) =>
        Get(email)
            .And<User>(_ => UserErrors.UserAlreadyExists)
            .And(_ => AlphaInvitationCodeExists(alphaInvitationCode))
            .Map(_ =>
            {
                var hashedPassword = _bcryptService.GenerateHash(password);
                var user = new User
                {
                    Email = email,
                    Name = name,
                    Password = hashedPassword,
                    DateCreated = DateTime.UtcNow
                };
                _burstChatContext.Users.Add(user);
                _burstChatContext.SaveChanges();
                return user;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<User> Update(User user) =>
        (user?.Ok() ?? UserErrors.UserNotFound)
            .And(user => Get(user.Id))
            .Map(storedUser =>
            {
                storedUser.Email = user!.Email;
                storedUser.Name = user!.Name;
                storedUser.Avatar = user!.Avatar;
                _burstChatContext.SaveChanges();
                return storedUser;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Unit> Delete(long id) =>
        Get(id)
            .Map(user =>
            {
                _burstChatContext.Users.Remove(user);
                return Unit.Instance;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<IEnumerable<Server>> GetSubscriptions(long userId) =>
        _burstChatContext
            .Map(bc =>
                bc.Servers.Include(server => server.Subscriptions)
                    .Where(server =>
                        server.Subscriptions.Any(subscription => subscription.UserId == userId)
                    )
                    .ToList()
                    .AsEnumerable()
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<IEnumerable<PrivateGroup>> GetPrivateGroups(long userId) =>
        Get(userId)
            .Map(user =>
                _burstChatContext
                    .PrivateGroups.Include(pmg => pmg.Users.Where(u => u.Id == user.Id))
                    .Include(pmg => pmg.Messages)
                    .ToList()
                    .AsEnumerable()
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<IEnumerable<DirectMessaging>> GetDirectMessaging(long userId) =>
        Get(userId)
            .Map(user =>
                _burstChatContext
                    .DirectMessaging.Where(d =>
                        d.FirstParticipantUserId == user.Id || d.SecondParticipantUserId == user.Id
                    )
                    .ToList()
                    .AsEnumerable()
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<User> Validate(string email, string password) =>
        Get(email)
            .And(user =>
                _bcryptService.VerifyHash(password, user.Password)
                    ? user.Ok()
                    : UserErrors.UserPasswordDidNotMatch
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<string> IssueOneTimePassword(string email) =>
        _burstChatContext
            .And(bc =>
            {
                var user = _burstChatContext
                    .Users.Include(u => u.OneTimePasswords)
                    .FirstOrDefault(u => u.Email == email);

                if (user is null)
                    return UserErrors.UserNotFound;

                var dateCreated = DateTime.UtcNow;
                // TODO: Implement a better solution for one time pass generation.
                var timedOneTimePass = Guid.NewGuid().ToString();
                var oneTimePassword = new OneTimePassword
                {
                    OTP = _bcryptService.GenerateHash(timedOneTimePass),
                    DateCreated = dateCreated,
                    ExpirationDate = dateCreated.AddMinutes(15)
                };
                user.OneTimePasswords.Add(oneTimePassword);
                _burstChatContext.SaveChanges();
                return timedOneTimePass.Ok();
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Unit> ChangePassword(string email, string oneTimePass, string password) =>
        _burstChatContext
            .Map(bc => bc.Users.FirstOrDefault(u => u.Email == email))
            .And(user => user?.Ok() ?? UserErrors.UserOneTimePasswordInvalid)
            .And(user =>
            {
                var otp = _burstChatContext
                    .Users.Where(u => u.Id == user.Id)
                    .Include(u => u.OneTimePasswords)
                    .Select(u => u.OneTimePasswords.Where(o => o.ExpirationDate >= DateTime.UtcNow))
                    .AsEnumerable()
                    .SelectMany(_ => _)
                    .FirstOrDefault(o => _bcryptService.VerifyHash(oneTimePass, o.OTP));
                return otp is not null ? user.Ok() : UserErrors.UserOneTimePasswordInvalid;
            })
            .Map(user =>
            {
                var hashedPassword = _bcryptService.GenerateHash(password);
                user.Password = hashedPassword;
                _burstChatContext.SaveChanges();
                return Unit.Instance;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<IEnumerable<Invitation>> GetInvitations(long userId) =>
        _burstChatContext
            .Map(bc =>
                bc.Invitations.Include(i => i.Server)
                    .Include(i => i.User)
                    .Where(i => i.UserId == userId && !i.Accepted && !i.Declined)
                    .ToList()
                    .AsEnumerable()
            )
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<Invitation> UpdateInvitation(long userId, UpdateInvitation data) =>
        _burstChatContext
            .Map(bc =>
                bc.Invitations.Include(i => i.Server)
                    .Include(i => i.User)
                    .FirstOrDefault(i => i.Id == data.InvitationId && i.UserId == userId)
            )
            .And(inv => inv?.Ok() ?? UserErrors.CouldNotUpdateInvitation)
            .Map(inv =>
            {
                inv.Accepted = data.Accepted;
                inv.Declined = !data.Accepted;
                inv.DateUpdated = DateTime.UtcNow;

                if (inv.Accepted)
                {
                    var subscription = new Subscription
                    {
                        ServerId = inv.ServerId,
                        UserId = inv.UserId,
                    };
                    _burstChatContext.Subscriptions.Add(subscription);
                }

                _burstChatContext.SaveChanges();
                return inv;
            })
            .InspectErr(e => _logger.LogError(e.Message));

    public Result<IEnumerable<Claim>> GetClaims(User user) =>
        (user?.Ok() ?? UserErrors.UserNotFound).Map(user =>
            new Claim[]
            {
                new Claim("email", user.Email),
                new Claim("sub", user.Id.ToString()),
                new Claim("username", user.Name)
            }.AsEnumerable()
        );
}
