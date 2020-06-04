using System.Collections.Generic;

namespace BurstChat.Application.Models
{
    /// <summary>
    /// This class represents the base body required for a request to Asterisk's ARI.
    /// </summary>
    public class AsteriskFields
    {
        /// <summary>
        /// The list of fields for the request.
        /// </summary>
        public List<AsteriskProperty> Fields { get; set; }
    }
}
