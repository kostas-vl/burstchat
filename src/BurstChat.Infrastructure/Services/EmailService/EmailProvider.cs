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

namespace BurstChat.Infrastructure.Services.EmailService;

public class EmailProvider : IEmailService, IDisposable
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<EmailProvider> _logger;
    private readonly SmtpOptions _smtpOptions;
    private readonly SmtpClient _smtpClient;

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

    public async Task<Result<Unit>> SendOneTimePasswordAsync(
        string recipient,
        string oneTimePassword
    )
    {
        try
        {
            var sender = _smtpOptions.Sender;
            var subject = "BurstChat reset password";
            var body = $"BurstChat account one time password: {oneTimePassword}";
            await _smtpClient.SendMailAsync(sender, recipient, subject, body);
            return Unit.Ok;
        }
        catch (Exception e)
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                _logger.LogInformation($"[email: {recipient}] one time pass {oneTimePassword}");
                return Unit.Ok;
            }

            _logger.LogError(e.Message);
            return SystemErrors.Exception;
        }
    }

    public void Dispose()
    {
        _smtpClient?.Dispose();
    }
}
