using System;
using System.Linq;
using System.Net.Mail;
using BurstChat.Application.Errors;
using BurstChat.Application.Models;
using BurstChat.Application.Monads;

namespace BurstChat.Application.Services.ModelValidationService
{
    /// <summary>
    /// This class is the base implementation of the IModelValidationService.
    /// </summary>
    public class ModelValidationProvider : IModelValidationService
    {
        /// <summary>
        /// This method will check if the provided credentials instance has a value.
        /// </summary>
        /// <param name="credentials">The credentials instance to be checked</param>
        /// <returns>An either monad</returns>
        public Either<Credentials, Error> CredentialsHasValue(Credentials credentials) =>
            credentials is { }
                ? new Success<Credentials, Error>(credentials)
                : new Failure<Credentials, Error>(ModelErrors.CredentialsNotProvided());

        /// <summary>
        /// This method will check if the provided registration instance has a value.
        /// </summary>
        /// <param name="registration">The registration instance to be checked</param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> RegistrationHasValue(Registration registration) =>
            registration is { }
                ? new Success<Registration, Error>(registration)
                : new Failure<Registration, Error>(ModelErrors.RegistrationNotProvided());

        /// <summary>
        /// This method will check if the provided registration contains an alpha invitation code.
        /// </summary>
        /// <param name="registration">The registration instance to be checked</param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> AlphaInvitationCodeProvided(Registration registration) =>
            registration.AlphaInvitationCode != Guid.Empty
                ? new Success<Registration, Error>(registration)
                : new Failure<Registration, Error>(AlphaInvitationErrors.AlphaInvitationCodeIsNotValid());

        /// <summary>
        /// This method will check if the provided registration instance has a valid user name provided.
        /// </summary>
        /// <param name="registration">The registration instance that the user name will be checked</param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> NameIsValid(Registration registration) =>
            !String.IsNullOrEmpty(registration.Name)
            && !String.IsNullOrWhiteSpace(registration.Name)
                ? new Success<Registration, Error>(registration)
                : new Failure<Registration, Error>(ModelErrors.NameInvalid());

        /// <summary>
        /// This method will check the provided email under all necessary rules.
        /// </summary>
        /// <param name="email">The email value</param>
        /// <returns>A boolean that represents if the password meets all requirements</returns>
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

        /// <summary>
        /// This method will check if the credentials email has a proper value.
        /// </summary>
        /// <param name="credentials">The credentials instance that contains the email</param>
        /// <returns>An either monad</returns>
        private Either<Credentials, Error> EmailIsValid(Credentials credentials) =>
            EmailIsValid(credentials.Email)
                ? new Success<Credentials, Error>(credentials)
                : new Failure<Credentials, Error>(ModelErrors.EmailInvalid());

        /// <summary>
        /// This method will check if the registration email has a value.
        /// </summary>
        /// <param name="registration">The registration instance that contains the email</param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> EmailIsValid(Registration registration) =>
            EmailIsValid(registration.Email)
                ? new Success<Registration, Error>(registration)
                : new Failure<Registration, Error>(ModelErrors.EmailInvalid());

        /// <summary>
        /// This method will check if the change password email has a value.
        /// </summary>
        /// <param name="changePassword">The change password instance that contains the email</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> EmailIsValid(ChangePassword changePassword) =>
            EmailIsValid(changePassword.Email)
                ? new Success<ChangePassword, Error>(changePassword)
                : new Success<ChangePassword, Error>(changePassword);

        /// <summary>
        /// This method will check the provided password under all neccessary rules.
        /// </summary>
        /// <param name="password">The password value</param>
        /// <returns>A boolean that represents if the password meets all requirements</returns>
        private bool PasswordIsValid(string password) =>
            !String.IsNullOrEmpty(password)
            && !String.IsNullOrWhiteSpace(password)
            && password.Length >= 12
            && password.Any(c => Char.IsLetterOrDigit(c));

        /// <summary>
        /// This method will check whether the password in the credentials instance satisfies all the
        /// appropriate rules.
        /// </summary>
        /// <param name="credentials">The credentials instance from which the password will be checked</param>
        /// <returns>An either monad</returns>
        private Either<Credentials, Error> PasswordIsValid(Credentials credentials) =>
            PasswordIsValid(credentials.Password)
                ? new Success<Credentials, Error>(credentials)
                : new Failure<Credentials, Error>(ModelErrors.PasswordInvalid());

