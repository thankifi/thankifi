using System.Collections.Generic;
using MediatR;
using Thankify.Core.Domain.Gratitude.Dto;

namespace Thankify.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitude
{
    public class GetBulkAllFiltersGratitudeQuery : IRequest<IEnumerable<GratitudeDto>>
    {
        public GetBulkAllFiltersGratitudeQuery()
        {
            Language = "eng";
            Different = false;
        }
        
        public string? Name { get; set; }
        public string? Signature { get; set; }
        public string Language { get; set; }
        public string? Category { get; set; }
        public bool Different { get; set; }
    }
}