using Thankifi.Common.Pagination;

namespace Thankifi.Api.Model.V1.Requests.Language
{
    public record RetrieveByLanguageQueryParameters : QueryStringParameters
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
        /// List of flavours to apply to the gratitude. Currently available: shouting, mocking, leet.
        /// </summary>
        public string[]? Flavours { get; init; }

        /// <summary>
        /// Filter gratitudes by categories.
        /// </summary>
        public string[]? Categories { get; init; }
    }
}