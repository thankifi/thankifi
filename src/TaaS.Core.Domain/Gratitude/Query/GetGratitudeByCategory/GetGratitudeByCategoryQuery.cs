using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeByCategory
{
    public class GetGratitudeByCategoryQuery : IRequest<GratitudeDto?>
    {
        public GetGratitudeByCategoryQuery(int? categoryId, string language, string name = "Alice", string signature = "Bob")
        {
            CategoryId = categoryId;
            Language = language;
            Name = name;
            Signature = signature;
        }

        public GetGratitudeByCategoryQuery(string categoryTitle, string language, string name = "Alice", string signature = "Bob")
        {
            CategoryTitle = categoryTitle;
            Language = language;
            Name = name;
            Signature = signature;
        }

        public int? CategoryId { get; }
        public string? CategoryTitle { get; }
        public string Language { get; }
        public string Name { get; }
        public string Signature { get; }
    }
}