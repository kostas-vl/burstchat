using System;
using BurstChat.Shared.Errors;
using BurstChat.Shared.Monads;
using BurstChat.Api.Models;

namespace BurstChat.Api.Services.ModelValidationService
{
    /// <summary>
    ///   This interface exposes methods for validating various API models.
    /// </summary>
    public interface IModelValidationService
    {
        /// <summary>
        ///   This method will perform a series of validations on the provided credentails instance
        ///   and return the appropriate Either monad.
        /// </summary>
        /// <param name="credentials">The credentials model instance to be validated</param>
        Either<Credentials, Error> ValidateCredentials(Credentials credentials);

        /// <summary>
        ///   This method will perform a series of validation on the provided registation instance
        ///   and return the appropriate Either monad.
        /// </summary>
        /// <param name="registation">The registration model instance to be validated</param>
        /// <returns>An either monad</returns>
        Either<Registration, Error> ValidateRegistration(Registration registration);

        /// <summary>
        ///   This method will perform a series of validations on the provide change password instance
        ///   and return the appropriate Either monad.
        /// </summary>
        /// <param name="changePassword">The change password model instance to be validated</param>
        /// <returns>An either monad</returns>
        Either<ChangePassword, Error> ValidateChangePassword(ChangePassword changePassword);
    }
}
