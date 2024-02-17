using System;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Application.Models;

namespace BurstChat.Infrastructure.Services.AsteriskService;

public interface IAsteriskService
{
    Task<Either<AsteriskEndpoint, Error>> GetAsync(string endpoint);

    Task<Either<AsteriskEndpoint, Error>> PostAsync(string endpoint, Guid password);
}
