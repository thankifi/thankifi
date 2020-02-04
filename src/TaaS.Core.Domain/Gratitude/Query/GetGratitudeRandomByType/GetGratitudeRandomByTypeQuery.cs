using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;
using TaaS.Core.Entity;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeRandomByType
{
    public class GetGratitudeRandomByTypeQuery : IRequest<GratitudeDto?>
    {
        public GetGratitudeRandomByTypeQuery(GratitudeType type, string language, string name = "Alice", string signature = "Bob")
        {
            Language = language.ToLower();
            Type = type;
            Name = name;
            Signature = signature;
        }

        public string Language { get; }
        public GratitudeType Type { get; }
        public string Name { get; }
        public string Signature { get; }
    }
}