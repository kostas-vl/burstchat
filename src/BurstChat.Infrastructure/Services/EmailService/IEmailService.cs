using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;

namespace BurstChat.Infrastructure.Services.EmailService;

public interface IEmailService
{
    Task<Either<Unit, Error>> SendOneTimePasswordAsync(string recipient, string oneTimePassword);
}
