using MediatR;
using TaaS.Core.Entity;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeRandomByType
{
    public class GetGratitudeRandomByTypeQuery : IRequest<Entity.Gratitude>
    {
        public GetGratitudeRandomByTypeQuery(GratitudeType type, string language = "en", string name = "Alice", string signature = "Bob")
        {
            Language = language.ToLower();
            Type = type;
            Name = name;
            Signature = signature;
        }

        public string Language { get; }
        public GratitudeType Type { get; set; }
        public string Name { get; }
        public string Signature { get; }
    }
}