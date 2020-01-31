using MediatR;

namespace TaaS.Core.Domain.Query.GetGratitudeById
{
    public class GetGratitudeByIdQuery : IRequest<(int, string)>
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