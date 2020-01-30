using MediatR;

namespace TaaS.Core.Domain.Query.GetSignedGratitudeRandom
{
    public class GetSignedGratitudeRandomQuery : IRequest<(int, string)>
    {
        public GetSignedGratitudeRandomQuery(string signature = "Bob")
        {
            Signature = signature;
        }

        public string Signature { get; }
    }
}