#nullable enable

using System.Collections.Generic;
using BurstChat.Domain.Schema.Alpha;

namespace BurstChat.Infrastructure.Options
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
