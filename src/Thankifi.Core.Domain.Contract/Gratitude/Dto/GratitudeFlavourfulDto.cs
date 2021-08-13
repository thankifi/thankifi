using System.Collections.Generic;

namespace Thankifi.Core.Domain.Contract.Gratitude.Dto
{
    public record GratitudeFlavourfulDto : GratitudeDto
    {
        public IEnumerable<FlavourDto> Flavours { get; init; }
    }
}