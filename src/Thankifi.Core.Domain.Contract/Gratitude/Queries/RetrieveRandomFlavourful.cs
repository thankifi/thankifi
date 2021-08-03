using Incremental.Common.Sourcing.Abstractions.Queries;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;

namespace Thankifi.Core.Domain.Contract.Gratitude.Queries
{
    public record RetrieveRandomFlavourful : IQuery<GratitudeFlavourfulDto>
    {
        public string? Subject { get; init; }
        public string? Signature { get; init; }
        public string[]? Categories { get; init; }
        public string[]? Languages { get; set; }
    }
}