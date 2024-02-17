#nullable enable

using System.Collections.Generic;
using BurstChat.Domain.Schema.Alpha;

namespace BurstChat.Infrastructure.Options;

public class AlphaInvitationCodesOptions
{
    public IEnumerable<AlphaInvitation>? AlphaCodes { get; set; }
}
