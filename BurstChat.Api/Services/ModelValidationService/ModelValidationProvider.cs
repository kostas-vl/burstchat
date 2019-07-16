using System;
using System.Linq;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Api.Errors;
using BurstChat.Api.Models;

namespace BurstChat.Api.Services.ModelValidationService
{
    /// <summary>
    ///   This class is the base implementation of the IModelValidationService.
    /// </summary>
    public class ModelValidationProvider : IModelValidationService
    {
        /// <summary>
        ///   This method will check the provided password under all neccessary rules.
        /// </summary>
        /// <param name="password">The password value</param>
        /// <returns>A boolean that represents if the password meets all requirements</returns>
        private bool PasswordIsValid(string password) =>
            !String.IsNullOrEmpty(password)
            && !String.IsNullOrWhiteSpace(password)
            && password.Length >= 12
            && password.Any(c => Char.IsLetterOrDigit(c))
            && password.Any(c => Char.IsSymbol(c));

        /// <summary> 
        ///   This method will check if the provided registration instance has a value.
        /// </summary>
        /// <param name="registration">The registration instance to be checked</param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> RegistrationHasValue(Registration registration)
        {
            if (registration != null)
                return new Success<Registration, Error>(registration);
            else
                return new Failure<Registration, Error>(SystemErrors.Exception());
        }
       
        /// <summary>
        ///   This method will check if the registration email has a value.
        /// </summary>
        /// <param name="registration">The registration instance that contains the email</param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> EmailIsValid(Registration registration)
        {
            var emailHasValue = !String.IsNullOrEmpty(registration.Email) 
                                && !String.IsNullOrWhiteSpace(registration.Email);

            if (emailHasValue)
                return new Success<Registration, Error>(registration);
            else
                return new Failure<Registration, Error>(SystemErrors.Exception());
        }

        /// <summary>
        ///   This method will check whether the password in the registration instance satisfies all the
        ///   appropriate rules.
        /// </summary>
        /// <param name="registration">The registration instance from which the password will be checked</param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> PasswordIsValid(Registration registration)
        {
            var passwordIsValid = PasswordIsValid(registration.Password);

            if (passwordIsValid)
                return new Success<Registration, Error>(registration);
            else
                return new Failure<Registration, Error>(SystemErrors.Exception());
        }

        /// <summary>
        ///   This method will check if the confirm password is the same as the registration password.
        /// </summary>
        /// <param name="registration">
        ///   The registration instance from which the password properties wil
        ///   be checked.
        /// </param>
        /// <returns>An either monad</returns>
        private Either<Registration, Error> ConfirmPasswordIsValid(Registration registration)
        {
            var password = registration.Password;
            var confirmPassword = registration.ConfirmPassword;
            var confirmPasswordIsValid = String.CompareOrdinal(password, confirmPassword) == 0;

            if (confirmPasswordIsValid)
                return new Success<Registration, Error>(registration);
            else
                return new Failure<Registration, Error>(SystemErrors.Exception());
        }

        /// <summary>
        ///   This method will check whether the change password instance has a value.
        /// </summary>
        /// <param name="changePassword">The change password instance to be checked</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> ChangePasswordHasValue(ChangePassword changePassword)
        {
            if (changePassword != null)
                return new Success<ChangePassword, Error>(changePassword);
            else
                return new Failure<ChangePassword, Error>(SystemErrors.Exception());
        }

        /// <summary>
        ///   This method will check whether the one time password is valid.
        /// </summary>
        /// <param name="changePassword">The change password instance that contains the one time pass</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> OneTimePassIsValid(ChangePassword changePassword)
        {
            var oneTimePassIsValid = !String.IsNullOrEmpty(changePassword.OneTimePassword)
                                     && !String.IsNullOrWhiteSpace(changePassword.OneTimePassword);
            if (oneTimePassIsValid)
                return new Success<ChangePassword, Error>(changePassword);
            else
                return new Failure<ChangePassword, Error>(SystemErrors.Exception());
        }

        /// <summary>
        ///   This methos will check whether the new password fullfils a set of specific requirements.
        /// </summary>
        /// <param name="changePassword">The changePassword instance that contains the new password</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> NewPasswordIsValid(ChangePassword changePassword)
        {
            var newPasswordIsValid = PasswordIsValid(changePassword.NewPassword);

            if (newPasswordIsValid)
                return new Success<ChangePassword, Error>(changePassword);
            else
                return new Failure<ChangePassword, Error>(SystemErrors.Exception());
        }

        /// <summary>
        ///   This method will check if the new password is the same as the confirm new password.
        /// </summary>
        /// <param name="changePassword">The change password instance from which the properties will be used</param>
        /// <returns>An either monad</returns>
        private Either<ChangePassword, Error> ConfirmNewPasswordIsValid(ChangePassword changePassword)
        {
            var newPassword = changePassword.NewPassword;
            var confirmNewPassword = changePassword.ConfirmNewPassword;
            var confirmationIsValid = String.CompareOrdinal(newPassword, confirmNewPassword) == 0;

            if (confirmationIsValid)
                return new Success<ChangePassword, Error>(changePassword);
            else
                return new Failure<ChangePassword, Error>(SystemErrors.Exception());
        }

        /// <summary>
        ///   This method will perform a series of validation on the provided registation instance
        ///   and return the appropriate Either monad.
        /// </summary>
        /// <param name="registation">The registration model instance to be validated</param>
        /// <returns>An either monad</returns>
        public Either<Registration, Error> ValidateRegistration(Registration registration) =>
            RegistrationHasValue(registration)
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
                .Bind(OneTimePassIsValid)
                .Bind(NewPasswordIsValid)
                .Bind(ConfirmNewPasswordIsValid);
    }
}
