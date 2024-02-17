using System.Threading.Tasks;
using BurstChat.Application.Monads;

namespace BurstChat.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task<Result<Unit>> SendOneTimePasswordAsync(string recipient, string oneTimePassword);
}
