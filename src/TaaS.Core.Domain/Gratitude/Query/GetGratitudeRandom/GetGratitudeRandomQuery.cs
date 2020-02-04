using MediatR;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeRandom
{
    public class GetGratitudeRandomQuery : IRequest<Entity.Gratitude>
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