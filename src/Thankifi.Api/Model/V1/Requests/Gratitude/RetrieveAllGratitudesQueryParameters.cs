using Thankifi.Common.Pagination;

namespace Thankifi.Api.Model.V1.Requests.Gratitude
{
    public record RetrieveAllGratitudesQueryParameters : QueryStringParameters
    {
        /// <summary>
        /// Subject receiving the gratitude.
        /// </summary>
        public string? Subject { get; init; }
        
        /// <summary>
        /// Signature to attach at the end of the gratitude.
        /// </summary>
        public string? Signature { get; init; }

        /// <summary>
        /// List of flavours to apply to the gratitude.
        /// </summary>
        public string[]? Flavours {get; init; }
    }
}