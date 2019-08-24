using System;

namespace BurstChat.IdentityServer.Services.BCryptService
{
    /// <summary>
    /// This interface exposes methods for generating and validating hashes using the BCrypt hashing function.
    /// </summary>
    public interface IBCryptService
    {
        /// <summary>
        ///   This method will generate an appropriate hash for the provided value parameter.
        /// </summary>
        /// <param name="value">The string value to be hashed</param>
        /// <returns>The hashed string</returns>
        string GenerateHash(string value);

        /// <summary>
        ///   This method will verify the provided value against the provided hash.
        /// </summary>
        /// <param name="value">The string value to be checked</param>
        /// <param name="hash">The hash value to be checked</param>
        /// <returns>
        ///   A boolean that specifies whether the value can be transformed into the provided hash
        /// </returns>
        bool VerifyHash(string value, string hash);
    }
}
