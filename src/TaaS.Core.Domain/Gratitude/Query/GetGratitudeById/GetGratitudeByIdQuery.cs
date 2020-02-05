using System.Collections.Generic;
using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeById
{
    public class GetGratitudeByIdQuery : IRequest<GratitudeDto?>
    {
        public GetGratitudeByIdQuery()
        {
            Filters = new List<string>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Signature { get; set; }
        public List<string> Filters { get; set; }
    }
}