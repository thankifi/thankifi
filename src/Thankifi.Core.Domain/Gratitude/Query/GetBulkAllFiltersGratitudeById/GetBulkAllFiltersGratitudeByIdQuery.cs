using System.Collections.Generic;
using MediatR;
using Thankifi.Core.Domain.Gratitude.Dto;

namespace Thankifi.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitudeById
{
    public class GetBulkAllFiltersGratitudeByIdQuery : IRequest<IEnumerable<GratitudeDto>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Signature { get; set; }
    }
}