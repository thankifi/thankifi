using System;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Common.Pagination;

namespace Thankifi.Api.Model.V1.Requests.Gratitude
{
    public record RetrieveGratitudeByIdFlavourfulQueryParameters
    {
        /// <summary>
        /// Subject receiving the gratitude.
        /// </summary>
        public string? Subject { get; init; }
        
        /// <summary>
        /// Signature to attach at the end of the gratitude.
        /// </summary>
        public string? Signature { get; init; }
    }
}