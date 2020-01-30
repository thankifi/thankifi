using MediatR;

namespace TaaS.Core.Domain.Query.GetGratitudeRandom
{
    public class GetGratitudeRandomQuery : IRequest<(int, string)>
    {
        public GetGratitudeRandomQuery(string name = "Alice", string signature = "Bob")
        {
            Name = name;
            Signature = signature;
        }

        public string Name { get; }
        public string Signature { get; }
    }
}