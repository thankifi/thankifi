using System.Collections.Generic;

namespace Thankifi.Api.Model.V1.Responses;

public record GratitudeFlavourfulViewModel : GratitudeViewModel
{
    public IEnumerable<FlavourViewModel> Flavours { get; init; }
}