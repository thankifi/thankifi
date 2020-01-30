namespace TaaS.Core.Domain.Query.GetSignedGratitudeRandom
{
    public class GetRandomGratitudeSignedQuery
    {
        public GetRandomGratitudeSignedQuery(string signature = "Bob")
        {
            Signature = signature;
        }

        public string Signature { get; }
    }
}