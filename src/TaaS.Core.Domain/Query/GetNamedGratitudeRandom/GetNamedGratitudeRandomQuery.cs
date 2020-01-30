using MediatR;

namespace TaaS.Core.Domain.Query.GetNamedGratitudeRandom
{
    public class GetRandomGratitudeNamedQuery : IRequest<(int, string)>
    {
        public GetRandomGratitudeNamedQuery(string name = "Alice")
        {
            Name = name;
        }

        public string Name { get; }
    }
}