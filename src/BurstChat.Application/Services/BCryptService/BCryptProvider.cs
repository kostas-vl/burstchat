namespace BurstChat.Application.Services.BCryptService
{
    /// <summary>
    /// This class is the base implementation of the IBCryptService interface.
    /// </summary>
    public class BCryptProvider : IBCryptService
    {
        private readonly int _workFactor = 16;
        private readonly BCrypt.Net.HashType _hashType = BCrypt.Net.HashType.SHA384;

        /// <summary>
        /// This method will generate an appropriate hash for the provided value parameter.
        /// </summary>
        /// <param name="value">The string value to be hashed</param>
        /// <returns>The hashed string</returns>
        public string GenerateHash(string value)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(value, _workFactor, _hashType);
        }

        /// <summary>
        /// This method will verify the provided value against the provided hash.
        /// </summary>
        /// <param name="value">The string value to be checked</param>
        /// <param name="hash">The hash value to be checked</param>
        /// <returns>
        /// A boolean that specifies whether the value can be transformed into the provided hash
        /// </returns>
        public bool VerifyHash(string value, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(value, hash, _hashType);
        }
    }
}
