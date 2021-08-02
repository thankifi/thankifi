using System.Collections.Generic;
using Thankifi.Core.Domain.Contract.Category.Dto;

namespace Thankifi.Core.Domain.Contract.Gratitude.Dto
{
    public record GratitudeDto
    {
        public int Id { get; init; }
        public string Language { get; init; }
        public string Text { get; init; }
        public IEnumerable<CategoryDto> Categories { get; init; }
    }
}