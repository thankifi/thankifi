using MediatR;

namespace TaaS.Core.Domain.Query.GetGratitudeRandom
{
    public class GetGratitudeRandomQuery : IRequest<(int, string)>
    {
        public GetGratitudeRandomQuery(string language = "en", string name = "Alice", string signature = "Bob")
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