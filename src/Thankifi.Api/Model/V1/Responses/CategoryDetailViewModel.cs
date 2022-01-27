using System.Collections.Generic;

namespace Thankifi.Api.Model.V1.Responses;

public record CategoryDetailViewModel : CategoryViewModel
{
    public int Count { get; init; }
    public IEnumerable<GratitudeViewModel> Gratitudes { get; init; }
}