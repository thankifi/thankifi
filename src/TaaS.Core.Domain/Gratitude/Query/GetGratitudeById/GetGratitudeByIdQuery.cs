using MediatR;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeById
{
    public class GetGratitudeByIdQuery : IRequest<Entity.Gratitude>
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