using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeRandom
{
    public class GetGratitudeRandomQuery : IRequest<GratitudeDto?>
    {
        public GetGratitudeRandomQuery(string language, string name = "Alice", string signature = "Bob")
        {
            Language = language.ToLower();
            Name = name;
            Signature = signature;
        }

        public string Language { get; }
        public string Name { get; }
        public string Signature { get; }
    }
}