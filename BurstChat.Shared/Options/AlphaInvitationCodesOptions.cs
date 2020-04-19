using System;
using System.Collections.Generic;
using BurstChat.Shared.Schema.Alpha;

namespace BurstChat.Shared.Options
{
    /// <summary>
    /// This class contains configuration properties for the Alpha invitation codes.
    /// </summary>
    public class AlphaInvitationCodesOptions
    {
        /// <summary>
        /// An enumerable of all alpha codes.
        /// </summary>
        public IEnumerable<AlphaInvitation>? AlphaCodes { get; set; }
    }
}