        /// <summary>
        /// This method will check whether the password in the registration instance satisfies all the
        /// appropriate rules.
        /// </summary>
        /// <param name="registration">The registration instance from which the password will be checked</param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> PasswordIsValid(Registration registration) =>
            PasswordIsValid(registration.Password)
                ? new Success<Registration, Error>(registration)
                : new Failure<Registration, Error>(ModelErrors.PasswordInvalid());

        /// <summary>
        /// This method will check if the confirm password is the same as the registration password.
        /// </summary>
        /// <param name="registration">
        /// The registration instance from which the password properties wil
        /// be checked.
        /// </param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> ConfirmPasswordIsValid(Registration registration) =>
            String.CompareOrdinal(registration.Password, registration.ConfirmPassword) == 0
                ? new Success<Registration, Error>(registration)
                : new Failure<Registration, Error>(ModelErrors.ConfirmPasswordInvalid());
            
        /// <summary>
        /// This method will check whether the change password instance has a value.
        /// </summary>
        /// <param name="changePassword">The change password instance to be checked</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> ChangePasswordHasValue(ChangePassword changePassword) =>
            changePassword is { }
                ? new Success<ChangePassword, Error>(changePassword)
                : new Failure<ChangePassword, Error>(ModelErrors.ChangePasswordNotProvided());

        /// <summary>
        /// This method will check whether the one time password is valid.
        /// </summary>
        /// <param name="changePassword">The change password instance that contains the one time pass</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> OneTimePassIsValid(ChangePassword changePassword) =>
            !String.IsNullOrEmpty(changePassword.OneTimePassword)
            && !String.IsNullOrWhiteSpace(changePassword.OneTimePassword)
                ? new Success<ChangePassword, Error>(changePassword)
                : new Failure<ChangePassword, Error>(ModelErrors.OneTimePasswordNotProvided());

        /// <summary>
        /// This methos will check whether the new password fullfils a set of specific requirements.
        /// </summary>
        /// <param name="changePassword">The changePassword instance that contains the new password</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> NewPasswordIsValid(ChangePassword changePassword) =>
            PasswordIsValid(changePassword.NewPassword)
                ? new Success<ChangePassword, Error>(changePassword)
                : new Failure<ChangePassword, Error>(ModelErrors.PasswordInvalid());

        /// <summary>
        /// This method will check if the new password is the same as the confirm new password.
        /// </summary>
        /// <param name="changePassword">The change password instance from which the properties will be used</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> ConfirmNewPasswordIsValid(ChangePassword changePassword) => 
            String.CompareOrdinal(changePassword.NewPassword, changePassword.ConfirmNewPassword) == 0
                ? new Success<ChangePassword, Error>(changePassword)
                : new Failure<ChangePassword, Error>(ModelErrors.ConfirmPasswordInvalid());

        /// <summary>
        ///   This method will perform a series of validations on the provided credentails instance
        ///   and return the appropriate Either monad.
        /// </summary>
        /// <param name="credentials">The credentials model instance to be validated</param>
        public Either<Credentials, Error> ValidateCredentials(Credentials credentials) =>
            CredentialsHasValue(credentials)
                .Bind(EmailIsValid)
                .Bind(PasswordIsValid);

        /// <summary>
        ///   This method will perform a series of validation on the provided registation instance
        ///   and return the appropriate Either monad.
        /// </summary>
        /// <param name="registation">The registration model instance to be validated</param>
        /// <returns>An either monad</returns>
        public Either<Registration, Error> ValidateRegistration(Registration registration) =>
            RegistrationHasValue(registration)
                .Bind(AlphaInvitationCodeProvided)
                .Bind(NameIsValid)
                .Bind(EmailIsValid)
                .Bind(PasswordIsValid)
                .Bind(ConfirmPasswordIsValid);

        /// <summary>
        ///   This method will perform a series of validations on the provide change password instance
        ///   and return the appropriate Either monad.
        /// </summary>
        /// <param name="changePassword">The change password model instance to be validated</param>
        /// <returns>An either monad</returns>
        public Either<ChangePassword, Error> ValidateChangePassword(ChangePassword changePassword) =>
            ChangePasswordHasValue(changePassword)
                .Bind(EmailIsValid)
                .Bind(OneTimePassIsValid)
                .Bind(NewPasswordIsValid)
                .Bind(ConfirmNewPasswordIsValid);
    }
}
