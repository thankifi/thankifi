using MediatR;
using TaaS.Core.Entity;

namespace TaaS.Core.Domain.Query.GetGratitudeRandomByType
{
    public class GetGratitudeRandomByTypeQuery : IRequest<(int, string)>
    {
        public GetGratitudeRandomByTypeQuery(GratitudeType type, string name = "Alice", string signature = "Bob")
        {
            Type = type;
            Name = name;
            Signature = signature;
        }

        public GratitudeType Type { get; set; }
        public string Name { get; }
        public string Signature { get; }
    }
}