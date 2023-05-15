using Incremental.Common.Sourcing.Abstractions.Queries;
using Incremental.Common.Pagination;
using Thankifi.Core.Domain.Contract.Category.Dto;

namespace Thankifi.Core.Domain.Contract.Category.Queries;

public record RetrieveAll : Query<PaginatedList<CategoryDto>>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}