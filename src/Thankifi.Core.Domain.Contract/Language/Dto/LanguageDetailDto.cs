using Incremental.Common.Pagination;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;

namespace Thankifi.Core.Domain.Contract.Language.Dto
{
    public record LanguageDetailDto : LanguageDto
    {
        public int Count { get; init; }
        public PaginatedList<GratitudeDto> Gratitudes { get; init; }
    }
}