using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeById
{
    public class GetGratitudeByIdQuery : IRequest<GratitudeDto?>
    {
        public GetGratitudeByIdQuery(int id, string name = "Alice", string signature = "Bob")
        {
            Id = id;
            Name = name;
            Signature = signature;
        }

        public int Id { get; }
        public string Name { get; }
        public string Signature { get; }
    }
}