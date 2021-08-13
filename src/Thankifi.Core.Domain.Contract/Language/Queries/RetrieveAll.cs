using Incremental.Common.Sourcing.Abstractions.Queries;
using Incremental.Common.Pagination;
using Thankifi.Core.Domain.Contract.Language.Dto;

namespace Thankifi.Core.Domain.Contract.Language.Queries
{
    public record RetrieveAll : IQuery<PaginatedList<LanguageDto>>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
    }
}