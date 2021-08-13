using System.Collections.Generic;
using Incremental.Common.Pagination;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;

namespace Thankifi.Core.Domain.Contract.Category.Dto
{
    public record CategoryDetailDto : CategoryDto
    {
        public int Count { get; init; }
        public PaginatedList<GratitudeDto> Gratitudes { get; init; }
    }
}