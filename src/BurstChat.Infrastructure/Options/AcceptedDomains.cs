using System.Collections.Generic;
using System.Linq;

namespace BurstChat.Infrastructure.Options
{
    public class AcceptedDomainsOptions
    {
        /// <summary>
        ///   The various domains accepted by the cors policy.
        /// </summary>
        public IEnumerable<string> Cors { get; set; } = Enumerable.Empty<string>();
    }
}
