using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BurstChat.Application.Errors;
using BurstChat.Application.Monads;
using BurstChat.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BurstChat.Infrastructure.Services.EmailService
{
    /// <summary>
    /// This class is the base implementation of the IEmailService.
    /// </summary>
    public class EmailProvider : IEmailService, IDisposable
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<EmailProvider> _logger;
        private readonly SmtpOptions _smtpOptions;
        private readonly SmtpClient _smtpClient;

        /// <summary>
        /// Creates a new instance of EmailProvider.
        /// </summary>
        public EmailProvider(
            IWebHostEnvironment webHostEnvironment,
            ILogger<EmailProvider> logger,
            IOptions<SmtpOptions> smtpOptions
        )
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _smtpOptions = smtpOptions?.Value ?? throw new Exception("SmtpOptions are required");
            _smtpClient = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port)
            {
                Credentials = new NetworkCredential(_smtpOptions.Username, _smtpOptions.Password)
            };
        }

        /// <summary>
        /// Sends an email to the provided recipient that contains an one time password
        /// </summary>
        /// <param name="recipient">The recipient email address</param>
        /// <param name="oneTimePassword">The one time password to be sent</param>
        /// <returns>A task of an either monad</returns>
        public async Task<Either<Unit, Error>> SendOneTimePasswordAsync(string recipient, string oneTimePassword)
        {
            try
            {
                var sender = _smtpOptions.Sender;
                var subject = "BurstChat reset password";
                var body = $"BurstChat account one time password: {oneTimePassword}";
                await _smtpClient.SendMailAsync(sender, recipient, subject, body);
                return new Success<Unit, Error>(new Unit());
            }
            catch (Exception e)
            {
                if (_webHostEnvironment.IsDevelopment())
                {
                    _logger.LogInformation($"[email: {recipient}] one time pass {oneTimePassword}");
                    return new Success<Unit, Error>(new Unit());
                }

                _logger.LogError(e.Message);
                return new Failure<Unit, Error>(SystemErrors.Exception());
            }
        }

        /// <summary>
        /// Executes any necessary code for the disposal of the service.
        /// </summary>
        public void Dispose()
        {
            _smtpClient?.Dispose();
        }
    }
}
