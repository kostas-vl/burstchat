using System;

namespace BurstChat.Shared.Errors
{
    /// <summary>
    /// An enumeration that represents the type of error that the application did not
    /// execute properly.
    /// </summary>
    public enum ErrorType
    {
        System,
        DataProcess,
        Validation
    }
}
