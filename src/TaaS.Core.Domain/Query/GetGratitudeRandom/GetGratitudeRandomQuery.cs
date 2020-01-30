using MediatR;

namespace TaaS.Core.Domain.Query.GetGratitudeRandom
{
    public class GetRandomGratitudeQuery : IRequest<(int, string)>
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