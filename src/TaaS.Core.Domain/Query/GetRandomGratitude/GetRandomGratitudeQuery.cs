using MediatR;

namespace TaaS.Core.Domain.Query.GetRandomGratitude
{
    public class GetRandomGratitudeQuery : IRequest<string>
    {
        public GetRandomGratitudeQuery(string name = "Alice", string signature = "Bob")
        {
            Name = name;
            Signature = signature;
        }

        public string Name { get; }
        public string Signature { get; }
    }
}