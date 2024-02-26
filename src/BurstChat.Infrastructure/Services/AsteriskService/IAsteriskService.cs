using System;
using System.Threading.Tasks;
using BurstChat.Application.Models;
using BurstChat.Application.Monads;

namespace BurstChat.Infrastructure.Services.AsteriskService;

public interface IAsteriskService
{
    Task<Result<AsteriskEndpoint>> GetAsync(string endpoint);

    Task<Result<AsteriskEndpoint>> PostAsync(string endpoint, Guid password);
}
