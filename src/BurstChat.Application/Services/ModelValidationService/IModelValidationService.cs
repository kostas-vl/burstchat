using BurstChat.Application.Models;
using BurstChat.Application.Monads;

namespace BurstChat.Application.Services.ModelValidationService
{
    /// <summary>
    /// This interface exposes methods for validating various API models.
    /// </summary>
    public interface IModelValidationService
    {
        /// <summary>
        /// This method will perform a series of validations on the provided credentails instance
        /// and return the appropriate Result monad.
        /// </summary>
        /// <param name="credentials">The credentials model instance to be validated</param>
        Result<Credentials> ValidateCredentials(Credentials credentials);

        /// <summary>
        /// This method will perform a series of validation on the provided registation instance
        /// and return the appropriate Result monad.
        /// </summary>
        /// <param name="registation">The registration model instance to be validated</param>
        /// <returns>An either monad</returns>
        Result<Registration> ValidateRegistration(Registration registration);

        /// <summary>
        /// This method will perform a series of validations on the provide change password instance
        /// and return the appropriate Result monad.
        /// </summary>
        /// <param name="changePassword">The change password model instance to be validated</param>
        /// <returns>An either monad</returns>
        Result<ChangePassword> ValidateChangePassword(ChangePassword changePassword);
    }
}
