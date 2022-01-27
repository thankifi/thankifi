using Incremental.Common.Sourcing.Abstractions.Queries;
using Thankifi.Core.Domain.Contract.Category.Dto;

namespace Thankifi.Core.Domain.Contract.Category.Queries;

public record RetrieveBySlug : IQuery<CategoryDetailDto?>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string Slug { get; init; }
    public string? Subject { get; init; }
    public string? Signature { get; init; }
    public string[]? Flavours { get; init; }
    public string[]? Languages { get; init; }
}