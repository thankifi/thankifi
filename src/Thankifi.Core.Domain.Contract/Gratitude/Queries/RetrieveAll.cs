using Incremental.Common.Sourcing.Abstractions.Queries;
using Incremental.Common.Pagination;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;

namespace Thankifi.Core.Domain.Contract.Gratitude.Queries;

public record RetrieveAll : Query<PaginatedList<GratitudeDto>>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string? Subject { get; init; }
    public string? Signature { get; init; }
    public string[]? Flavours {get; init; }
}