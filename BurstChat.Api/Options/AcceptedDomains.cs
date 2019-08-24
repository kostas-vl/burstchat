using System;
using System.Collections.Generic;

namespace BurstChat.Api.Options
{
    public class AcceptedDomainsOptions
    {
        /// <summary>
        ///   The various domains accepted by the cors policy.
        /// </summary>
        public IEnumerable<string> Cors
        {
            get; set;
        }
    }
}