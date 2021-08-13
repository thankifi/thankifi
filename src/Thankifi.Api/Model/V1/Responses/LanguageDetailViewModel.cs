using System.Collections.Generic;

namespace Thankifi.Api.Model.V1.Responses
{
    public record LanguageDetailViewModel : LanguageViewModel
    {
        public int Count { get; set; }
        public IEnumerable<GratitudeViewModel> Gratitudes { get; init; }
    }
}