using System;

namespace BurstChat.Application.Errors
{
    /// <summary>
    /// This class contains static methods that represent errors associated with alpha invitations errors
    /// when the application tries to either fetch or transform data.
    /// </summary>
    public static class AlphaInvitationErrors
    {
        public static Error AlphaInvitationCodeIsNotValid() => new(ErrorLevel.Critical,
                                                                   ErrorType.Validation,
                                                                   "The alpha invitation code is not valid");
    }
}
