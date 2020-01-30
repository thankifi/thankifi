using MediatR;

namespace TaaS.Core.Domain.Query.GetNamedGratitudeRandom
{
    public class GetNamedGratitudeRandomQuery : IRequest<(int, string)>
    {
        public GetNamedGratitudeRandomQuery(string name = "Alice")
        {
            Name = name;
        }

        public string Name { get; }
    }
}