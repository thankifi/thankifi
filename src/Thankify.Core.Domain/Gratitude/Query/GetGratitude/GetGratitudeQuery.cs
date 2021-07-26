using System.Collections.Generic;
using MediatR;
using Thankify.Core.Domain.Gratitude.Dto;

namespace Thankify.Core.Domain.Gratitude.Query.GetGratitude
{
    public class GetGratitudeQuery : IRequest<GratitudeDto?>
    {
        public GetGratitudeQuery()
        {
            Language = "eng";
            Filters = new List<string>();
        }

        public string? Name { get; set; }
        public string? Signature { get; set; }
        public string Language { get; set; }
        public string? Category { get; set; }
        public List<string> Filters { get; set; }
    }
}