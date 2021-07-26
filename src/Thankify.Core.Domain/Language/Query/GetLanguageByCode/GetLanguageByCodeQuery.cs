using MediatR;
using Thankify.Core.Domain.Language.Dto;

namespace Thankify.Core.Domain.Language.Query.GetLanguageByCode
{
    public class GetLanguageByCodeQuery : IRequest<LanguageDetailDto?>
    {
        public string Code { get; set; }
    }
}