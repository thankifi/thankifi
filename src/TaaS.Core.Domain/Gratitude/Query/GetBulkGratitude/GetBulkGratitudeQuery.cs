using System.Collections.Generic;
using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;

namespace TaaS.Core.Domain.Gratitude.Query.GetBulkGratitude
{
    public class GetBulkGratitudeQuery : IRequest<IEnumerable<GratitudeDto>>
    {
        private int _quantity;

        public GetBulkGratitudeQuery()
        {
            Quantity = 5;
            Language = "eng";
            Filters = new List<string>();
        }

        public int Quantity
        {
            get => _quantity;
            set => _quantity = value > 50 ? 50 : value;
        }

        public string? Name { get; set; }
        public string? Signature { get; set; }
        public string Language { get; set; }
        public string? Category { get; set; }
        public List<string> Filters { get; set; }
    }
}