using System.Collections.Generic;
using System.Linq;

namespace BurstChat.Infrastructure.Options;

public class AcceptedDomainsOptions
{
    public IEnumerable<string> Cors { get; set; } = Enumerable.Empty<string>();
}
