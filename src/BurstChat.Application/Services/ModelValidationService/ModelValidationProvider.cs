using System;
using System.Linq;
using System.Net.Mail;
using BurstChat.Application.Errors;
using BurstChat.Application.Models;
using BurstChat.Application.Monads;

namespace BurstChat.Application.Services.ModelValidationService;

public class ModelValidationProvider : IModelValidationService
{
    public Result<Credentials> CredentialsHasValue(Credentials credentials) =>
        credentials?.Ok() ?? ModelErrors.CredentialsNotProvided;

    private Result<Registration> RegistrationHasValue(Registration registration) =>
        registration?.Ok() ?? ModelErrors.RegistrationNotProvided;

    private Result<Registration> AlphaInvitationCodeProvided(Registration registration) =>
        registration.AlphaInvitationCode != Guid.Empty
            ? registration.Ok()
            : AlphaInvitationErrors.AlphaInvitationCodeIsNotValid;

    private Result<Registration> NameIsValid(Registration registration) =>
        !String.IsNullOrEmpty(registration.Name)
        && !String.IsNullOrWhiteSpace(registration.Name)
            ? registration.Ok()
            : ModelErrors.NameInvalid;

    private bool EmailIsValid(string email)
    {
        var notEmpty = !String.IsNullOrEmpty(email)
                       && !String.IsNullOrWhiteSpace(email);

        if (notEmpty)
        {
            try
            {
                var address = new MailAddress(email);
            }
            catch
            {
                return false;
            }

            return true;
        }

        return false;
    }

    private Result<Credentials> EmailIsValid(Credentials credentials) =>
        EmailIsValid(credentials.Email) ? credentials.Ok() : ModelErrors.EmailInvalid;

    private Result<Registration> EmailIsValid(Registration registration) =>
        EmailIsValid(registration.Email) ? registration.Ok() : ModelErrors.EmailInvalid;

    private Result<ChangePassword> EmailIsValid(ChangePassword changePassword) =>
        EmailIsValid(changePassword.Email) ? changePassword.Ok() : ModelErrors.EmailInvalid;

    private bool PasswordIsValid(string password) =>
        !String.IsNullOrEmpty(password)
        && !String.IsNullOrWhiteSpace(password)
        && password.Length >= 12
        && password.Any(c => Char.IsLetterOrDigit(c));

    private Result<Credentials> PasswordIsValid(Credentials credentials) =>
        PasswordIsValid(credentials.Password) ? credentials.Ok() : ModelErrors.PasswordInvalid;

    private Result<Registration> PasswordIsValid(Registration registration) =>
        PasswordIsValid(registration.Password) ? registration.Ok() : ModelErrors.PasswordInvalid;

    private Result<Registration> ConfirmPasswordIsValid(Registration registration) =>
        String.CompareOrdinal(registration.Password, registration.ConfirmPassword) == 0
            ? registration.Ok()
            : ModelErrors.ConfirmPasswordInvalid;

    private Result<ChangePassword> ChangePasswordHasValue(ChangePassword changePassword) =>
        changePassword?.Ok() ?? ModelErrors.ChangePasswordNotProvided;

    private Result<ChangePassword> OneTimePassIsValid(ChangePassword changePassword) =>
        !String.IsNullOrEmpty(changePassword.OneTimePassword)
        && !String.IsNullOrWhiteSpace(changePassword.OneTimePassword)
            ? changePassword.Ok()
            : ModelErrors.OneTimePasswordNotProvided;

    private Result<ChangePassword> NewPasswordIsValid(ChangePassword changePassword) =>
        PasswordIsValid(changePassword.NewPassword)
            ? changePassword.Ok()
            : ModelErrors.PasswordInvalid;

    private Result<ChangePassword> ConfirmNewPasswordIsValid(ChangePassword changePassword) =>
        String.CompareOrdinal(changePassword.NewPassword, changePassword.ConfirmNewPassword) == 0
            ? changePassword.Ok()
            : ModelErrors.ConfirmPasswordInvalid;

    public Result<Credentials> ValidateCredentials(Credentials credentials) =>
        CredentialsHasValue(credentials)
            .And(EmailIsValid)
            .And(PasswordIsValid);

    public Result<Registration> ValidateRegistration(Registration registration) =>
        RegistrationHasValue(registration)
            .And(AlphaInvitationCodeProvided)
            .And(NameIsValid)
            .And(EmailIsValid)
            .And(PasswordIsValid)
            .And(ConfirmPasswordIsValid);

    public Result<ChangePassword> ValidateChangePassword(ChangePassword changePassword) =>
        ChangePasswordHasValue(changePassword)
            .And(EmailIsValid)
            .And(OneTimePassIsValid)
            .And(NewPasswordIsValid)
            .And(ConfirmNewPasswordIsValid);
}
