using System;
using System.Threading.Tasks;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;

namespace BurstChat.Shared.Services.EmailService
{
    /// <summary>
    /// This interface is a contract that can be used for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email to the provided recipient that contains an one time password
        /// </summary>
        /// <param name="recipient">The recipient email address</param>
        /// <param name="oneTimePassword">The one time password to be sent</param>
        /// <returns>A task of an either monad</returns> 
        Task<Either<Unit, Error>> SendOneTimePasswordAsync(string recipient, string oneTimePassword);
    }
}
